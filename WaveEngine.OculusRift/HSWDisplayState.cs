using System.Runtime.InteropServices;

namespace WaveEngine.OculusRift
{
    // Used by ovrHmd_GetHSWDisplayState to report the current display state.
    [StructLayout(LayoutKind.Sequential)]
    public struct HSWDisplayState
    {
        public bool Displayed;  // If true then the warning should be currently visible
        // and the following variables have meaning. Else there is no
        // warning being displayed for this application on the given HMD.
        private double StartTime;   // Absolute time when the warning was first displayed. See ovr_GetTimeInSeconds().
        private double DismissibleTime; // Earliest absolute time when the warning can be dismissed. May be a time in the past.
    }
}
