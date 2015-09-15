#region File Description
//-----------------------------------------------------------------------------
// EventDelegates
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

using WaveEngine.Networking.Messages;

namespace WaveEngine.Networking
{
    /// <summary>
    /// Host discovered delegate.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="host">The host.</param>
    public delegate void HostDiscovered(object sender, Host host);

    /// <summary>
    /// Message received delegate.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="receivedMessage">The received message.</param>
    public delegate void MessageReceived(object sender, IncomingMessage receivedMessage);

    /// <summary>
    /// Host Connected delegate.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="host">The host.</param>
    public delegate void HostConnected(object sender, Host host);

    /// <summary>
    /// Host disconnected delegate.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="host">The host.</param>
    public delegate void HostDisconnected(object sender, Host host);
}
