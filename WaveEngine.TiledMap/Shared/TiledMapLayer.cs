#region File Description
//-----------------------------------------------------------------------------
// TiledMapLayer
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using TiledSharp;
using WaveEngine.Common.Attributes;
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
    [DataContract]
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

        /// The TMX Layer name
        [DataMember]
        private string tmxLayerName;

        /// <summary>
        /// The layer is loaded
        /// </summary>
        private bool isLayerLoaded;

        #region Properties
        /// <summary>
        /// Gets the layer names.
        /// </summary>
        /// <value>
        /// The layer names.
        /// </value>
        [DontRenderProperty]
        public IEnumerable<string> LayerNames
        {
            get
            {
                if (this.isLayerLoaded)
                {
                    return this.tiledMap.TmxMap.Layers.Select(l => l.Name);
                }

                return new List<string>();
            }
        }

        /// <summary>
        /// Gets the tiles list
        /// </summary>
        public IEnumerable<LayerTile> Tiles
        {
            get { return this.tiles; }
        }

        /// <summary>
        /// Gets or sets the TMX Layer name
        /// </summary>
        [RenderPropertyAsSelector("LayerNames")]
        public string TmxLayerName
        {
            get
            {
                return this.tmxLayerName;
            }

            set
            {
                this.tmxLayerName = value;
                if (this.isInitialized)
                {
                    this.RefreshLayer();
                }
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="TiledMapLayer" /> class.
        /// </summary>
        public TiledMapLayer()
        {
        }

        /// <summary>
        /// Sets the default values
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.isLayerLoaded = false;
        }

        /// <summary>
        /// Initializes this component
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            this.LoadLayer();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets a <see cref="NeighboursCollection"/> that contains the neighbour of the spefied tile.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">The tile argument can not be null</exception>
        public NeighboursCollection GetNeighboursFromTile(LayerTile tile)
        {
            if (tile == null)
            {
                throw new ArgumentNullException("The tile argument can not be null");
            }

            if (!this.isLayerLoaded)
            {
                return null;
            }

            switch (tiledMap.Orientation)
            {
                case TiledMapOrientationType.Orthogonal:
                    return new OrthogonalNeighbours(this, tile);

                case TiledMapOrientationType.Isometric:
                    return new IsometricNeighbours(this, tile);

                case TiledMapOrientationType.Staggered:
                    return new StaggeredNeighbours(this, tile, tiledMap.StaggerAxis, tiledMap.StaggerIndex);

                case TiledMapOrientationType.Hexagonal:
                    return new HexagonalNeighbours(this, tile, tiledMap.StaggerAxis, tiledMap.StaggerIndex);

                default:
                    return null;
            }
        }

        /// <summary>
        /// Get Tile coordinates (x, y) by world position
        /// </summary>
        /// <param name="position">The world position</param>
        public LayerTile GetLayerTileByWorldPosition(Vector2 position)
        {
            if (!this.isLayerLoaded)
            {
                return null;
            }

            int tileX, tileY;
            this.tiledMap.GetTileCoordinatesByWorldPosition(position, out tileX, out tileY);
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
            LayerTile result = null;

            if (this.isLayerLoaded
             && x >= 0
             && x < this.tiledMap.Width
             && y >= 0
             && y < this.tiledMap.Height)
            {
                result = this.tileTable[x, y];
            }

            return result;
        }

        /// <summary>
        /// Dispose this component
        /// </summary>
        public void Dispose()
        {
            this.UnloadLayer();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Refresh the layer
        /// </summary>
        private void RefreshLayer()
        {
            this.UnloadLayer();
            this.LoadLayer();
        }

        /// <summary>
        /// Unload layer
        /// </summary>
        private void UnloadLayer()
        {
            this.tiledMap = null;
            this.tileTable = null;
            this.tiles = null;
            this.tmxLayer = null;            

            this.isLayerLoaded = false;
        }

        /// <summary>
        /// Initialize Layer
        /// </summary>
        private void LoadLayer()
        {
            this.tiles = new List<LayerTile>();

            if(this.Owner.Parent == null)
            {
                return;
            }

            this.tiledMap = this.Owner.Parent.FindComponent<TiledMap>();

            if (this.tiledMap != null && this.tiledMap.TmxMap != null)
            {
                this.tmxLayer = this.tiledMap.TmxMap.Layers.FirstOrDefault(l => l.Name == this.tmxLayerName);

                if (this.tmxLayer != null)
                {
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

                        Vector2 tileLocalPosition;
                        this.tiledMap.GetTilePosition(tile.X, tile.Y, selectedTileset, out tileLocalPosition);
                        tile.LocalPosition = tileLocalPosition;

                        int x = i % this.tiledMap.Width;
                        int y = i / this.tiledMap.Width;
                        this.tileTable[x, y] = tile;
                    }

                    this.isLayerLoaded = true;
                    this.NeedRefresh = true;
                }
            }
        }
        #endregion
    }
}
