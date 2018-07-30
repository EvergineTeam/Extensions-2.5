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
    /// This class represent an incoming message.
    /// </summary>
    public class IncomingMessage
    {
        /// <summary>
        /// The message.
        /// </summary>
        internal readonly NetIncomingMessage Message;

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
        /// Gets the inner NetIncomingMessage
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
        /// Initializes a new instance of the <see cref="IncomingMessage"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public IncomingMessage(NetIncomingMessage message)
        {
            this.Message = message;

            this.Type = (MessageType)this.ReadByte();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Reads the next string.
        /// </summary>
        /// <returns>The next string in the message.</returns>
        public string ReadString()
        {
            return this.Message.ReadString();
        }

        /// <summary>
        /// Reads the next byte (or bits).
        /// </summary>
        /// <param name="numberOfBits">The number of bits that should be read. By default 8 bits.</param>
        /// <exception cref="ArgumentException">ReadByte(bits) can only read between 1 and 8 bits</exception>
        /// <returns>The next byte (or bits) in the message.</returns>
        public byte ReadByte(int numberOfBits = 8)
        {
            if (numberOfBits <= 0 || numberOfBits > 8)
            {
                throw new ArgumentException("ReadByte(bits) can only read between 1 and 8 bits", nameof(numberOfBits));
            }

            return this.Message.ReadByte(numberOfBits);
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
        /// Reads the next int16.
        /// </summary>
        /// <returns>The next int16 in the message.</returns>
        public short ReadInt16()
        {
            return this.Message.ReadInt16();
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
        /// Reads the next int64.
        /// </summary>
        /// <returns>The next int64 in the message.</returns>
        public long ReadInt64()
        {
            return this.Message.ReadInt64();
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
        /// Reads the next <see cref="Vector2"/>.
        /// </summary>
        /// <returns>The next <see cref="Vector2"/> in the message.</returns>
        public Vector2 ReadVector2()
        {
            return this.Message.ReadVector2();
        }

        /// <summary>
        /// Reads the next <see cref="Vector3"/>.
        /// </summary>
        /// <returns>The next <see cref="Vector3"/> in the message.</returns>
        public Vector3 ReadVector3()
        {
            return this.Message.ReadVector3();
        }

        /// <summary>
        /// Reads the next <see cref="Vector4"/>.
        /// </summary>
        /// <returns>The next <see cref="Vector4"/> in the message.</returns>
        public Vector4 ReadVector4()
        {
            return this.Message.ReadVector4();
        }

        /// <summary>
        /// Reads the next <see cref="Color"/>.
        /// </summary>
        /// <returns>The next <see cref="Color"/> in the message.</returns>
        public Color ReadColor()
        {
            return this.Message.ReadColor();
        }

        /// <summary>
        /// Reads the next <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="highPrecision">Indicates whether the <see cref="TimeSpan"/> will be read using high precision (ticks) or not (milliseconds)</param>
        /// <remarks>
        /// High precision allows full <see cref="TimeSpan"/> range, otherwise it will be limited to "+-24.20:31:23.647" (+-24 days, 20 hours, 31 minutes, 23 seconds and 647 milliseconds).
        /// </remarks>
        /// <returns>The next <see cref="TimeSpan"/> in the message.</returns>
        public TimeSpan ReadTimeSpan(bool highPrecision)
        {
            return this.Message.ReadTimeSpan(highPrecision);
        }

        /// <summary>
        /// Reads the next <see cref="DateTime"/>.
        /// </summary>
        /// <returns>The next <see cref="DateTime"/> in the message.</returns>
        public DateTime ReadDateTime()
        {
            return this.Message.ReadDateTime();
        }

        /// <summary>
        /// Reads the next <see cref="Quaternion"/>.
        /// </summary>
        /// <returns>The next <see cref="Quaternion"/> in the message.</returns>
        public Quaternion ReadQuaternion()
        {
            return this.Message.ReadQuaternion();
        }

        /// <summary>
        /// Reads the next <see cref="Matrix"/>.
        /// </summary>
        /// <returns>The next <see cref="Matrix"/> in the message.</returns>
        public Matrix ReadMatrix()
        {
            return this.Message.ReadMatrix();
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
            this.Message.Position = (sizeof(MessageType) + offsetBytes) * 8;
        }

        #endregion
    }
}
