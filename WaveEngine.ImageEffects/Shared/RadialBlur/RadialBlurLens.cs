#region File Description
//-----------------------------------------------------------------------------
// RadialBblurLens
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
    /// Represent a RadialBblur as postprocessing filter.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.ImageEffects")]
    public class RadialBlurLens : Lens
    {
        #region Properties

        /// <summary>
        /// Gets or sets the nsamples, default value is 10.
        /// </summary>
        /// <value>
        /// The nsamples.
        /// </value>
        [DataMember]
        [RenderPropertyAsInput(0, 30)]
        public int Nsamples
        {
            get
            {
                return (this.material as RadialBlurMaterial).Nsamples;
            }

            set
            {
                (this.material as RadialBlurMaterial).Nsamples = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of the blur, default value is 0.1f.
        /// </summary>
        /// <value>
        /// The width of the blur.
        /// </value>
        [DataMember]
        [RenderPropertyAsSlider(0.0f, 1.0f, 0.1f)]
        public float BlurWidth
        {
            get
            {
                return (this.material as RadialBlurMaterial).BlurWidth;
            }

            set
            {
                (this.material as RadialBlurMaterial).BlurWidth = value;
            }
        }

        /// <summary>
        /// Gets or sets the center, default value is (0.5f, 0.5f).
        /// </summary>
        /// <value>
        /// The center.
        /// </value>
        [DataMember]
        public Vector2 Center
        {
            get
            {
                return (this.material as RadialBlurMaterial).Center;
            }

            set
            {
                (this.material as RadialBlurMaterial).Center = value;
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="RadialBlurLens"/> class.
        /// </summary>
        public RadialBlurLens()
            : base()
        {
        }

        /// <summary>
        /// Sets default values for this instance.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.material = new RadialBlurMaterial();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Renders to image.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Render(TimeSpan gameTime)
        {
            var mat = this.material as RadialBlurMaterial;
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
