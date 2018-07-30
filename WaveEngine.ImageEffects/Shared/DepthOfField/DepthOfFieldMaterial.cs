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
    /// Depth of field effect.
    /// </summary>
    public class DepthOfFieldMaterial : Material
    {
        /// <summary>
        /// Effect's steps
        /// </summary>
        public enum Passes
        {
            /// <summary>
            /// Down sampler
            /// </summary>
            DownSampler,

            /// <summary>
            /// The blur
            /// </summary>
            Blur,

            /// <summary>
            /// Combine
            /// </summary>
            Combine,
        }

        /// <summary>
        /// Steps of this effects
        /// </summary>
        public Passes Pass;

        /// <summary>
        /// The pixel offset
        /// </summary>
        public Vector2 TexcoordOffset;

        /// <summary>
        /// The blur scale
        /// </summary>
        public float BlurScale;

        /// <summary>
        /// The texture
        /// </summary>
        private Texture texture;

        /// <summary>
        /// The texture1
        /// </summary>
        private Texture texture1;

        /// <summary>
        /// The depth texture
        /// </summary>
        private Texture depthTexture;

        /// <summary>
        /// The techniques
        /// </summary>
        private static ShaderTechnique[] techniques =
        {
            new ShaderTechnique("DownSampler", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "psDown", VertexPositionTexture.VertexFormat),
            new ShaderTechnique("Blur", "vsBlur", "psBlur", VertexPositionTexture.VertexFormat),
            new ShaderTechnique("Combine", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "psCombine", VertexPositionTexture.VertexFormat),
        };

        #region Struct

        /// <summary>
        /// Shader parameters.
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Size = 32)]
        private struct DOFEffectParameters
        {
            [FieldOffset(0)]
            public Vector2 TexcoordOffset;

            [FieldOffset(8)]
            public float FocusDistance;

            [FieldOffset(12)]
            public float FocusRange;

            [FieldOffset(16)]
            public float NearPlane;

            [FieldOffset(20)]
            public float FarParam;

            [FieldOffset(24)]
            public float BlurScale;
        }
        #endregion

        /// <summary>
        /// Handle the shader parameters struct.
        /// </summary>
        private DOFEffectParameters shaderParameters;

        #region Properties

        /// <summary>
        /// Gets or sets focus Distance
        /// </summary>
        public float FocusDistance { get; set; }

        /// <summary>
        /// Gets or sets focus range
        /// </summary>
        public float FocusRange { get; set; }

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
                if (value == null)
                {
                    throw new NullReferenceException("Texture cannot be null.");
                }

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

        /// <summary>
        /// Gets or sets the Depth Texture.
        /// </summary>
        public Texture DepthTexture
        {
            get
            {
                return this.depthTexture;
            }

            set
            {
                this.depthTexture = value;
            }
        }
        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="DepthOfFieldMaterial"/> class.
        /// </summary>
        public DepthOfFieldMaterial()
            : base(DefaultLayers.Opaque)
        {
        }

        /// <summary>
        /// Initialize default values for this instance.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.BlurScale = 4.0f;
            this.FocusRange = 2;
            this.FocusDistance = 4;
            this.TexcoordOffset = Vector2.Zero;
            this.shaderParameters = new DOFEffectParameters();
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
                this.shaderParameters.TexcoordOffset = this.TexcoordOffset;

                if (this.Pass == Passes.Combine)
                {
                    Camera camera = this.renderManager.CurrentDrawingCamera;

                    this.shaderParameters.FocusDistance = this.FocusDistance;
                    this.shaderParameters.FocusRange = this.FocusRange;
                    this.shaderParameters.NearPlane = camera.NearPlane;
                    this.shaderParameters.FarParam = camera.FarPlane / (camera.FarPlane - camera.NearPlane);
                    this.shaderParameters.BlurScale = this.BlurScale;
                    this.Parameters = this.shaderParameters;

                    this.depthTexture = this.renderManager.GraphicsDevice.RenderTargets.DefaultDepthTexture;

                    if (this.depthTexture != null)
                    {
                        this.graphicsDevice.SetTexture(this.depthTexture, 2);
                    }
                }

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
        #endregion
    }
}
