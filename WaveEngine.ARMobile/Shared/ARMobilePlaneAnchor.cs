// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

using WaveEngine.Common.Math;

namespace WaveEngine.ARMobile
{
    /// <summary>
    ///  The AR mobile anchor for planes
    /// </summary>
    public class ARMobilePlaneAnchor : ARMobileAnchor
    {
        /// <summary>
        /// The plane local position. It must be transformed by the <see cref="ARMobileAnchor.Transform"/> to get the center world coordinates.
        /// </summary>
        public Vector3 Center;

        /// <summary>
        /// The estimated width and length of the plane.
        /// </summary>
        public Vector3 Size;

        /// <summary>
        /// The 3D vertices of a convex polygon approximating the plane.
        /// These values are in the plane's local x-z plane (y=0) and must be transformed
        /// by the <see cref="ARMobileAnchor.Transform"/> to get the boundary in world coordinates.
        /// </summary>
        public Vector3[] BoundaryPolygon;

        /// <summary>
        /// The type of this plane
        /// </summary>
        public PlaneAnchorType Type;
    }
}
