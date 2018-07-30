// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
#endregion

namespace WaveEngine.Networking.Connection
{
    /// <summary>
    /// Represents the arguments of the host connected event.
    /// </summary>
    public class HostConnectedEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        /// Gets the endpoint of the new host.
        /// </summary>
        public NetworkEndpoint HostEndpoint { get; private set; }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="HostConnectedEventArgs" /> class.
        /// </summary>
        /// <param name="hostEndpoint">The endpoint of the new client</param>
        public HostConnectedEventArgs(NetworkEndpoint hostEndpoint)
        {
            this.HostEndpoint = hostEndpoint;
        }

        #endregion
    }
}
