// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using WaveEngine.Networking.Client.Players;
using WaveEngine.Networking.Connection.Messages;
#endregion

namespace WaveEngine.Networking.Client
{
    /// <summary>
    /// Represents the arguments of the message received from player event.
    /// </summary>
    public class MessageFromPlayerEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        /// Gets the sender player.
        /// </summary>
        public RemoteNetworkPlayer FromPlayer { get; private set; }

        /// <summary>
        /// Gets the received message
        /// </summary>
        public IncomingMessage ReceivedMessage { get; private set; }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageFromPlayerEventArgs" /> class.
        /// </summary>
        /// <param name="fromPlayer">The players that sent the message</param>
        /// <param name="receivedMessage">The received message</param>
        public MessageFromPlayerEventArgs(RemoteNetworkPlayer fromPlayer, IncomingMessage receivedMessage)
        {
            this.FromPlayer = fromPlayer;
            this.ReceivedMessage = receivedMessage;
        }

        #endregion
    }
}
