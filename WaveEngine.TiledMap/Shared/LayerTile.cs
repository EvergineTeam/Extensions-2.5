#region File Description
//-----------------------------------------------------------------------------
// Tile
//
// Copyright © 2014 Wave Corporation
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
        /// Gets the global tile ID.
        /// </summary>
        public int Gid
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
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="LayerTile" /> class.
        /// </summary>>
        /// <param name="tmxTile">The TMX parsed tile.</param>
        /// <param name="tileset">Th</param>
        public LayerTile(TmxLayerTile tmxTile, Tileset tileset)
        {
            this.Gid = tmxTile.Gid;
            this.X = tmxTile.X;
            this.Y = tmxTile.Y;
            this.HorizontalFlip = tmxTile.HorizontalFlip;
            this.VerticalFlip = tmxTile.VerticalFlip;
            this.DiagonalFlip = tmxTile.DiagonalFlip;

            if (tileset != null)
            {
                this.Tileset = tileset;
                int tileId = this.Gid - this.Tileset.FirstGid;

                this.TilesetTile = this.Tileset.TilesTable[tileId];
            }
        }
        #endregion
    }
}
