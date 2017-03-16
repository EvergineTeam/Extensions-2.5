#region File Description
//-----------------------------------------------------------------------------
// INetworkService
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Threading.Tasks;
using WaveEngine.Framework;
using WaveEngine.Networking.Messages;

namespace WaveEngine.Networking
{
    /// <summary>
    /// The network service interface
    /// </summary>
    public interface INetworkService : IDisposable
    {
        /// <summary>
        /// Occurs when [host message received].
        /// </summary>
        event MessageReceived HostMessageReceived;

        /// <summary>
        /// Occurs when [client message received].
        /// </summary>
        event MessageReceived ClientMessageReceived;

        /// <summary>
        /// Occurs when [host connected].
        /// </summary>
        event HostConnected HostConnected;

        /// <summary>
        /// Occurs when [host discovered].
        /// </summary>
        event HostDiscovered HostDiscovered;

        /// <summary>
        /// Initializes the host.
        /// </summary>
        /// <param name="applicationIdentifier">The application identifier.</param>
        /// <param name="port">The port.</param>
        void InitializeHost(string applicationIdentifier, int port);

        /// <summary>
        /// Discoveries the hosts.
        /// </summary>
        /// <param name="applicationIdentifier">The application identifier.</param>
        /// <param name="port">The port.</param>
        void DiscoveryHosts(string applicationIdentifier, int port);

        /// <summary>
        /// Connects the specified application identifier.
        /// </summary>
        /// <param name="applicationIdentifier">The application identifier.</param>
        /// <param name="host">The host.</param>
        void Connect(string applicationIdentifier, Host host);

        /// <summary>
        /// Connects the specified host.
        /// </summary>
        /// <param name="host">The host.</param>
        void Connect(Host host);

        /// <summary>
        /// Connects the asynchronous.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <returns>An awaitable task</returns>
        Task ConnectAsync(Host host);

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Shutdowns the host.
        /// </summary>
        void ShutdownHost();

        /// <summary>
        /// Sends to server.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="deliveryMethod">The delivery method.</param>
        void SendToServer(OutgoingMessage message, DeliveryMethod deliveryMethod);

        /// <summary>
        /// Sends to clients.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="deliveryMethod">The delivery method.</param>
        void SendToClients(OutgoingMessage message, DeliveryMethod deliveryMethod);

        /// <summary>
        /// Res the send to clients.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="deliveryMethod">The delivery method.</param>
        void ReSendToClients(IncomingMessage obj, DeliveryMethod deliveryMethod);

        /// <summary>
        /// Creates the server message.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The outgoing message</returns>
        OutgoingMessage CreateServerMessage(MessageType type = MessageType.Data);

        /// <summary>
        /// Creates the client message.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The outgoing message</returns>
        OutgoingMessage CreateClientMessage(MessageType type = MessageType.Data);

        /// <summary>
        /// Registers the scene and return its manager.
        /// </summary>
        /// <param name="scene">The scene.</param>
        /// <param name="sceneId">The scene identifier.</param>
        /// <returns>The NetworkManager associated with the scene.</returns>
        NetworkManager RegisterScene(Scene scene, string sceneId);
    }
}