#region File Description
//-----------------------------------------------------------------------------
// TiledMapLayer
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
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.TiledMap
{
    /// <summary>
    /// Gets the tile layer in the TiledMap
    /// </summary>
    public class TiledMapLayer : Component, IDisposable
    {
        /// <summary>
        /// The tmx parsed layer
        /// </summary>
        private TmxLayer tmxLayer;

        /// <summary>
        /// The tile map instance.
        /// </summary>
        private TiledMap tiledMap;

        /// <summary>
        /// Tile list of this layer
        /// </summary>
        private List<LayerTile> tiles;

        /// <summary>
        /// Tile arranged by its map coordinates
        /// </summary>
        private LayerTile[,] tileTable;

        /// <summary>
        /// Meshes need to be refreshed
        /// </summary>
        internal bool NeedRefresh;

        #region Properties
        /// <summary>
        /// Gets the tiles list
        /// </summary>
        public IEnumerable<LayerTile> Tiles
        {
            get { return this.tiles; }
        }

        /// <summary>
        /// Gets the tiled map
        /// </summary>
        public TiledMap TiledMap
        {
            get { return this.tiledMap; }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="TiledMapLayer" /> class.
        /// </summary>
        /// <param name="tmxLayer">The tmx parsed layer.</param>
        public TiledMapLayer(TmxLayer tmxLayer)
        {
            this.tmxLayer = tmxLayer;
            this.tiles = new List<LayerTile>();
            this.NeedRefresh = true;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Dispose this component
        /// </summary>
        public void Dispose()
        {
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Initializes this component
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            this.tiledMap = this.Owner.Parent.FindComponent<TiledMap>();

            this.tileTable = new LayerTile[this.tiledMap.Width, this.tiledMap.Height];

            for (int i = 0; i < this.tmxLayer.Tiles.Count; i++)
            {
                var tmxTile = this.tmxLayer.Tiles[i];

                Tileset selectedTileset = null;

                if (tmxTile.Gid > 0)
                {
                    foreach (var tileset in this.tiledMap.Tilesets)
                    {
                        if (tmxTile.Gid <= tileset.LastGid)
                        {
                            selectedTileset = tileset;
                            break;
                        }

                    }
                }

                LayerTile tile = new LayerTile(tmxTile, selectedTileset);
                this.tiles.Add(tile);

                int x = i % this.tiledMap.Width;
                int y = i / this.tiledMap.Width;
                this.tileTable[x, y] = tile;
            }
        }
        #endregion


        #region Public Methods

        /// <summary>
        /// Get Tile coordinates (x, y) by world position
        /// </summary>
        /// <param name="position">The world position</param>
        public LayerTile GetLayerTileByWorldPosition(Vector2 position)
        {
            int tileX, tileY;
            this.tiledMap.GetTileCoordinatesByPosition(position, out tileX, out tileY);
            return this.GetLayerTileByMapCoordinates(tileX, tileY);
        }


        /// <summary>
        /// Gets a tile by its map coordinates
        /// </summary>        
        /// <param name="x">The X coord of the tile.</param>
        /// <param name="y">The Y coord of the tile.</param>
        /// <returns></returns>
        public LayerTile GetLayerTileByMapCoordinates(int x, int y)
        {
            if (x < 0 || x >= this.tiledMap.Width)
            {
                throw new IndexOutOfRangeException(string.Format("x param must be in [{0}, {1}] range", 0, this.tiledMap.Width - 1));
            }

            if (y < 0 || y >= this.tiledMap.Height)
            {
                throw new IndexOutOfRangeException(string.Format("y param must be in [{0}, {1}] range", 0, this.tiledMap.Height - 1));
            }

            return this.tileTable[x, y];
        }
        #endregion
    }
}
