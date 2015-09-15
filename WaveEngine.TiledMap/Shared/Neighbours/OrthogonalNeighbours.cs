#region File Description
//-----------------------------------------------------------------------------
// OrthogonalNeighbours
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace WaveEngine.TiledMap
{
    public class OrthogonalNeighbours : NeighboursCollection
    {
        #region Properties
        /// <summary>
        /// Gets the top neighbour.
        /// </summary>
        /// <value>
        /// The top neighbour.
        /// </value>
        public override LayerTile Top
        {
            get { return this.tileMapLayer.GetLayerTileByMapCoordinates(this.center.X, this.center.Y - 1); }
        }

        /// <summary>
        /// Gets the top right neighbour.
        /// </summary>
        /// <value>
        /// The top right neighbour.
        /// </value>
        public override LayerTile TopRight
        {
            get { return this.tileMapLayer.GetLayerTileByMapCoordinates(this.center.X + 1, this.center.Y - 1); }
        }

        /// <summary>
        /// Gets the right neighbour.
        /// </summary>
        /// <value>
        /// The right neighbour.
        /// </value>
        public override LayerTile Right
        {
            get { return this.tileMapLayer.GetLayerTileByMapCoordinates(this.center.X + 1, this.center.Y); }
        }

        /// <summary>
        /// Gets the bottom right neighbour.
        /// </summary>
        /// <value>
        /// The bottom right neighbour.
        /// </value>
        public override LayerTile BottomRight
        {
            get { return this.tileMapLayer.GetLayerTileByMapCoordinates(this.center.X + 1, this.center.Y + 1); }
        }

        /// <summary>
        /// Gets the bottom neighbour.
        /// </summary>
        /// <value>
        /// The bottom neighbour.
        /// </value>
        public override LayerTile Bottom
        {
            get { return this.tileMapLayer.GetLayerTileByMapCoordinates(this.center.X, this.center.Y + 1); }
        }

        /// <summary>
        /// Gets the bottom left neighbour.
        /// </summary>
        /// <value>
        /// The bottom left neighbour.
        /// </value>
        public override LayerTile BottomLeft
        {
            get { return this.tileMapLayer.GetLayerTileByMapCoordinates(this.center.X - 1, this.center.Y + 1); }
        }

        /// <summary>
        /// Gets the left neighbour.
        /// </summary>
        /// <value>
        /// The left neighbour.
        /// </value>
        public override LayerTile Left
        {
            get { return this.tileMapLayer.GetLayerTileByMapCoordinates(this.center.X - 1, this.center.Y); }
        }

        /// <summary>
        /// Gets the top left neighbour.
        /// </summary>
        /// <value>
        /// The top left neighbour.
        /// </value>
        public override LayerTile TopLeft
        {
            get { return this.tileMapLayer.GetLayerTileByMapCoordinates(this.center.X - 1, this.center.Y - 1); }
        } 
        #endregion

        #region Initilization
        /// <summary>
        /// Initializes a new instance of the <see cref="OrthogonalNeighbours"/> class.
        /// </summary>
        /// <param name="tileMapLayer">The tile map layer.</param>
        /// <param name="center">The center.</param>
        public OrthogonalNeighbours(TiledMapLayer tileMapLayer, LayerTile center)
            : base(tileMapLayer, center)
        {

        } 
        #endregion
    }
}
