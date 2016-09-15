#region File Description
//-----------------------------------------------------------------------------
// LightShaftLens
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
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
#endregion

namespace WaveEngine.ImageEffects
{
    /// <summary>
    /// Represent a Light shaft as postprocessing filter.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.ImageEffects")]
    public class LightShaftLens : Lens
    {
        /// <summary>
        /// The blur downsample factor
        /// </summary>
        private float downSampleFactor;

        /// <summary>
        /// The directional light entity path
        /// </summary>
        private string directionalLightPath;

        #region Properties
        /// <summary>
        /// Gets or sets AutoIntensity, default value is true.
        /// </summary>
        [DataMember]
        public bool AutoIntensity
        {
            get
            {
                return (this.material as LightShaftMaterial).AutoIntensity;
            }

            set
            {
                (this.material as LightShaftMaterial).AutoIntensity = value;
            }
        }

        /// <summary>
        /// Gets or sets HaloEnabled, default value is true.
        /// </summary>
        [DataMember]
        public bool HaloEnabled
        {
            get
            {
                return (this.material as LightShaftMaterial).HaloEnabled;
            }

            set
            {
                (this.material as LightShaftMaterial).HaloEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets Blend, default value is 0.8f.
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(0.01f, 2.0f, 0.01f)]
        public float Blend
        {
            get
            {
                return (this.material as LightShaftMaterial).Blend;
            }

            set
            {
                (this.material as LightShaftMaterial).Blend = value;
            }
        }

        /// <summary>
        /// Gets or sets density, default value is 0.6f.
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(0.01f, 5.0f, 0.01f)]
        public float Density
        {
            get
            {
                return (this.material as LightShaftMaterial).Density;
            }

            set
            {
                (this.material as LightShaftMaterial).Density = value;
            }
        }

        /// <summary>
        /// Gets or sets Weight, default value is 0.8f.
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(0.01f, 1.0f, 0.01f)]
        public float Weight
        {
            get
            {
                return (this.material as LightShaftMaterial).Weight;
            }

            set
            {
                (this.material as LightShaftMaterial).Weight = value;
            }
        }

        /// <summary>
        /// Gets or sets Decay, default value is 0.86f.
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(0.01f, 1.0f, 0.01f)]
        public float Decay
        {
            get
            {
                return (this.material as LightShaftMaterial).Decay;
            }

            set
            {
                (this.material as LightShaftMaterial).Decay = value;
            }
        }

        /// <summary>
        /// Gets or sets Exposure, default value is 0.5f.
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(0.0f, 2.0f, 0.01f)]
        public float Exposure
        {
            get
            {
                return (this.material as LightShaftMaterial).Exposure;
            }

            set
            {
                (this.material as LightShaftMaterial).Exposure = value;
            }
        }

        /// <summary>
        /// Gets or sets SunIntensity, default value is 0.15f.
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(0, 2, 0.01f)]
        public float SunIntensity
        {
            get
            {
                return (this.material as LightShaftMaterial).SunIntensity;
            }

            set
            {
                (this.material as LightShaftMaterial).SunIntensity = value;
            }
        }

        /// <summary>
        /// Gets or sets SunRadius, default value is 0.2f.
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(0.0f, 2.0f, 0.01f)]
        public float SunRadius
        {
            get
            {
                return (this.material as LightShaftMaterial).Radius;
            }

            set
            {
                (this.material as LightShaftMaterial).Radius = value;
            }
        }

        /// <summary>
        /// Gets or sets SunEdgeSharpness, default value is 0.2f.
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(0.0f, 2.0f, 0.01f)]
        public float SunEdgeSharpness
        {
            get
            {
                return (this.material as LightShaftMaterial).EdgeSharpness;
            }

            set
            {
                (this.material as LightShaftMaterial).EdgeSharpness = value;
            }
        }

        /// <summary>
        /// Down sample factor, by default is 4;
        /// </summary>
        [DataMember]
        [RenderPropertyAsSlider(1, 10, 1f)]
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
        /// Light Shaft quality, Low by default.
        /// </summary>
        [DataMember]
        public LightShaftMaterial.EffectQuality Quality
        {
            get
            {
                return (this.material as LightShaftMaterial).Quality;
            }

            set
            {
                (this.material as LightShaftMaterial).Quality = value;
            }
        }

        /// <summary>
        /// Light Shaft quality, Low by default.
        /// </summary>
        [DataMember]
        [RenderPropertyAsEntity(new String[] { "WaveEngine.Framework.Graphics.DirectionalLightProperties" })]
        public string DirectionalLight
        {
            get
            {
                return this.directionalLightPath;
            }
            set
            {
                this.directionalLightPath = value;

                if(this.isInitialized)
                {
                    this.RefrehsDirectionalLight();
                }
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="LightShaftLens"/> class.
        /// </summary>
        public LightShaftLens()
            : base()
        {
        }

        /// <summary>
        /// Sets default values for this instance.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.downSampleFactor = 2;
            this.material = new LightShaftMaterial();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Initialize the lens
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            this.RefrehsDirectionalLight();
        }

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

            var mat = this.material as LightShaftMaterial;
            if (mat.TexcoordOffset == Vector2.Zero)
            {
                mat.TexcoordOffset.X = 1f / this.Source.Width;
                mat.TexcoordOffset.Y = 1f / this.Source.Height;
            }

            int width = (int)(this.Source.Width / this.downSampleFactor);
            int height = (int)(this.Source.Height / this.downSampleFactor);

            RenderTarget rt1 = graphicsDevice.RenderTargets.GetTemporalRenderTarget(width, height);
            RenderTarget rt2 = graphicsDevice.RenderTargets.GetTemporalRenderTarget(width, height);
            graphicsDevice.RenderState.Viewport = new Viewport(0, 0, width, height);

            // Black mask 
            mat.Pass = LightShaftMaterial.Passes.BlackMask;
            mat.Texture = this.Source;
            this.RenderToImage(rt1, this.material);

            // LightShaft
            mat.Pass = LightShaftMaterial.Passes.LightShaft;
            mat.Texture = rt1;
            this.RenderToImage(rt2, this.material);

            // UpCombine
            graphicsDevice.RenderState.Viewport = new Viewport(0, 0, this.Source.Width, this.Source.Height);
            mat.Pass = LightShaftMaterial.Passes.Combine;
            mat.Texture = this.Source;
            mat.Texture1 = rt2;
            this.RenderToImage(this.Destination, this.material);

            mat.Texture = null;
            mat.Texture1 = null;

            graphicsDevice.RenderTargets.ReleaseTemporalRenderTarget(rt1);
            graphicsDevice.RenderTargets.ReleaseTemporalRenderTarget(rt2);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Refresh directional light
        /// </summary>
        private void RefrehsDirectionalLight()
        {
            (this.material as LightShaftMaterial).DirectionalLight = null;

            if (string.IsNullOrEmpty(this.directionalLightPath) || this.material == null)
            {
                return;
            }

            Entity entity = this.EntityManager.Find(this.directionalLightPath);
            if (entity != null)
            {
                (this.material as LightShaftMaterial).DirectionalLight = entity.FindComponent<DirectionalLightProperties>();
            }
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
