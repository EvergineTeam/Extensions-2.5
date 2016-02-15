#region File Description
//-----------------------------------------------------------------------------
// NetworkServer
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

using System.Threading;
using System.Threading.Tasks;
using Lidgren.Network;
using WaveEngine.Networking.Messages;

namespace WaveEngine.Networking
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
        /// The read task cancellation token source
        /// </summary>
        private CancellationTokenSource readTaskCancellationTokenSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkServer" /> class.
        /// </summary>
        /// <param name="applicationIdentifier">The application identifier.</param>
        /// <param name="port">The port.</param>
        public NetworkServer(string applicationIdentifier, int port)
        {
            var config = new NetPeerConfiguration(applicationIdentifier);
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.Port = port;
            this.server = new NetServer(config);
        }

        /// <summary>
        /// Occurs when [message received].
        /// </summary>
        public event MessageReceived MessageReceived;

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            this.server.Start();
            Task.Factory.StartNew(this.ReadTask, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Reads the task.
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
                        this.OnMessageReceived(new IncomingMessage(message));
                        break;
                    case NetIncomingMessageType.DiscoveryRequest:
                        this.server.SendDiscoveryResponse(null, message.SenderEndPoint);
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        ////TODO Connected and disconnected events
                        break;
                    case NetIncomingMessageType.Error:
                    case NetIncomingMessageType.UnconnectedData:
                    case NetIncomingMessageType.ConnectionApproval:
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
        /// Shutdowns this instance.
        /// </summary>
        public void Shutdown()
        {
            this.readTaskCancellationTokenSource.Cancel();
            this.server.Shutdown(string.Empty);
        }

        /// <summary>
        /// Sends the specified to send message.
        /// </summary>
        /// <param name="toSendMessage">To send message.</param>
        /// <param name="deliveryMethod">The delivery method.</param>
        public void Send(OutgoingMessage toSendMessage, DeliveryMethod deliveryMethod)
        {
            foreach (var netConnection in this.server.Connections)
            {
                var message = this.server.CreateMessage(toSendMessage.Message.LengthBytes);
                message.Write(toSendMessage.Message.Data);
                this.server.SendMessage(message, netConnection, (NetDeliveryMethod)deliveryMethod);
            }
        }

        /// <summary>
        /// Sends the specified incoming message.
        /// </summary>
        /// <param name="incomingMessage">The incoming message.</param>
        /// <param name="deliveryMethod">The delivery method.</param>
        public void Send(IncomingMessage incomingMessage, DeliveryMethod deliveryMethod)
        {
            for (int index = 0; index < this.server.Connections.Count; index++)
            {
                var netConnection = this.server.Connections[index];
                if (netConnection != incomingMessage.Message.SenderConnection)
                {
                    var message = this.server.CreateMessage(incomingMessage.Message.LengthBytes);
                    message.Write(incomingMessage.Message.Data);
                    this.server.SendMessage(message, netConnection, (NetDeliveryMethod)deliveryMethod);
                }
            }
        }

        /// <summary>
        /// Creates the message.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The outgoing message</returns>
        public OutgoingMessage CreateMessage(MessageType type = MessageType.Data)
        {
            return new OutgoingMessage(this.server.CreateMessage(), type);
        }

        /// <summary>
        /// Called when [message received].
        /// </summary>
        /// <param name="receivedMessage">The received message.</param>
        protected virtual void OnMessageReceived(IncomingMessage receivedMessage)
        {
            var handler = this.MessageReceived;
            if (handler != null)
            {
                handler(this, receivedMessage);
            }
        }
    }
}
