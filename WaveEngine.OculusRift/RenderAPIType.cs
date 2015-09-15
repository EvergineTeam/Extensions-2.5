#region File Description
//-----------------------------------------------------------------------------
// RenderAPIType
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

namespace WaveEngine.OculusRift
{
    /// <summary>
    /// Platform-independent Rendering Configuration.
    /// These types are used to hide platform-specific details when passing
    /// render device, OS and texture data to the APIs.
    /// The benefit of having these wrappers vs. platform-specific API functions is
    /// that they allow game glue code to be portable. A typical example is an
    /// engine that has multiple back ends, say GL and D3D. Portable code that calls
    /// these back ends may also use LibOVR. To do this, back ends can be modified
    /// to return portable types such as ovrTexture and ovrRenderAPIConfig.
    /// </summary>
    public enum RenderAPIType
    {
        /// <summary>
        /// Unknown type.
        /// </summary>
        None,

        /// <summary>
        /// OpenGL type.
        /// </summary>
        OpenGL,

        /// <summary>
        /// OpenGL ES.
        /// </summary>
        Android_GLES,

        /// <summary>
        /// DirectX 9.
        /// </summary>
        D3D9,

        /// <summary>
        /// Directx 10.
        /// </summary>
        D3D10,

        /// <summary>
        /// DirectX 11.
        /// </summary>
        D3D11,

        /// <summary>
        /// Num of platforms.
        /// </summary>
        Count
    }
}
