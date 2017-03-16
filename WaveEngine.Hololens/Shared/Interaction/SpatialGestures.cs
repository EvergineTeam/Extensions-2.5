#region File Description
//-----------------------------------------------------------------------------
// SpatialGestureSettings
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
#endregion

namespace WaveEngine.Hololens.Interaction
{
    /// <summary>
    /// Spatial input enabled gestures
    /// </summary>
    [Flags]
    public enum SpatialGestures : System.UInt32
    {
        /// <summary>
        /// Disable support for gestures.
        /// </summary>
        None = 0,

        /// <summary>
        /// Enable support for the tap gesture.
        /// </summary>
        Tap = 1,

        /// <summary>
        /// Enable support for the double-tap gesture.
        /// </summary>
        DoubleTap = 2,

        /// <summary>
        /// Enable support for the hold gesture.
        /// </summary>
        Hold = 4,

        /// <summary>
        /// Enable support for the manipulation gesture, tracking changes to the hand's position.
        /// </summary>
        ManipulationTranslate = 8,

        /// <summary>
        /// Enable support for the navigation gesture, in the horizontal axis.
        /// </summary>
        NavigationX = 16,

        /// <summary>
        /// Enable support for the navigation gesture, in the vertical axis.
        /// </summary>
        NavigationY = 32,

        /// <summary>
        /// Enable support for the navigation gesture, in the depth axis.
        /// </summary>
        NavigationZ = 64,

        /// <summary>
        /// Enable support for the navigation gesture, in the horizontal axis using rails (guides).
        /// </summary>
        NavigationRailsX = 128,

        /// <summary>
        /// Enable support for the navigation gesture, in the vertical axis using rails (guides).
        /// </summary>
        NavigationRailsY = 256,
          
        /// <summary>
        /// Enable support for the navigation gesture, in the depth axis using rails (guides).
        /// </summary>
        NavigationRailsZ = 512
    }
}
