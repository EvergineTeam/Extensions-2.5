// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Usings Statements
using System;
#endregion

namespace WaveEngine.Kinect.Enums
{
    /// <summary>
    /// Sensor Properties Enumeration
    /// </summary>
    [Flags]
    public enum KinectSources
    {
        /// <summary>
        /// The none
        /// </summary>
        None = 0,

        /// <summary>
        /// The color
        /// </summary>
        Color = 1,

        /// <summary>
        /// The infrared
        /// </summary>
        Infrared = 2,

        /// <summary>
        /// The long exposure infrared
        /// </summary>
        //// LongExposureInfrared = 4,

        /// <summary>
        /// The depth
        /// </summary>
        Depth = 8,

        /// <summary>
        /// The body index
        /// </summary>
        //// BodyIndex = 16,

        /// <summary>
        /// The body
        /// </summary>
        Body = 32,

        /// <summary>
        /// The audio
        /// </summary>
        //// Audio = 64,
        Face = 128,
    }
}
