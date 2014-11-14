using WaveEngine.Common.Math;

namespace WaveEngine.OculusRift
{
    // Platform-independent part of rendering API-configuration data.
    // It is a part of ovrRenderAPIConfig, passed to ovrHmd_Configure.
    public struct RenderAPIConfigHeader
    {
        public RenderAPIType API;
        public Size2 RTSize;
        public int Multisample;
    }
}
