// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

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
    /// The represent the TMX image layer
    /// </summary>
    public class TiledMapImageLayer
    {
        #region Properties

        /// <summary>
        /// Gets the image layer name
        /// </summary>
        public string ImageLayerName { get; private set; }

        /// <summary>
        /// Gets the image layer Opacity
        /// </summary>
        public double Opacity { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this image layer is visible
        /// </summary>
        public bool Visible { get; private set; }

        /// <summary>
        /// Gets the image layer offset
        /// </summary>
        public Vector2 Offset { get; private set; }

        /// <summary>
        /// Gets the image layer image texture.
        /// </summary>
        public string ImagePath
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the object layer properties
        /// </summary>
        public IReadOnlyDictionary<string, string> Properties { get; private set; }
        #endregion

        #region Initialziation

        /// <summary>
        /// Initializes a new instance of the <see cref="TiledMapImageLayer" /> class.
        /// </summary>
        /// <param name="tmxImageLayer">The TMX parsed image layer</param>
        /// <param name="tiledMap">The tiled map</param>
        public TiledMapImageLayer(TmxImageLayer tmxImageLayer, TiledMap tiledMap)
        {
            this.ImageLayerName = tmxImageLayer.Name;
            this.Opacity = tmxImageLayer.Opacity;
            this.Visible = tmxImageLayer.Visible;
            this.Offset = new Vector2((float)tmxImageLayer.OffsetX, (float)tmxImageLayer.OffsetY);

            this.Properties = new Dictionary<string, string>(tmxImageLayer.Properties);

            string fullPath = tmxImageLayer.Image.Source;
            string relativePath = fullPath.Substring(fullPath.IndexOf("Content"));
            this.ImagePath = relativePath;
        }
        #endregion
    }
}
