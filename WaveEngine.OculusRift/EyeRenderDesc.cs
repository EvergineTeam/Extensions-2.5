using System.Runtime.InteropServices;
using WaveEngine.Common.Math;

namespace WaveEngine.OculusRift
{
    // Rendering information for each eye, computed by either ovrHmd_ConfigureRendering().
    // or ovrHmd_GetRenderDesc() based on the specified Fov.
    // Note that the rendering viewport is not included here as it can be 
    // specified separately and modified per frame though:
    //    (a) calling ovrHmd_GetRenderScaleAndOffset with game-rendered api,
    // or (b) passing different values in ovrTexture in case of SDK-rendered distortion.
    [StructLayout(LayoutKind.Sequential)]
    public struct EyeRenderDesc
    {
        public EyeType Eye;
        public FovPort Fov;
        public Rect DistortedViewport; // Distortion viewport
        public Vector2 PixelsPerTanAngleAtCenter;   // How many display pixels will fit in tan(angle) = 1.
        public Vector3 ViewAdjust;  // Translation to be applied to view matrix.
    }
}
