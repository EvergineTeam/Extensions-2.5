#region File Description
//-----------------------------------------------------------------------------
// BloomMaterial
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
    /// Fast Blur effect.
    /// </summary>
    public class BloomMaterial : Material
    {
        public enum Passes
        {
            /// <summary>
            /// Down sampler
            /// </summary>
            DownSampler,

            /// <summary>
            /// The bloom
            /// </summary>
            Bloom,

            /// <summary>
            /// Up combine
            /// </summary>
            UpCombine,
        }

        public Passes Pass;

        /// <summary>
        /// The pixel offset
        /// </summary>
        public Vector2 TexcoordOffset;

        /// <summary>
        /// The bloom tint
        /// </summary>
        public Vector3 BloomTint;

        /// <summary>
        /// The intensity
        /// </summary>
        public float Intensity;

        /// <summary>
        /// The bloom threshold
        /// </summary>
        public float BloomThreshold;

        /// <summary>
        /// The bloom scale
        /// </summary>
        public float BloomScale;

        /// <summary>
        /// The texture
        /// </summary>
        private Texture texture;

        /// <summary>
        /// The texture1
        /// </summary>
        private Texture texture1;

        /// <summary>
        /// The techniques
        /// </summary>
        private static ShaderTechnique[] techniques =
        {
            new ShaderTechnique("DownSampler", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "BloompsBloomDown", VertexPositionTexture.VertexFormat),
            new ShaderTechnique("Bloom", "BloomvsBloom", "BloompsBloom", VertexPositionTexture.VertexFormat),
            new ShaderTechnique("UpCombine", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "BloompsUpCombine", VertexPositionTexture.VertexFormat),
        };

        #region Struct
        /// <summary>
        /// Shader parameters.
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Size = 32)]
        private struct BloomEffectParameters
        {
            [FieldOffset(0)]
            public Vector2 TexcoordOffset;

            [FieldOffset(8)]
            public float BloomThreshold;

            [FieldOffset(12)]
            public float BloomScale;

            [FieldOffset(16)]
            public Vector3 BloomTint;

            [FieldOffset(28)]
            public float Intensity;
        }
        #endregion

        /// <summary>
        /// Handle the shader parameters struct.
        /// </summary>
        private BloomEffectParameters shaderParameters;

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
        /// Gets or sets the texture1.
        /// </summary>
        /// <value>
        /// The texture1.
        /// </value>
        /// <exception cref="System.NullReferenceException">Texture cannot be null.</exception>
        public Texture Texture1
        {
            get
            {
                return this.texture1;
            }

            set
            {
                this.texture1 = value;
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
        /// Initializes a new instance of the <see cref="BloomMaterial"/> class.
        /// </summary>
        public BloomMaterial()
            : base(DefaultLayers.Opaque)
        {
            this.SamplerMode = AddressMode.LinearClamp;
            this.TexcoordOffset = Vector2.Zero;
            this.BloomThreshold = 0.4f;
            this.BloomScale = 8.0f;
            this.Intensity = 3f;
            this.BloomTint = Color.White.ToVector3();

            this.shaderParameters = new BloomEffectParameters();
            this.shaderParameters.TexcoordOffset = this.TexcoordOffset;
            this.shaderParameters.BloomTint = this.BloomTint;
            this.shaderParameters.Intensity = this.Intensity;
            this.shaderParameters.BloomThreshold = this.BloomThreshold;
            this.shaderParameters.BloomScale = this.BloomScale;
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
                this.shaderParameters.TexcoordOffset = this.TexcoordOffset;
                this.shaderParameters.BloomTint = this.BloomTint;
                this.shaderParameters.Intensity = this.Intensity;
                this.shaderParameters.BloomThreshold = this.BloomThreshold;
                this.shaderParameters.BloomScale = this.BloomScale;
                this.Parameters = this.shaderParameters;

                if (this.texture != null)
                {
                    this.graphicsDevice.SetTexture(this.texture, 0);
                }

                if (this.texture1 != null)
                {
                    this.graphicsDevice.SetTexture(this.texture1, 1);
                }
            }
        }

        /// <summary>
        /// Apply this material
        /// </summary>
        /// <param name="renderManager">The renderManager</param>
        public override void Apply(Framework.Managers.RenderManager renderManager)
        {
            base.Apply(renderManager);
        }
        #endregion
    }
}
