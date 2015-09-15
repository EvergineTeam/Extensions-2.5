#region File Description
//-----------------------------------------------------------------------------
// HSWDisplayState
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Runtime.InteropServices;
#endregion

namespace WaveEngine.OculusRift
{
    /// <summary>
    /// Used by ovrHmd_GetHSWDisplayState to report the current display state.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct HSWDisplayState
    {
        /// <summary>
        /// If true then the warning should be currently visible
        /// and the following variables have meaning. Else there is no
        /// warning being displayed for this application on the given HMD.
        /// </summary>
        public bool Displayed;          
                                        
        /// <summary>
        /// Absolute time when the warning was first displayed. See ovr_GetTimeInSeconds().
        /// </summary>
        private double startTime;       

        /// <summary>
        /// Earliest absolute time when the warning can be dismissed. May be a time in the past.
        /// </summary>
        private double dismissibleTime; 
    }
}
