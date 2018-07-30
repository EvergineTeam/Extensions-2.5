// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

namespace WaveEngine.ARMobile
{
    /// <summary>
    /// Options for how a scene coordinate system is constructed based on real-world device motion
    /// </summary>
    public enum ARMobileWorldAlignment
    {
        /// <summary>
        /// The coordinate system's y-axis is parallel to gravity,
        /// and its origin is the initial position of the device.
        /// </summary>
        Gravity = 0,

        /// <summary>
        /// The coordinate system's y-axis is parallel to gravity,
        /// its x- and z-axes are oriented to compass heading,
        /// and its origin is the initial position of the device. (ARKit only)
        /// </summary>
        GravityAndHeading = 1,

        /// <summary>
        /// The scene coordinate system is locked to match the orientation of the camera. (ARKit only)
        /// </summary>
        Camera = 2
    }
}
