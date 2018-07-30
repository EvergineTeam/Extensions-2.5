// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using WaveEngine.Networking.Connection.Messages;
#endregion

namespace WaveEngine.Networking.Connection
{
    /// <summary>
    /// Represents the arguments of the client connection request event.
    /// </summary>
    public class ClientConnectingEventArgs : EventArgs
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

        /// <summary>
        /// Gets a value indicating whether the connection request is rejected.
        /// </summary>
        public bool IsRejected { get; private set; }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientConnectingEventArgs" /> class.
        /// </summary>
        /// <param name="clientEndpoint">The endpoint of the client</param>
        /// <param name="hailMessage">The received hail message</param>
        public ClientConnectingEventArgs(NetworkEndpoint clientEndpoint, IncomingMessage hailMessage)
        {
            this.ClientEndpoint = clientEndpoint;
            this.HailMessage = hailMessage;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Mark the connection request as rejected.
        /// </summary>
        public void Reject()
        {
            this.IsRejected = true;
        }

        #endregion
    }
}
