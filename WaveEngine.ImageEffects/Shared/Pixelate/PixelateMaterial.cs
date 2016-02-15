#region File Description
//-----------------------------------------------------------------------------
// PixelateMaterial
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
    /// Pixelate postprocessing effect.
    /// </summary>
    public class PixelateMaterial : Material
    {
        /// <summary>
        /// The divisions
        /// </summary>
        public Vector2 PixelSize;

        /// <summary>
        /// The texture
        /// </summary>
        private Texture texture;

        /// <summary>
        /// The techniques
        /// </summary>
        private static ShaderTechnique[] techniques =
        {
            new ShaderTechnique("Pixelate", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "PixelatepsPixel", VertexPositionTexture.VertexFormat),
        };

        #region Struct
        /// <summary>
        /// Shader parameters.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Size = 16)]
        private struct PixelateEffectParameters
        {
            /// <summary>
            /// The divisions
            /// </summary>
            public Vector2 PixelSize;
        }
        #endregion

        /// <summary>
        /// Handle the shader parameters struct.
        /// </summary>
        private PixelateEffectParameters shaderParameters;

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
        /// Initializes a new instance of the <see cref="PixelateMaterial"/> class.
        /// </summary>
        public PixelateMaterial()
            : base(DefaultLayers.Opaque)
        {
            this.SamplerMode = AddressMode.LinearClamp;
            this.PixelSize = new Vector2(10);

            this.shaderParameters = new PixelateEffectParameters();
            this.shaderParameters.PixelSize = this.PixelSize;
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
                this.shaderParameters.PixelSize = this.PixelSize;
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
