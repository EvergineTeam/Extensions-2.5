#region File Description
//-----------------------------------------------------------------------------
// SepiaMaterial
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
    /// Sepia postprocessing effect.
    /// </summary>
    public class SepiaMaterial : Material
    {
        /// <summary>
        /// The image tone
        /// </summary>
        public Vector3 ImageTone;

        /// <summary>
        /// The desaturation
        /// </summary>
        public float Desaturation;

        /// <summary>
        /// The dark tone
        /// </summary>
        public Vector3 DarkTone;

        /// <summary>
        /// The toning
        /// </summary>
        public float Toning;

        /// <summary>
        /// The grey transfer
        /// </summary>
        public Vector3 GreyTransfer;

        /// <summary>
        /// The global alpha
        /// </summary>
        public float GlobalAlpha;

        /// <summary>
        /// The texture
        /// </summary>
        private Texture texture;

        /// <summary>
        /// The techniques
        /// </summary>
        private static ShaderTechnique[] techniques =
        {
            new ShaderTechnique("Sepia", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "SepiapsSepia", VertexPositionTexture.VertexFormat),
        };

        #region Struct
        /// <summary>
        /// Shader parameters.
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Size = 48)]
        private struct SepiaEffectParameters
        {
            /// <summary>
            /// The image tone
            /// </summary>
            [FieldOffset(0)]
            public Vector3 ImageTone;

            /// <summary>
            /// The desaturation
            /// </summary>
            [FieldOffset(12)]
            public float Desaturation;

            /// <summary>
            /// The dark tone
            /// </summary>
            [FieldOffset(16)]
            public Vector3 DarkTone;

            /// <summary>
            /// The toning
            /// </summary>
            [FieldOffset(28)]
            public float Toning;

            /// <summary>
            /// The grey transfer
            /// </summary>
            [FieldOffset(32)]
            public Vector3 GreyTransfer;

            /// <summary>
            /// The global alpha
            /// </summary>
            [FieldOffset(44)]
            public float GlobalAlpha;

        }
        #endregion

        /// <summary>
        /// Handle the shader parameters struct.
        /// </summary>
        private SepiaEffectParameters shaderParameters;

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
        /// Initializes a new instance of the <see cref="SepiaMaterial"/> class.
        /// </summary>
        public SepiaMaterial()
            : base(DefaultLayers.Opaque)
        {

            this.SamplerMode = AddressMode.LinearClamp;
            this.ImageTone = new Vector3(0.815f, 0.666f, 0f);
            this.Desaturation = 1.0f;
            this.DarkTone = new Vector3(0.313f, 0.258f, 0f);
            this.Toning = 0.35f;
            this.GreyTransfer = new Vector3(0.3f, 0.59f, 0.11f);
            this.GlobalAlpha = 1.0f;

            this.shaderParameters = new SepiaEffectParameters();
            this.shaderParameters.ImageTone = this.ImageTone;
            this.shaderParameters.Desaturation = this.Desaturation;
            this.shaderParameters.DarkTone = this.DarkTone;
            this.shaderParameters.Toning = this.Toning;
            this.shaderParameters.GreyTransfer = this.GreyTransfer;
            this.shaderParameters.GlobalAlpha = this.GlobalAlpha;
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
                this.shaderParameters.ImageTone = this.ImageTone;
                this.shaderParameters.Desaturation = this.Desaturation;
                this.shaderParameters.DarkTone = this.DarkTone;
                this.shaderParameters.Toning = this.Toning;
                this.shaderParameters.GreyTransfer = this.GreyTransfer;
                this.shaderParameters.GlobalAlpha = this.GlobalAlpha;
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
