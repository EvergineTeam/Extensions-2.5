// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using WaveEngine.Networking.Connection.Messages;
#endregion

namespace WaveEngine.Networking.Connection
{
    /// <summary>
    /// Represents the arguments of the client connected event.
    /// </summary>
    public class ClientConnectedEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        /// Gets the endpoint of the new client.
        /// </summary>
        public NetworkEndpoint ClientEndpoint { get; private set; }

        /// <summary>
        /// Gets the received hail message of the client.
        /// </summary>
        public IncomingMessage HailMessage { get; private set; }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientConnectedEventArgs" /> class.
        /// </summary>
        /// <param name="clientEndpoint">The endpoint of the client</param>
        /// <param name="hailMessage">The received hail message</param>
        public ClientConnectedEventArgs(NetworkEndpoint clientEndpoint, IncomingMessage hailMessage)
        {
            this.ClientEndpoint = clientEndpoint;
            this.HailMessage = hailMessage;
        }

        #endregion
    }
}
