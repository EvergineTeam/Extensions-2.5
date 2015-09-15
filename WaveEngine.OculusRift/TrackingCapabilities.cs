#region File Description
//-----------------------------------------------------------------------------
// TrackingCapabilities
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
#endregion

namespace WaveEngine.OculusRift
{
    /// <summary>
    /// Tracking capability bits reported by device.
    /// Used with ovrHmd_ConfigureTracking.
    /// </summary>
    [Flags]
    public enum TrackingCapabilities : uint
    {
        /// <summary>
        /// None behavior.
        /// </summary>
        None = 0,

        /// <summary>
        /// Supports orientation tracking (IMU).
        /// </summary>
        Orientation = 0x10,         

        /// <summary>
        /// Supports yaw correction through magnetometer or other means.
        /// </summary>
        MagYawCorrection = 0x20,    

        /// <summary>
        /// Supports positional tracking.
        /// </summary>
        Position = 0x40,            

        /// <summary>
        /// Overwrites other flags; indicates that implementation
        /// </summary>
        Idle = 0x100,               
    }
}
