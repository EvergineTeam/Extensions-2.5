#region File Description
//-----------------------------------------------------------------------------
// BokehMaterial
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Runtime.InteropServices;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Graphics.VertexFormats;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;

#endregion

namespace WaveEngine.ImageEffects
{
    /// <summary>
    /// Depth of field bokeh effect.
    /// </summary>
    public class BokehMaterial : Material
    {
        /// <summary>
        /// Effect quality
        /// </summary>
        public enum EffectQuality
        {
            /// <summary>
            /// Only 6 samples
            /// </summary>
            Low = 1,

            /// <summary>
            /// 8 samples
            /// </summary>
            Medium = 2,

            /// <summary>
            /// 10 samples
            /// </summary>
            High = 3
        }

        /// <summary>
        /// Effect's steps
        /// </summary>
        public enum Passes
        {
            /// <summary>
            /// CoC map
            /// </summary>
            CoCMap,

            /// <summary>
            /// The blur
            /// </summary>
            HorizontalBlur,

            /// <summary>
            /// Combine
            /// </summary>
            DiagonalBlurCombine,
        }

        /// <summary>
        /// Effects steps
        /// </summary>
        public Passes Pass;

        /// <summary>
        /// The texture
        /// </summary>
        private Texture texture;

        /// <summary>
        /// The depth texture
        /// </summary>
        private Texture depthTexture;

        /// <summary>
        /// The techniques
        /// </summary>
        private static ShaderTechnique[] techniques =
        {
            new ShaderTechnique("CoCMap", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "psCoCMap", VertexPositionTexture.VertexFormat),
            new ShaderTechnique("HorizontalBlur_Low", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "psHorizontalBlur", VertexPositionTexture.VertexFormat, null, new string[]{"LOW"}),
            new ShaderTechnique("HorizontalBlur_Medium", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "psHorizontalBlur", VertexPositionTexture.VertexFormat, null, new string[]{"MEDIUM"}),
            new ShaderTechnique("HorizontalBlur_High", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "psHorizontalBlur", VertexPositionTexture.VertexFormat, null, new string[]{"HIGH"}),
            new ShaderTechnique("DiagonalBlurCombine_Low", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "psDiagonalBlurCombine", VertexPositionTexture.VertexFormat, null, new string[]{"LOW"}),
            new ShaderTechnique("DiagonalBlurCombine_Medium", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "psDiagonalBlurCombine", VertexPositionTexture.VertexFormat, null, new string[]{"MEDIUM"}),
            new ShaderTechnique("DiagonalBlurCombine_High", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "psDiagonalBlurCombine", VertexPositionTexture.VertexFormat, null, new string[]{"HIGH"}),
        };

        #region Struct
        /// <summary>
        /// Shader parameters.
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Size = 32)]
        private struct BokehEffectParameters
        {
            [FieldOffset(0)]
            public Vector2 BlurDisp;

            [FieldOffset(8)]
            public float Aperture;

            [FieldOffset(12)]
            public float LastCoeff;

            [FieldOffset(16)]
            public float FocalDistance;

            [FieldOffset(20)]
            public float NearPlane;

            [FieldOffset(24)]
            public float FarParam;

            [FieldOffset(28)]
            public float FilmWidth;
        }
        #endregion

        /// <summary>
        /// Handle the shader parameters struct.
        /// </summary>
        private BokehEffectParameters shaderParameters;

        #region Properties
        /// <summary>
        /// Effect quality, Low by default.
        /// </summary>
        public EffectQuality Quality { get; set; }

        /// <summary>
        /// Focus Distance
        /// </summary>
        public float FocalDistance { get; set; }

        /// <summary>
        /// Focus range
        /// </summary>
        public float FocalLength { get; set; }

        /// <summary>
        /// Aperture
        /// </summary>
        public float Aperture { get; set; }

        /// <summary>
        /// Film width, 24 mm by default.
        /// </summary>
        public float FilmWidth { get; set; }

        /// <summary>
        /// Calculated offset.
        /// </summary>
        public Vector2 BlurDisp { get; set; }

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
                if (value == null)
                {
                    throw new NullReferenceException("Texture cannot be null.");
                }

                this.texture = value;
            }
        }

        /// <summary>
        /// Gets or sets the Depth Texture.
        /// </summary>
        public Texture DepthTexture
        {
            get
            {
                return this.depthTexture;
            }

            set
            {
                this.depthTexture = value;
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
                int index = 0;

                switch (this.Pass)
                {
                    case Passes.HorizontalBlur:
                        index = (int)this.Quality;
                        break;
                    case Passes.DiagonalBlurCombine:
                        index = 3 + (int)this.Quality;
                        break;
                    case Passes.CoCMap:
                    default:
                        index = 0;
                        break;
                }

                return techniques[index].Name;
            }
        }

        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="BokehMaterial"/> class.
        /// </summary>
        public BokehMaterial()
            : base(DefaultLayers.Opaque)
        {
        }

        /// <summary>
        /// Initialize default values for this instance.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.SamplerMode = AddressMode.LinearClamp;
            this.Quality = EffectQuality.Low;
            this.Aperture = 0.35f;
            this.FocalLength = 0.05f;
            this.FocalDistance = 20;
            this.FilmWidth = 0.024f;
            this.shaderParameters = new BokehEffectParameters();
            this.Parameters = this.shaderParameters;

            this.InitializeTechniques(techniques);
        }

        /// <summary>
        /// Initializes the specified assets.
        /// </summary>
        /// <param name="assets">The assets.</param>
        public override void Initialize(WaveEngine.Framework.Services.AssetsContainer assets)
        {
            base.Initialize(assets);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Applies the pass.
        /// </summary>
        /// <param name="cached">The efect is cached.</param>
        public override void SetParameters(bool cached)
        {
            Camera camera = this.renderManager.CurrentDrawingCamera;

            this.shaderParameters.BlurDisp = this.BlurDisp;
            this.shaderParameters.Aperture = this.Aperture;
            this.shaderParameters.LastCoeff = this.FocalLength / (this.FocalDistance - this.FocalLength);
            this.shaderParameters.FocalDistance = this.FocalDistance;
            this.shaderParameters.NearPlane = camera.NearPlane;
            this.shaderParameters.FarParam = camera.FarPlane / (camera.FarPlane - camera.NearPlane);
            this.shaderParameters.FilmWidth = this.FilmWidth;
            this.Parameters = this.shaderParameters;

            depthTexture = this.renderManager.GraphicsDevice.RenderTargets.DefaultDepthTexture;

            if (this.texture != null)
            {
                this.graphicsDevice.SetTexture(this.texture, 0);
            }

            if (this.depthTexture != null)
            {
                this.graphicsDevice.SetTexture(this.depthTexture, 1);
            }
        }

        #endregion
    }
}
