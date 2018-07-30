// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using WaveEngine.Networking.Connection.Messages;
using WaveEngine.Networking.Players;
#endregion

namespace WaveEngine.Networking.Rooms
{
    /// <summary>
    /// This class represents the network room
    /// </summary>
    public abstract class BaseNetworkRoom : IRoomInfo, INetworkRoom
    {
        /// <summary>
        /// Constant that defines the player id that will be assigned to the first player that enters the room
        /// </summary>
        protected const int FirstPlayerId = 0;

        /// <summary>
        /// Internal counter that indicates the next assignable player id
        /// </summary>
        private int lastPlayerId;

        /// <summary>
        /// Internal variable that stores the previous master client id
        /// </summary>
        private int lastMasterClientId = -1;

        /// <summary>
        /// Room field flags used for sync.
        /// </summary>
        [Flags]
        protected enum RoomFieldsFlags : byte
        {
            /// <summary>
            /// No field flag
            /// </summary>
            None = 0,

            /// <summary>
            /// Is visible field flag
            /// </summary>
            IsVisible = 2,

            /// <summary>
            /// Maximum players field flag
            /// </summary>
            MaxPlayers = 4,

            /// <summary>
            /// Custom properties field flag
            /// </summary>
            CustomProperties = 8,

            /// <summary>
            /// All fields flag
            /// </summary>
            All = byte.MaxValue
        }

        /// <summary>
        /// Internal room info
        /// </summary>
        protected RoomInfo internalRoomInfo;

        /// <summary>
        /// While inside a Room, this is the list of players who are also in that room.
        /// </summary>
        private Dictionary<int, BaseNetworkPlayer> playersInRoom;

        private bool isVisible;

        #region Properties

        /// <inheritdoc />
        public int MasterClientId { get; protected set; }

        /// <inheritdoc />
        public virtual bool IsVisible
        {
            get { return this.isVisible; }
            set { this.isVisible = value; }
        }

        /// <inheritdoc />
        public NetworkPropertiesTable CustomProperties { get; private set; }

        /// <inheritdoc />
        public IEnumerable<INetworkPlayer> AllPlayers
        {
            get
            {
                return this.playersInRoom.Values;
            }
        }

        /// <inheritdoc />
        public string Name
        {
            get
            {
                return this.internalRoomInfo.Name;
            }
        }

