// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;

namespace WaveEngine.ARMobile
{
    /// <summary>
    /// Options for whether and how flat surfaces are detected in captured images
    /// </summary>
    [Flags]
    public enum PlaneDetectionType
    {
        /// <summary>
        /// Plane detection is disabled
        /// </summary>
        None = 0,

        /// <summary>
        /// The session detects planar surfaces that are perpendicular to gravity
        /// </summary>
        Horizontal = 1,

        /// <summary>
        /// The session detects surfaces that are parallel to gravity (regardless of other orientation). (ARKit 1.5 only, not supported on ARCore)
        /// </summary>
        Vertical = 2
    }
}
