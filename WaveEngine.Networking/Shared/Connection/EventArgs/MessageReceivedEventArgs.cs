// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using WaveEngine.Networking.Connection.Messages;
#endregion

namespace WaveEngine.Networking.Connection
{
    /// <summary>
    /// Represents the arguments of the message received event.
    /// </summary>
    public class MessageReceivedEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        /// Gets the endpoint of the sender.
        /// </summary>
        public NetworkEndpoint FromEndpoint { get; private set; }

        /// <summary>
        /// Gets the received message
        /// </summary>
        public IncomingMessage ReceivedMessage { get; private set; }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageReceivedEventArgs" /> class.
        /// </summary>
        /// <param name="fromEndpoint">The endpoint of the sender</param>
        /// <param name="receivedMessage">The received message</param>
        public MessageReceivedEventArgs(NetworkEndpoint fromEndpoint, IncomingMessage receivedMessage)
        {
            this.FromEndpoint = fromEndpoint;
            this.ReceivedMessage = receivedMessage;
        }

        #endregion
    }
}
