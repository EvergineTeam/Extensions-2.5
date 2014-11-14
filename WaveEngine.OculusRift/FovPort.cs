using System.Runtime.InteropServices;

namespace WaveEngine.OculusRift
{
    [StructLayout(LayoutKind.Sequential)]
    public struct FovPort
    {
        public float UpTan;
        public float DownTan;
        public float LeftTan;
        public float RightTan;
    }
}
