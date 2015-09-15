#region File Description
//-----------------------------------------------------------------------------
// DistortionCapabilities
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
#endregion

namespace WaveEngine.OculusRift
{
    /// <summary>
    /// Distortion capabilities.
    /// </summary>
    [Flags]
    public enum DistortionCapabilities : uint
    {
        /// <summary>
        /// None value.
        /// </summary>
        None = 0,

        /// <summary>
        /// Supports chromatic aberration correction.
        /// </summary>
        Chromatic = 0x01,

        /// <summary>
        /// Supports timewarp.
        /// </summary>
        TimeWarp = 0x02,    
  
        /// <summary>
        /// Supports vignetting around the edges of the view.
        /// </summary>
        Vignette = 0x08,    

        /// <summary>
        /// Do not save and restore the graphics state when rendering distortion.
        /// </summary>
        NoRestore = 0x10,   

        /// <summary>
        /// Flip the vertical texture coordinate of input images. 
        /// </summary>
        FlipInput = 0x20,   

        /// <summary>
        /// Assume input images are in SRGB gamma-corrected color space.
        /// </summary>
        SRGB = 0x40,        

        /// <summary>
        /// Overdrive brightness transitions to dampen high contrast artifacts on DK2+ displays
        /// </summary>
        Overdrive = 0x80,   

        /// <summary>
        /// Use when profiling with timewarp to remove false positives
        /// </summary>
        ProfileNoTimewarpSpinWaits = 0x10000, 
    }
}
