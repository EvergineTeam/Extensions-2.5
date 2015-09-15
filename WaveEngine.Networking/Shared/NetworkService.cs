#region File Description
//-----------------------------------------------------------------------------
// NetworkService
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Threading.Tasks;
using WaveEngine.Common;
using WaveEngine.Framework;
using WaveEngine.Networking.Messages;

namespace WaveEngine.Networking
{
    /// <summary>
    /// The network service
    /// </summary>
    public class NetworkService : Service, INetworkService
    {
        /// <summary>
        /// The client identifier
        /// </summary>
        private string clientIdentifier = Guid.NewGuid().ToString();

        /// <summary>
        /// The factory
        /// </summary>
        private readonly INetworkFactory factory;

        /// <summary>
        /// The network server
        /// </summary>
        private INetworkServer networkServer;

        /// <summary>
        /// The network client
        /// </summary>
        private INetworkClient networkClient;

        /// <summary>
        /// Gets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        public string ClientIdentifier
        {
            get { return this.clientIdentifier; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkService"/> class.
        /// </summary>
        public NetworkService()
        {
            this.factory = new NetworkFactory();
        }

        /// <summary>
        /// Occurs when [host message received].
        /// </summary>
        public event MessageReceived HostMessageReceived;

        /// <summary>
        /// Occurs when [client message received].
        /// </summary>
        public event MessageReceived ClientMessageReceived;

        /// <summary>
        /// Occurs when [host connected].
        /// </summary>
        public event HostConnected HostConnected;

        /// <summary>
        /// Occurs when [host disconnected].
        /// </summary>
        public event HostConnected HostDisconnected;

        /// <summary>
        /// Occurs when [host discovered].
        /// </summary>
        public event HostDiscovered HostDiscovered;

        /// <summary>
        /// Initializes the host.
        /// </summary>
        /// <param name="applicationIdentifier">The application identifier.</param>
        /// <param name="port">The port.</param>
        /// <exception cref="System.ArgumentException">Application identifier parameter can't be null or empty.</exception>
        /// <exception cref="System.InvalidOperationException">You can't call multiple times to InitializeHost method.</exception>
        public void InitializeHost(string applicationIdentifier, int port)
        {
            if (string.IsNullOrEmpty(applicationIdentifier))
            {
                throw new ArgumentException("Application identifier parameter can't be null or empty.");
            }

            if (this.networkServer != null)
            {
                throw new InvalidOperationException("You can't call multiple times to InitializeHost method.");
            }

            try
            {
                this.networkServer = this.factory.CreateNetworkServer(applicationIdentifier, port);
                this.networkServer.Start();
                this.networkServer.MessageReceived += (sender, message) => this.OnHostMessageReceived(message);
            }
            catch
            {
                this.networkServer = null;
                throw;
            }
        }

        /// <summary>
        /// Discoveries the hosts.
        /// </summary>
        /// <param name="applicationIdentifier">The application identifier.</param>
        /// <param name="port">The port.</param>
        /// <exception cref="System.ArgumentException">Application identifier parameter can't be null or empty.</exception>
        /// <exception cref="System.InvalidOperationException">You can't call discovery method while is connected to a host. Call before to disconnect method.</exception>
        public void DiscoveryHosts(string applicationIdentifier, int port)
        {
            if (string.IsNullOrEmpty(applicationIdentifier))
            {
                throw new ArgumentException("Application identifier parameter can't be null or empty.");
            }

            this.EnsureClient(applicationIdentifier);

            if (this.networkClient.IsConnected)
            {
                throw new InvalidOperationException("You can't call discovery method while is connected to a host. Call before to disconnect method.");
            }

            this.networkClient.DiscoverHosts(port);
        }

        /// <summary>
        /// Ensures the client.
        /// </summary>
        /// <param name="applicationIdentifier">The application identifier.</param>
        private void EnsureClient(string applicationIdentifier)
        {
            if (this.networkClient == null)
            {
                this.networkClient = this.factory.CreateNetworkClient(applicationIdentifier);
                this.networkClient.HostConnected += (sender, host) => this.OnHostConnected(host);
                this.networkClient.HostDisconnected += (sender, host) => this.OnHostDisconnected(host);
                this.networkClient.HostDiscovered += (sender, host) => this.OnHostDiscovered(host);
                this.networkClient.MessageReceived += (sender, message) => this.OnClientMessageReceived(message);
            }
        }

        /// <summary>
        /// Connects the specified application identifier.
        /// </summary>
        /// <param name="applicationIdentifier">The application identifier.</param>
        /// <param name="host">The host.</param>
        /// <exception cref="System.ArgumentException">Application identifier parameter can't be null or empty.</exception>
        public void Connect(string applicationIdentifier, Host host)
        {
            if (string.IsNullOrEmpty(applicationIdentifier))
            {
                throw new ArgumentException("Application identifier parameter can't be null or empty.");
            }

            this.EnsureClient(applicationIdentifier);
            this.Connect(host);
        }

        /// <summary>
        /// Connects the specified host.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <exception cref="System.InvalidOperationException">
        /// You can't call this overload connect method whithout discover hosts. Call before to discover hosts method.
        /// or
        /// You can't call connect method while is connected to a host. Call before to disconnect method.
        /// </exception>
        public void Connect(Host host)
        {
            if (this.networkClient == null)
            {
                throw new InvalidOperationException("You can't call this overload connect method whithout discover hosts. Call before to discover hosts method.");
            }

            if (this.networkClient.IsConnected)
            {
                throw new InvalidOperationException("You can't call connect method while is connected to a host. Call before to disconnect method.");
            }

            this.networkClient.Connect(host);
        }

        /// <summary>
        /// Connects the asynchronous.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <returns>An awaitable task</returns>
        public async Task ConnectAsync(Host host)
        {
            var tcs = new TaskCompletionSource<bool>();
            HostConnected connectHandler = (sender, connectedHost) =>
            {
                if (connectedHost.Address == host.Address && connectedHost.Port == host.Port)
                {
                    tcs.SetResult(true);
                }
            };
            this.HostConnected += connectHandler;
            try
            {
                this.Connect(host);
                await tcs.Task;
            }
            finally
            {
                this.HostConnected -= connectHandler;
            }
        }

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        public void Disconnect()
        {
            if (this.networkClient != null)
            {
                this.networkClient.Disconnect();
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            this.Disconnect();
            this.ShutdownHost();
        }

        /// <summary>
        /// Shutdowns the host.
        /// </summary>
        public void ShutdownHost()
        {
            if (this.networkServer != null)
            {
                this.networkServer.Shutdown();
            }
        }

        /// <summary>
        /// Sends to server.
        /// </summary>
        /// <param name="messageToSend">The message to send.</param>
        /// <param name="deliveryMethod">The delivery method.</param>
        public void SendToServer(OutgoingMessage messageToSend, DeliveryMethod deliveryMethod)
        {
            this.networkClient.Send(messageToSend, deliveryMethod);
        }

        /// <summary>
        /// Sends to clients.
        /// </summary>
        /// <param name="messageToSend">The message to send.</param>
        /// <param name="deliveryMethod">The delivery method.</param>
        public void SendToClients(OutgoingMessage messageToSend, DeliveryMethod deliveryMethod)
        {
            this.networkServer.Send(messageToSend, deliveryMethod);
        }

        /// <summary>
        /// Res the send to clients.
        /// </summary>
        /// <param name="obj">The object.</param>
        public void ReSendToClients(IncomingMessage obj)
        {
            this.networkServer.Send(obj, (DeliveryMethod)obj.Message.DeliveryMethod);
        }

        /// <summary>
        /// Res the send to clients.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="deliveryMethod">The delivery method.</param>
        public void ReSendToClients(IncomingMessage obj, DeliveryMethod deliveryMethod)
        {
            this.networkServer.Send(obj, deliveryMethod);
        }

        /// <summary>
        /// Creates the server message.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The outgoing message.</returns>
        public OutgoingMessage CreateServerMessage(MessageType type = MessageType.Data)
        {
            return this.networkServer.CreateMessage(type);
        }

        /// <summary>
        /// Creates the client message.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The outgoing message.</returns>
        public OutgoingMessage CreateClientMessage(MessageType type = MessageType.Data)
        {
            return this.networkClient.CreateMessage(type);
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
        /// Called when [host message received].
        /// </summary>
        /// <param name="receivedMessage">The received message.</param>
        protected virtual void OnHostMessageReceived(IncomingMessage receivedMessage)
        {
            if (receivedMessage.Type == MessageType.Synchronization)
            {
                this.ReSendToClients(receivedMessage);
                return;
            }

            var handler = this.HostMessageReceived;
            if (handler != null)
            {
                handler(this, receivedMessage);
            }
        }

        /// <summary>
        /// Called when [client message received].
        /// </summary>
        /// <param name="receivedMessage">The received message.</param>
        protected virtual void OnClientMessageReceived(IncomingMessage receivedMessage)
        {
            if (receivedMessage.Type == MessageType.Synchronization)
            {
                NetworkManager.HandleMenssage(receivedMessage);
                return;
            }

            var handler = this.ClientMessageReceived;
            if (handler != null)
            {
                handler(this, receivedMessage);
            }
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

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Initialize()
        {
        }

        /// <summary>
        /// Terminates this instance.
        /// </summary>
        protected override void Terminate()
        {
            this.Dispose();
        }

        /// <summary>
        /// Registers the scene and return its manager.
        /// </summary>
        /// <param name="scene">The scene.</param>
        /// <param name="sceneId">The scene identifier.</param>
        /// <returns>The NetworkManager associated with the scene.</returns>
        public NetworkManager RegisterScene(Scene scene, string sceneId)
        {
            var networkManager = new NetworkManager(scene, this);
            scene.Closed += (e, args) =>
            {
                networkManager.Dispose();
            };

            networkManager.RegisterScene(sceneId);
            return networkManager;
        }
    }
}
