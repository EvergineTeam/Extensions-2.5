// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

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
    /// Distance-based fog effect.
    /// </summary>
    public class FogMaterial : Material
    {
        /// <summary>
        /// Available techniques
        /// </summary>
        public enum Techniques
        {
            /// <summary>
            /// Linear fog based on start and end parameters.
            /// </summary>
            Linear = 0,

            /// <summary>
            /// Exponencial fog.
            /// </summary>
            Exponencial = 1,

            /// <summary>
            /// Exponencial squared fog.
            /// </summary>
            ExponencialSquared = 2
        }

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
            new ShaderTechnique("Linear", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "psFog", VertexPositionTexture.VertexFormat),
            new ShaderTechnique("Exponencial", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "psFogExp", VertexPositionTexture.VertexFormat),
            new ShaderTechnique("ExponencialSquared", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "psFogExp2", VertexPositionTexture.VertexFormat),
        };

        #region Struct

        /// <summary>
        /// Shader parameters.
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Size = 32)]
        private struct FogEffectParameters
        {
            [FieldOffset(0)]
            public Vector3 FogColor;

            [FieldOffset(12)]
            public float FogDensity;

            [FieldOffset(16)]
            public float ZParamA;

            [FieldOffset(20)]
            public float ZParamB;

            [FieldOffset(24)]
            public float FogStart;

            [FieldOffset(28)]
            public float FogEnd;
        }
        #endregion

        /// <summary>
        /// Handle the shader parameters struct.
        /// </summary>
        private FogEffectParameters shaderParameters;

        #region Properties

        /// <summary>
        /// Gets or sets the technique.
        /// </summary>
        /// <value>
        /// The technique.
        /// </value>
        public Techniques Technique { get; set; }

        /// <summary>
        /// Gets or sets start fog.
        /// </summary>
        public float StartFog { get; set; }

        /// <summary>
        /// Gets or sets end fog.
        /// </summary>
        public float EndFog { get; set; }

        /// <summary>
        /// Gets or sets focus Distance
        /// </summary>
        public Color FogColor { get; set; }

        /// <summary>
        /// Gets or sets focus range
        /// </summary>
        public float FogDensity { get; set; }

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
                int index = (int)this.Technique;
                return techniques[index].Name;
            }
        }
        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="FogMaterial"/> class.
        /// </summary>
        public FogMaterial()
            : base(DefaultLayers.Opaque)
        {
        }

        /// <summary>
        /// Default value for this instance.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.Technique = Techniques.Exponencial;
            this.StartFog = 0.02f;
            this.EndFog = 0.8f;
            this.FogColor = new Color(0.5f, 0.6f, 0.7f);
            this.FogDensity = 10f;
            this.shaderParameters = new FogEffectParameters();
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
            if (!cached)
            {
                Camera camera = this.renderManager.CurrentDrawingCamera;

                this.shaderParameters.FogStart = this.StartFog;
                this.shaderParameters.FogEnd = this.EndFog;
                this.shaderParameters.FogColor = this.FogColor.ToVector3();
                this.shaderParameters.FogDensity = this.FogDensity;
                this.shaderParameters.ZParamA = 1f - (camera.FarPlane / camera.NearPlane);
                this.shaderParameters.ZParamB = camera.FarPlane / camera.NearPlane;
                this.Parameters = this.shaderParameters;

                if (this.texture != null)
                {
                    this.graphicsDevice.SetTexture(this.texture, 0);
                }

                this.depthTexture = camera.GBufferDepthBuffer;

                if (this.depthTexture != null)
                {
                    this.graphicsDevice.SetTexture(this.depthTexture, 1);
                }
            }
        }
        #endregion
    }
}
