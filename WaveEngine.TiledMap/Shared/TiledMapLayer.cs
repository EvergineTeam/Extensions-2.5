#region File Description
//-----------------------------------------------------------------------------
// TiledMapLayer
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

                Vector2 tileLocalPosition;
                this.GetTilePosition(tile, selectedTileset, out tileLocalPosition);
                tile.LocalPosition = tileLocalPosition;

                int x = i % this.tiledMap.Width;
                int y = i / this.tiledMap.Width;
                this.tileTable[x, y] = tile;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Obtains the tile position
        /// </summary>
        /// <param name="tile">The tile</param>
        /// <param name="tileset">The tileset</param>
        /// <param name="position">The tile position</param>
        internal void GetTilePosition(LayerTile tile, Tileset tileset, out Vector2 position)
        {
            switch (this.tiledMap.Orientation)
            {
                case TiledMapOrientationType.Orthogonal:
                    position = new Vector2(
                        tile.X * this.tiledMap.TileWidth,
                        tile.Y * this.tiledMap.TileHeight);
                    break;

                case TiledMapOrientationType.Isometric:
                    position = new Vector2(
                        ((tile.X - tile.Y) * this.tiledMap.TileWidth * 0.5f) + (this.tiledMap.Height * this.tiledMap.TileWidth * 0.5f) - this.tiledMap.TileWidth * 0.5f,
                        (tile.X + tile.Y) * this.tiledMap.TileHeight * 0.5f);
                    break;

                case TiledMapOrientationType.Staggered:
                case TiledMapOrientationType.Hexagonal:

                    int sideLengthX = 0;
                    int sideLengthY = 0;

                    float rowSize = 0;
                    float columSize = 0;
                    int staggerIndexSign = this.tiledMap.StaggerIndex == TiledMapStaggerIndexType.Odd ? 1 : -1;
                    var staggerAxisOffset = this.tiledMap.StaggerIndex == TiledMapStaggerIndexType.Even ? 0.5f : 0;

                    if (this.tiledMap.Orientation == TiledMapOrientationType.Hexagonal)
                    {
                        if (this.tiledMap.StaggerAxis == TiledMapStaggerAxisType.X)
                        {
                            sideLengthX = this.tiledMap.HexSideLength;
                        }
                        else
                        {
                            sideLengthY = this.tiledMap.HexSideLength;
                        }
                    }

                    if (this.tiledMap.StaggerAxis == TiledMapStaggerAxisType.X)
                    {
                        rowSize = (tile.X / 2) + ((tile.X % 2) * 0.5f);
                        columSize = staggerAxisOffset + tile.Y + ((tile.X % 2) * 0.5f * staggerIndexSign);
                    }
                    else
                    {
                        rowSize = staggerAxisOffset + tile.X + ((tile.Y % 2) * 0.5f * staggerIndexSign);
                        columSize = tile.Y * 0.5f;
                    }

                    position = new Vector2(
                            rowSize * (this.tiledMap.TileWidth + sideLengthX),
                            columSize * (this.tiledMap.TileHeight + sideLengthY));
                    break;

                default:
                    position = Vector2.Zero;
                    break;
            }

            position.X += tileset.XDrawingOffset;
            position.Y += tileset.YDrawingOffset + this.tiledMap.TileHeight - tileset.TileHeight;
        }

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

            if (x >= 0
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
        }
        #endregion
    }
}
