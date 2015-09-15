#region File Description
//-----------------------------------------------------------------------------
// OutgoingMessage
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

using Lidgren.Network;

namespace WaveEngine.Networking.Messages
{
    /// <summary>
    /// This class represent an outgoing message.
    /// </summary>
    public struct OutgoingMessage
    {
        /// <summary>
        /// The message.
        /// </summary>
        internal readonly NetOutgoingMessage Message;

        /// <summary>
        /// Gets the message type.
        /// </summary>
        /// <value>
        /// The message type.
        /// </value>
        public MessageType Type { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OutgoingMessage"/> struct.
        /// </summary>
        /// <param name="message">The message.</param>
        public OutgoingMessage(NetOutgoingMessage message)
            : this(message, MessageType.Data)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OutgoingMessage"/> struct.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="type">The type.</param>
        public OutgoingMessage(NetOutgoingMessage message, MessageType type)
            : this()
        {
            this.Message = message;
            this.Type = type;

            this.Write((int)this.Type);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Write(string value)
        {
            this.Message.Write(value);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Write(bool value)
        {
            this.Message.Write(value);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Write(int value)
        {
            this.Message.Write(value);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Write(float value)
        {
            this.Message.Write(value);
        }

        /// <summary>
        /// Writes the specified byte array, preceded with the length in an int.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Write(byte[] value)
        {
            this.Write(value.Length);
            this.Message.Write(value);
        }
    }
}