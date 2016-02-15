#region File Description
//-----------------------------------------------------------------------------
// TilesetTerrain
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
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
    public class TilesetTerrain
    {
        #region Properties
        /// <summary>
        /// Gets the terrain name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the terrain properties
        /// </summary>
        public Dictionary<string, string> Properties { get; private set; }

        /// <summary>
        /// Gets the tile ID
        /// </summary>
        public int Tile { get; private set; } 
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="TilesetTerrain" /> class.
        /// </summary>
        /// <param name="tmxTerrain">The tmx terrain</param>
        public TilesetTerrain(TmxTerrain tmxTerrain)
        {
            this.Name = tmxTerrain.Name;
            this.Properties = new Dictionary<string, string>(tmxTerrain.Properties);
            this.Tile = tmxTerrain.Tile;
        }
    }
}