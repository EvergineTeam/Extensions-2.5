#region File Description
//-----------------------------------------------------------------------------
// SSAOLens
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
    /// Represent a Screen Space Ambient Occlusion filter.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.ImageEffects")]
    public class SSAOLens : Lens
    {
        /// <summary>
        /// The blur downsample factor
        /// </summary>
        private float downSampleFactor;

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether [only Ambient Occlusion].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [only ao]; otherwise, <c>false</c>.
        /// </value>
        public bool OnlyAO { get; set; }

        /// <summary>
        /// Blur Downsample factor.
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(1, 8, 1f)]
        public float DownSampleFactor
        {
            get
            {
                return this.downSampleFactor;
            }

            set
            {
                this.downSampleFactor = value;
            }
        }

        /// <summary>
        /// Gets or sets the AO intensity. Default value is 2.
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(1, 4, 0.1f)]
        public float AOIntensity
        {
            get
            {
                return (this.material as SSAOMaterial).AOIntensity;
            }

            set
            {
                (this.material as SSAOMaterial).AOIntensity = value;
            }
        }

        /// <summary>
        /// Gets or sets the distance threshold, default value is 1f.
        /// </summary>
        /// <value>
        /// Down up scale.
        /// </value>
        [DataMember]
        public float DistanceThreshold
        {
            get
            {
                return (this.material as SSAOMaterial).DistanceThreshold;
            }

            set
            {
                (this.material as SSAOMaterial).DistanceThreshold = value;
            }
        }

        /// <summary>
        /// Gets or sets FilterRadius, default value is 0.01f.
        /// </summary>
        /// <value>
        /// Down up scale.
        /// </value>
        [DataMember]
        public Vector2 FilterRadius
        {
            get
            {
                return (this.material as SSAOMaterial).FilterRadius;
            }

            set
            {
                (this.material as SSAOMaterial).FilterRadius = value;
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="SSAOLens"/> class.
        /// </summary>
        public SSAOLens()
            : base()
        {
        }

        /// <summary>
        /// Sets default values for this instance.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.material = new SSAOMaterial();
            this.downSampleFactor = 2;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Renders to image.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Render(TimeSpan gameTime)
        {
            var mat = this.material as SSAOMaterial;
            int width = (int)(this.Source.Width / this.downSampleFactor);
            int height = (int)(this.Source.Height / this.downSampleFactor);

            RenderTarget rt1 = graphicsDevice.RenderTargets.GetTemporalRenderTarget(width, height);
            graphicsDevice.RenderState.Viewport = new Viewport(0, 0, width, height);

            // AO
            mat.Pass = SSAOMaterial.Passes.SSAO;
            mat.Texture = null;
            mat.AOTexture = null;
            this.RenderToImage(rt1, this.material);

            // UpCombine
            graphicsDevice.RenderState.Viewport = new Viewport(0, 0, this.Source.Width, this.Source.Height);
            if (OnlyAO)
            {
                mat.Pass = SSAOMaterial.Passes.OnlyAO;
                mat.Texture = this.Source;
                mat.AOTexture = rt1;
            }
            else
            {
                mat.Pass = SSAOMaterial.Passes.Combine;
                mat.Texture = this.Source;
                mat.AOTexture = rt1;
            }

            this.RenderToImage(this.Destination, this.material);

            mat.Texture = null;
            mat.AOTexture = null;

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
