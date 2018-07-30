// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;

namespace WaveEngine.ARMobile
{
    /// <summary>
    /// Possible types of hit-test searching to perform or type of objects found by a search.
    /// </summary>
    [Flags]
    public enum ARMobileHitType
    {
        /// <summary>
        /// A point automatically identified as part of a continuous surface, but without a corresponding anchor.
        /// </summary>
        FeaturePoint = 1,

        /// <summary>
        /// A real-world planar surface detected by the search (without a corresponding anchor), whose orientation is perpendicular to gravity.
        /// For ARCore, this option is the same as <see cref="ExistingPlane"/>
        /// </summary>
        EstimatedHorizontalPlane = 2,

        /// <summary>
        /// A real-world planar surface detected by the search, whose orientation is parallel to gravity. (ARKit 1.5 only, not supported on ARCore)
        /// </summary>
        EstimatedVerticalPlane = 4,

        /// <summary>
        /// A plane anchor already in the scene (detected with the <see cref="ARMobileService.PlaneDetection"/> option), without considering the plane's size.
        /// </summary>
        ExistingPlane = 8,

        /// <summary>
        /// A plane anchor already in the scene (detected with the <see cref="ARMobileService.PlaneDetection"/> option), respecting the plane's limited size.
        /// </summary>
        ExistingPlaneUsingExtent = 16,

        /// <summary>
        /// A plane anchor already in the scene (detected with the <see cref="ARMobileService.PlaneDetection"/> option), respecting the plane's estimated size and shape.
        /// </summary>
        ExistingPlaneUsingGeometry = 32
    }
}
