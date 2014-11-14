
namespace WaveEngine.OculusRift
{
    /// <summary>
    /// Specifies which eye is being used for rendering.
    /// This type explicitly does not include a third "NoStereo" option, as such is
    /// not required for an HMD-centered API.
    /// </summary>
    public enum EyeType
    {
        Left = 0,
        Right = 1,
    }
}
