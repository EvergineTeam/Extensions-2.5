#region File Description
//-----------------------------------------------------------------------------
// MotionBlurMaterial
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
    /// this class represent a camera motion blur material.
    /// </summary>
    public class MotionBlurMaterial : Material
    {
        /// <summary>
        /// Motion blur quality
        /// </summary>
        public enum EffectQuality
        {
            /// <summary>
            /// Only 4 samples
            /// </summary>
            Low = 0,

            /// <summary>
            /// 6 samples
            /// </summary>
            Medium = 1,

            /// <summary>
            /// 9 samples
            /// </summary>
            High = 2
        }

        /// <summary>
        /// The texture
        /// </summary>
        private Texture texture;

        /// <summary>
        /// The texture1
        /// </summary>
        private Texture depthTexture;

        /// <summary>
        /// Previous view projection Matrix.
        /// </summary>
        private Matrix previousViewProjection;

        /// <summary>
        /// The techniques
        /// </summary>
        private static ShaderTechnique[] techniques =
        {
            new ShaderTechnique("MotionBlurLow", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "psMotionBlur", VertexPositionTexture.VertexFormat, null, new string[]{"LOW"}),
            new ShaderTechnique("MotionBlurMedium", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "psMotionBlur", VertexPositionTexture.VertexFormat, null, new string[]{"MEDIUM"}),
            new ShaderTechnique("MotionBlurHigh", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "psMotionBlur", VertexPositionTexture.VertexFormat, null, new string[]{"HIGH"}),
        };

        #region Struct
        /// <summary>
        /// Shader parameters.
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Size = 144)]
        private struct MotionBlurEffectParameters
        {
            [FieldOffset(0)]
            public float BlurLength;

            [FieldOffset(16)]
            public Matrix ViewProjectionInverse;

            [FieldOffset(80)]
            public Matrix PreviousViewProjection;
        }
        #endregion

        /// <summary>
        /// Handle the shader parameters struct.
        /// </summary>
        private MotionBlurEffectParameters shaderParameters;

        #region Properties
        /// <summary>
        /// Blur ratio.
        /// </summary>
        public float BlurLength { get; set; }

        /// <summary>
        /// Motion blur quality effect, Low by default.
        /// </summary>
        public EffectQuality Quality { get; set; }

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
        /// Gets the current technique.
        /// </summary>
        /// <value>
        /// The current technique.
        /// </value>
        public override string CurrentTechnique
        {
            get
            {
                int index = (int)this.Quality;
                return techniques[index].Name;
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
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="MotionBlurMaterial"/> class.
        /// </summary>
        public MotionBlurMaterial()
            : base(DefaultLayers.Opaque)
        {
        }

        /// <summary>
        /// Initialize default values.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.SamplerMode = AddressMode.LinearClamp;
            this.BlurLength = 0.5f;
            this.Quality = EffectQuality.Medium;
            this.shaderParameters = new MotionBlurEffectParameters();
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

                // Set PreviousViewProjection Matrix
                this.graphicsDevice.ToShaderMatrix(ref this.previousViewProjection, out this.shaderParameters.PreviousViewProjection);

                this.shaderParameters.BlurLength = this.BlurLength;
                this.Parameters = this.shaderParameters;

                if (this.texture != null)
                {
                    this.graphicsDevice.SetTexture(this.texture, 0);
                }

                // Set Depth texture
                //if (camera.GBufferRT1 != null)
                //{
                //    this.depthTexture = camera.GBufferRT1;
                //}
                //else if (this.renderManager.GraphicsDevice.RenderTargets.IsDepthAsTextureSupported)
                //{
                depthTexture = this.renderManager.GraphicsDevice.RenderTargets.DefaultDepthTexture;
                //}
                //else
                //{
                //    depthTexture = null;
                //}

                if (this.depthTexture != null)
                {
                    this.graphicsDevice.SetTexture(this.depthTexture, 1);
                }

                // Set the previousViewProjection to the next frame
                this.previousViewProjection = viewProjection;
            }
        }
        #endregion
    }
}
