#region File Description
//-----------------------------------------------------------------------------
// INetworkClient
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

using WaveEngine.Networking.Messages;

namespace WaveEngine.Networking
{
    /// <summary>
    /// The network client interface.
    /// </summary>
    public interface INetworkClient
    {
        /// <summary>
        /// Occurs when [host disconnected].
        /// </summary>
        event HostConnected HostDisconnected;

        /// <summary>
        /// Occurs when [host connected].
        /// </summary>
        event HostConnected HostConnected;

        /// <summary>
        /// Occurs when [host discovered].
        /// </summary>
        event HostDiscovered HostDiscovered;

        /// <summary>
        /// Occurs when [message received].
        /// </summary>
        event MessageReceived MessageReceived;

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

        /// <summary>
        /// Discovers the hosts.
        /// </summary>
        /// <param name="port">The port.</param>
        void DiscoverHosts(int port);

        /// <summary>
        /// Connects the specified host.
        /// </summary>
        /// <param name="host">The host.</param>
        void Connect(Host host);

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Sends the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="deliveryMethod">The delivery method.</param>
        void Send(OutgoingMessage message, DeliveryMethod deliveryMethod);

        /// <summary>
        /// Creates a new outgoing message.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The create outgoing message.</returns>
        OutgoingMessage CreateMessage(MessageType type = MessageType.Data);
    }
}