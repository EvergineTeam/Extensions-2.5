#region File Description
//-----------------------------------------------------------------------------
// SSAOMaterial
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
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;

#endregion

namespace WaveEngine.ImageEffects
{
    /// <summary>
    /// this class represent a screen space ambient occlusion material.
    /// </summary>
    public class SSAOMaterial : Material
    {
        public enum Passes
        {
            /// <summary>
            /// SSAO
            /// </summary>
            SSAO,

            /// <summary>
            /// Combine
            /// </summary>
            Combine,

            /// <summary>
            /// The only AO
            /// </summary>
            OnlyAO
        }

        /// <summary>
        /// Steps of this effects
        /// </summary>
        public Passes Pass;

        /// <summary>
        /// The texture
        /// </summary>
        private Texture texture;

        /// <summary>
        /// The texture
        /// </summary>
        private Texture aoTexture;

        /// <summary>
        /// The depth texture
        /// </summary>
        private Texture depthTexture;

        /// <summary>
        /// The gbuffer texture
        /// </summary>
        private Texture gBufferTexture;

        /// <summary>
        /// The techniques
        /// </summary>
        private static ShaderTechnique[] techniques =
        {
            new ShaderTechnique("SSAO", string.Empty, "vsSSAO", string.Empty, "psSSAO", VertexPositionTexture.VertexFormat),
            new ShaderTechnique("Combine", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "psSSAOCombine", VertexPositionTexture.VertexFormat),
            new ShaderTechnique("OnlyAO", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "psOnlyAO", VertexPositionTexture.VertexFormat),
        };

        #region Struct
        /// <summary>
        /// Shader parameters.
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Size = 80)]
        private struct SSAOEffectParameters
        {
            [FieldOffset(0)]
            public float DistanceThreshold;

            [FieldOffset(4)]
            public float AOintensity;

            [FieldOffset(8)]
            public Vector2 FilterRadius;

            [FieldOffset(16)]
            public Matrix ViewProjectionInverse;
        }
        #endregion

        /// <summary>
        /// Handle the shader parameters struct.
        /// </summary>
        private SSAOEffectParameters shaderParameters;

        #region Properties
        /// <summary>
        /// Gets or sets the ao intensity.
        /// </summary>
        /// <value>
        /// The ao intensity.
        /// </value>
        public float AOIntensity { get; set; }

        /// <summary>
        /// Distance Threshold
        /// </summary>
        public float DistanceThreshold { get; set; }

        /// <summary>
        /// Filter Radius
        /// </summary>
        public Vector2 FilterRadius { get; set; }

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
        /// Gets or sets the texture1.
        /// </summary>
        /// <value>
        /// The texture1.
        /// </value>
        /// <exception cref="System.NullReferenceException">Texture cannot be null.</exception>
        public Texture AOTexture
        {
            get
            {
                return this.aoTexture;
            }

            set
            {
                this.aoTexture = value;
            }
        }

        /// <summary>
        /// Gets or sets the GBuffer Texture.
        /// </summary>
        public Texture GBufferTexture
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
                int index = (int)this.Pass;
                return techniques[index].Name;
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="SSAOMaterial"/> class.
        /// </summary>
        public SSAOMaterial()
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
            this.DistanceThreshold = 1.5f;
            this.AOIntensity = 2;
            this.FilterRadius = new Vector2(0.015f);

            this.shaderParameters = new SSAOEffectParameters();
            this.shaderParameters.DistanceThreshold = this.DistanceThreshold;
            this.shaderParameters.FilterRadius = this.FilterRadius;
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

                // Set ViewProjectionInverse Matrix
                Matrix viewProjection = camera.ViewProjection;
                this.graphicsDevice.ToPlatformViewMatrix(ref viewProjection, out viewProjection);
                Matrix viewProjectionInverse;
                Matrix.Invert(ref viewProjection, out viewProjectionInverse);
                this.graphicsDevice.ToShaderMatrix(ref viewProjectionInverse, out this.shaderParameters.ViewProjectionInverse);

                this.shaderParameters.DistanceThreshold = this.DistanceThreshold;
                this.shaderParameters.FilterRadius = this.FilterRadius;
                this.shaderParameters.AOintensity = this.AOIntensity;

                this.Parameters = this.shaderParameters;
                
                this.gBufferTexture = camera.GBufferRT0;
                this.depthTexture = camera.GBufferDepthBuffer;

                if (this.texture != null)
                {
                    this.graphicsDevice.SetTexture(this.texture, 0);
                }

                if (this.aoTexture != null)
                {
                    this.graphicsDevice.SetTexture(this.aoTexture, 1);
                }

                if (this.gBufferTexture != null)
                {
                    this.graphicsDevice.SetTexture(this.gBufferTexture, 2);
                }

                if (this.depthTexture != null)
                {
                    this.graphicsDevice.SetTexture(this.depthTexture, 3);
                }
            }
        }
        #endregion
    }
}
