// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WaveEngine.Framework.Threading;
using WaveEngine.Networking.Connection.Messages;
using WaveEngine.Networking.Messages;
#endregion

namespace WaveEngine.Networking.Connection
{
    /// <summary>
    /// The network server
    /// </summary>
    public class NetworkServer : INetworkServer
    {
        /// <summary>
        /// The server
        /// </summary>
        private readonly NetServer server;

        /// <summary>
        /// Used to raise network events in a exclusive separated scheduler when Wave Engine is
        /// not running
        /// </summary>
        private Lazy<ConcurrentExclusiveSchedulerPair> concurrentSchedulerPair = new Lazy<ConcurrentExclusiveSchedulerPair>();

        /// <summary>
        /// The read task cancellation token source
        /// </summary>
        private CancellationTokenSource readTaskCancellationTokenSource;

        private Dictionary<NetworkEndpoint, NetConnection> connectionsByEndpoint;

        #region Events

        /// <inheritdoc />
        public event EventHandler<ClientConnectingEventArgs> ClientConnecting;

        /// <inheritdoc />
        public event EventHandler<ClientConnectedEventArgs> ClientConnected;

        /// <inheritdoc />
        public event EventHandler<ClientDisconnectedEventArgs> ClientDisconnected;

        /// <inheritdoc />
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkServer" /> class.
        /// </summary>
        /// <param name="applicationIdentifier">The application identifier.</param>
        /// <param name="port">The port.</param>
        /// <param name="pingInterval">Ping interval in seconds.</param>
        /// <param name="connectionTimeout">Connection timeout in seconds.</param>
        public NetworkServer(string applicationIdentifier, int port, float pingInterval, float connectionTimeout)
        {
            var config = new NetPeerConfiguration(applicationIdentifier);
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            config.PingInterval = pingInterval;
            config.ConnectionTimeout = connectionTimeout;
            config.UseMessageRecycling = false;
            config.Port = port;
            this.server = new NetServer(config);
            this.connectionsByEndpoint = new Dictionary<NetworkEndpoint, NetConnection>();
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public void Start()
        {
            this.server.Start();
            Task.Factory.StartNew(this.ReadTask, TaskCreationOptions.LongRunning);
        }

        /// <inheritdoc />
        public void Shutdown()
        {
            this.readTaskCancellationTokenSource.Cancel();
            this.server.Shutdown(string.Empty);
            this.connectionsByEndpoint.Clear();
        }

        /// <inheritdoc />
        public void Send(OutgoingMessage toSendMessage, DeliveryMethod deliveryMethod)
        {
            foreach (var netConnection in this.server.Connections)
            {
                this.InternalSend(toSendMessage.Message, deliveryMethod, netConnection);
            }
        }

        /// <inheritdoc />
        public void Send(OutgoingMessage toSendMessage, DeliveryMethod deliveryMethod, NetworkEndpoint destinationClient)
        {
            if (destinationClient == null)
            {
                throw new ArgumentNullException(nameof(destinationClient));
            }

            NetConnection netConnection;
            if (this.connectionsByEndpoint.TryGetValue(destinationClient, out netConnection))
            {
                this.InternalSend(toSendMessage.Message, deliveryMethod, netConnection);
            }
        }

        /// <inheritdoc />
        public void Send(OutgoingMessage toSendMessage, DeliveryMethod deliveryMethod, IEnumerable<NetworkEndpoint> destinationClients)
        {
            if (destinationClients == null)
            {
                throw new ArgumentNullException(nameof(destinationClients));
            }

            foreach (var clientEndpoint in destinationClients)
            {
                this.Send(toSendMessage, deliveryMethod, clientEndpoint);
            }
        }

        /// <inheritdoc />
        public void Send(IncomingMessage incomingMessage, DeliveryMethod deliveryMethod)
        {
            for (int index = 0; index < this.server.Connections.Count; index++)
            {
                var netConnection = this.server.Connections[index];
                if (netConnection != incomingMessage.Message.SenderConnection)
                {
                    this.InternalSend(incomingMessage.Message, deliveryMethod, netConnection);
                }
            }
        }

        /// <inheritdoc />
        public void Send(IncomingMessage toSendMessage, DeliveryMethod deliveryMethod, NetworkEndpoint destinationClient)
        {
            if (destinationClient == null)
            {
                throw new ArgumentNullException(nameof(destinationClient));
            }

            NetConnection netConnection;
            if (this.connectionsByEndpoint.TryGetValue(destinationClient, out netConnection))
            {
                this.InternalSend(toSendMessage.Message, deliveryMethod, netConnection);
            }
        }

        /// <inheritdoc />
        public void Send(IncomingMessage toSendMessage, DeliveryMethod deliveryMethod, IEnumerable<NetworkEndpoint> destinationClients)
        {
            if (destinationClients == null)
            {
                throw new ArgumentNullException(nameof(destinationClients));
            }

            foreach (var clientEndpoint in destinationClients)
            {
                this.Send(toSendMessage, deliveryMethod, clientEndpoint);
            }
        }

        /// <inheritdoc />
        public OutgoingMessage CreateMessage(MessageType type = MessageType.Data)
        {
            return new OutgoingMessage(this.server.CreateMessage(), type);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Internal method to send a NetBuffer message
        /// </summary>
        /// <param name="netMessage">The NetBuffer message</param>
        /// <param name="deliveryMethod">The delivery method</param>
        /// <param name="netConnection">The destination netconection</param>
        private void InternalSend(NetBuffer netMessage, DeliveryMethod deliveryMethod, NetConnection netConnection)
        {
            var message = this.server.CreateMessage(netMessage.LengthBytes);
            message.Write(netMessage.Data);
            this.server.SendMessage(message, netConnection, (NetDeliveryMethod)deliveryMethod);
        }

        /// <summary>
        /// The read task loop.
        /// </summary>
        private void ReadTask()
        {
            this.readTaskCancellationTokenSource = new CancellationTokenSource();
            while (!this.readTaskCancellationTokenSource.IsCancellationRequested)
            {
                var message = this.server.WaitMessage(100);
                if (message == null)
                {
                    continue;
                }

                switch (message.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        this.OnMessageReceived(message.GetSenderEndPoint(), new IncomingMessage(message));
                        break;
                    case NetIncomingMessageType.DiscoveryRequest:
                        var response = this.server.CreateMessage();
                        response.Write(this.server.Configuration.AppIdentifier);
                        this.server.SendDiscoveryResponse(response, message.SenderEndPoint);
                        break;
                    case NetIncomingMessageType.ConnectionApproval:
                        var rejected = false;
                        var connectingHandler = this.ClientConnecting;
                        if (connectingHandler != null)
                        {
                            var clientConnectionRequest = new ClientConnectingEventArgs(message.GetSenderEndPoint(), new IncomingMessage(message));
                            connectingHandler(this, clientConnectionRequest);
                            rejected = clientConnectionRequest.IsRejected;
                        }

                        if (rejected)
                        {
                            message.SenderConnection.Deny();
                        }
                        else
                        {
                            message.SenderConnection.Approve();
                        }

                        break;
                    case NetIncomingMessageType.StatusChanged:
                        var clientEndPoint = message.GetSenderEndPoint();
                        var status = (NetConnectionStatus)message.ReadByte();
                        switch (status)
                        {
                            case NetConnectionStatus.Connected:
                                this.connectionsByEndpoint[clientEndPoint] = message.SenderConnection;
                                var remoteHailMessage = message.SenderConnection.RemoteHailMessage;
                                var hailMessage = remoteHailMessage?.LengthBytes > 0 ? new IncomingMessage(remoteHailMessage) : null;
                                this.OnClientConnected(clientEndPoint, hailMessage);
                                break;
                            case NetConnectionStatus.Disconnected:
                                this.connectionsByEndpoint.Remove(clientEndPoint);
                                this.OnClientDisconnected(clientEndPoint);
                                break;
                        }
#if DEBUG
                        var reason = message.ReadString();
                        System.Diagnostics.Debug.WriteLine("New server status: " + status + " (" + reason + ")");
#endif
                        break;
                    case NetIncomingMessageType.Error:
                    case NetIncomingMessageType.UnconnectedData:
                    case NetIncomingMessageType.Receipt:
                    case NetIncomingMessageType.DiscoveryResponse:
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.ErrorMessage:
                    case NetIncomingMessageType.NatIntroductionSuccess:
                    case NetIncomingMessageType.ConnectionLatencyUpdated:
                        break;
                }
            }
        }

        /// <summary>
        /// Called when [client connected].
        /// </summary>
        /// <param name="client">The client endpoint.</param>
        /// <param name="hailMessage">The hail message received from the client</param>
        protected virtual void OnClientConnected(NetworkEndpoint client, IncomingMessage hailMessage)
        {
            this.InternalRaiseEvent(() => this.ClientConnected?.Invoke(this, new ClientConnectedEventArgs(client, hailMessage)));
        }

        /// <summary>
        /// Called when [client disconnected].
        /// </summary>
        /// <param name="client">The client endpoint.</param>
        protected virtual void OnClientDisconnected(NetworkEndpoint client)
        {
            this.InternalRaiseEvent(() => this.ClientDisconnected?.Invoke(this, new ClientDisconnectedEventArgs(client)));
        }

        /// <summary>
        /// Called when a message is received by the server.
        /// </summary>
        /// <param name="client">The client endpoint that sent the message.</param>
        /// <param name="receivedMessage">The received message.</param>
        protected virtual void OnMessageReceived(NetworkEndpoint client, IncomingMessage receivedMessage)
        {
            this.InternalRaiseEvent(() => this.MessageReceived?.Invoke(this, new MessageReceivedEventArgs(client, receivedMessage)));
        }

        private void InternalRaiseEvent(Action eventAction)
        {
            if (Framework.Game.Current != null)
            {
                WaveForegroundTask.Run(eventAction);
            }
            else
            {
                Task.Factory.StartNew(eventAction, CancellationToken.None, TaskCreationOptions.HideScheduler, this.concurrentSchedulerPair.Value.ExclusiveScheduler);
            }
        }

        #endregion
    }
}
