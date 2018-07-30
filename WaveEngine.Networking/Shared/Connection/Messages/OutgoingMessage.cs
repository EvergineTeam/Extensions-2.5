// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using Lidgren.Network;
using System;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
#endregion

namespace WaveEngine.Networking.Connection.Messages
{
    /// <summary>
    /// This class represent an outgoing message.
    /// </summary>
    public class OutgoingMessage
    {
        /// <summary>
        /// The message.
        /// </summary>
        internal readonly NetOutgoingMessage Message;

        #region Properties

        /// <summary>
        /// Gets the length of the used portion of the buffer in bytes
        /// </summary>
        public int LengthBytes
        {
            get
            {
                return this.Message.LengthBytes;
            }
        }

        /// <summary>
        /// Gets the length of the used portion of the buffer in bits
        /// </summary>
        public int LengthBits
        {
            get
            {
                return this.Message.LengthBits;
            }
        }

        /// <summary>
        /// Gets the message type.
        /// </summary>
        /// <value>
        /// The message type.
        /// </value>
        internal MessageType Type { get; private set; }

        /// <summary>
        /// Gets the inner NetOutgoingMessage
        /// </summary>
        public object InnerMessage
        {
            get
            {
                return this.Message;
            }
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="OutgoingMessage"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public OutgoingMessage(NetOutgoingMessage message)
            : this(message, MessageType.Data)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="OutgoingMessage"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="type">The type.</param>
        internal OutgoingMessage(NetOutgoingMessage message, MessageType type)
        {
            this.Message = message;
            this.Type = type;

            this.Write((byte)this.Type);
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
        /// Writes the specified byte (or bits).
        /// </summary>
        /// <param name="source">The value</param>
        /// <param name="numberOfBits">The number of bits that should be read. By default 8 bits.</param>
        /// <exception cref="ArgumentException">ReadByte(bits) can only read between 1 and 8 bits.</exception>
        public void Write(byte source, int numberOfBits = 8)
        {
            if (numberOfBits <= 0 || numberOfBits > 8)
            {
                throw new ArgumentException("Write(byte, numberOfBits) can only read between 1 and 8 bits", nameof(numberOfBits));
            }

            this.Message.Write(source, numberOfBits);
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
        public void Write(short value)
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
        public void Write(long value)
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
        /// Writes all bytes in an array, preceded with the length as an integer.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Write(byte[] value)
        {
            this.Write(value, 0, value.Length);
        }

        /// <summary>
        /// Writes the specified number of bytes from an array, preceded with the length as an integer.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="offsetInBytes">Offset in bytes.</param>
        /// <param name="numberOfBytes">Numbers of bytes to write.</param>
        public void Write(byte[] value, int offsetInBytes, int numberOfBytes)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (offsetInBytes < 0 ||
                numberOfBytes < 0)
            {
                throw new ArgumentOutOfRangeException($"{nameof(offsetInBytes)}, or {nameof(numberOfBytes)} is less than 0");
            }

            if (value.Length < (offsetInBytes + numberOfBytes))
            {
                throw new ArgumentException($"The number of bytes in {nameof(value)} is less than {nameof(offsetInBytes)} plus {nameof(numberOfBytes)}");
            }

            var lenght = numberOfBytes - offsetInBytes;
            this.Write(lenght);
            this.Message.Write(value, offsetInBytes, numberOfBytes);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Write(Vector2 value)
        {
            this.Message.Write(value);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Write(Vector3 value)
        {
            this.Message.Write(value);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Write(Vector4 value)
        {
            this.Message.Write(value);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Write(Color value)
        {
            this.Message.Write(value);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="highPrecision">Indicates whether the <see cref="TimeSpan"/> will be written using high precision (ticks) or not (milliseconds)</param>
        /// <remarks>
        /// High precision allows full <see cref="TimeSpan"/> range, otherwise it will be limited to "+-24.20:31:23.647" (+-24 days, 20 hours, 31 minutes, 23 seconds and 647 milliseconds).
        /// </remarks>
        public void Write(TimeSpan value, bool highPrecision)
        {
            this.Message.Write(value, highPrecision);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Write(DateTime value)
        {
            this.Message.Write(value);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Write(Quaternion value)
        {
            this.Message.Write(value);
        }

        /// <summary>
        /// Writes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Write(Matrix value)
        {
            this.Message.Write(value);
        }

        /// <summary>
        /// Append all the bits of message to this message
        /// </summary>
        /// <param name="message">The message</param>
        public void Write(OutgoingMessage message)
        {
            var messageData = message.Message.Data;
            this.Message.Write(messageData, sizeof(MessageType), messageData.Length - sizeof(MessageType));
        }

        #endregion
    }
}
