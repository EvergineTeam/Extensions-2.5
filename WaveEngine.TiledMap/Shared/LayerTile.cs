#region File Description
//-----------------------------------------------------------------------------
// Tile
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledSharp;
using WaveEngine.Common.Math;
#endregion

namespace WaveEngine.TiledMap
{
    /// <summary>
    /// Represent a single Tile in a Layer
    /// </summary>
    public class LayerTile
    {
        #region Properties
        /// <summary>
        /// Gets the tile ID.
        /// </summary>
        public int Id
        {
            get;
            private set;
        }

        /// <summary>
        /// The X coordinate of the Tile.
        /// </summary>
        public int X
        {
            get;
            private set;
        }

        /// <summary>
        /// The Y coordinate of the Tile.
        /// </summary>
        public int Y
        {
            get;
            private set;
        }

        /// <summary>
        /// The tile has Horizontal Flip
        /// </summary>
        public bool HorizontalFlip
        {
            get;
            private set;
        }

        /// <summary>
        /// The tile has Vertical Flip
        /// </summary>
        public bool VerticalFlip
        {
            get;
            private set;
        }

        /// <summary>
        /// The tile has Diagonal Flip
        /// </summary>
        public bool DiagonalFlip
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the associated tileset
        /// </summary>
        public Tileset Tileset
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the tileset tile
        /// </summary>
        public TilesetTile TilesetTile
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the tile local position
        /// </summary>
        public Vector2 LocalPosition
        {
            get;
            internal set;
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="LayerTile" /> class.
        /// </summary>>
        /// <param name="tmxTile">The TMX parsed tile.</param>
        /// <param name="tileset">Th</param>
        public LayerTile(TmxLayerTile tmxTile, Tileset tileset)
        {
            this.X = tmxTile.X;
            this.Y = tmxTile.Y;
            this.HorizontalFlip = tmxTile.HorizontalFlip;
            this.VerticalFlip = tmxTile.VerticalFlip;
            this.DiagonalFlip = tmxTile.DiagonalFlip;

            if (tileset != null)
            {
                this.Tileset = tileset;
                this.Id = tmxTile.Gid - this.Tileset.FirstGid;

                this.TilesetTile = this.Tileset.TilesTable[this.Id];
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}, {1} [{2}]", this.X, this.Y, this.Id);
        }
        #endregion
    }
}
