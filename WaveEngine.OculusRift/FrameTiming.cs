using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WaveEngine.OculusRift
{
    [StructLayout(LayoutKind.Sequential)]
    public struct FrameTiming
    {
        // The amount of time that has passed since the previous frame returned
        // BeginFrameSeconds value, usable for movement scaling.
        // This will be clamped to no more than 0.1 seconds to prevent
        // excessive movement after pauses for loading or initialization.
        public float DeltaSeconds;

        // It is generally expected that the following hold:
        // ThisFrameSeconds < TimewarpPointSeconds < NextFrameSeconds <
        // EyeScanoutSeconds[EyeOrder[0]] <= ScanoutMidpointSeconds <= EyeScanoutSeconds[EyeOrder[1]]

        // Absolute time value of when rendering of this frame began or is expected to
        // begin; generally equal to NextFrameSeconds of the previous frame. Can be used
        // for animation timing.
        public double ThisFrameSeconds;

        // Absolute point when IMU expects to be sampled for this frame.
        public double TimewarpPointSeconds;

        // Absolute time when frame Present + GPU Flush will finish, and the next frame starts.
        public double NextFrameSeconds;

        // Time when when half of the screen will be scanned out. Can be passes as a prediction
        // value to ovrHmd_GetTrackingState() go get general orientation.
        public double ScanoutMidpointSeconds;
        
        // Timing points when each eye will be scanned out to display. Used for rendering each eye. 
        public double[] EyeScanoutSeconds;

        internal FrameTiming(FrameTiming_Raw raw)
        {
            this.DeltaSeconds = raw.DeltaSeconds;
            this.ThisFrameSeconds = raw.ThisFrameSeconds;
            this.TimewarpPointSeconds = raw.TimewarpPointSeconds;
            this.NextFrameSeconds = raw.NextFrameSeconds;
            this.ScanoutMidpointSeconds = raw.ScanoutMidpointSeconds;
            this.EyeScanoutSeconds = new double[2] { raw.EyeScanoutSeconds_0, raw.EyeScanoutSeconds_1 };
        }
    };

    // Internal description for ovrFrameTiming; must match C 'ovrFrameTiming' layout.
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal struct FrameTiming_Raw
    {
        public float DeltaSeconds;
        public double ThisFrameSeconds;
        public double TimewarpPointSeconds;
        public double NextFrameSeconds;
        public double ScanoutMidpointSeconds;
        // C# arrays are dynamic and thus not supported as return values, so just expand the struct.
        public double EyeScanoutSeconds_0;
        public double EyeScanoutSeconds_1;
    };
}
