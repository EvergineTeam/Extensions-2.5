#region File Description
//-----------------------------------------------------------------------------
// TextureFormat
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

namespace WaveEngine.OculusRift
{
    /// <summary>
    /// Texture format.
    /// </summary>
    public enum TextureFormat
    {
        /// <summary>
        /// Depth type.
        /// </summary>
        Depth = 0x8000,

        /// <summary>
        /// Generate Mipmaps
        /// </summary>
        GenMipmaps = 0x20000,

        /// <summary>
        /// Render Target
        /// </summary>
        RenderTarget = 0x10000,

        /// <summary>
        /// RGBA type.
        /// </summary>
        RGBA = 0x100,

        /// <summary>
        /// Samples Mask
        /// </summary>
        SamplesMask = 0xff,

        /// <summary>
        /// Type Mask
        /// </summary>
        TypeMask = 0xff00
    }
}
