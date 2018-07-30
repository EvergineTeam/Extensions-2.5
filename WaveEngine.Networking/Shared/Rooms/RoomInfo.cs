// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using WaveEngine.Networking.Connection.Messages;
using WaveEngine.Networking.Messages;
#endregion

namespace WaveEngine.Networking.Rooms
{
    /// <summary>
    /// This class represents the info of a room in the lobby
    /// </summary>
    public class RoomInfo : IRoomInfo
    {
        /// <summary>
        /// RoomInfo field flags used for sync.
        /// </summary>
        [Flags]
        protected enum RoomInfoFieldsFlags : byte
        {
            /// <summary>
            /// No field flag
            /// </summary>
            None = 0,

            /// <summary>
            /// Name field flag
            /// </summary>
            Name = 2,

            /// <summary>
            /// Player count field flag
            /// </summary>
            PlayerCount = 4,

            /// <summary>
            /// Maximum players field flag
            /// </summary>
            MaxPlayers = 8,

            /// <summary>
            /// Lobby properties field flag
            /// </summary>
            PropertiesListedInLobby = 16,

            /// <summary>
            /// All fields flag
            /// </summary>
            All = byte.MaxValue
        }

        /// <summary>
        /// Indicates what fields need to be synchronized.
        /// </summary>
        private RoomInfoFieldsFlags pendingSyncFields;

        /// <summary>
        /// Backing field for property.
        /// </summary>
        private byte playerCount;

        /// <summary>
        /// Backing field for property.
        /// </summary>
        private byte maxPlayers;

        /// <summary>
        /// Backing field for property.
        /// </summary>
        private HashSet<string> propertiesListedInLobby;

        #region Properties

        /// <inheritdoc />
        public string Name { get; private set; }

        /// <inheritdoc />
        public byte PlayerCount
        {
            get
            {
                return this.playerCount;
            }

            internal set
            {
                if (this.playerCount != value)
                {
                    this.playerCount = value;
                    this.pendingSyncFields |= RoomInfoFieldsFlags.PlayerCount;
                }
            }
        }

        /// <inheritdoc />
        public bool IsFull
        {
            get
            {
                return this.PlayerCount >= byte.MaxValue ||
                       (this.MaxPlayers != 0 && this.PlayerCount >= this.MaxPlayers);
            }
        }

        /// <inheritdoc />
        public byte MaxPlayers
        {
            get
            {
                return this.maxPlayers;
            }

            internal set
            {
                if (this.maxPlayers != value)
                {
                    this.maxPlayers = value;
                    this.pendingSyncFields |= RoomInfoFieldsFlags.MaxPlayers;
                }
            }
        }

        /// <inheritdoc />
        public HashSet<string> PropertiesListedInLobby
        {
            get
            {
                return this.propertiesListedInLobby;
            }

            private set
            {
                if (this.propertiesListedInLobby != value)
                {
                    this.propertiesListedInLobby = value;
                    this.pendingSyncFields |= RoomInfoFieldsFlags.PropertiesListedInLobby;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the local room needs to be sync or not.
        /// </summary>
        internal bool NeedSync
        {
            get
            {
                return this.pendingSyncFields != RoomInfoFieldsFlags.None;
            }
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomInfo" /> class.
        /// </summary>
        internal RoomInfo()
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomInfo" /> class based on the given message.
        /// </summary>
        /// <param name="message">The received message</param>
        /// <returns>A <see cref="RoomInfo" /> instance</returns>
        internal static RoomInfo FromMessage(IncomingMessage message)
        {
            var playerRoom = new RoomInfo();
            playerRoom.ReadFromMessage(message);
            return playerRoom;
        }

        /// <summary>
        /// Refresh this instance fields based on the room options.
        /// </summary>
        /// <param name="options">The room options</param>
        internal void ReadOptions(RoomOptions options)
        {
            this.Name = options.RoomName;
            this.MaxPlayers = options.MaxPlayers;
            this.PropertiesListedInLobby = options.PropertiesListedInLobby;
        }

        /// <summary>
        /// Refresh this instance fields based on the given message.
        /// </summary>
        /// <param name="message">The received message</param>
        internal void ReadFromMessage(IncomingMessage message)
        {
            var changedFields = (RoomInfoFieldsFlags)message.ReadByte();

            if (changedFields.HasFlag(RoomInfoFieldsFlags.Name))
            {
                var roomName = message.ReadString();
                if (this.Name == null)
                {
                    this.Name = roomName;
                }
                else if (this.Name != roomName)
                {
                    throw new InvalidOperationException("Invalid incoming message");
                }
            }

            if (changedFields.HasFlag(RoomInfoFieldsFlags.PlayerCount))
            {
                this.playerCount = message.ReadByte();
            }

            if (changedFields.HasFlag(RoomInfoFieldsFlags.MaxPlayers))
            {
                this.maxPlayers = message.ReadByte();
            }

            if (changedFields.HasFlag(RoomInfoFieldsFlags.PropertiesListedInLobby))
            {
                this.PropertiesListedInLobby = new HashSet<string>(message.ReadStringArray());
            }
        }

        /// <summary>
        /// Writes this instance fields to an outgoing message.
        /// </summary>
        /// <param name="outgoingMessage">The outgoing message</param>
        internal void WriteToMessage(OutgoingMessage outgoingMessage)
        {
            this.WriteSyncMessage(outgoingMessage, RoomInfoFieldsFlags.All);
        }

        /// <summary>
        /// Writes the properties to be sync on an outgoing message.
        /// </summary>
        /// <param name="message">The outgoing message</param>
        internal void WriteSyncMessage(OutgoingMessage message)
        {
            this.WriteSyncMessage(message, this.pendingSyncFields);
            this.pendingSyncFields = RoomInfoFieldsFlags.None;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Writes the properties to be sync on an outgoing message.
        /// </summary>
        /// <param name="message">The outgoing message</param>
        /// <param name="includedFields">Indicates the fields that must synchronized</param>
        protected void WriteSyncMessage(OutgoingMessage message, RoomInfoFieldsFlags includedFields)
        {
            message.Write((byte)includedFields);

            if (includedFields.HasFlag(RoomInfoFieldsFlags.Name))
            {
                message.Write(this.Name);
            }

            if (includedFields.HasFlag(RoomInfoFieldsFlags.PlayerCount))
            {
                message.Write(this.playerCount);
            }

            if (includedFields.HasFlag(RoomInfoFieldsFlags.MaxPlayers))
            {
                message.Write(this.maxPlayers);
            }

            if (includedFields.HasFlag(RoomInfoFieldsFlags.PropertiesListedInLobby))
            {
                message.Write(this.propertiesListedInLobby);
            }
        }

        #endregion
    }
}
