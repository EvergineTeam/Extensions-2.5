#region File Description
//-----------------------------------------------------------------------------
// SobelMaterial
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
    /// Sobel postprocessing effect.
    /// </summary>
    public class SobelMaterial : Material
    {
        /// <summary>
        /// Sobel Effect type
        /// </summary>
        public enum SobelEffect
        {
            /// <summary>
            /// The sobel
            /// </summary>
            Sobel,

            /// <summary>
            /// The sobel edge
            /// </summary>
            SobelEdge,

            /// <summary>
            /// The sobel edge color
            /// </summary>
            SobelEdgeColor
        }

        /// <summary>
        /// The effect
        /// </summary>
        public SobelEffect Effect;

        /// <summary>
        /// The threshold
        /// </summary>
        public float Threshold;

        /// <summary>
        /// The pixel offset
        /// </summary>
        public Vector2 TexcoordOffset;

        /// <summary>
        /// The texture
        /// </summary>
        private Texture texture;

        /// <summary>
        /// The techniques
        /// </summary>
        private static ShaderTechnique[] techniques =
        {
            new ShaderTechnique("Sobel", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "SobelpsSobel", VertexPositionTexture.VertexFormat),
            new ShaderTechnique("SobelEdge", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "SobelpsSobelEdge", VertexPositionTexture.VertexFormat),
            new ShaderTechnique("SobelEdgeColor", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "SobelpsSobelEdgeColor", VertexPositionTexture.VertexFormat),
        };

        #region Struct
        /// <summary>
        /// Shader parameters.
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Size = 16)]
        private struct SobelEffectParameters
        {
            /// <summary>
            /// Used has threshold to detect most edges.
            /// </summary>
            [FieldOffset(0)]
            public float Threshold;

            /// <summary>
            /// The pixel offset
            /// </summary>
            [FieldOffset(4)]
            public Vector2 TexcoordOffset;
        }
        #endregion

        /// <summary>
        /// Handle the shader parameters struct.
        /// </summary>
        private SobelEffectParameters shaderParameters;

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
                int index = (int)Effect;
                return techniques[index].Name;
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="SobelMaterial"/> class.
        /// </summary>
        public SobelMaterial()
            : base(DefaultLayers.Opaque)
        {
            this.SamplerMode = AddressMode.LinearClamp;
            this.Effect = SobelEffect.Sobel;
            this.Threshold = 0.0049f;
            this.TexcoordOffset = Vector2.Zero;

            this.shaderParameters = new SobelEffectParameters();
            this.shaderParameters.Threshold = this.Threshold;
            this.shaderParameters.TexcoordOffset = this.TexcoordOffset;
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
                this.shaderParameters.Threshold = this.Threshold;
                this.shaderParameters.TexcoordOffset = this.TexcoordOffset;
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
