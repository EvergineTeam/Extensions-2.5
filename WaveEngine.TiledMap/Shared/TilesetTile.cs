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
    public class TilesetTile
    {
        #region Properties
        /// <summary>
        /// Gets the tile ID
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// Gets the tile probability in a terrain
        /// </summary>
        public double Probability { get; private set; }

        /// <summary>
        /// Gets the tileset tile properties
        /// </summary>
        public Dictionary<string, string> Properties { get; private set; }

        /// <summary>
        /// Gets the terrain associated edges
        /// </summary>
        public List<TilesetTerrain> TerrainEdges { get; private set; }

        /// <summary>
        /// Gets the terrain at the bottom left edge
        /// </summary>
        public TilesetTerrain BottomLeft { get; private set; }

        /// <summary>
        /// Gets the terrain at the bottom right edge
        /// </summary>
        public TilesetTerrain BottomRight { get; private set; }

        /// <summary>
        /// Gets the terrain at the top left edge
        /// </summary>
        public TilesetTerrain TopLeft { get; private set; }

        /// <summary>
        /// Gets the terrain at the top right edge
        /// </summary>
        public TilesetTerrain TopRight { get; private set; }

        /// <summary>
        /// Gets the associated tileset
        /// </summary>
        public Tileset Tileset { get; private set; }
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="TilesetTile" /> class.
        /// </summary>>
        /// <param name="tmxTilesetTile">The TMX parsed tileset tile.</param>
        /// <param name="tileset">The associated tileset</param>
        public TilesetTile(TmxTilesetTile tmxTilesetTile, Tileset tileset)
        {
            this.Tileset = tileset;
            this.ID = tmxTilesetTile.Id;
            this.Probability = tmxTilesetTile.Probability;
            this.Properties = new Dictionary<string, string>(tmxTilesetTile.Properties);

            this.TerrainEdges = new List<TilesetTerrain>();

            if (tmxTilesetTile.BottomRight != null)
            {
                this.BottomRight = this.Tileset.Terrains[tmxTilesetTile.BottomRight.Name];
                this.TerrainEdges.Add(this.BottomRight);
            }

            if (tmxTilesetTile.BottomLeft != null)
            {
                this.BottomLeft = this.Tileset.Terrains[tmxTilesetTile.BottomLeft.Name];
                this.TerrainEdges.Add(this.BottomLeft);
            }

            if (tmxTilesetTile.TopRight != null)
            {
                this.TopRight = this.Tileset.Terrains[tmxTilesetTile.TopRight.Name];
                this.TerrainEdges.Add(this.TopRight);
            }

            if (tmxTilesetTile.TopLeft != null)
            {
                this.TopLeft = this.Tileset.Terrains[tmxTilesetTile.TopLeft.Name];
                this.TerrainEdges.Add(this.TopLeft);
            }
        }
        #endregion
    }
}
