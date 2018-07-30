// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace WaveEngine.TiledMap
{
    /// <summary>
    /// IsometricNeighbours
    /// </summary>
    public class IsometricNeighbours : OrthogonalNeighbours
    {
        #region Initilization

        /// <summary>
        /// Initializes a new instance of the <see cref="IsometricNeighbours"/> class.
        /// </summary>
        /// <param name="tileMapLayer">The tile map layer.</param>
        /// <param name="center">The center.</param>
        public IsometricNeighbours(TiledMapLayer tileMapLayer, LayerTile center)
            : base(tileMapLayer, center)
        {
        }
        #endregion
    }
}
