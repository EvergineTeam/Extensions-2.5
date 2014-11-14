using System;

namespace WaveEngine.OculusRift
{
    [Flags]
    public enum DistortionCapabilities : uint
    {
        None = 0,
        Chromatic = 0x01,   //  Supports chromatic aberration correction.
        TimeWarp = 0x02,    //  Supports timewarp.
        Vignette = 0x08,    //  Supports vignetting around the edges of the view.
        NoRestore = 0x10,   //  Do not save and restore the graphics state when rendering distortion.
        FlipInput = 0x20,   //  Flip the vertical texture coordinate of input images. 
        SRGB = 0x40,    //  Assume input images are in SRGB gamma-corrected color space.
        Overdrive = 0x80,   //  Overdrive brightness transitions to dampen high contrast artifacts on DK2+ displays
        ProfileNoTimewarpSpinWaits = 0x10000,   // Use when profiling with timewarp to remove false positives
    }
}
