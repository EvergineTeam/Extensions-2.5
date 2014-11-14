using System;

namespace WaveEngine.OculusRift
{
    [Flags]
    public enum HMDCapabilities : uint
    {
        None = 0,
        Present = 0x0001,   //  This HMD exists (as opposed to being unplugged).
        Available = 2,  //  HMD and is sensor is available for Ownership use, i.e.

        //  is not owned by another app.
        Captured = 4,   //  'true' if we captured ownership for this Hmd.

        // These flags are intended for use with the new driver display mode.
        ExtendDesktop = 8,  // Read only, means display driver is in compatibility mode.

        // Modifiable flags (through ovrHmd_SetEnabledCaps).
        NoMirrorToWindow = 0x2000,  // Disables mirrowing of HMD output to the window;

        // may improve rendering performance slightly (only if ExtendDesktop is off).
        DisplayOff = 0x40,  // Turns off Oculus HMD screen and output (only if ExtendDesktop is off).

        LowPersistence = 0x80,  //  Supports low persistence mode.
        DynamicPrediction = 0x200,  //  Adjust prediction dynamically based on DK2 Latency.

        // Support rendering without VSync for debugging
        NoVSync = 0x1000,

        // These bits can be modified by ovrHmd_SetEnabledCaps.
        WritableMask = 0x33f0,

        // These flags are currently passed into the service. May change without notice.
        ServiceMask = 0x23f0,
    }
}
