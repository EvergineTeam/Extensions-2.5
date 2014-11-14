
namespace WaveEngine.OculusRift
{
    public enum StatusBits : uint
    {
        None = 0,
        OrientationTracked = 1, // Orientation is currently tracked (connected and in use).
        PositionTracked = 2,    // Position is currently tracked (false if out of range).
        CameraPoseTracked = 4,  // Camera pose is currently tracked.
        HmdConnected = 0x80,    // Position tracking HW is conceded.
        PositionConnected = 0x20,   // HMD Display is available & connected.
    }
}
