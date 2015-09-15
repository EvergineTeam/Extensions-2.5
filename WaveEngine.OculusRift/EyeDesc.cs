#region File Description
//-----------------------------------------------------------------------------
// EyeDesc
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
    /// Info per eye.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct EyeDesc
    {
        /// <summary>
        /// The eye
        /// </summary>
        public EyeType Eye;

        /// <summary>
        /// The texture size
        /// </summary>
        public Size2 TextureSize;

        /// <summary>
        /// The render viewport
        /// </summary>
        public Rect RenderViewport;

        /// <summary>
        /// The fov
        /// </summary>
        public FovPort Fov;
    }
}
