// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Usings Statements
#endregion

namespace WaveEngine.Vuforia
{
    /// <summary>
    /// The world center mode defines how the relative coordinates between
    /// Trackables and camera are translated into Wave Engine world coordinates.
    /// If a world center is present, the ARCamera in the scene is
    /// transformed with respect to that.
    /// </summary>
    public enum WorldCenterMode
    {
        /// <summary>
        /// The camera uses the first Trackable that comes into view as the world
        /// center (world center changes during runtime).
        /// </summary>
        FirstTarget,

        /// <summary>
        /// User defines a single Trackable that defines the world center.
        /// </summary>
        SpecificTarget,

        /// <summary>
        /// Do not define a world center but only move Trackables with respect
        /// to a fixed ARCamera.
        /// </summary>
        Camera,
    }
}
