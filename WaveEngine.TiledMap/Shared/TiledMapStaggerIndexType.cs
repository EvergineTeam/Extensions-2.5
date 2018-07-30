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
    /// Stagger Index. For staggered maps, indicates if the tiles index is odd or even.
    /// </summary>
    public enum TiledMapStaggerIndexType
    {
        /// <summary>
        /// Odd
        /// </summary>
        Odd = 0,

        /// <summary>
        /// Even
        /// </summary>
        Even = 1,
    }
}
