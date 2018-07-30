// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using WaveEngine.Networking.Connection.Messages;
using WaveEngine.Networking.Rooms;
using WaveEngine.Networking.Server.Players;
#endregion

namespace WaveEngine.Networking.Server.Rooms
{
    /// <summary>
    /// This class represents a room from the server
    /// </summary>
    public class ServerRoom : LocalNetworkRoom, INetworkRoom
    {
        /// <summary>
        /// List that contains the latest synchronized players keys.
        /// </summary>
        private int[] lastSyncPlayerKeys;

        #region Properties

        /// <summary>
        /// Gets the "list" of all the players who are in that room. Only updated while inside a Room.
        /// </summary>
        public new IEnumerable<ServerPlayer> AllPlayers
        {
            get
            {
                return base.AllPlayers.Cast<ServerPlayer>();
            }
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerRoom" /> class.
        /// </summary>
        /// <param name="roomOptions">The room options for the new room</param>
        public ServerRoom(RoomOptions roomOptions)
            : base(roomOptions)
        {
            this.lastSyncPlayerKeys = new int[] { FirstPlayerId };
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a player in the room if it is possible.
        /// </summary>
        /// <param name="player">The player to add</param>
        /// <returns>
        /// <c>true</c> if the player can enter the room; otherwise, <c>false</c>.
        /// </returns>
        internal bool AddPlayer(ServerPlayer player)
        {
            var isNotFull = !this.IsFull;
            if (isNotFull)
            {
                this.InternalAddPlayer(player);
            }

            return isNotFull;
        }

        /// <summary>
        /// Removes the players from the room
        /// </summary>
        /// <param name="player">The player to remove</param>
        internal void RemovePlayer(ServerPlayer player)
        {
            if (player.Room != this)
            {
                throw new InvalidOperationException($"Player {player.ServerKey} is not in the room");
            }

            this.InternalRemovePlayer(player.Id);
        }

        /// <summary>
        /// Writes all room fields to an outgoing message.
        /// </summary>
        /// <param name="message">The outgoing message</param>
        /// <param name="joinedPlayer">The player that will receive the message</param>
        internal void WriteJoinToMessage(OutgoingMessage message, ServerPlayer joinedPlayer)
        {
            this.WriteToMessage(message, RoomFieldsFlags.All);
            message.Write(this.PlayerCount - 1);
            foreach (var player in this.AllPlayers)
            {
                if (player != joinedPlayer)
                {
                    message.Write(player.Id);
                    player.WriteToMessage(message);
                }
            }

            message.Write(joinedPlayer.Id);

            this.CustomProperties.ForceFullSync();
            this.CustomProperties.WriteToMessage(message);
        }

        /// <summary>
        /// Writes the player list modifications to an outgoing message.
        /// </summary>
        /// <param name="message">The outgoing message</param>
        internal void WriteSyncPlayersListToMessage(OutgoingMessage message)
        {
            var currentKeys = this.PlayerIds;

            var removedPlayers = this.lastSyncPlayerKeys.Except(currentKeys);
            message.Write(removedPlayers.Count());
            foreach (var playerKey in removedPlayers)
            {
                message.Write(playerKey);
            }

            var includedPlayers = currentKeys.Except(this.lastSyncPlayerKeys);
            message.Write(includedPlayers.Count());
            foreach (var playerKey in includedPlayers)
            {
                this.GetPlayer<ServerPlayer>(playerKey).WriteToMessage(message);
            }

            this.lastSyncPlayerKeys = currentKeys.ToArray();
        }

        #endregion
    }
}
