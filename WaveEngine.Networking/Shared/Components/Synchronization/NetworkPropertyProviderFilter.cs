// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
#endregion

namespace WaveEngine.Networking.Components
{
    /// <summary>
    /// Defines the available filters to be used during <see cref="NetworkCustomPropertiesProvider"/>
    /// dependency resolution
    /// </summary>
    public enum NetworkPropertyProviderFilter
    {
        /// <summary>
        /// Any <see cref="NetworkCustomPropertiesProvider"/> is valid
        /// </summary>
        Any = 0,

        /// <summary>
        /// Only <see cref="NetworkPlayerProvider"/> components are valid
        /// </summary>
        Player = 1,

        /// <summary>
        /// Only <see cref="NetworkRoomProvider"/> components are valid
        /// </summary>
        Room = 2
    }
}
