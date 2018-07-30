// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System.Collections.Generic;
using WaveEngine.Networking.Connection.Messages;
using WaveEngine.Networking.Messages;
#endregion

namespace WaveEngine.Networking
{
    /// <summary>
    /// This class describes the options for a new room
    /// </summary>
    public class RoomOptions
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name to create a room with. Must be unique and not in use or can't be created.
        /// If null, the server will assign a GUID as name.
        /// </summary>
        public string RoomName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this room will be in the list of visible rooms that can be seen from the lobby
        /// (i.e. players that are connected to the server, but do not reside in a room). Important is that these rooms can
        /// still be joined, as long as the client knows the exact name of the room.
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// Gets or sets a value that determines the maximum amount of players in this room. If set to 0 (by default)
        /// the number is unlimited.
        /// </summary>
        public byte MaxPlayers { get; set; }

        /// <summary>
        /// Gets or sets a set of string properties that are in the RoomInfo of the Lobby.
        /// This list is defined when creating the room and can't be changed afterwards.
        /// </summary>
        /// <remarks>
        /// You could name properties that are not set from the beginning. Those will be synchronized with the lobby when added later on.
        /// </remarks>
        public HashSet<string> PropertiesListedInLobby { get; set; }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomOptions" /> class.
        /// </summary>
        public RoomOptions()
        {
            this.PropertiesListedInLobby = new HashSet<string>();
            this.IsVisible = true;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Read this instance fields from an incoming message.
        /// </summary>
        /// <param name="incomingMessage">The incoming message</param>
        internal void Read(IncomingMessage incomingMessage)
        {
            this.RoomName = incomingMessage.ReadString();
            this.IsVisible = incomingMessage.ReadBoolean();
            this.MaxPlayers = incomingMessage.ReadByte();
            this.PropertiesListedInLobby = new HashSet<string>(incomingMessage.ReadStringArray());
        }

        /// <summary>
        /// Writes this instance fields to an outgoing message.
        /// </summary>
        /// <param name="outgoingMessage">The outgoing message</param>
        internal void Write(OutgoingMessage outgoingMessage)
        {
            outgoingMessage.Write(this.RoomName);
            outgoingMessage.Write(this.IsVisible);
            outgoingMessage.Write(this.MaxPlayers);
            outgoingMessage.Write(this.PropertiesListedInLobby);
        }

        #endregion
    }
}
