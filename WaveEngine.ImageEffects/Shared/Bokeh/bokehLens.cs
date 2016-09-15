#region File Description
//-----------------------------------------------------------------------------
// DepthOfFieldLens
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

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
        /// The ratio of the lens's focal length to the diameter of the entrance pupil.
        /// </summary>
        #region Properties
        [RenderPropertyAsSlider(0.1f, 50, 1f)]
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
                (this.material as BokehMaterial).Aperture = this.FocalLength / fstops;
            }
        }

        /// <summary>
        /// Focus distance of the lens.
        /// </summary>
        [DataMember]
        [RenderPropertyAsFInput(0, MinLimit = 0, MaxLimit = 1000)]
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
        /// Focus range of the lens.
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(0.0001f, 1.0f, 0.001f)]
        public float FocalLength
        {
            get
            {
                return (this.material as BokehMaterial).FocalLength;
            }

            set
            {
                (this.material as BokehMaterial).FocalLength = value;

                //Update apeture
                (this.material as BokehMaterial).Aperture = value / fstops;
            }
        }

        /// <summary>
        /// Film width, 24 mm by default.
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(0.0001f, 0.1f, 0.001f)]
        public float FilmWidth
        {
            get
            {
                return (this.material as BokehMaterial).FilmWidth;
            }

            set
            {
                (this.material as BokehMaterial).FilmWidth = value;
            }
        }

        /// <summary>
        /// Maximun Circle of Confusion diameter.
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(0.001f, 0.1f, 0.001f)]
        public float MaxBlur
        {
            get
            {
                return this.maxBlur;
            }

            set
            {
                this.maxBlur = value;
            }
        }

        /// <summary>
        /// Bokeh quality, Low by default.
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
            mat.BlurDisp = (new Vector2(0, 1f) / aspect) * maxBlur;
            mat.Texture = rt0;
            this.RenderToImage(rt1, this.material);

            // Diagonal blur pass
            mat.Pass = BokehMaterial.Passes.DiagonalBlurCombine;
            mat.BlurDisp = (new Vector2(1.0f, 0.57735f) / aspect) * maxBlur;
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
