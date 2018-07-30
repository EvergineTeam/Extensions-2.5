// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using WaveEngine.Networking.Client.Players;
using WaveEngine.Networking.Connection.Messages;
using WaveEngine.Networking.Messages;
using WaveEngine.Networking.Players;
#endregion

namespace WaveEngine.Networking.Rooms
{
    /// <summary>
    /// This class represents the room where the player has joined
    /// </summary>
    public class LocalNetworkRoom : BaseNetworkRoom, INetworkRoom
    {
        /// <summary>
        /// Indicates what fields need to be synchronized in the next update.
        /// </summary>
        private RoomFieldsFlags pendingSyncFields;

        #region Properties

        /// <inheritdoc />
        /// <summary>
        /// Gets or sets a value indicating whether the room is listed in its lobby.
        /// </summary>
        public override bool IsVisible
        {
            get
            {
                return base.IsVisible;
            }

            set
            {
                if (base.IsVisible != value)
                {
                    base.IsVisible = value;
                    this.pendingSyncFields |= RoomFieldsFlags.IsVisible;
                }
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Gets or sets the limit of players for this room. This property is shown in lobby, too.
        /// If the room is full (players count == maxplayers), joining this room will fail.
        /// </summary>
        public override byte MaxPlayers
        {
            get
            {
                return base.MaxPlayers;
            }

            set
            {
                if (base.MaxPlayers != value)
                {
                    base.MaxPlayers = value;
                    this.pendingSyncFields |= RoomFieldsFlags.MaxPlayers;
                }
            }
        }

        /// <summary>
        /// Gets the "list" of remote players who are also in that room. Only updated while inside a Room.
        /// </summary>
        public IEnumerable<RemoteNetworkPlayer> RemotePlayers
        {
            get
            {
                return this.AllPlayers
                           .Where(p => p is RemoteNetworkPlayer)
                           .Cast<RemoteNetworkPlayer>();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the local room needs to be sync or not.
        /// </summary>
        internal bool NeedSync
        {
            get
            {
                return this.pendingSyncFields != RoomFieldsFlags.None ||
                    this.CustomProperties.NeedSync;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a remote player is joining the room
        /// </summary>
        public event EventHandler<RemoteNetworkPlayer> PlayerJoining;

        /// <summary>
        /// Occurs when a remote player joined the room
        /// </summary>
        public event EventHandler<RemoteNetworkPlayer> PlayerJoined;

        /// <summary>
        /// Occurs when a remote player is leaving the room
        /// </summary>
        public event EventHandler<RemoteNetworkPlayer> PlayerLeaving;

        /// <summary>
        /// Occurs when a remote player left the room
        /// </summary>
        public event EventHandler<RemoteNetworkPlayer> PlayerLeft;

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalNetworkRoom" /> class.
        /// Used by <see cref="FromJoinMessage"/> method only.
        /// </summary>
        /// <param name="roomOptions">The room options for the new room</param>
        protected LocalNetworkRoom(RoomOptions roomOptions)
            : base(roomOptions)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalNetworkRoom" /> class.
        /// </summary>
        /// <param name="roomOptions">The room options for the new room</param>
        /// <param name="localPlayer">The client local player</param>
        public LocalNetworkRoom(RoomOptions roomOptions, LocalNetworkPlayer localPlayer)
            : this(roomOptions)
        {
            this.InternalAddPlayer(localPlayer);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalNetworkRoom" /> class based on the given message.
        /// </summary>
        /// <param name="message">The received message</param>
        /// <param name="localPlayer">The client local player</param>
        /// <param name="roomOptions">The room options for the new room</param>
        /// <returns>A new <see cref="LocalNetworkRoom" /> instance</returns>
        internal static LocalNetworkRoom FromJoinMessage(IncomingMessage message, LocalNetworkPlayer localPlayer, RoomOptions roomOptions)
        {
            var playerRoom = new LocalNetworkRoom(roomOptions);
            playerRoom.ReadFromMessage(message);

            var playerCount = message.ReadInt32();
            for (int i = 0; i < playerCount; i++)
            {
                var playerId = message.ReadPlayerId();
                var remotePlayer = RemoteNetworkPlayer.FromMessage(message);
                playerRoom.InternalAddPlayer(remotePlayer, playerId);
            }

            var localPlayerId = message.ReadPlayerId();
            playerRoom.InternalAddPlayer(localPlayer, localPlayerId);

            playerRoom.CustomProperties.ReadFromMessage(message);

            return playerRoom;
        }

        /// <summary>
        /// Writes the fields to be sync on an outgoing message.
        /// </summary>
        /// <param name="message">The outgoing message</param>
        /// <param name="forceAllFields">Indicates if all fields must be forced to sync</param>
        internal void WriteSyncMessage(OutgoingMessage message, bool forceAllFields = false)
        {
            if (forceAllFields)
            {
                this.pendingSyncFields = RoomFieldsFlags.All;
            }
            else if (this.CustomProperties.NeedSync)
            {
                this.pendingSyncFields |= RoomFieldsFlags.CustomProperties;
            }

            this.WriteToMessage(message, this.pendingSyncFields);

            this.pendingSyncFields = RoomFieldsFlags.None;
        }

        /// <summary>
        /// Refresh the player list based on the given message.
        /// </summary>
        /// <param name="message">The received message</param>
        internal void ReadSyncPlayersListFromMessage(IncomingMessage message)
        {
            var removedPlayersCount = message.ReadInt32();
            for (int i = 0; i < removedPlayersCount; i++)
            {
                var playerId = message.ReadPlayerId();
                var remotePlayer = this.GetPlayer<RemoteNetworkPlayer>(playerId);
                this.PlayerLeaving?.Invoke(this, remotePlayer);
                this.InternalRemovePlayer(playerId);
                this.PlayerLeft?.Invoke(this, remotePlayer);
            }

            var includedPlayersCount = message.ReadInt32();
            for (int i = 0; i < includedPlayersCount; i++)
            {
                var remotePlayer = RemoteNetworkPlayer.FromMessage(message);
                this.PlayerJoining?.Invoke(this, remotePlayer);
                this.InternalAddPlayer(remotePlayer);
                this.PlayerJoined?.Invoke(this, remotePlayer);
            }
        }

        /// <summary>
        /// Refresh the player fields based on the given message.
        /// </summary>
        /// <param name="message">The received message</param>
        internal void ReadSyncPlayerPropertiesFromMessage(IncomingMessage message)
        {
            var playerId = message.ReadPlayerId();

            var player = this.GetPlayer<BaseNetworkPlayer>(playerId);
            player.ReadFromMessage(message);
        }

        #endregion
    }
}
