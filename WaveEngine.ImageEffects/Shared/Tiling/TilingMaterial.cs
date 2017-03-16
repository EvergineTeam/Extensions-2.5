#region File Description
//-----------------------------------------------------------------------------
// TilingMaterial
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
    /// Tiling effect.
    /// </summary>
    public class TilingMaterial : Material
    {
        /// <summary>
        /// The edge color
        /// </summary>
        public Vector3 EdgeColor;

        /// <summary>
        /// The number tiles
        /// </summary>
        public float NumTiles;

        /// <summary>
        /// The threshhold
        /// </summary>
        public float Threshhold;

        /// <summary>
        /// The texture
        /// </summary>
        private Texture texture;

        /// <summary>
        /// The techniques
        /// </summary>
        private static ShaderTechnique[] techniques =
        {
            new ShaderTechnique("Tiling", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "TilingpsTiling", VertexPositionTexture.VertexFormat),
        };

        #region Struct
        /// <summary>
        /// Shader parameters.
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Size = 32)]
        private struct TilingEffectParameters
        {
            [FieldOffset(0)]
            public Vector3 EdgeColor;

            [FieldOffset(12)]
            public float NumTiles;

            [FieldOffset(16)]
            public float Threshhold;
        }
        #endregion

        /// <summary>
        /// Handle the shader parameters struct.
        /// </summary>
        private TilingEffectParameters shaderParameters;

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
        public TilingMaterial()
            : base(DefaultLayers.Opaque)
        {
            this.SamplerMode = AddressMode.LinearClamp;
            this.EdgeColor = new Vector3(0.7f);
            this.NumTiles = 75f;
            this.Threshhold = 0.15f;

            this.shaderParameters = new TilingEffectParameters();
            this.shaderParameters.EdgeColor = this.EdgeColor;
            this.shaderParameters.NumTiles = this.NumTiles;
            this.shaderParameters.Threshhold = this.Threshhold;
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
                this.shaderParameters.EdgeColor = this.EdgeColor;
                this.shaderParameters.NumTiles = this.NumTiles;
                this.shaderParameters.Threshhold = this.Threshhold;
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
