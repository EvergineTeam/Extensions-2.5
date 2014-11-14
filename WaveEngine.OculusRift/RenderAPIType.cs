
namespace WaveEngine.OculusRift
{
    //-----------------------------------------------------------------------------------
    // ***** Platform-independent Rendering Configuration

    // These types are used to hide platform-specific details when passing
    // render device, OS and texture data to the APIs.
    //
    // The benefit of having these wrappers vs. platform-specific API functions is
    // that they allow game glue code to be portable. A typical example is an
    // engine that has multiple back ends, say GL and D3D. Portable code that calls
    // these back ends may also use LibOVR. To do this, back ends can be modified
    // to return portable types such as ovrTexture and ovrRenderAPIConfig.
    public enum RenderAPIType
    {
        None,
        OpenGL,
        Android_GLES,
        D3D9,
        D3D10,
        D3D11,
        Count
    }
}
