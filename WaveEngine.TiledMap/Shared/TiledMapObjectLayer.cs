#region File Description
//-----------------------------------------------------------------------------
// TiledMapObjectLayer
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
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
#endregion

namespace WaveEngine.TiledMap
{
    /// <summary>
    /// The represent the TMX object layer 
    /// </summary>
    public class TiledMapObjectLayer
    {
        #region Properties
        /// <summary>
        /// Gets the object layer name
        /// </summary>
        public string ObjectLayerName { get; private set; }

        /// <summary>
        /// Gets the object layer color
        /// </summary>
        public Color Color { get; private set; }

        /// <summary>
        /// Gets the object layer Opacity
        /// </summary>
        public double Opacity { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this object layer is visible
        /// </summary>
        public bool Visible { get; private set; }

        /// <summary>
        /// Gets the object layer offset
        /// </summary>
        public Vector2 Offset { get; private set; }

        /// <summary>
        /// Gets the objects contained in the object layer.
        /// </summary>
        public List<TiledMapObject> Objects { get; private set; }

        /// <summary>
        /// Gets the object layer properties
        /// </summary>
        public IReadOnlyDictionary<string, string> Properties { get; private set; } 
        #endregion

        #region Initialziation
        /// <summary>
        /// Initializes a new instance of the <see cref="TiledMapObjectLayer" /> class.
        /// </summary>
        /// <param name="tmxObjectLayer">The TMX parsed object layer</param>
        public TiledMapObjectLayer(TmxObjectGroup tmxObjectLayer)
        {
            this.ObjectLayerName = tmxObjectLayer.Name;
            this.Color = new Color(tmxObjectLayer.Color.R, tmxObjectLayer.Color.G, tmxObjectLayer.Color.B, tmxObjectLayer.Color.A);
            this.Opacity = tmxObjectLayer.Opacity;
            this.Visible = tmxObjectLayer.Visible;
            this.Offset = new Vector2((float)tmxObjectLayer.OffsetX, (float)tmxObjectLayer.OffsetY);

            this.Objects = new List<TiledMapObject>();
            this.Properties = new Dictionary<string, string>(tmxObjectLayer.Properties);

            foreach(var tmxObject in tmxObjectLayer.Objects)
            {
                var tiledMapObject = new TiledMapObject(tmxObject);
                this.Objects.Add(tiledMapObject);
            }
        } 
        #endregion
    }
}
