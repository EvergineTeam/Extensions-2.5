using System.Runtime.InteropServices;
using WaveEngine.Common.Math;

namespace WaveEngine.OculusRift
{
    [StructLayout(LayoutKind.Sequential)]
    public struct EyeDesc
    {
        public EyeType Eye;
        public Size2 TextureSize;
        public Rect RenderViewport;
        public FovPort Fov;
    }
}
