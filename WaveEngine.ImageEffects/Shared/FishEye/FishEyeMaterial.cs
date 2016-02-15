#region File Description
//-----------------------------------------------------------------------------
// FishEyeMaterial
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
    /// FishEye displacement effect.
    /// </summary>
    public class FishEyeMaterial : Material
    {
        /// <summary>
        /// The intensity
        /// </summary>
        public Vector2 Intensity;

        /// <summary>
        /// The texture
        /// </summary>
        private Texture texture;

        /// <summary>
        /// The techniques
        /// </summary>
        private static ShaderTechnique[] techniques =
        {
            new ShaderTechnique("FishEye", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "FishEyepsFishEye", VertexPositionTexture.VertexFormat),
        };

        #region Struct
        /// <summary>
        /// Shader parameters.
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Size = 16)]
        private struct FishEyeEffectParameters
        {
            /// <summary>
            /// The intesity
            /// </summary>
            [FieldOffset(0)]
            public Vector2 Intensity;
        }
        #endregion

        /// <summary>
        /// Handle the shader parameters struct.
        /// </summary>
        private FishEyeEffectParameters shaderParameters;

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
        public FishEyeMaterial()
            : base(DefaultLayers.Opaque)
        {
            this.Intensity = new Vector2(0.001f);
            this.shaderParameters = new FishEyeEffectParameters();
            this.shaderParameters.Intensity = this.Intensity;
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
                this.shaderParameters = new FishEyeEffectParameters();
                this.shaderParameters.Intensity = this.Intensity;

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
