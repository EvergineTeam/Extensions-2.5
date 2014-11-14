using System.Runtime.InteropServices;
using WaveEngine.Common.Math;

namespace WaveEngine.OculusRift
{
    // Platform-independent part of eye texture descriptor.
    // It is a part of ovrTexture, passed to ovrHmd_EndFrame.
    //  - If RenderViewport is all zeros, will be used.
    [StructLayout(LayoutKind.Sequential)]
    public struct TextureHeader
    {
        public RenderAPIType API;
        public Size2 TextureSize;
        public Rect RenderViewport;    // Pixel viewport in texture that holds eye image.
    }
}
