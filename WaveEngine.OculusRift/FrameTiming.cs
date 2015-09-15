#region File Description
//-----------------------------------------------------------------------------
// FrameTiming
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace WaveEngine.OculusRift
{
    /// <summary>
    /// FrameTiming represent a interval.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FrameTiming
    {
        /// <summary>
        /// The amount of time that has passed since the previous frame returned
        /// BeginFrameSeconds value, usable for movement scaling.
        /// This will be clamped to no more than 0.1 seconds to prevent
        /// excessive movement after pauses for loading or initialization.
        /// </summary>
        public float DeltaSeconds;

        /// <summary>
        /// It is generally expected that the following hold:
        /// Absolute time value of when rendering of this frame began or is expected to
        /// begin; generally equal to NextFrameSeconds of the previous frame. Can be used
        /// for animation timing.
        /// </summary>
        public double ThisFrameSeconds;

        /// <summary>
        /// Absolute point when IMU expects to be sampled for this frame.
        /// </summary>
        public double TimewarpPointSeconds;

        /// <summary>
        /// Absolute time when frame Present + GPU Flush will finish, and the next frame starts.
        /// </summary>
        public double NextFrameSeconds;

        /// <summary>
        /// Time when when half of the screen will be scanned out. Can be passes as a prediction
        /// value to ovrHmd_GetTrackingState() go get general orientation.
        /// </summary>
        public double ScanoutMidpointSeconds;

        /// <summary>
        /// Timing points when each eye will be scanned out to display. Used for rendering each eye. 
        /// </summary>
        public double[] EyeScanoutSeconds;

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameTiming"/> struct.
        /// </summary>
        /// <param name="raw">The raw.</param>
        internal FrameTiming(FrameTiming_Raw raw)
        {
            this.DeltaSeconds = raw.DeltaSeconds;
            this.ThisFrameSeconds = raw.ThisFrameSeconds;
            this.TimewarpPointSeconds = raw.TimewarpPointSeconds;
            this.NextFrameSeconds = raw.NextFrameSeconds;
            this.ScanoutMidpointSeconds = raw.ScanoutMidpointSeconds;
            this.EyeScanoutSeconds = new double[2] { raw.EyeScanoutSeconds0, raw.EyeScanoutSeconds1 };
        }
    }

    /// <summary>
    /// Internal description for ovrFrameTiming; must match C 'ovrFrameTiming' layout.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal struct FrameTiming_Raw
    {
        /// <summary>
        /// Delta seconds.
        /// </summary>
        public float DeltaSeconds;

        /// <summary>
        /// This frame seconds
        /// </summary>
        public double ThisFrameSeconds;

        /// <summary>
        /// Timewrap Point Seconds.
        /// </summary>
        public double TimewarpPointSeconds;

        /// <summary>
        /// Next Frame seconds.
        /// </summary>
        public double NextFrameSeconds;

        /// <summary>
        /// Scanout Midpoint seconds.
        /// </summary>
        public double ScanoutMidpointSeconds;

        /// <summary>
        /// C# arrays are dynamic and thus not supported as return values, so just expand the struct.
        /// </summary>
        public double EyeScanoutSeconds0;

        /// <summary>
        /// C# arrays are dynamic and thus not supported as return values, so just expand the struct.
        /// </summary>
        public double EyeScanoutSeconds1;
    }
}
