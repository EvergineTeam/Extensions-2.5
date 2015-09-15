#region File Description
//-----------------------------------------------------------------------------
// TiltShiftMaterial
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
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
    /// TiltShiftMaterial effect.
    /// </summary>
    public class TiltShiftMaterial : Material
    {
        public enum Passes
        {
            /// <summary>
            /// Down sampler
            /// </summary>
            Simple,

            /// <summary>
            /// FastBlur
            /// </summary>
            FastBlur,

            /// <summary>
            /// TiltShift
            /// </summary>
            TiltShift,
        }

        /// <summary>
        /// The current pass
        /// </summary>
        public Passes Pass;

        /// <summary>
        /// TiltShift power;
        /// </summary>
        public float Power;

        /// <summary>
        /// TiltShift Position;
        /// </summary>
        public float TiltPosition;

        /// <summary>
        /// The pixel offset
        /// </summary>
        public Vector2 TexcoordOffset;

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
            new ShaderTechnique("Simple", "ImageEffectMaterial", "ImageEffectvsImageEffect", "ImageEffectMaterial", "ImageEffectpsImageEffect", VertexPositionTexture.VertexFormat),
            new ShaderTechnique("FastBlur", string.Empty, "TiltShiftvsFastBlur", string.Empty, "TiltShiftpsFastBlur", VertexPositionTexture.VertexFormat),
            new ShaderTechnique("TiltShift", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "TiltShiftpsTiltShift", VertexPositionTexture.VertexFormat),
        };

        #region Struct
        /// <summary>
        /// Shader parameters.
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Size = 16)]
        private struct TiltShiftEffectParameters
        {
            [FieldOffset(0)]
            public Vector2 TexcoordOffset;

            [FieldOffset(8)]
            public float Power;

            [FieldOffset(12)]
            public float TiltPosition;
        }
        #endregion

        /// <summary>
        /// Handle the shader parameters struct.
        /// </summary>
        private TiltShiftEffectParameters shaderParameters;

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
        /// Initializes a new instance of the <see cref="PosterizeMaterial"/> class.
        /// </summary>
        public TiltShiftMaterial()
            : base(DefaultLayers.Opaque)
        {
            this.SamplerMode = AddressMode.LinearClamp;
            this.TexcoordOffset = Vector2.Zero;
            this.Power = 3;
            this.TiltPosition = 0.5f;

            this.shaderParameters = new TiltShiftEffectParameters();
            this.shaderParameters.TexcoordOffset = this.TexcoordOffset;
            this.shaderParameters.Power = this.Power;
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
                this.shaderParameters.Power = this.Power;
                this.shaderParameters.TiltPosition = this.TiltPosition;
                this.Parameters = this.shaderParameters;

                if (this.Texture != null)
                {
                    this.graphicsDevice.SetTexture(this.Texture, 0);
                }

                if (this.texture1 != null)
                {
                    this.graphicsDevice.SetTexture(this.texture1, 1);
                }
            }
        }
        #endregion
    }
}
