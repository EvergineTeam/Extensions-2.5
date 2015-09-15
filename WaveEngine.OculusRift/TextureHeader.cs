#region File Description
//-----------------------------------------------------------------------------
// TextureHeader
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Runtime.InteropServices;
using WaveEngine.Common.Math;
#endregion

namespace WaveEngine.OculusRift
{
    /// <summary>
    /// Platform-independent part of eye texture descriptor.
    /// It is a part of ovrTexture, passed to ovrHmd_EndFrame.
    ///  - If RenderViewport is all zeros, will be used.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct TextureHeader
    {
        /// <summary>
        /// The API
        /// </summary>
        public RenderAPIType API;

        /// <summary>
        /// The texture size
        /// </summary>
        public Size2 TextureSize;
        
        /// <summary>
        /// The render viewport
        /// <remarks>Pixel viewport in texture that holds eye image.</remarks>
        /// </summary>
        public Rect RenderViewport;
    }
}
