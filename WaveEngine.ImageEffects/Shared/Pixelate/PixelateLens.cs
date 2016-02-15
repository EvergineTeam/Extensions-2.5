#region File Description
//-----------------------------------------------------------------------------
// PixelateLens
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Usings Statements
using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.ImageEffects
{
    /// <summary>
    /// Represent a Pixelate post processing.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.ImageEffects")]
    public class PixelateLens : Lens
    {
        /// <summary>
        /// The divisions
        /// </summary>
        private Vector2 pixelSize;

        #region Properties
        /// <summary>
        /// Gets or sets the image tone, default value is Vector2(10).
        /// </summary>
        [DataMember]
        public Vector2 PixelSize
        {
            get
            {
                return this.pixelSize;
            }

            set
            {
                if (value == null)
                {
                    throw new NullReferenceException("PixelSize cannot be null.");
                }

                if (value.X < 0 || value.Y < 0)
                {
                    throw new InvalidOperationException("Out of range, value > 0");
                }

                this.pixelSize = value;
            }
        }

        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="SepiaLens"/> class.
        /// </summary>
        public PixelateLens()
            : base()
        {
        }

        /// <summary>
        /// Sets default values for this instance.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.material = new PixelateMaterial();
            this.pixelSize = new Vector2(10.0f);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Renders to image.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Render(TimeSpan gameTime)
        {
            var mat = (this.material as PixelateMaterial);
            mat.Texture = this.Source;
            mat.PixelSize.X = this.Source.Width / pixelSize.X;
            mat.PixelSize.Y = this.Source.Height / pixelSize.Y;

            this.RenderToImage(this.Destination, this.material);

            mat.Texture = null;
        }

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
