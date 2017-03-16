#region File Description
//-----------------------------------------------------------------------------
// FilmGrainMaterial
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Runtime.InteropServices;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Graphics.VertexFormats;
using WaveEngine.Common.IO;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;
using System.Reflection;
using WaveEngine.Common.Helpers;
#endregion

namespace WaveEngine.ImageEffects
{
    /// <summary>
    /// ChromaticAberration effect.
    /// </summary>
    public class FilmGrainMaterial : Material
    {
        /// <summary>
        /// The grain intensity minimum
        /// </summary>
        public float GrainIntensityMin;

        /// <summary>
        /// The grain intensity maximum
        /// </summary>
        public float GrainIntensityMax;

        /// <summary>
        /// The grain size
        /// </summary>
        public float GrainSize;

        /// <summary>
        /// The screen width
        /// </summary>
        public int ScreenWidth;

        /// <summary>
        /// The screen heigth
        /// </summary>
        public int ScreenHeigth;

        /// <summary>
        /// The grain offset scale
        /// </summary>
        private Vector4 grainOffsetScale;

        /// <summary>
        /// The intensity
        /// </summary>
        private float Intensity;

        /// <summary>
        /// The path grain texture
        /// </summary>
        private string pathGrainTexture;

        /// <summary>
        /// The grain texture
        /// </summary>
        private Texture grainTexture;

        /// <summary>
        /// The texture
        /// </summary>
        private Texture texture;

        /// <summary>
        /// The random
        /// </summary>
        private Random random;

        /// <summary>
        /// The techniques
        /// </summary>
        private static ShaderTechnique[] techniques =
        {
            new ShaderTechnique("FilmGrain", "FilmGrainvsFilmGrain", "FilmGrainpsFilmGrain", VertexPositionTexture.VertexFormat),
        };

        #region Struct
        /// <summary>
        /// Shader parameters.
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Size = 32)]
        private struct FilmGrainEffectParameters
        {
            [FieldOffset(0)]
            public Vector4 GrainOffsetScale;

            [FieldOffset(16)]
            public float Intensity;
        }
        #endregion

        /// <summary>
        /// Handle the shader parameters struct.
        /// </summary>
        private FilmGrainEffectParameters shaderParameters;

        #region Properties

        /// <summary>
        /// Gets or sets the texture.
        /// </summary>
        /// <value>
        /// The texture.
        /// </value>
        public Texture Texture
        {
            get
            {
                return this.texture;
            }

            set
            {
                this.texture = value;
            }
        }

        /// <summary>
        /// Gets or sets the GrainTexture.
        /// </summary>
        /// <value>
        /// The texture.
        /// </value>
        public Texture GrainTexture
        {
            get
            {
                return this.grainTexture;
            }

            set
            {
                this.grainTexture = value;
            }
        }

        /// <summary>
        /// Gets the current technique.
        /// </summary>
        /// <value>
        /// The current technique.
        /// </value>
        public override string CurrentTechnique
        {
            get
            {
                return techniques[0].Name;
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="FilmGrainMaterial"/> class.
        /// </summary>
        public FilmGrainMaterial() : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilmGrainMaterial" /> class.
        /// </summary>
        /// <param name="pathGrainTexture">The path grain/noise texture.</param>
        public FilmGrainMaterial(string pathGrainTexture)
            : base(DefaultLayers.Opaque)
        {
            this.pathGrainTexture = pathGrainTexture;
            this.SamplerMode = AddressMode.LinearClamp;
            this.GrainIntensityMin = 0.1f;
            this.GrainIntensityMax = 0.2f;
            this.GrainSize = 2.0f;

            this.random = new Random(23);

            this.shaderParameters = new FilmGrainEffectParameters();
            SetShaderParamenters();

            this.InitializeTechniques(techniques);
        }

        /// <summary>
        /// Initializes the specified assets.
        /// </summary>
        /// <param name="assets">The assets.</param>
        public override void Initialize(WaveEngine.Framework.Services.AssetsContainer assets)
        {
            base.Initialize(assets);

            if (!string.IsNullOrEmpty(this.pathGrainTexture))
            {
                this.grainTexture = assets.LoadAsset<Texture2D>(this.pathGrainTexture);
            }
            else
            {
                var assembly = this.GetMemberAssembly();
                var currentNamespace = assembly.GetName().Name;

                var textureResourcePath = currentNamespace + ".FilmGrain.NoiseEffectGrain.wpk";
                var textureStream = ResourceLoader.GetEmbeddedResourceStream(assembly, textureResourcePath);

                this.grainTexture = assets.LoadAsset<Texture2D>(textureResourcePath, textureStream);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Applies the pass.
        /// </summary>
        /// <param name="cached">The efect is cached.</param>
        public override void SetParameters(bool cached)
        {
            if (!cached)
            {
                SetShaderParamenters();
                this.SamplerMode = AddressMode.LinearClamp;
                if (this.Texture != null)
                {
                    this.graphicsDevice.SetTexture(this.Texture, 0);
                }

                this.SamplerMode = AddressMode.LinearWrap;
                if (this.grainTexture != null)
                {
                    this.graphicsDevice.SetTexture(this.grainTexture, 1);
                }
            }
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Sets the shader paramenters.
        /// </summary>
        private void SetShaderParamenters()
        {
            GrainIntensityMin =  MathHelper.Clamp(GrainIntensityMin, 0.0f, 5.0f);
            GrainIntensityMax = MathHelper.Clamp(GrainIntensityMax, 0.0f, 5.0f);
            GrainSize = MathHelper.Clamp(GrainSize, 0.1f, 50.0f);

            float grainScale = 1.0f / this.GrainSize;

            if (this.grainTexture == null)
            {
                this.grainOffsetScale = new Vector4(1);
            }
            else
            {
                this.grainOffsetScale = new Vector4(
                    (float)random.NextDouble(),
                    (float)random.NextDouble(),
                    (float)this.ScreenWidth / (float)this.grainTexture.Width * grainScale,
                    (float)this.ScreenHeigth / (float)this.grainTexture.Height * grainScale
                );
            }

            this.Intensity = ((float)random.NextDouble() * (this.GrainIntensityMax - this.GrainIntensityMin)) + this.GrainIntensityMin;

            this.shaderParameters.GrainOffsetScale = this.grainOffsetScale;
            this.shaderParameters.Intensity = this.Intensity;

            this.Parameters = this.shaderParameters;
        }

        #endregion
    }
}
