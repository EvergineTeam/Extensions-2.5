#region File Description
//-----------------------------------------------------------------------------
// EyeRenderDesc
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
   /// Rendering information for each eye, computed by either ovrHmd_ConfigureRendering().
   /// or ovrHmd_GetRenderDesc() based on the specified Fov.
   /// Note that the rendering viewport is not included here as it can be 
   /// specified separately and modified per frame though:
   ///    (a) calling ovrHmd_GetRenderScaleAndOffset with game-rendered api,
   /// or (b) passing different values in ovrTexture in case of SDK-rendered distortion.
   /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct EyeRenderDesc
    {
        /// <summary>
        /// Eye type.
        /// </summary>
        public EyeType Eye;

        /// <summary>
        /// Fov type.
        /// </summary>
        public FovPort Fov;

        /// <summary>
        /// Distortion viewport
        /// </summary>
        public Rect DistortedViewport; 
 
        /// <summary>
        /// How many display pixels will fit in tan(angle) = 1.
        /// </summary>
        public Vector2 PixelsPerTanAngleAtCenter;

        /// <summary>
        /// Translation to be applied to view matrix.
        /// </summary>
        public Vector3 ViewAdjust;  
    }
}
