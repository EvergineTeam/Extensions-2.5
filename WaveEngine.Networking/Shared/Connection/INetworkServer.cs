// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using WaveEngine.Networking.Connection.Messages;
using WaveEngine.Networking.Messages;
#endregion

namespace WaveEngine.Networking.Connection
{
    /// <summary>
    /// The network server interface.
    /// </summary>
    public interface INetworkServer
    {
        #region Events

        /// <summary>
        /// Occurs when the server receives a new client connection request.
        /// </summary>
        event EventHandler<ClientConnectingEventArgs> ClientConnecting;

        /// <summary>
        /// Occurs when the server receives a new client connection.
        /// </summary>
        /// <remarks>
        /// The incoming message represent the hail message sent by the client.
        /// </remarks>
        event EventHandler<ClientConnectedEventArgs> ClientConnected;

        /// <summary>
        /// Occurs when the server lose the connection with a client.
        /// </summary>
        event EventHandler<ClientDisconnectedEventArgs> ClientDisconnected;

        /// <summary>
        /// Occurs when a message is received by the server.
        /// </summary>
        event EventHandler<MessageReceivedEventArgs> MessageReceived;

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts the server.
        /// </summary>
        void Start();

        /// <summary>
        /// Shutdowns the server.
        /// </summary>
        void Shutdown();

        /// <summary>
        /// Sends the specified outgoing message to all the clients.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="deliveryMethod">The delivery method.</param>
        void Send(OutgoingMessage message, DeliveryMethod deliveryMethod);

        /// <summary>
        /// Sends the specified outgoing message to the specified client.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="deliveryMethod">The delivery method.</param>
        /// <param name="destinationClient">The destination client.</param>
        void Send(OutgoingMessage message, DeliveryMethod deliveryMethod, NetworkEndpoint destinationClient);

        /// <summary>
        /// Sends the specified outgoing message to the specified clients.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="deliveryMethod">The delivery method.</param>
        /// <param name="destinationClients">The destination clients.</param>
        void Send(OutgoingMessage message, DeliveryMethod deliveryMethod, IEnumerable<NetworkEndpoint> destinationClients);

        /// <summary>
        /// Sends the specified incoming message to others clients.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="deliveryMethod">The delivery method.</param>
        void Send(IncomingMessage message, DeliveryMethod deliveryMethod);

        /// <summary>
        /// Sends the specified incoming message to the specified client.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="deliveryMethod">The delivery method.</param>
        /// <param name="destinationClient">The destination client.</param>
        void Send(IncomingMessage message, DeliveryMethod deliveryMethod, NetworkEndpoint destinationClient);

        /// <summary>
        /// Sends the specified incoming message to the specified clients.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="deliveryMethod">The delivery method.</param>
        /// <param name="destinationClients">The destination clients.</param>
        void Send(IncomingMessage message, DeliveryMethod deliveryMethod, IEnumerable<NetworkEndpoint> destinationClients);

        /// <summary>
        /// Creates a new outgoing message.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The created outgoing message.</returns>
        OutgoingMessage CreateMessage(MessageType type = MessageType.Data);

        #endregion
    }
}
