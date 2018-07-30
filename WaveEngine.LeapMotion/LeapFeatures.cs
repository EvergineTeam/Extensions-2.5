// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Usings Statements
using System;
#endregion

namespace WaveEngine.LeapMotion
{
    /// <summary>
    /// Supported LeapMotion features.
    /// </summary>
    [Flags]
    public enum LeapFeatures
    {
        /// <summary>
        /// None will be updated.
        /// </summary>
        None = 0,

        /// <summary>
        /// Update the hands.
        /// </summary>
        Hands = 1,

        /// <summary>
        /// Update gestures.
        /// </summary>
        Gestures = 2,

        /// <summary>
        /// Update service textures.
        /// </summary>
        CameraImages = 4,

        /// <summary>
        /// Enable HMD mode.
        /// </summary>
        HMDMode = 8,
    }
}
