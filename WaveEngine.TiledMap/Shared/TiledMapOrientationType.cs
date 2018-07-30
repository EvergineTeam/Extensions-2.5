// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace WaveEngine.TiledMap
{
    /// <summary>
    /// Map orientation. Tiled supports "orthogonal", "isometric", "staggered" and "hexagonal" (since 0.11.0) at the moment.
    /// </summary>
    public enum TiledMapOrientationType
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown,

        /// <summary>
        /// Orthogonal
        /// </summary>
        Orthogonal,

        /// <summary>
        /// Isometric
        /// </summary>
        Isometric,

        /// <summary>
        /// Staggered
        /// </summary>
        Staggered,

        /// <summary>
        /// Hexagonal
        /// </summary>
        Hexagonal
    }
}
