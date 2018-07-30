// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
#endregion

namespace WaveEngine.Networking.Connection
{
    /// <summary>
    /// Represents the arguments of the host discovered event.
    /// </summary>
    public class HostDiscoveredEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        /// Gets the discovered host endpoint.
        /// </summary>
        public NetworkEndpoint Host { get; private set; }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="HostDiscoveredEventArgs" /> class.
        /// </summary>
        /// <param name="host">The discovered host endpoint</param>
        public HostDiscoveredEventArgs(NetworkEndpoint host)
        {
            this.Host = host;
        }

        #endregion
    }
}
