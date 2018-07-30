// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
#endregion

namespace WaveEngine.Networking.Connection
{
    /// <summary>
    /// Represents the arguments of the host disconnected event.
    /// </summary>
    public class HostDisconnectedEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        /// Gets the endpoint of the disconnected host.
        /// </summary>
        public NetworkEndpoint HostEndpoint { get; private set; }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="HostDisconnectedEventArgs" /> class.
        /// </summary>
        /// <param name="hostEndpoint">The endpoint of the disconnected host</param>
        public HostDisconnectedEventArgs(NetworkEndpoint hostEndpoint)
        {
            this.HostEndpoint = hostEndpoint;
        }

        #endregion
    }
}
