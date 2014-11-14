#region File Description
//-----------------------------------------------------------------------------
// AntialiasingLens
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
    /// Represent an antaliasing as postprocessing filter.
    /// </summary>
    public class AntialiasingLens : Lens
    {
        #region Properties

        /// <summary>
        /// Gets or sets the span_max.
        /// </summary>
        /// <value>
        /// The span_max.
        /// </value>
        public float Span_max
        {
            get
            {
                return (this.material as AntialiasingMaterial).Span_max;
            }

            set
            {
                (this.material as AntialiasingMaterial).Span_max = value;
            }
        }

        /// <summary>
        /// Gets or sets the reduce_ mul.
        /// </summary>
        /// <value>
        /// The reduce_ mul.
        /// </value>
        public float Reduce_Mul
        {
            get
            {
                return (this.material as AntialiasingMaterial).Reduce_Mul;
            }

            set
            {
                (this.material as AntialiasingMaterial).Reduce_Mul = value;
            }
        }

        /// <summary>
        /// Gets or sets the reduce_ minimum.
        /// </summary>
        /// <value>
        /// The reduce_ minimum.
        /// </value>
        public float Reduce_Min
        {
            get
            {
                return (this.material as AntialiasingMaterial).Reduce_Min;
            }

            set
            {
                (this.material as AntialiasingMaterial).Reduce_Min = value;
            }
        }

        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="AntialiasingLens"/> class.
        /// </summary>
        public AntialiasingLens()
        {
            this.material = new AntialiasingMaterial();
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Renders to image.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Render(TimeSpan gameTime)
        {
            var mat = (this.material as AntialiasingMaterial);
            if(mat.TexcoordOffset == Vector2.Zero)
            {
                mat.TexcoordOffset.X = 1f / this.Source.Width;
                mat.TexcoordOffset.Y = 1f / this.Source.Height;
            }

            mat.Texture = this.Source;
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
