using System.Runtime.InteropServices;
using WaveEngine.Common.Math;

namespace WaveEngine.OculusRift
{
    // Full pose (rigid body) configuration with first and second derivatives.
    [StructLayout(LayoutKind.Sequential)]
    public struct PoseStateF
    {
        public PoseF Pose;
        public Vector3 AngularVelocity;
        public Vector3 LinearVelocity;
        public Vector3 AngularAcceleration;
        public Vector3 LinearAcceleration;
        public double TimeInSeconds;    // Absolute time of this state sample.
    }
}
