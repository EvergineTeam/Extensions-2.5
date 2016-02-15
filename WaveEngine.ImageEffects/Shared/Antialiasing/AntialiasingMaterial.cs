#region File Description
//-----------------------------------------------------------------------------
// SobelMaterial
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
    /// Antialiasing effect.
    /// </summary>
    public class AntialiasingMaterial : Material
    {
        /// <summary>
        /// The texcoord offset
        /// </summary>
        public Vector2 TexcoordOffset;

        /// <summary>
        /// The span_max
        /// </summary>
        public float Span_max;

        /// <summary>
        /// The reduce_ mul
        /// </summary>
        public float Reduce_Mul;

        /// <summary>
        /// The reduce_ minimum
        /// </summary>
        public float Reduce_Min;

        /// <summary>
        /// The texture
        /// </summary>
        private Texture texture;

        /// <summary>
        /// The techniques
        /// </summary>
        private static ShaderTechnique[] techniques =
        {
            new ShaderTechnique("FXAA", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "AntialiasingpsFXAA", VertexPositionTexture.VertexFormat),
        };

        #region Struct
        /// <summary>
        /// Shader parameters.
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Size = 32)]
        private struct AntialiasingEffectParameters
        {
            [FieldOffset(0)]
            public Vector2 TexcoordOffset;

            [FieldOffset(8)]
            public float Span_max;

            [FieldOffset(12)]
            public float Reduce_Mul;

            [FieldOffset(16)]
            public float Reduce_Min;
        }
        #endregion

        /// <summary>
        /// Handle the shader parameters struct.
        /// </summary>
        private AntialiasingEffectParameters shaderParameters;

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
        /// Initializes a new instance of the <see cref="AntialiasingMaterial"/> class.
        /// </summary>
        public AntialiasingMaterial()
            : base(DefaultLayers.Opaque)
        {
            this.SamplerMode = AddressMode.LinearClamp;
            this.TexcoordOffset = Vector2.Zero;
            this.Span_max = 8.0f;
            this.Reduce_Mul = 1.0f / 8.0f;
            this.Reduce_Min = 1.0f / 128.0f;

            this.shaderParameters = new AntialiasingEffectParameters();
            this.shaderParameters.TexcoordOffset = this.TexcoordOffset;
            this.shaderParameters.Span_max= this.Span_max;
            this.shaderParameters.Reduce_Mul = this.Reduce_Mul;
            this.shaderParameters.Reduce_Min = this.Reduce_Min;
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
                this.shaderParameters.Span_max = this.Span_max;
                this.shaderParameters.Reduce_Mul = this.Reduce_Mul;
                this.shaderParameters.Reduce_Min = this.Reduce_Min;
                this.Parameters = this.shaderParameters;

                if (this.Texture != null)
                {
                    this.graphicsDevice.SetTexture(this.Texture, 0);
                }
            }
        }
        #endregion
    }
}
