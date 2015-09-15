#region File Description
//-----------------------------------------------------------------------------
// INetworkServer
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

using WaveEngine.Networking.Messages;

namespace WaveEngine.Networking
{
    /// <summary>
    /// The network server interface.
    /// </summary>
    public interface INetworkServer
    {
        /// <summary>
        /// Occurs when [message received].
        /// </summary>
        event MessageReceived MessageReceived;

        /// <summary>
        /// Starts this instance.
        /// </summary>
        void Start();

        /// <summary>
        /// Shutdowns this instance.
        /// </summary>
        void Shutdown();

        /// <summary>
        /// Sends the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="deliveryMethod">The delivery method.</param>
        void Send(OutgoingMessage message, DeliveryMethod deliveryMethod);

        /// <summary>
        /// Sends the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="deliveryMethod">The delivery method.</param>
        void Send(IncomingMessage message, DeliveryMethod deliveryMethod);

        /// <summary>
        /// Creates a new outgoing message.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The created outgoing message.</returns>
        OutgoingMessage CreateMessage(MessageType type = MessageType.Data);
    }
}