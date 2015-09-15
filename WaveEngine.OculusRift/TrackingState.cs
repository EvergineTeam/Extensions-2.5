#region File Description
//-----------------------------------------------------------------------------
// TrackingState
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Runtime.InteropServices;
#endregion

namespace WaveEngine.OculusRift
{
    /// <summary>
    /// Tracking state at a given absolute time (describes HMD location, etc).
    /// Returned by ovrHmd_GetTrackingState.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct TrackingState
    {
        /// <summary>
        /// Predicted pose configuration at requested absolute time.
        /// The look-ahead interval is equal to (HeadPose.TimeInSeconds - RawSensorData.TimeInSeconds).
        /// </summary>
        public PoseStateF HeadPose;

        /// <summary>
        /// Current orientation and position of the external camera, if present.
        /// This pose will include camera tilt (roll and pitch). For a leveled coordinate
        /// system use LeveledCameraPose instead.
        /// </summary>
        public PoseF CameraPose;

        /// <summary>
        /// Camera frame aligned with gravity.
        /// This value includes position and yaw of the camera, but not roll and pitch.
        /// Can be used as a reference point to render real-world objects in the correct place.
        /// </summary>
        public PoseF LeveledCameraPose;

        /// <summary>
        /// Most recent sensor data received from the HMD.
        /// </summary>
        public SensorData RawSensorData;

        /// <summary>
        /// Sensor status described by ovrStatusBits.
        /// </summary>
        public StatusBits StatusFlags;
    }
}
