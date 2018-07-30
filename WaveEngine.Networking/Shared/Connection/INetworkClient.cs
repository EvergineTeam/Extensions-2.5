// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using WaveEngine.Networking.Connection.Messages;
using WaveEngine.Networking.Messages;
#endregion

namespace WaveEngine.Networking.Connection
{
    /// <summary>
    /// The network client interface.
    /// </summary>
    public interface INetworkClient
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        bool IsConnected { get; }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        long Identifier { get; }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a host message is received by the client.
        /// </summary>
        event EventHandler<MessageReceivedEventArgs> MessageReceived;

        /// <summary>
        /// Occurs when the client is connected to a host.
        /// </summary>
        event EventHandler<HostConnectedEventArgs> HostConnected;

        /// <summary>
        /// Occurs when the client is disconnected from the host.
        /// </summary>
        event EventHandler<HostDisconnectedEventArgs> HostDisconnected;

        /// <summary>
        /// Occurs when a new host with the same application identifier is discovered.
        /// </summary>
        event EventHandler<HostDiscoveredEventArgs> HostDiscovered;

        #endregion

        #region Public Methods

        /// <summary>
        /// Emit a discovery signal to all hosts on your subnet.
        /// Hosts with the same appId and port will be notified by the <see cref="HostDisconnected"/> event.
        /// </summary>
        /// <param name="port">The expected port.</param>
        void DiscoverHosts(int port);

        /// <summary>
        /// Connects to a remote host.
        /// </summary>
        /// <param name="host">The remote host endpoint to connect to.</param>
        void Connect(NetworkEndpoint host);

        /// <summary>
        /// Connects to a remote host.
        /// </summary>
        /// <param name="host">The remote host endpoint to connect to.</param>
        /// <param name="hailMessage">The hail message to pass</param>
        void Connect(NetworkEndpoint host, OutgoingMessage hailMessage);

        /// <summary>
        /// Disconnect from server
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Sends the specified message to host.
        /// </summary>
        /// <param name="toSendMessage">To message to send.</param>
        /// <param name="deliveryMethod">The delivery method.</param>
        void Send(OutgoingMessage toSendMessage, DeliveryMethod deliveryMethod);

        /// <summary>
        /// Creates a new outgoing message.
        /// </summary>
        /// <param name="type">The message type.</param>
        /// <returns>The create outgoing message.</returns>
        OutgoingMessage CreateMessage(MessageType type = MessageType.Data);

        #endregion
    }
}
