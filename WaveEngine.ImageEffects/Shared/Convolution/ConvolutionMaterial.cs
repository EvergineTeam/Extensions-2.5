#region File Description
//-----------------------------------------------------------------------------
// ConvolutionMaterial
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
    /// Laplace effect.
    /// </summary>
    public class ConvolutionMaterial : Material
    {
        /// <summary>
        /// Filter type
        /// </summary>
        public enum FilterType
        {
            /// <summary>
            /// The laplace
            /// </summary>
            Laplace = 0,

            /// <summary>
            /// The laplace grey scale
            /// </summary>
            LaplaceGreyScale,

            /// <summary>
            /// The sharpen
            /// </summary>
            Sharpen,

            /// <summary>
            /// The blur 3x3
            /// </summary>
            Blur3x3,

            /// <summary>
            /// The blur5x5
            /// </summary>
            Blur5x5,

            /// <summary>
            /// The emboss
            /// </summary>
            Emboss
        }

        /// <summary>
        /// The filter
        /// </summary>
        public FilterType Filter;

        /// <summary>
        /// The pixel offset
        /// </summary>
        public Vector2 TexcoordOffset;

        /// <summary>
        /// The number tiles
        /// </summary>
        public float Scale;

        /// <summary>
        /// The texture
        /// </summary>
        private Texture texture;

        /// <summary>
        /// The techniques
        /// </summary>
        private static ShaderTechnique[] techniques =
        {
            new ShaderTechnique("Laplace", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "ConvolutionpsLaplace", VertexPositionTexture.VertexFormat),
            new ShaderTechnique("LaplaceGreyScale", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "ConvolutionpsLaplaceGreyScale", VertexPositionTexture.VertexFormat),
            new ShaderTechnique("Sharpen", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "ConvolutionpsSharpen", VertexPositionTexture.VertexFormat),
            new ShaderTechnique("Blur3x3", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "ConvolutionpsBlur3x3", VertexPositionTexture.VertexFormat),
            new ShaderTechnique("Blur5x5", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "ConvolutionpsBlur5x5", VertexPositionTexture.VertexFormat),
            new ShaderTechnique("Emboss", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "ConvolutionpsEmboss", VertexPositionTexture.VertexFormat),
        };

        #region Struct
        /// <summary>
        /// Shader parameters.
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Size = 16)]
        private struct ConvolutionEffectParameters
        {
            [FieldOffset(0)]
            public Vector2 TexcoordOffset;

            [FieldOffset(8)]
            public float Scale;
        }
        #endregion

        /// <summary>
        /// Handle the shader parameters struct.
        /// </summary>
        private ConvolutionEffectParameters shaderParameters;

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
                int index = (int)this.Filter;
                return techniques[index].Name;
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="AntialiasingMaterial"/> class.
        /// </summary>
        public ConvolutionMaterial()
            : base(DefaultLayers.Opaque)
        {
            this.SamplerMode = AddressMode.LinearClamp;
            this.TexcoordOffset = Vector2.Zero;
            this.Scale = 1.0f;

            this.shaderParameters = new ConvolutionEffectParameters();
            this.shaderParameters.TexcoordOffset = this.TexcoordOffset;
            this.shaderParameters.Scale = this.Scale;
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
                this.shaderParameters.Scale = this.Scale;
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
