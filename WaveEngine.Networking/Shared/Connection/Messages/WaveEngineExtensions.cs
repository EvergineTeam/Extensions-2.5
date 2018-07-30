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
    /// Extension methods to read and write Wave Engine types in or from a <see cref="NetBuffer"/>.
    /// </summary>
    public static class WaveEngineExtensions
    {
        #region Public Methods

        /// <summary>
        /// Write a <see cref="Vector2"/> to the specified <see cref="NetBuffer"/>.
        /// </summary>
        /// <param name="message">The buffer to write to</param>
        /// <param name="value">The <see cref="Vector2"/> value</param>
        public static void Write(this NetBuffer message, Vector2 value)
        {
            message.Write(value.X);
            message.Write(value.Y);
        }

        /// <summary>
        /// Reads a <see cref="Vector2"/> value from the <see cref="NetBuffer"/>.
        /// </summary>
        /// <param name="message">The buffer to read from</param>
        /// <returns>A <see cref="Vector2"/> value</returns>
        public static Vector2 ReadVector2(this NetBuffer message)
        {
            return new Vector2(message.ReadFloat(), message.ReadFloat());
        }

        /// <summary>
        /// Write a <see cref="Vector3"/> to the specified <see cref="NetBuffer"/>.
        /// </summary>
        /// <param name="message">The buffer to write to</param>
        /// <param name="value">The <see cref="Vector3"/> value</param>
        public static void Write(this NetBuffer message, Vector3 value)
        {
            message.Write(value.X);
            message.Write(value.Y);
            message.Write(value.Z);
        }

        /// <summary>
        /// Reads a <see cref="Vector3"/> value from the <see cref="NetBuffer"/>.
        /// </summary>
        /// <param name="message">The buffer to read from</param>
        /// <returns>A <see cref="Vector3"/> value</returns>
        public static Vector3 ReadVector3(this NetBuffer message)
        {
            return new Vector3(message.ReadFloat(), message.ReadFloat(), message.ReadFloat());
        }

        /// <summary>
        /// Write a <see cref="Vector4"/> to the specified <see cref="NetBuffer"/>.
        /// </summary>
        /// <param name="message">The buffer to write to</param>
        /// <param name="value">The <see cref="Vector4"/> value</param>
        public static void Write(this NetBuffer message, Vector4 value)
        {
            message.Write(value.X);
            message.Write(value.Y);
            message.Write(value.Z);
            message.Write(value.W);
        }

        /// <summary>
        /// Reads a <see cref="Vector4"/> value from the <see cref="NetBuffer"/>.
        /// </summary>
        /// <param name="message">The buffer to read from</param>
        /// <returns>A <see cref="Vector4"/> value</returns>
        public static Vector4 ReadVector4(this NetBuffer message)
        {
            return new Vector4(message.ReadFloat(), message.ReadFloat(), message.ReadFloat(), message.ReadFloat());
        }

        /// <summary>
        /// Write a <see cref="Color"/> to the specified <see cref="NetBuffer"/>.
        /// </summary>
        /// <param name="message">The buffer to write to</param>
        /// <param name="value">The <see cref="Color"/> value</param>
        public static void Write(this NetBuffer message, Color value)
        {
            message.Write(value.R);
            message.Write(value.G);
            message.Write(value.B);
            message.Write(value.A);
        }

        /// <summary>
        /// Reads a <see cref="Color"/> value from the <see cref="NetBuffer"/>.
        /// </summary>
        /// <param name="message">The buffer to write to</param>
        /// <returns>A <see cref="Color"/> value</returns>
        public static Color ReadColor(this NetBuffer message)
        {
            return new Color(message.ReadByte(), message.ReadByte(), message.ReadByte(), message.ReadByte());
        }

        /// <summary>
        /// Write a <see cref="TimeSpan"/> to the specified <see cref="NetBuffer"/>.
        /// </summary>
        /// <param name="message">The buffer to write to</param>
        /// <param name="value">The <see cref="TimeSpan"/> value</param>
        /// <param name="highPrecision">
        /// Indicates whether the <see cref="TimeSpan"/> will be written using high precision (ticks)
        /// or not (milliseconds)
        /// </param>
        /// <remarks>
        /// High precision allows full <see cref="TimeSpan"/> range, otherwise it will be limited to "+-24.20:31:23.647"
        /// (+-24 days, 20 hours, 31 minutes, 23 seconds and 647 milliseconds)
        /// </remarks>
        public static void Write(this NetBuffer message, TimeSpan value, bool highPrecision)
        {
            if (highPrecision)
            {
                message.Write(value.Ticks);
            }
            else
            {
                message.Write((int)value.TotalMilliseconds);
            }
        }

        /// <summary>
        /// Reads a <see cref="TimeSpan"/> value from the <see cref="NetBuffer"/>.
        /// </summary>
        /// <param name="message">The buffer to write to</param>
        /// <param name="highPrecision">
        /// Indicates whether the <see cref="TimeSpan"/> will be read using high precision (ticks) or not (milliseconds)
        /// </param>
        /// <remarks>
        /// High precision allows full <see cref="TimeSpan"/> range, otherwise it will be limited to "+-24.20:31:23.647"
        /// (+-24 days, 20 hours, 31 minutes, 23 seconds and 647 milliseconds)
        /// </remarks>
        /// <returns>A <see cref="TimeSpan"/> value</returns>
        public static TimeSpan ReadTimeSpan(this NetBuffer message, bool highPrecision)
        {
            if (highPrecision)
            {
                return TimeSpan.FromTicks(message.ReadInt64());
            }
            else
            {
                return TimeSpan.FromMilliseconds(message.ReadInt32());
            }
        }

        /// <summary>
        /// Write a <see cref="DateTime"/> to the specified <see cref="NetBuffer"/> using a 64 bits data.
        /// </summary>
        /// <param name="message">The buffer to write to</param>
        /// <param name="value">The <see cref="DateTime"/> value</param>
        public static void Write(this NetBuffer message, DateTime value)
        {
            message.Write(value.ToBinary());
        }

        /// <summary>
        /// Reads a <see cref="DateTime"/> value from the <see cref="NetBuffer"/> using a 64 bits data.
        /// </summary>
        /// <param name="message">The buffer to write to</param>
        /// <returns>A <see cref="DateTime"/> value</returns>
        public static DateTime ReadDateTime(this NetBuffer message)
        {
            return DateTime.FromBinary(message.ReadInt64());
        }

        /// <summary>
        /// Write a <see cref="Quaternion"/> to the specified <see cref="NetBuffer"/>.
        /// </summary>
        /// <param name="message">The buffer to write to</param>
        /// <param name="value">The <see cref="Quaternion"/> value</param>
        public static void Write(this NetBuffer message, Quaternion value)
        {
            message.Write(value.X);
            message.Write(value.Y);
            message.Write(value.Z);
            message.Write(value.W);
        }

        /// <summary>
        /// Reads a <see cref="Quaternion"/> value from the <see cref="NetBuffer"/>.
        /// </summary>
        /// <param name="message">The buffer to write to</param>
        /// <returns>A <see cref="Quaternion"/> value</returns>
        public static Quaternion ReadQuaternion(this NetBuffer message)
        {
            return new Quaternion(message.ReadFloat(), message.ReadFloat(), message.ReadFloat(), message.ReadFloat());
        }

        /// <summary>
        /// Write a <see cref="Matrix"/> to the specified <see cref="NetBuffer"/>.
        /// </summary>
        /// <param name="message">The buffer to write to</param>
        /// <param name="value">The <see cref="Matrix"/> value</param>
        public static void Write(this NetBuffer message, Matrix value)
        {
            message.Write(value.M11);
            message.Write(value.M12);
            message.Write(value.M13);
            message.Write(value.M14);

            message.Write(value.M21);
            message.Write(value.M22);
            message.Write(value.M23);
            message.Write(value.M24);

            message.Write(value.M31);
            message.Write(value.M32);
            message.Write(value.M33);
            message.Write(value.M34);

            message.Write(value.M41);
            message.Write(value.M42);
            message.Write(value.M43);
            message.Write(value.M44);
        }

        /// <summary>
        /// Reads a <see cref="Matrix"/> value from the <see cref="NetBuffer"/>.
        /// </summary>
        /// <param name="message">The buffer to write to</param>
        /// <returns>A <see cref="Matrix"/> value</returns>
        public static Matrix ReadMatrix(this NetBuffer message)
        {
            return new Matrix(
                message.ReadFloat(),
                message.ReadFloat(),
                message.ReadFloat(),
                message.ReadFloat(),
                message.ReadFloat(),
                message.ReadFloat(),
                message.ReadFloat(),
                message.ReadFloat(),
                message.ReadFloat(),
                message.ReadFloat(),
                message.ReadFloat(),
                message.ReadFloat(),
                message.ReadFloat(),
                message.ReadFloat(),
                message.ReadFloat(),
                message.ReadFloat());
        }

        /// <summary>
        /// Write a <see cref="NetworkEndpoint"/> to the specified <see cref="NetBuffer"/>.
        /// </summary>
        /// <param name="message">The buffer to write to</param>
        /// <param name="value">The <see cref="NetworkEndpoint"/> value</param>
        public static void Write(this NetBuffer message, NetworkEndpoint value)
        {
            message.Write(value.Address);
            message.Write((ushort)value.Port);
        }

        /// <summary>
        /// Reads a <see cref="NetworkEndpoint"/> value from the <see cref="NetBuffer"/>.
        /// </summary>
        /// <param name="message">The buffer to write to</param>
        /// <returns>A <see cref="NetworkEndpoint"/> value</returns>
        public static NetworkEndpoint ReadNetworkEndpoint(this NetBuffer message)
        {
            return new NetworkEndpoint(message.ReadString(), message.ReadUInt16());
        }

        /// <summary>
        /// Gets the sender endpoint from a <see cref="NetIncomingMessage"/>.
        /// </summary>
        /// <param name="message">The incoming message</param>
        /// <returns>The sender endpoint</returns>
        internal static NetworkEndpoint GetSenderEndPoint(this NetIncomingMessage message)
        {
            var senderEndPoint = message.SenderEndPoint;

            return new NetworkEndpoint(senderEndPoint.Address.ToString(), senderEndPoint.Port);
        }

        #endregion
    }
}
