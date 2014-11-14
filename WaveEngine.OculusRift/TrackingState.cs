using System.Runtime.InteropServices;

namespace WaveEngine.OculusRift
{

    /// <summary>
    /// Tracking state at a given absolute time (describes HMD location, etc).
    /// Returned by ovrHmd_GetTrackingState.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct TrackingState
    {
        // Predicted pose configuration at requested absolute time.
        // The look-ahead interval is equal to (HeadPose.TimeInSeconds - RawSensorData.TimeInSeconds).
        public PoseStateF HeadPose;

        // Current orientation and position of the external camera, if present.
        // This pose will include camera tilt (roll and pitch). For a leveled coordinate
        // system use LeveledCameraPose instead.
        public PoseF CameraPose;

        // Camera frame aligned with gravity.
        // This value includes position and yaw of the camera, but not roll and pitch.
        // Can be used as a reference point to render real-world objects in the correct place.
        public PoseF LeveledCameraPose;

        // Most recent sensor data received from the HMD.
        public SensorData RawSensorData;

        // Sensor status described by ovrStatusBits.
        public StatusBits StatusFlags;
    }
}
