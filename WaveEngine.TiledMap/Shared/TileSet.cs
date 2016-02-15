#region File Description
//-----------------------------------------------------------------------------
// TileSet
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledSharp;
using WaveEngine.Common.Graphics;
using WaveEngine.Framework.Graphics;
using WaveEngine.Materials;
#endregion

namespace WaveEngine.TiledMap
{
    /// <summary>
    /// A TMX tileset wrapper class 
    /// </summary>
    public class Tileset : IDisposable
    {
        /// <summary>
        /// The Tiled map
        /// </summary>
        private TiledMap tiledMap;

        /// <summary>
        /// The tileset terrains
        /// </summary>
        private Dictionary<string, TilesetTerrain> terrains;

        /// <summary>
        /// The tileset tiles properties
        /// </summary>
        private List<TilesetTile> tiles;

        #region Properties
        /// <summary>
        /// Gets the name of the tileset.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the (maximum) width of the tiles in this tileset.
        /// </summary>
        public int TileWidth
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the (maximum) height of the tiles in this tileset.
        /// </summary>
        public int TileHeight
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the first global tile ID of this tileset (this global ID maps to the first tile in this tileset).
        /// </summary>
        public int FirstGid
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the last global tile ID of this tileset (this global ID maps to the last tile in this tileset).
        /// </summary>
        public int LastGid
        {
            //get { return tmxTileset.FirstGid + (this.XTilesCount * this.YTilesCount); }
            get;
            private set;
        }

        /// <summary>
        /// Gets the spacing in pixels between the tiles in this tileset (applies to the tileset image).
        /// </summary>
        public int Spacing
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the margin around the tiles in this tileset (applies to the tileset image).
        /// </summary>
        public int Margin
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the number of tiles in the X axis
        /// </summary>
        public int XTilesCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the number of tiles in the Y axis
        /// </summary>
        public int YTilesCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the drawing offset in the X axis.
        /// </summary>
        public int XDrawingOffset
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the drawing offset in the Y axis.
        /// </summary>
        public int YDrawingOffset
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the tileset image texture.
        /// </summary>
        public Texture2D Image
        {
            get;
            private set;
        }

        /// <summary>
        /// Thetileset terrains
        /// </summary>
        public ReadOnlyDictionary<string, TilesetTerrain> Terrains
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the tileset tiles arranged in a table
        /// </summary>
        public TilesetTile[] TilesTable { get; private set; }
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="Tileset" /> class.
        /// </summary>
        /// <param name="tmxTileset">The TMX parsed tileset</param>
        /// <param name="tiledMap">The tiled map</param>
        public Tileset(TmxTileset tmxTileset, TiledMap tiledMap)
        {
            this.tiledMap = tiledMap;

            this.Name = tmxTileset.Name;
            this.TileWidth = tmxTileset.TileWidth;
            this.TileHeight = tmxTileset.TileHeight;
            this.Spacing = tmxTileset.Spacing;
            this.Margin = tmxTileset.Margin;
            this.XTilesCount = (int)(tmxTileset.Image.Width - (2 * this.Margin) + this.Spacing) / (this.TileWidth + this.Spacing);
            this.YTilesCount = (int)(tmxTileset.Image.Height - (2 * this.Margin) + this.Spacing) / (this.TileHeight + this.Spacing);
            this.XDrawingOffset = (int)tmxTileset.TileOffset.X;
            this.YDrawingOffset = (int)tmxTileset.TileOffset.Y;

            this.FirstGid = tmxTileset.FirstGid;
            this.LastGid = tmxTileset.FirstGid + (this.XTilesCount * this.YTilesCount) - 1;

            this.terrains = new Dictionary<string, TilesetTerrain>();
            this.Terrains = new ReadOnlyDictionary<string, TilesetTerrain>(this.terrains);
            foreach (var tmxTerrain in tmxTileset.Terrains)
            {
                this.terrains.Add(tmxTerrain.Name, new TilesetTerrain(tmxTerrain));
            }

            this.tiles = new List<TilesetTile>();
            this.TilesTable = new TilesetTile[this.XTilesCount * this.YTilesCount];
            foreach (var tmxTilesetTile in tmxTileset.Tiles)
            {
                var tilesetTile = new TilesetTile(tmxTilesetTile, this);

                this.tiles.Add(tilesetTile);
                this.TilesTable[tilesetTile.ID] = tilesetTile;
            }            

            string fullPath = tmxTileset.Image.Source;            
            string relativePath = fullPath.Substring(fullPath.IndexOf("Content"));
            this.Image = this.tiledMap.Owner.Scene.Assets.LoadAsset<Texture2D>(relativePath);            
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Dispose this tileset instance
        /// </summary>
        public void Dispose()
        {
            this.tiledMap.Owner.Scene.Assets.UnloadAsset(this.Image.AssetPath);
        }
        #endregion
    }
}
