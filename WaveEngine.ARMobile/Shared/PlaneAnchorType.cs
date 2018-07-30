// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

namespace WaveEngine.ARMobile
{
    /// <summary>
    /// Simple summary of the normal vector of a plane, for filtering purposes
    /// </summary>
    public enum PlaneAnchorType
    {
        /// <summary>
        /// A horizontal plane facing upward (e.g. floor or tabletop)
        /// </summary>
        HorizontalUpwardFacing = 0,

        /// <summary>
        /// A horizontal plane facing downward (e.g. a ceiling)
        /// </summary>
        HorizontalDownwardFacing = 1,

        /// <summary>
        /// A vertical plane (e.g. a wall)
        /// </summary>
        Vertical = 2,
    }
}
