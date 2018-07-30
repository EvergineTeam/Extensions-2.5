// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
#endregion

namespace WaveEngine.Networking.Client
{
    /// <summary>
    /// State values for a client.
    /// </summary>
    public enum ClientStates
    {
        /// <summary>The client is no longer connected (to any server). Connect to a server to go on.</summary>
        Disconnected,

        /// <summary>Connected to a matchmaking server. You might use matchmaking now.</summary>
        InLobby,

        /// <summary>Transition state while joining or creating a room on the server.</summary>
        Joining,

        /// <summary>The client entered a room. The CurrentRoom and Players are known and you can now raise events.</summary>
        Joined,

        /// <summary>Transition state when leaving a room.</summary>
        Leaving,
    }
}
