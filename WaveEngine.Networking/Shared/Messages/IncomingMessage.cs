#region File Description
//-----------------------------------------------------------------------------
// IncomingMessage
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

using Lidgren.Network;

namespace WaveEngine.Networking.Messages
{
    /// <summary>
    /// This class represent an incoming message.
    /// </summary>
    public struct IncomingMessage
    {
        /// <summary>
        /// The message.
        /// </summary>
        internal readonly NetIncomingMessage Message;

        /// <summary>
        /// Gets the message type.
        /// </summary>
        /// <value>
        /// The message type.
        /// </value>
        public MessageType Type { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IncomingMessage"/> struct.
        /// </summary>
        /// <param name="message">The message.</param>
        public IncomingMessage(NetIncomingMessage message)
            : this()
        {
            this.Message = message;

            this.Type = (MessageType)this.ReadInt32();
        }

        /// <summary>
        /// Reads the next string.
        /// </summary>
        /// <returns>The next string in the message.</returns>
        public string ReadString()
        {
            return this.Message.ReadString();
        }

        /// <summary>
        /// Reads the bytes of length of next int.
        /// </summary>
        /// <returns>The next byte array.</returns>
        public byte[] ReadBytes()
        {
            var size = this.ReadInt32();
            return this.Message.ReadBytes(size);
        }

        /// <summary>
        /// Reads the next boolean.
        /// </summary>
        /// <returns>The next boolean in the message.</returns>
        public bool ReadBoolean()
        {
            return this.Message.ReadBoolean();
        }

        /// <summary>
        /// Reads the next int32.
        /// </summary>
        /// <returns>The next int32 in the message.</returns>
        public int ReadInt32()
        {
            return this.Message.ReadInt32();
        }

        /// <summary>
        /// Reads the next single.
        /// </summary>
        /// <returns>The next single in the message.</returns>
        public float ReadSingle()
        {
            return this.Message.ReadSingle();
        }

        /// <summary>
        /// Gets the message data.
        /// </summary>
        /// <returns>The message byte[].</returns>
        public byte[] GetData()
        {
            return this.Message.Data;
        }

        /// <summary>
        /// Seeks the specified offset bytes.
        /// </summary>
        /// <param name="offsetBytes">The offset bytes.</param>
        public void Seek(int offsetBytes)
        {
            this.Message.Position = 32 + (offsetBytes / 8);
        }
    }
}