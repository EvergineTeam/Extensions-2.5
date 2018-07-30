// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System.Collections.Generic;
using WaveEngine.Networking.Connection.Messages;
using WaveEngine.Networking.Rooms;
#endregion

namespace WaveEngine.Networking.Messages
{
    /// <summary>
    /// This class defines an operation to refresh the rooms in a lobby
    /// </summary>
    internal class RefreshLobbyRoomsMessage
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the content of this message is absolute.
        /// </summary>
        public bool IsAbsolute { get; set; }

        /// <summary>
        /// Gets a list with the rooms included in the lobby
        /// </summary>
        public List<RoomInfo> IncludedRooms { get; private set; }

        /// <summary>
        /// Gets a list with the removed rooms from the lobby
        /// </summary>
        public List<string> RemovedRooms { get; private set; }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshLobbyRoomsMessage" /> class.
        /// </summary>
        public RefreshLobbyRoomsMessage()
        {
            this.IncludedRooms = new List<RoomInfo>();
            this.RemovedRooms = new List<string>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Read this instance fields from an incoming message.
        /// </summary>
        /// <param name="incomingMessage">The inconming message</param>
        public void Read(IncomingMessage incomingMessage)
        {
            this.IncludedRooms.Clear();
            this.IsAbsolute = incomingMessage.ReadBoolean();
            incomingMessage.ReadRoomInfoList(this.IncludedRooms);
            this.RemovedRooms = new List<string>(incomingMessage.ReadStringArray());
        }

        /// <summary>
        /// Writes this instance fields to an outgoing message.
        /// </summary>
        /// <param name="outgoingMessage">The outgoing message</param>
        public void Write(OutgoingMessage outgoingMessage)
        {
            outgoingMessage.Write(this.IsAbsolute);
            outgoingMessage.Write(this.IncludedRooms);
            outgoingMessage.Write(this.RemovedRooms);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshLobbyRoomsMessage" /> class based on the given message.
        /// </summary>
        /// <param name="message">The received message</param>
        /// <returns>A <see cref="RefreshLobbyRoomsMessage" /> instance</returns>
        public static RefreshLobbyRoomsMessage FromMessage(IncomingMessage message)
        {
            var result = new RefreshLobbyRoomsMessage();
            result.Read(message);

            return result;
        }

        #endregion
    }
}
