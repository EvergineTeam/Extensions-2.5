// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Usings Statements
using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.ImageEffects
{
    /// <summary>
    /// Represent a Depth of Field bokeh using poligonaly aperture as postprocessing filter.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.ImageEffects")]
    public class BokehLens : Lens
    {
        private float fstops;
        private float maxBlur;

        /// <summary>
        /// Gets or sets the ratio of the lens's focal length to the diameter of the entrance pupil.
        /// </summary>
        #region Properties
        [RenderPropertyAsSlider(0.0f, 50.0f, 1.0f)]
        [DataMember]
        public float FStops
        {
            get
            {
                return this.fstops;
            }

            set
            {
                this.fstops = value;

                // Update aperture
                (this.material as BokehMaterial).Aperture = this.FocalLength / (this.fstops * 100);
            }
        }

        /// <summary>
        /// Gets or sets focus distance of the lens.
        /// </summary>
        [DataMember]
        public float FocalDistance
        {
            get
            {
                return (this.material as BokehMaterial).FocalDistance;
            }

            set
            {
                (this.material as BokehMaterial).FocalDistance = value;
            }
        }

        /// <summary>
        /// Gets or sets focus range of the lens.
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(0.0f, 100.0f, 1.0f)]
        public float FocalLength
        {
            get
            {
                return (this.material as BokehMaterial).FocalLength * 100;
            }

            set
            {
                float focal = value / 100;
                (this.material as BokehMaterial).FocalLength = focal;

                // Update apeture
                (this.material as BokehMaterial).Aperture = focal / this.fstops;
            }
        }

        /// <summary>
        /// Gets or sets film width, 24 mm by default.
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(0.0f, 100.0f, 1.0f)]
        public float FilmWidth
        {
            get
            {
                return (this.material as BokehMaterial).FilmWidth * 1000;
            }

            set
            {
                (this.material as BokehMaterial).FilmWidth = value / 1000;
            }
        }

        /// <summary>
        /// Gets or sets maximun Circle of Confusion diameter.
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(1.0f, 100.0f, 1.0f)]
        public float MaxBlur
        {
            get
            {
                return this.maxBlur * 1000;
            }

            set
            {
                this.maxBlur = value / 1000;
            }
        }

        /// <summary>
        /// Gets or sets shine Threshold
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(0.0f, 1.0f, 0.1f)]
        public float ShineThreshold
        {
            get
            {
                return (this.material as BokehMaterial).ShineThreshold;
            }

            set
            {
                (this.material as BokehMaterial).ShineThreshold = value;
            }
        }

        /// <summary>
        /// Gets or sets shine amount
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(0.0f, 3.0f, 0.1f)]
        public float ShineAmount
        {
            get
            {
                return (this.material as BokehMaterial).ShineAmount;
            }

            set
            {
                (this.material as BokehMaterial).ShineAmount = value;
            }
        }

        /// <summary>
        /// Gets or sets bokeh quality, Low by default.
        /// </summary>
        [DataMember]
        public BokehMaterial.EffectQuality Quality
        {
            get
            {
                return (this.material as BokehMaterial).Quality;
            }

            set
            {
                (this.material as BokehMaterial).Quality = value;
            }
        }
        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="BokehLens"/> class.
        /// </summary>
        public BokehLens()
            : base()
        {
        }

        /// <summary>
        /// Sets default values for this instance.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.material = new BokehMaterial();
            this.fstops = 0.15f;
            this.maxBlur = 0.02f;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Renders to image.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Render(TimeSpan gameTime)
        {
            if (this.Source == null)
            {
                return;
            }

            var mat = this.material as BokehMaterial;

            RenderTarget rt0 = graphicsDevice.RenderTargets.GetTemporalRenderTarget(this.Source.Width, this.Source.Height);
            RenderTarget rt1 = graphicsDevice.RenderTargets.GetTemporalRenderTarget(this.Source.Width, this.Source.Height);

            Vector2 aspect = new Vector2((float)rt0.Width / rt0.Height, 1);

            // CoC Map
            mat.Pass = BokehMaterial.Passes.CoCMap;
            mat.Texture = this.Source;
            this.RenderToImage(rt0, this.material);

            // Horizontal blur pass
            mat.Pass = BokehMaterial.Passes.HorizontalBlur;
            mat.BlurDisp = (new Vector2(0, 1f) / aspect) * this.maxBlur;
            mat.Texture = rt0;
            this.RenderToImage(rt1, this.material);

            // Diagonal blur pass
            mat.Pass = BokehMaterial.Passes.DiagonalBlurCombine;
            mat.BlurDisp = (new Vector2(1.0f, 0.57735f) / aspect) * this.maxBlur;
            mat.Texture = rt1;
            this.RenderToImage(this.Destination, this.material);

            graphicsDevice.RenderTargets.ReleaseTemporalRenderTarget(rt0);
            graphicsDevice.RenderTargets.ReleaseTemporalRenderTarget(rt1);
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
