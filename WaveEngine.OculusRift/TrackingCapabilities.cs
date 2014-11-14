using System;

namespace WaveEngine.OculusRift
{
    /// <summary>
    /// Tracking capability bits reported by device.
    /// Used with ovrHmd_ConfigureTracking.
    /// </summary>
    [Flags]
    public enum TrackingCapabilities : uint
    {
        None = 0,
        Orientation = 0x10, //  Supports orientation tracking (IMU).
        MagYawCorrection = 0x20,    //  Supports yaw correction through magnetometer or other means.
        Position = 0x40,    //  Supports positional tracking.
        Idle = 0x100,   //  Overwrites other flags; indicates that implementation
    }
}
