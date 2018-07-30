// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using Lidgren.Network;
using System;
using System.Threading;
using System.Threading.Tasks;
using WaveEngine.Framework.Threading;
using WaveEngine.Networking.Connection.Messages;
using WaveEngine.Networking.Messages;
#endregion

namespace WaveEngine.Networking.Connection
{
    /// <summary>
    /// The network client
    /// </summary>
    public class NetworkClient : INetworkClient
    {
        /// <summary>
        /// The client
        /// </summary>
        private readonly NetClient client;

        /// <summary>
        /// The client started
        /// </summary>
        private bool clientStarted;

        /// <summary>
        /// The read task cancellation token source
        /// </summary>
        private CancellationTokenSource readTaskCancellationTokenSource;

        #region Properties

        /// <inheritdoc />
        public bool IsConnected { get; private set; }

        /// <inheritdoc />
        public long Identifier
        {
            get { return this.client.UniqueIdentifier; }
        }

        #endregion

        #region Events

        /// <inheritdoc />
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        /// <inheritdoc />
        public event EventHandler<HostConnectedEventArgs> HostConnected;

        /// <inheritdoc />
        public event EventHandler<HostDisconnectedEventArgs> HostDisconnected;

        /// <inheritdoc />
        public event EventHandler<HostDiscoveredEventArgs> HostDiscovered;

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkClient"/> class.
        /// </summary>
        /// <param name="applicationIdentifier">The application identifier.</param>
        /// <param name="pingInterval">Ping interval in seconds.</param>
        /// <param name="connectionTimeout">Connection timeout in seconds.</param>
        public NetworkClient(string applicationIdentifier, float pingInterval, float connectionTimeout)
        {
            var config = new NetPeerConfiguration(applicationIdentifier);
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            config.PingInterval = pingInterval;
            config.ConnectionTimeout = connectionTimeout;
            config.UseMessageRecycling = false;
            this.client = new NetClient(config);
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public void DiscoverHosts(int port)
        {
            this.EnsureClientStarted();
            this.client.DiscoverLocalPeers(port);
        }

        /// <inheritdoc />
        public void Connect(NetworkEndpoint host)
        {
            this.EnsureClientStarted();
            this.client.Connect(host.Address, host.Port);
            this.IsConnected = true;
        }

        /// <inheritdoc />
        public void Connect(NetworkEndpoint host, OutgoingMessage hailMessage)
        {
            this.EnsureClientStarted();
            this.client.Connect(host.Address, host.Port, hailMessage.Message);
            this.IsConnected = true;
        }

        /// <inheritdoc />
        public void Disconnect()
        {
            this.readTaskCancellationTokenSource.Cancel();
            this.client.Disconnect(string.Empty);
            this.IsConnected = false;
        }

        /// <inheritdoc />
        public void Send(OutgoingMessage toSendMessage, DeliveryMethod deliveryMethod)
        {
            this.client.SendMessage(toSendMessage.Message, (NetDeliveryMethod)deliveryMethod);
        }

        /// <inheritdoc />
        public OutgoingMessage CreateMessage(MessageType type = MessageType.Data)
        {
            return new OutgoingMessage(this.client.CreateMessage(), type);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Ensures that the client has been started.
        /// </summary>
        private void EnsureClientStarted()
        {
            if (!this.clientStarted)
            {
                this.clientStarted = true;
                this.client.Start();
                Task.Factory.StartNew(this.ReadTask, TaskCreationOptions.LongRunning);
            }
        }

        /// <summary>
        /// Reads the task.
        /// </summary>
        private void ReadTask()
        {
            this.readTaskCancellationTokenSource = new CancellationTokenSource();
            while (!this.readTaskCancellationTokenSource.IsCancellationRequested)
            {
                var message = this.client.WaitMessage(100);
                if (message == null)
                {
                    continue;
                }

                switch (message.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        this.OnMessageReceived(message.GetSenderEndPoint(), new IncomingMessage(message));
                        break;
                    case NetIncomingMessageType.DiscoveryResponse:
                        var serverAppId = message.ReadString();
                        if (serverAppId == this.client.Configuration.AppIdentifier)
                        {
                            this.OnHostDiscovered(message.GetSenderEndPoint());
                        }

                        break;
                    case NetIncomingMessageType.StatusChanged:
                        var status = (NetConnectionStatus)message.ReadByte();
                        switch (status)
                        {
                            case NetConnectionStatus.Connected:
                                this.OnHostConnected(message.GetSenderEndPoint());
                                break;
                            case NetConnectionStatus.Disconnected:
                                this.OnHostDisconnected(message.GetSenderEndPoint());
                                break;
                        }
#if DEBUG
                        var reason = message.ReadString();
                        System.Diagnostics.Debug.WriteLine("New client status: " + status + " (" + reason + ")");
#endif

                        break;
                    case NetIncomingMessageType.Error:
                    case NetIncomingMessageType.UnconnectedData:
                    case NetIncomingMessageType.ConnectionApproval:
                    case NetIncomingMessageType.Receipt:
                    case NetIncomingMessageType.DiscoveryRequest:
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.ErrorMessage:
                    case NetIncomingMessageType.NatIntroductionSuccess:
                    case NetIncomingMessageType.ConnectionLatencyUpdated:
                        break;
                }
            }

            this.clientStarted = false;
        }

        /// <summary>
        /// Called when the client connects with the host.
        /// </summary>
        /// <param name="host">The host.</param>
        protected virtual void OnHostConnected(NetworkEndpoint host)
        {
            WaveForegroundTask.Run(() => this.HostConnected?.Invoke(this, new HostConnectedEventArgs(host)));
        }

        /// <summary>
        /// Called when a host with the same application identifier is discovered.
        /// </summary>
        /// <param name="host">The discovered host endpoint.</param>
        protected virtual void OnHostDiscovered(NetworkEndpoint host)
        {
            WaveForegroundTask.Run(() => this.HostDiscovered?.Invoke(this, new HostDiscoveredEventArgs(host)));
        }

        /// <summary>
        /// Called when message is received.
        /// </summary>
        /// <param name="host">The host endpoint that sent the message.</param>
        /// <param name="receivedMessage">The received message.</param>
        protected virtual void OnMessageReceived(NetworkEndpoint host, IncomingMessage receivedMessage)
        {
            WaveForegroundTask.Run(() => this.MessageReceived?.Invoke(this, new MessageReceivedEventArgs(host, receivedMessage)));
        }

        /// <summary>
        /// Called when the client lose the connection with the host.
        /// </summary>
        /// <param name="host">The host endpoint.</param>
        protected virtual void OnHostDisconnected(NetworkEndpoint host)
        {
            WaveForegroundTask.Run(() => this.HostDisconnected?.Invoke(this, new HostDisconnectedEventArgs(host)));
        }

        #endregion
    }
}
