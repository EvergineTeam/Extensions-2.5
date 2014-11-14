using System.Runtime.InteropServices;
using WaveEngine.Common.Math;

namespace WaveEngine.OculusRift
{
    // Position and orientation together.
    [StructLayout(LayoutKind.Sequential)]
    public struct PoseF
    {
        public Quaternion Orientation;
        public Vector3 Position;
    }
}
