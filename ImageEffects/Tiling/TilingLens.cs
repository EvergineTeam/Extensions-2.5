#region File Description
//-----------------------------------------------------------------------------
// TilingLens
//
// Copyright © 2014 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Usings Statements
using System;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.ImageEffects
{
    /// <summary>
    /// Represent an tiling as postprocessing filter.
    /// </summary>
    public class TilingLens : Lens
    {
        #region Properties

        /// <summary>
        /// Gets or sets the span_max, default value is Vector3(0.7f, 0.7f, 0.7f).
        /// </summary>
        /// <value>
        /// The span_max.
        /// </value>
        public Vector3 EdgeColor
        {
            get
            {
                return (this.material as TilingMaterial).EdgeColor;
            }

            set
            {
                (this.material as TilingMaterial).EdgeColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the reduce_ mul, default value 75f.
        /// </summary>
        /// <value>
        /// The reduce_ mul.
        /// </value>
        public float NumTiles
        {
            get
            {
                return (this.material as TilingMaterial).NumTiles;
            }

            set
            {
                (this.material as TilingMaterial).NumTiles = value;
            }
        }

        /// <summary>
        /// Gets or sets the reduce_ minimum, default value 0.15f.
        /// </summary>
        /// <value>
        /// The reduce_ minimum.
        /// </value>
        public float Threshhold
        {
            get
            {
                return (this.material as TilingMaterial).Threshhold;
            }

            set
            {
                (this.material as TilingMaterial).Threshhold = value;
            }
        }

        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="AntialiasingLens"/> class.
        /// </summary>
        public TilingLens()
        {
            this.material = new TilingMaterial();
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Renders to image.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Render(TimeSpan gameTime)
        {
            (this.material as TilingMaterial).Texture = this.Source;
            this.RenderToImage(this.Destination, this.material);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
        }
        #endregion
    }
}
