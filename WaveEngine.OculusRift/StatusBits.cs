#region File Description
//-----------------------------------------------------------------------------
// StatusBits
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

namespace WaveEngine.OculusRift
{
    /// <summary>
    /// Status bits.
    /// </summary>
    public enum StatusBits : uint
    {
        /// <summary>
        /// None behavior.
        /// </summary>
        None = 0,

        /// <summary>
        /// Orientation is currently tracked (connected and in use).
        /// </summary>
        OrientationTracked = 1,     

        /// <summary>
        /// Position is currently tracked (false if out of range).
        /// </summary>
        PositionTracked = 2,        

        /// <summary>
        /// Camera pose is currently tracked.
        /// </summary>
        CameraPoseTracked = 4,      

        /// <summary>
        /// Position tracking HW is conceded.
        /// </summary>
        HmdConnected = 0x80,        

        /// <summary>
        /// HMD Display is available and connected.
        /// </summary>
        PositionConnected = 0x20,   
    }
}
