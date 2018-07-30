// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
#endregion

namespace WaveEngine.Networking.Connection
{
    /// <summary>
    /// Represents the arguments of the client disconnected event.
    /// </summary>
    public class ClientDisconnectedEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        /// Gets the endpoint of the disconnected client.
        /// </summary>
        public NetworkEndpoint ClientEndpoint { get; private set; }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientDisconnectedEventArgs" /> class.
        /// </summary>
        /// <param name="clientEndpoint">The endpoint of the disconnected client</param>
        public ClientDisconnectedEventArgs(NetworkEndpoint clientEndpoint)
        {
            this.ClientEndpoint = clientEndpoint;
        }

        #endregion
    }
}
