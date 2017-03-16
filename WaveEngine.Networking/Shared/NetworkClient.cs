#region File Description
//-----------------------------------------------------------------------------
// NetworkClient
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Threading;
using System.Threading.Tasks;
using Lidgren.Network;
using WaveEngine.Networking.Messages;

namespace WaveEngine.Networking
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

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public long Identifier
        {
            get { return this.client.UniqueIdentifier; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkClient"/> class.
        /// </summary>
        /// <param name="applicationIdentifier">The application identifier.</param>
        public NetworkClient(string applicationIdentifier)
        {
            var config = new NetPeerConfiguration(applicationIdentifier);
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            config.UseMessageRecycling = false;
            this.client = new NetClient(config);
        }

        /// <summary>
        /// Occurs when [host disconnected].
        /// </summary>
        public event HostConnected HostDisconnected;

        /// <summary>
        /// Occurs when [host connected].
        /// </summary>
        public event HostConnected HostConnected;

        /// <summary>
        /// Occurs when [host discovered].
        /// </summary>
        public event HostDiscovered HostDiscovered;

        /// <summary>
        /// Occurs when [message received].
        /// </summary>
        public event MessageReceived MessageReceived;

        /// <summary>
        /// Discovers the hosts.
        /// </summary>
        /// <param name="port">The port.</param>
        public void DiscoverHosts(int port)
        {
            this.EnsureClientStarted();
            this.client.DiscoverLocalPeers(port);
        }

        /// <summary>
        /// Ensures the client started.
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
                        this.OnMessageReceived(new IncomingMessage(message));
                        break;
                    case NetIncomingMessageType.DiscoveryResponse:
                        this.OnHostDiscovered(new Host(message));
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        var status = (NetConnectionStatus)message.ReadByte();
                        switch (status)
                        {
                            case NetConnectionStatus.Connected:
                                this.OnHostConnected(new Host(message));
                                break;
                            case NetConnectionStatus.Disconnected:
                                this.OnHostDisconnected(new Host(message));
                                break;
                        }

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
        /// Connects the specified host.
        /// </summary>
        /// <param name="host">The host.</param>
        public void Connect(Host host)
        {
            this.EnsureClientStarted();
            this.client.Connect(host.Address, host.Port);
            this.IsConnected = true;
        }

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        public void Disconnect()
        {
            this.readTaskCancellationTokenSource.Cancel();
            this.client.Disconnect(string.Empty);
            this.IsConnected = false;
        }

        /// <summary>
        /// Sends the specified to send message.
        /// </summary>
        /// <param name="toSendMessage">To send message.</param>
        /// <param name="deliveryMethod">The delivery method.</param>
        public void Send(OutgoingMessage toSendMessage, DeliveryMethod deliveryMethod)
        {
            this.client.SendMessage(toSendMessage.Message, (NetDeliveryMethod)deliveryMethod);
        }

        /// <summary>
        /// Creates the message.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The ooutgoing message</returns>
        public OutgoingMessage CreateMessage(MessageType type = MessageType.Data)
        {
            return new OutgoingMessage(this.client.CreateMessage(), type);
        }

        /// <summary>
        /// Called when [host connected].
        /// </summary>
        /// <param name="host">The host.</param>
        protected virtual void OnHostConnected(Host host)
        {
            var handler = this.HostConnected;
            if (handler != null)
            {
                handler(this, host);
            }
        }

        /// <summary>
        /// Called when [host discovered].
        /// </summary>
        /// <param name="host">The host.</param>
        protected virtual void OnHostDiscovered(Host host)
        {
            var handler = this.HostDiscovered;
            if (handler != null)
            {
                handler(this, host);
            }
        }

        /// <summary>
        /// Called when [message received].
        /// </summary>
        /// <param name="receivedmessage">The receivedmessage.</param>
        protected virtual void OnMessageReceived(IncomingMessage receivedmessage)
        {
            var handler = this.MessageReceived;
            if (handler != null)
            {
                handler(this, receivedmessage);
            }
        }

        /// <summary>
        /// Called when [host disconnected].
        /// </summary>
        /// <param name="host">The host.</param>
        protected virtual void OnHostDisconnected(Host host)
        {
            var handler = this.HostDisconnected;
            if (handler != null)
            {
                handler(this, host);
            }
        }
    }
}