        /// <inheritdoc />
        public byte PlayerCount
        {
            get
            {
                return (byte)this.playersInRoom.Count;
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
        public virtual byte MaxPlayers
        {
            get
            {
                return this.internalRoomInfo.MaxPlayers;
            }

            set
            {
                this.internalRoomInfo.MaxPlayers = value;
            }
        }

        /// <inheritdoc />
        public HashSet<string> PropertiesListedInLobby
        {
            get
            {
                return this.internalRoomInfo.PropertiesListedInLobby;
            }
        }

        /// <summary>
        /// Gets the internal <see cref="RoomInfo"/>
        /// </summary>
        internal RoomInfo RoomInfo
        {
            get
            {
                this.internalRoomInfo.PlayerCount = this.PlayerCount;
                return this.internalRoomInfo;
            }
        }

        /// <summary>
        /// Gets the list of the ids of the players in the room
        /// </summary>
        internal IEnumerable<int> PlayerIds
        {
            get
            {
                return this.playersInRoom.Keys;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the master client id changes
        /// </summary>
        public event EventHandler MasterClientIdChanged;

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseNetworkRoom" /> class.
        /// </summary>
        protected BaseNetworkRoom()
        {
            this.lastPlayerId = FirstPlayerId;
            this.internalRoomInfo = new RoomInfo();
            this.playersInRoom = new Dictionary<int, BaseNetworkPlayer>();

            this.MasterClientId = FirstPlayerId;
            this.isVisible = true;
            this.CustomProperties = new NetworkPropertiesTable(isReadOnly: false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseNetworkRoom" /> class.
        /// </summary>
        /// <param name="options">The room options for the new room</param>
        protected BaseNetworkRoom(RoomOptions options)
            : this()
        {
            this.internalRoomInfo.ReadOptions(options);

            this.isVisible = options.IsVisible;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Tries to find the player with given player id.
        /// Only useful when in a Room, as Ids are only valid per Room.
        /// </summary>
        /// <param name="playerId">Player id of a player in this room.</param>
        /// <returns>Player or null.</returns>
        public INetworkPlayer GetPlayer(int playerId)
        {
            BaseNetworkPlayer result;
            this.playersInRoom.TryGetValue(playerId, out result);

            return result;
        }

        /// <summary>
        /// Tries to find the player with given player id.
        /// Only useful when in a Room, as Ids are only valid per Room.
        /// </summary>
        /// <typeparam name="T">A <see cref="INetworkPlayer"/> type.</typeparam>
        /// <param name="playerId">Player id of a player in this room.</param>
        /// <returns>Player or null.</returns>
        internal T GetPlayer<T>(int playerId)
            where T : class, INetworkPlayer
        {
            var player = this.GetPlayer(playerId);
            return player != null ? (T)player : null;
        }

        /// <summary>
        /// Refresh the player fields based on the given message.
        /// </summary>
        /// <param name="message">The received message</param>
        internal void ReadFromMessage(IncomingMessage message)
        {
            var changedFields = (RoomFieldsFlags)message.ReadByte();

            if (changedFields.HasFlag(RoomFieldsFlags.IsVisible))
            {
                this.IsVisible = message.ReadBoolean();
            }

            if (changedFields.HasFlag(RoomFieldsFlags.MaxPlayers))
            {
                this.MaxPlayers = message.ReadByte();
            }

            if (changedFields.HasFlag(RoomFieldsFlags.CustomProperties))
            {
                this.CustomProperties.ReadFromMessage(message);
            }

            this.OnChange(changedFields);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Writes the specified fields to an outgoing message.
        /// </summary>
        /// <param name="message">The outgoing message</param>
        /// <param name="includedFields">Indicates the fields that must synchronized</param>
        protected void WriteToMessage(OutgoingMessage message, RoomFieldsFlags includedFields)
        {
            message.Write((byte)includedFields);

            if (includedFields.HasFlag(RoomFieldsFlags.IsVisible))
            {
                message.Write(this.IsVisible);
            }

            if (includedFields.HasFlag(RoomFieldsFlags.MaxPlayers))
            {
                message.Write(this.MaxPlayers);
            }

            if (includedFields.HasFlag(RoomFieldsFlags.CustomProperties))
            {
                this.CustomProperties.WriteToMessage(message);
            }
        }

        /// <summary>
        /// Internal method to add a player in the players list.
        /// </summary>
        /// <param name="player">The player to add</param>
        protected void InternalAddPlayer(BaseNetworkPlayer player)
        {
            var playerId = this.lastPlayerId++;
            this.InternalAddPlayer(player, playerId);
        }

        /// <summary>
        /// Internal method to add a player in the players list.
        /// </summary>
        /// <param name="player">The player to add</param>
        /// <param name="playerId">The assigned player id</param>
        protected void InternalAddPlayer(BaseNetworkPlayer player, int playerId)
        {
            this.lastPlayerId = Math.Max(this.lastPlayerId, playerId + 1);

            this.playersInRoom.Add(playerId, player);

            this.RefreshMasterClientId();

            player.Id = playerId;
            player.Room = this;
        }

        /// <summary>
        /// Internal method to remove a player from the players list.
        /// </summary>
        /// <param name="playerId">The player id of the player to remove</param>
        protected void InternalRemovePlayer(int playerId)
        {
            BaseNetworkPlayer player;
            if (this.playersInRoom.TryGetValue(playerId, out player))
            {
                this.playersInRoom.Remove(playerId);
                player.Id = -1;
                player.Room = null;

                if (this.MasterClientId == playerId &&
                    this.playersInRoom.Count > 0)
                {
                    this.RefreshMasterClientId();
                }
            }
        }

        /// <summary>
        /// Called when one or more properties are refreshed.
        /// </summary>
        /// <param name="changedFields">Flag indicating what fields have been changed</param>
        protected virtual void OnChange(RoomFieldsFlags changedFields)
        {
        }

        /// <summary>
        /// Refreshes the master client id
        /// </summary>
        private void RefreshMasterClientId()
        {
            this.MasterClientId = this.playersInRoom.Keys.OrderBy(k => k).First();

            if (this.lastMasterClientId != this.MasterClientId)
            {
                this.lastMasterClientId = this.MasterClientId;

                this.MasterClientIdChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}
