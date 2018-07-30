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
    /// ChromaticAberration effect.
    /// </summary>
    public class ChromaticAberrationMaterial : Material
    {
        /// <summary>
        /// The chromatic aberration
        /// </summary>
        public float AberrationStrength;

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
            new ShaderTechnique("ChromaticAberration", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "ChromaticAberrationpsSimple", VertexPositionTexture.VertexFormat),
        };

        #region Struct

        /// <summary>
        /// Shader parameters.
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Size = 16)]
        private struct ChromaticAberrationEffectParameters
        {
            [FieldOffset(0)]
            public float AberrationStrength;

            [FieldOffset(4)]
            public Vector2 TexcoordOffset;
        }
        #endregion

        /// <summary>
        /// Handle the shader parameters struct.
        /// </summary>
        private ChromaticAberrationEffectParameters shaderParameters;

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
        /// Initializes a new instance of the <see cref="ChromaticAberrationMaterial"/> class.
        /// </summary>
        public ChromaticAberrationMaterial()
            : base(DefaultLayers.Opaque)
        {
            this.AberrationStrength = 10f;
            this.TexcoordOffset = Vector2.Zero;

            this.shaderParameters = new ChromaticAberrationEffectParameters();
            this.shaderParameters.AberrationStrength = this.AberrationStrength;
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
                this.shaderParameters.AberrationStrength = this.AberrationStrength;
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
