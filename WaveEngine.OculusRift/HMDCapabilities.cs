#region File Description
//-----------------------------------------------------------------------------
// HMDCapabilities
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
    /// HMD capabilities.
    /// </summary>
    [Flags]
    public enum HMDCapabilities : uint
    {
        /// <summary>
        /// None value.
        /// </summary>
        None = 0,

        /// <summary>
        /// This HMD exists (as opposed to being unplugged).
        /// </summary>
        Present = 0x0001,          

        /// <summary>
        /// HMD and is sensor is available for Ownership use, i.e.
        /// </summary>
        Available = 2,              

        /// <summary>
        /// is not owned by another app.
        /// 'true' if we captured ownership for this Hmd.
        /// </summary>
        Captured = 4, 

        /// <summary>
        /// These flags are intended for use with the new driver display mode.
        /// Read only, means display driver is in compatibility mode.
        /// </summary>
        ExtendDesktop = 8,         

        /// <summary>
        /// Modifiable flags (through ovrHmd_SetEnabledCaps).
        /// Disables mirrowing of HMD output to the window;
        /// </summary>
        NoMirrorToWindow = 0x2000,  

        /// <summary>
        /// May improve rendering performance slightly (only if ExtendDesktop is off).
        /// Turns off Oculus HMD screen and output (only if ExtendDesktop is off).
        /// </summary>
        DisplayOff = 0x40,           

        /// <summary>
        /// Supports low persistence mode.
        /// </summary>
        LowPersistence = 0x80,      

        /// <summary>
        /// Adjust prediction dynamically based on DK2 Latency.
        /// </summary>
        DynamicPrediction = 0x200,  

        /// <summary>
        /// Support rendering without VSync for debugging
        /// </summary>
        NoVSync = 0x1000,           

        /// <summary>
        /// These bits can be modified by ovrHmd_SetEnabledCaps.
        /// </summary>
        WritableMask = 0x33f0,      

        /// <summary>
        /// These flags are currently passed into the service. May change without notice.
        /// </summary>
        ServiceMask = 0x23f0,       
    }
}
