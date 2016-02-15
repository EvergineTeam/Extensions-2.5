#region File Description
//-----------------------------------------------------------------------------
// SobelLens
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Usings Statements
using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.ImageEffects
{
    /// <summary>
    /// Represent a edge detection post processing filter.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.ImageEffects")]
    public class SobelLens : Lens
    {
        #region Properties
        /// <summary>
        /// Gets or sets the effect, default value is Effect.Sobel.
        /// </summary>
        [DataMember]
        public SobelMaterial.SobelEffect Effect
        {
            get
            {
                return (this.material as SobelMaterial).Effect;
            }

            set
            {
                (this.material as SobelMaterial).Effect = value;
            }
        }

        /// <summary>
        /// Gets or sets the threshold, default value is 0.0049f.
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(0.0f, 1.0f, 0.01f)]
        public float Threshold
        {
            get
            {
                return (this.material as SobelMaterial).Threshold;
            }

            set
            {
                (this.material as SobelMaterial).Threshold = value;
            }
        }

        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="SobelLens"/> class.
        /// </summary>
        public SobelLens()
            : base()
        {
        }

        /// <summary>
        /// Sets default values for this instance.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.material = new SobelMaterial();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Renders to image.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Render(TimeSpan gameTime)
        {
            var mat = this.material as SobelMaterial;
            if (mat.TexcoordOffset == Vector2.Zero)
            {
                mat.TexcoordOffset.X = 1f / this.Source.Width;
                mat.TexcoordOffset.Y = 1f / this.Source.Height;
            }

            mat.Texture = this.Source;
            this.RenderToImage(this.Destination, this.material);

            mat.Texture = null;
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
