// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using Lidgren.Network;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Networking.Connection.Messages;
#endregion

namespace WaveEngine.Networking
{
    /// <summary>
    /// This class represents a properties table used for custom properties of network players and rooms.
    /// </summary>
    public class NetworkPropertiesTable
    {
        private ConcurrentDictionary<byte, bool> changedKeys;

        private ConcurrentDictionary<byte, NetBuffer> internalDictionary;

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the properties of this table are read only.
        /// </summary>
        public bool IsReadOnly
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether this properties table needs to be sync or not.
        /// </summary>
        public bool NeedSync
        {
            get
            {
                return this.changedKeys.Count > 0;
            }
        }

        /// <summary>
        /// Gets a enumerable that contains the existing keys in the properties table.
        /// </summary>
        public IEnumerable<byte> Keys
        {
            get
            {
                return this.internalDictionary.Keys;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a property in the properties table is added by a remote player.
        /// </summary>
        public event EventHandler<byte> PropertyAdded;

        /// <summary>
        /// Occurs when a property in the properties table is changed by a remote player.
        /// </summary>
        public event EventHandler<byte> PropertyChanged;

        /// <summary>
        /// Occurs when a property in the properties table is removed by a remote player.
        /// </summary>
        public event EventHandler<byte> PropertyRemoved;

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkPropertiesTable" /> class.
        /// </summary>
        /// <param name="isReadOnly">Indicates if the table will be read only</param>
        internal NetworkPropertiesTable(bool isReadOnly)
        {
            this.changedKeys = new ConcurrentDictionary<byte, bool>();
            this.internalDictionary = new ConcurrentDictionary<byte, NetBuffer>();
            this.IsReadOnly = isReadOnly;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Set a boolean value for the specified key in the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <param name="value">The boolean value</param>
        public void Set(byte key, bool value)
        {
            var buffer = new NetBuffer();
            buffer.Write(value);
            this.InternalSetBuffer(key, buffer);
        }

        /// <summary>
        /// Gets a boolean value from the specified key from the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <returns>A boolean value</returns>
        public bool GetBoolean(byte key)
        {
            var buffer = this.InternalGetBuffer(key);
            return buffer.ReadBoolean();
        }

        /// <summary>
        /// Gets a boolean value from the specified key from the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <param name="value">
        /// The value associated with the specified key, if the key exists; otherwise, the predefined value for the
        /// parameter type
        /// </param>
        /// <returns>
        /// <c>true</c> if the table contains a property for the specified key; otherwise, <c>false</c>
        /// </returns>
        public bool TryGetBoolean(byte key, out bool value)
        {
            NetBuffer buffer;
            bool exists = this.InternalTryGetBuffer(key, out buffer);
            value = default(bool);

            if (exists)
            {
                value = buffer.ReadBoolean();
            }

            return exists;
        }

        /// <summary>
        /// Set a byte value for the specified key in the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <param name="value">The byte value</param>
        public void Set(byte key, byte value)
        {
            var buffer = new NetBuffer();
            buffer.Write(value);
            this.InternalSetBuffer(key, buffer);
        }

        /// <summary>
        /// Gets a byte value from the specified key from the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <returns>A byte value</returns>
        public byte GetByte(byte key)
        {
            var buffer = this.InternalGetBuffer(key);
            return buffer.ReadByte();
        }

        /// <summary>
        /// Gets a byte value from the specified key from the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <param name="value">
        /// The value associated with the specified key, if the key exists; otherwise, the predefined value for the
        /// parameter type
        /// </param>
        /// <returns>
        /// <c>true</c> if the table contains a property for the specified key; otherwise, <c>false</c>
        /// </returns>
        public bool TryGetByte(byte key, out byte value)
        {
            NetBuffer buffer;
            bool exists = this.InternalTryGetBuffer(key, out buffer);
            value = default(byte);

            if (exists)
            {
                value = buffer.ReadByte();
            }

            return exists;
        }

        /// <summary>
        /// Set a integer value for the specified key in the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <param name="value">The integer value</param>
        public void Set(byte key, int value)
        {
            var buffer = new NetBuffer();
            buffer.Write(value);
            this.InternalSetBuffer(key, buffer);
        }

        /// <summary>
        /// Gets a integer value from the specified key from the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <returns>A integer value</returns>
        public int GetInt32(byte key)
        {
            var buffer = this.InternalGetBuffer(key);
            return buffer.ReadInt32();
        }

        /// <summary>
        /// Gets a integer value from the specified key from the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <param name="value">
        /// The value associated with the specified key, if the key exists; otherwise, the predefined value for the
        /// parameter type
        /// </param>
        /// <returns>
        /// <c>true</c> if the table contains a property for the specified key; otherwise, <c>false</c>
        /// </returns>
        public bool TryGetInt32(byte key, out int value)
        {
            NetBuffer buffer;
            bool exists = this.InternalTryGetBuffer(key, out buffer);
            value = default(int);

            if (exists)
            {
                value = buffer.ReadInt32();
            }

            return exists;
        }

        /// <summary>
        /// Set a long integer value for the specified key in the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <param name="value">The long integer value</param>
        public void Set(byte key, long value)
        {
            var buffer = new NetBuffer();
            buffer.Write(value);
            this.InternalSetBuffer(key, buffer);
        }

        /// <summary>
        /// Gets a long value from the specified key from the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <returns>A long value</returns>
        public long GetInt64(byte key)
        {
            var buffer = this.InternalGetBuffer(key);
            return buffer.ReadInt64();
        }

        /// <summary>
        /// Gets a long value from the specified key from the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <param name="value">
        /// The value associated with the specified key, if the key exists; otherwise, the predefined value for the
        /// parameter type
        /// </param>
        /// <returns>
        /// <c>true</c> if the table contains a property for the specified key; otherwise, <c>false</c>
        /// </returns>
        public bool TryGetInt64(byte key, out long value)
        {
            NetBuffer buffer;
            bool exists = this.InternalTryGetBuffer(key, out buffer);
            value = default(long);

            if (exists)
            {
                value = buffer.ReadInt64();
            }

            return exists;
        }

        /// <summary>
        /// Set a float value for the specified key in the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <param name="value">The float value</param>
        public void Set(byte key, float value)
        {
            var buffer = new NetBuffer();
            buffer.Write(value);
            this.InternalSetBuffer(key, buffer);
        }

        /// <summary>
        /// Gets a float value from the specified key from the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <returns>A float value</returns>
        public float GetFloat(byte key)
        {
            var buffer = this.InternalGetBuffer(key);
            return buffer.ReadFloat();
        }

        /// <summary>
        /// Gets a float value from the specified key from the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <param name="value">
        /// The value associated with the specified key, if the key exists; otherwise, the predefined value for the
        /// parameter type
        /// </param>
        /// <returns>
        /// <c>true</c> if the table contains a property for the specified key; otherwise, <c>false</c>
        /// </returns>
        public bool TryGetFloat(byte key, out float value)
        {
            NetBuffer buffer;
            bool exists = this.InternalTryGetBuffer(key, out buffer);
            value = default(float);

            if (exists)
            {
                value = buffer.ReadFloat();
            }

            return exists;
        }

        /// <summary>
        /// Set a string value for the specified key in the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <param name="value">The string value</param>
        public void Set(byte key, string value)
        {
            var buffer = new NetBuffer();
            buffer.Write(value);
            this.InternalSetBuffer(key, buffer);
        }

        /// <summary>
        /// Gets a string value from the specified key from the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <returns>A string value</returns>
        public string GetString(byte key)
        {
            var buffer = this.InternalGetBuffer(key);
            return buffer.ReadString();
        }

        /// <summary>
        /// Gets a string value from the specified key from the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <param name="value">
        /// The value associated with the specified key, if the key exists; otherwise, the predefined value for the
        /// parameter type
        /// </param>
        /// <returns>
        /// <c>true</c> if the table contains a property for the specified key; otherwise, <c>false</c>
        /// </returns>
        public bool TryGetString(byte key, out string value)
        {
            NetBuffer buffer;
            bool exists = this.InternalTryGetBuffer(key, out buffer);
            value = default(string);

            if (exists)
            {
                value = buffer.ReadString();
            }

            return exists;
        }

        /// <summary>
        /// Set a byte array for the specified key in the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <param name="value">The byte array</param>
        public void Set(byte key, byte[] value)
        {
            var buffer = new NetBuffer();
            buffer.Write(value);
            this.InternalSetBuffer(key, buffer);
        }

        /// <summary>
        /// Gets a byte array from the specified key from the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <returns>A byte array</returns>
        public byte[] GetBytes(byte key)
        {
            var buffer = this.InternalGetBuffer(key);
            return buffer.ReadBytes(buffer.LengthBytes);
        }

        /// <summary>
        /// Gets a byte array from the specified key from the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <param name="value">
        /// The byte array associated with the specified key, if the key exists; otherwise, the predefined value for
        /// the parameter type
        /// </param>
        /// <returns>
        /// <c>true</c> if the table contains a property for the specified key; otherwise, <c>false</c>
        /// </returns>
        public bool TryGetBytes(byte key, out byte[] value)
        {
            NetBuffer buffer;
            bool exists = this.InternalTryGetBuffer(key, out buffer);
            value = default(byte[]);

            if (exists)
            {
                value = buffer.ReadBytes(buffer.LengthBytes);
            }

            return exists;
        }

        /// <summary>
        /// Set a <see cref="Vector2"/> value for the specified key in the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <param name="value">The <see cref="Vector2"/> value</param>
        public void Set(byte key, Vector2 value)
        {
            var buffer = new NetBuffer();
            buffer.Write(value);
            this.InternalSetBuffer(key, buffer);
        }

        /// <summary>
        /// Gets a <see cref="Vector2"/> value from the specified key from the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <returns>A <see cref="Vector2"/> value</returns>
        public Vector2 GetVector2(byte key)
        {
            var buffer = this.InternalGetBuffer(key);
            return buffer.ReadVector2();
        }

        /// <summary>
        /// Gets a <see cref="Vector2"/> value from the specified key from the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <param name="value">
        /// The value associated with the specified key, if the key exists; otherwise, the predefined value for the
        /// parameter type
        /// </param>
        /// <returns>
        /// <c>true</c> if the table contains a property for the specified key; otherwise, <c>false</c>
        /// </returns>
        public bool TryGetVector2(byte key, out Vector2 value)
        {
            NetBuffer buffer;
            bool exists = this.InternalTryGetBuffer(key, out buffer);
            value = default(Vector2);

            if (exists)
            {
                value = buffer.ReadVector2();
            }

            return exists;
        }

        /// <summary>
        /// Set a <see cref="Vector3"/> value for the specified key in the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <param name="value">The <see cref="Vector3"/> value</param>
        public void Set(byte key, Vector3 value)
        {
            var buffer = new NetBuffer();
            buffer.Write(value);
            this.InternalSetBuffer(key, buffer);
        }

        /// <summary>
        /// Gets a <see cref="Vector3"/> value from the specified key from the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <returns>A <see cref="Vector3"/> value</returns>
        public Vector3 GetVector3(byte key)
        {
            var buffer = this.InternalGetBuffer(key);
            return buffer.ReadVector3();
        }

        /// <summary>
        /// Gets a <see cref="Vector3"/> value from the specified key from the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <param name="value">
        /// The value associated with the specified key, if the key exists; otherwise, the predefined value for the
        /// parameter type
        /// </param>
        /// <returns>
        /// <c>true</c> if the table contains a property for the specified key; otherwise, <c>false</c>
        /// </returns>
        public bool TryGetVector3(byte key, out Vector3 value)
        {
            NetBuffer buffer;
            bool exists = this.InternalTryGetBuffer(key, out buffer);
            value = default(Vector3);

            if (exists)
            {
                value = buffer.ReadVector3();
            }

            return exists;
        }

        /// <summary>
        /// Set a <see cref="Vector4"/> value for the specified key in the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <param name="value">The <see cref="Vector4"/> value</param>
        public void Set(byte key, Vector4 value)
        {
            var buffer = new NetBuffer();
            buffer.Write(value);
            this.InternalSetBuffer(key, buffer);
        }

        /// <summary>
        /// Gets a <see cref="Vector4"/> value from the specified key from the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <returns>A <see cref="Vector4"/> value</returns>
        public Vector4 GetVector4(byte key)
        {
            var buffer = this.InternalGetBuffer(key);
            return buffer.ReadVector4();
        }

        /// <summary>
        /// Gets a <see cref="Vector4"/> value from the specified key from the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <param name="value">
        /// The value associated with the specified key, if the key exists; otherwise, the predefined value for the
        /// parameter type
        /// </param>
        /// <returns>
        /// <c>true</c> if the table contains a property for the specified key; otherwise, <c>false</c>
        /// </returns>
        public bool TryGetVector4(byte key, out Vector4 value)
        {
            NetBuffer buffer;
            bool exists = this.InternalTryGetBuffer(key, out buffer);
            value = default(Vector4);

            if (exists)
            {
                value = buffer.ReadVector4();
            }

            return exists;
        }

        /// <summary>
        /// Set a <see cref="Color"/> value for the specified key in the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <param name="value">The <see cref="Color"/> value</param>
        public void Set(byte key, Color value)
        {
            var buffer = new NetBuffer();
            buffer.Write(value);
            this.InternalSetBuffer(key, buffer);
        }

        /// <summary>
        /// Gets a <see cref="Color"/> value from the specified key from the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <returns>A <see cref="Color"/> value</returns>
        public Color GetColor(byte key)
        {
            var buffer = this.InternalGetBuffer(key);
            return buffer.ReadColor();
        }

        /// <summary>
        /// Gets a <see cref="Color"/> value from the specified key from the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <param name="value">
        /// The value associated with the specified key, if the key exists; otherwise, the predefined value for the
        /// parameter type
        /// </param>
        /// <returns>
        /// <c>true</c> if the table contains a property for the specified key; otherwise, <c>false</c>
        /// </returns>
        public bool TryGetColor(byte key, out Color value)
        {
            NetBuffer buffer;
            bool exists = this.InternalTryGetBuffer(key, out buffer);
            value = default(Color);

            if (exists)
            {
                value = buffer.ReadColor();
            }

            return exists;
        }

        /// <summary>
        /// Set a <see cref="TimeSpan"/> value for the specified key in the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <param name="value">The <see cref="TimeSpan"/> value</param>
        /// <param name="highPrecision">
        /// Indicates whether the <see cref="TimeSpan"/> will be written using high precision (ticks)
        /// or not (milliseconds)
        /// </param>
        /// <remarks>
        /// High precision allows full <see cref="TimeSpan"/> range, otherwise it will be limited to
        /// "+-24.20:31:23.647" (+-24 days, 20 hours, 31 minutes, 23 seconds and 647 milliseconds).
        /// </remarks>
        public void Set(byte key, TimeSpan value, bool highPrecision)
        {
            var buffer = new NetBuffer();
            buffer.Write(value, highPrecision);
            this.InternalSetBuffer(key, buffer);
        }

        /// <summary>
        /// Gets a <see cref="TimeSpan"/> value from the specified key from the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <param name="highPrecision">
        /// Indicates whether the <see cref="TimeSpan"/> will be read using high precision (ticks)
        /// or not (milliseconds)
        /// </param>
        /// <remarks>
        /// High precision allows full <see cref="TimeSpan"/> range, otherwise it will be limited to
        /// "+-24.20:31:23.647" (+-24 days, 20 hours, 31 minutes, 23 seconds and 647 milliseconds)
        /// </remarks>
        /// <returns>A <see cref="TimeSpan"/> value</returns>
        public TimeSpan GetTimeSpan(byte key, bool highPrecision)
        {
            var buffer = this.InternalGetBuffer(key);
            return buffer.ReadTimeSpan(highPrecision);
        }

        /// <summary>
        /// Gets a <see cref="TimeSpan"/> value from the specified key from the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <param name="highPrecision">
        /// Indicates whether the <see cref="TimeSpan"/> will be read using high precision (ticks)
        /// or not (milliseconds)
        /// </param>
        /// <param name="value">
        /// The value associated with the specified key, if the key exists; otherwise, the predefined value for the
        /// parameter type
        /// </param>
        /// <remarks>
        /// High precision allows full <see cref="TimeSpan"/> range, otherwise it will be limited to
        /// "+-24.20:31:23.647" (+-24 days, 20 hours, 31 minutes, 23 seconds and 647 milliseconds).
        /// </remarks>
        /// <returns>
        /// <c>true</c> if the table contains a property for the specified key; otherwise, <c>false</c>.
        /// </returns>
        public bool TryGetTimeSpan(byte key, bool highPrecision, out TimeSpan value)
        {
            NetBuffer buffer;
            bool exists = this.InternalTryGetBuffer(key, out buffer);
            value = default(TimeSpan);

            if (exists)
            {
                value = buffer.ReadTimeSpan(highPrecision);
            }

            return exists;
        }

        /// <summary>
        /// Set a <see cref="DateTime"/> value for the specified key in the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <param name="value">The <see cref="DateTime"/> value</param>
        public void Set(byte key, DateTime value)
        {
            var buffer = new NetBuffer();
            buffer.Write(value);
            this.InternalSetBuffer(key, buffer);
        }

        /// <summary>
        /// Gets a <see cref="DateTime"/> value from the specified key from the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <returns>A <see cref="DateTime"/> value</returns>
        public DateTime GetDateTime(byte key)
        {
            var buffer = this.InternalGetBuffer(key);
            return buffer.ReadDateTime();
        }

        /// <summary>
        /// Gets a <see cref="DateTime"/> value from the specified key from the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <param name="value">
        /// The value associated with the specified key, if the key exists; otherwise, the predefined value for the
        /// parameter type
        /// </param>
        /// <returns>
        /// <c>true</c> if the table contains a property for the specified key; otherwise, <c>false</c>
        /// </returns>
        public bool TryGetDateTime(byte key, out DateTime value)
        {
            NetBuffer buffer;
            bool exists = this.InternalTryGetBuffer(key, out buffer);
            value = default(DateTime);

            if (exists)
            {
                value = buffer.ReadDateTime();
            }

            return exists;
        }

        /// <summary>
        /// Set a <see cref="Quaternion"/> value for the specified key in the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <param name="value">The <see cref="Quaternion"/> value</param>
        public void Set(byte key, Quaternion value)
        {
            var buffer = new NetBuffer();
            buffer.Write(value);
            this.InternalSetBuffer(key, buffer);
        }

        /// <summary>
        /// Gets a <see cref="Quaternion"/> value from the specified key from the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <returns>A <see cref="Quaternion"/> value</returns>
        public Quaternion GetQuaternion(byte key)
        {
            var buffer = this.InternalGetBuffer(key);
            return buffer.ReadQuaternion();
        }

        /// <summary>
        /// Gets a <see cref="Quaternion"/> value from the specified key from the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <param name="value">
        /// The value associated with the specified key, if the key exists; otherwise, the predefined value for the
        /// parameter type
        /// </param>
        /// <returns>
        /// <c>true</c> if the table contains a property for the specified key; otherwise, <c>false</c>
        /// </returns>
        public bool TryGetQuaternion(byte key, out Quaternion value)
        {
            NetBuffer buffer;
            bool exists = this.InternalTryGetBuffer(key, out buffer);
            value = default(Quaternion);

            if (exists)
            {
                value = buffer.ReadQuaternion();
            }

            return exists;
        }

        /// <summary>
        /// Set a <see cref="Matrix"/> value for the specified key in the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <param name="value">The <see cref="Matrix"/> value</param>
        public void Set(byte key, Matrix value)
        {
            var buffer = new NetBuffer();
            buffer.Write(value);
            this.InternalSetBuffer(key, buffer);
        }

        /// <summary>
        /// Gets a <see cref="Matrix"/> value from the specified key from the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <returns>A <see cref="Matrix"/> value</returns>
        public Matrix GetMatrix(byte key)
        {
            var buffer = this.InternalGetBuffer(key);
            return buffer.ReadMatrix();
        }

        /// <summary>
        /// Gets a <see cref="Matrix"/> value from the specified key from the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <param name="value">
        /// The value associated with the specified key, if the key exists; otherwise, the predefined value for the
        /// parameter type
        /// </param>
        /// <returns>
        /// <c>true</c> if the table contains a property for the specified key; otherwise, <c>false</c>
        /// </returns>
        public bool TryGetMatrix(byte key, out Matrix value)
        {
            NetBuffer buffer;
            bool exists = this.InternalTryGetBuffer(key, out buffer);
            value = default(Matrix);

            if (exists)
            {
                value = buffer.ReadMatrix();
            }

            return exists;
        }

        /// <summary>
        /// Set a <see cref="NetworkEndpoint"/> value for the specified key in the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <param name="value">The <see cref="NetworkEndpoint"/> value</param>
        public void Set(byte key, NetworkEndpoint value)
        {
            var buffer = new NetBuffer();
            buffer.Write(value);
            this.InternalSetBuffer(key, buffer);
        }

        /// <summary>
        /// Gets a <see cref="NetworkEndpoint"/> value from the specified key from the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <returns>A <see cref="NetworkEndpoint"/> value</returns>
        public NetworkEndpoint GetNetworkEndpoint(byte key)
        {
            var buffer = this.InternalGetBuffer(key);
            return buffer.ReadNetworkEndpoint();
        }

        /// <summary>
        /// Gets a <see cref="NetworkEndpoint"/> value from the specified key from the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <param name="value">
        /// The value associated with the specified key, if the key exists; otherwise, the predefined value for the
        /// parameter type
        /// </param>
        /// <returns>
        /// <c>true</c> if the table contains a property for the specified key; otherwise, <c>false</c>
        /// </returns>
        public bool TryGetNetworkEndpoint(byte key, out NetworkEndpoint value)
        {
            NetBuffer buffer;
            bool exists = this.InternalTryGetBuffer(key, out buffer);
            value = default(NetworkEndpoint);

            if (exists)
            {
                value = buffer.ReadNetworkEndpoint();
            }

            return exists;
        }

        /// <summary>
        /// Gets a <see cref="INetworkSerializable"/> value from the specified key from the properties table.
        /// </summary>
        /// <typeparam name="T">An <see cref="INetworkSerializable"/> type</typeparam>
        /// <param name="key">The byte key</param>
        /// <param name="value">The <see cref="INetworkSerializable"/> value</param>
        public void Set<T>(byte key, T value)
            where T : INetworkSerializable, new()
        {
            var buffer = new NetBuffer();
            value.Write(buffer);
            this.InternalSetBuffer(key, buffer);
        }

        /// <summary>
        /// Gets a <see cref="INetworkSerializable"/> value from the specified key from the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <typeparam name="T">An <see cref="INetworkSerializable"/> type</typeparam>
        /// <returns>A <see cref="INetworkSerializable"/> value</returns>
        public T GetSerializable<T>(byte key)
            where T : INetworkSerializable, new()
        {
            var buffer = this.InternalGetBuffer(key);
            var result = new T();
            result.Read(buffer);
            return result;
        }

        /// <summary>
        /// Gets a <see cref="INetworkSerializable"/> value from the specified key from the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <param name="value">
        /// The value associated with the specified key, if the key exists; otherwise, the predefined value for the
        /// parameter type
        /// </param>
        /// <typeparam name="T">An <see cref="INetworkSerializable"/> type</typeparam>
        /// <returns>
        /// <c>true</c> if the table contains a property for the specified key; otherwise, <c>false</c>
        /// </returns>
        public bool TryGetSerializable<T>(byte key, out T value)
            where T : INetworkSerializable, new()
        {
            NetBuffer buffer;
            bool exists = this.InternalTryGetBuffer(key, out buffer);
            value = new T();

            if (exists)
            {
                value.Read(buffer);
            }

            return exists;
        }

        /// <summary>
        ///  Determines whether the properties table contains the specified key.
        /// </summary>
        /// <param name="key">The byte key to locate in the properties table</param>
        /// <returns>
        /// <c>true</c> if the properties table contains an element with the specified key; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsKey(byte key)
        {
            return this.internalDictionary.ContainsKey(key);
        }

        /// <summary>
        /// Removes the value with the specified key from the properties table.
        /// </summary>
        /// <param name="key">The byte key</param>
        /// <returns>
        /// <c>true</c> if the element is successfully found and removed; otherwise, <c>false</c>.
        /// This method returns <c>false</c> if key is not found in the properties table.
        /// </returns>
        public bool Remove(byte key)
        {
            this.CheckIsReadOnly();

            NetBuffer removedBuffer;
            var isRemoved = this.internalDictionary.TryRemove(key, out removedBuffer);

            if (isRemoved)
            {
                this.changedKeys[key] = false;
            }

            return isRemoved;
        }

        /// <summary>
        /// Clears all the existing properties in the table
        /// </summary>
        public void Clear()
        {
            this.CheckIsReadOnly();

            lock (this.changedKeys)
            {
                foreach (var key in this.internalDictionary.Keys)
                {
                    this.changedKeys[key] = false;
                }
            }

            this.internalDictionary.Clear();
        }

        /// <summary>
        /// Forces to synchronize all properties in the table
        /// </summary>
        internal void ForceFullSync()
        {
            foreach (var key in this.internalDictionary.Keys)
            {
                this.changedKeys.TryAdd(key, true);
            }
        }

        /// <summary>
        /// Refresh the properties table from the given message.
        /// </summary>
        /// <param name="message">The received message</param>
        internal void ReadFromMessage(IncomingMessage message)
        {
            var changedKeysCount = message.ReadByte();
            for (int i = 0; i < changedKeysCount; i++)
            {
                var key = message.ReadByte();
                var newValue = new NetBuffer();
                newValue.Write(message.ReadBytes());

                var isNewProperty = !this.internalDictionary.ContainsKey(key);
                this.internalDictionary[key] = newValue;

                if (isNewProperty)
                {
                    this.PropertyAdded?.Invoke(this, key);
                }
                else
                {
                    this.PropertyChanged?.Invoke(this, key);
                }
            }

            var removedKeysCount = message.ReadByte();
            for (int i = 0; i < removedKeysCount; i++)
            {
                var key = message.ReadByte();

                NetBuffer value;
                this.internalDictionary.TryRemove(key, out value);

                this.PropertyRemoved?.Invoke(this, key);
            }
        }

        /// <summary>
        /// Writes the properties to be sync on an outgoing message.
        /// </summary>
        /// <param name="message">The outgoing message</param>
        internal void WriteToMessage(OutgoingMessage message)
        {
            var addedOrChangedKeys = new List<byte>();
            var removedKeys = new List<byte>();

            lock (this.changedKeys)
            {
                foreach (var pair in this.changedKeys)
                {
                    if (pair.Value)
                    {
                        addedOrChangedKeys.Add(pair.Key);
                    }
                    else
                    {
                        removedKeys.Add(pair.Key);
                    }
                }

                this.changedKeys.Clear();
            }

            message.Write((byte)addedOrChangedKeys.Count);
            foreach (var key in addedOrChangedKeys)
            {
                NetBuffer newValue;
                if (this.internalDictionary.TryGetValue(key, out newValue))
                {
                    message.Write(key);
                    message.Write(newValue.Data);
                }
            }

            message.Write((byte)removedKeys.Count);
            foreach (var key in removedKeys)
            {
                message.Write(key);
            }
        }

        #endregion

        #region Private Methods

        private bool InternalTryGetBuffer(byte key, out NetBuffer buffer)
        {
            var exists = this.internalDictionary.TryGetValue(key, out buffer);

            if (exists)
            {
                buffer.Position = 0;
            }

            return exists;
        }

        private NetBuffer InternalGetBuffer(byte key)
        {
            var buffer = this.internalDictionary[key];
            buffer.Position = 0;
            return buffer;
        }

        private void InternalSetBuffer(byte key, NetBuffer buffer)
        {
            this.CheckIsReadOnly();

            this.internalDictionary[key] = buffer;

            this.changedKeys[key] = true;
        }

        private void CheckIsReadOnly()
        {
            if (this.IsReadOnly)
            {
                throw new InvalidOperationException("This properties table is read only and cannot be written");
            }
        }

        #endregion
    }
}
