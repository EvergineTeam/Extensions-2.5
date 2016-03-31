#region File Description
//-----------------------------------------------------------------------------
// LensFlareMaterial
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
using System.Reflection;
using WaveEngine.Common.IO;
using WaveEngine.Common.Helpers;
#endregion

namespace WaveEngine.ImageEffects
{
    /// <summary>
    /// GPU Lens flares as image effects.
    /// </summary>
    public class LensFlareMaterial : Material
    {
        /// <summary>
        /// Passes for this material.
        /// </summary>
        public enum Passes
        {
            /// <summary>
            /// CombineUp pass.
            /// </summary>
            Combine,

            /// <summary>
            /// The blur pass.
            /// </summary>
            Blur,

            /// <summary>
            /// The LensFlare pass.
            /// </summary>
            LensFlare,

            /// <summary>
            /// Downsampler pass.
            /// </summary>
            DownSampler,
        }

        /// <summary>
        /// The pass
        /// </summary>
        public Passes Pass;
       
        /// <summary>
        /// The pixel offset
        /// </summary>
        public Vector2 TexcoordOffset;

        /// <summary>
        /// The texture
        /// </summary>
        private Texture texture;

        /// <summary>
        /// The lens flare texture
        /// </summary>
        private Texture lensFlareTexture;

        /// <summary>
        /// The lens color
        /// </summary>
        private Texture lensColorTexture;

        /// <summary>
        /// The lens dirt
        /// </summary>
        private Texture lensDirtTexture;

        /// <summary>
        /// The lens star
        /// </summary>
        private Texture lensStarTexture;

        /// <summary>
        /// The techniques
        /// </summary>
        private static ShaderTechnique[] techniques =
        {
            new ShaderTechnique("Combine", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "psLensFlareCombine", VertexPositionTexture.VertexFormat),
            new ShaderTechnique("Blur", string.Empty, "vsLensFlareBlur", string.Empty, "psLensFlareBlur", VertexPositionTexture.VertexFormat),            
            new ShaderTechnique("LensFlare", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "psLensFlare", VertexPositionTexture.VertexFormat),
            new ShaderTechnique("Down", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "psLensFlareDown", VertexPositionTexture.VertexFormat),            
        };

        #region Struct
        /// <summary>
        /// Shader parameters.
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Size = 64)]
        private struct LensFlareEffectParameters
        {
            [FieldOffset(0)]
            public Vector3 Bias;

            [FieldOffset(12)]
            public float HaloWidth;

            [FieldOffset(16)]
            public Vector3 Scale;

            [FieldOffset(28)]
            public float GhostDispersal;

            [FieldOffset(32)]
            public float Distortion;

            [FieldOffset(36)]
            public Vector2 TexcoordOffset;

            [FieldOffset(44)]
            public float Intensity;

            [FieldOffset(48)]
            public Vector2 StarRotation;
        }
        #endregion

        /// <summary>
        /// Handle the shader parameters struct.
        /// </summary>
        private LensFlareEffectParameters shaderParameters;

        #region Properties

        /// <summary>
        /// Bias.
        /// </summary>
        public float Bias { get; set; }

        /// <summary>
        /// Scale.
        /// </summary>
        public float Scale { get; set; }

        /// <summary>
        /// Gets or sets the ghost dispersal.
        /// </summary>
        public float GhostDispersal { get; set; }

        /// <summary>
        /// Gets or sets the width of the halo.
        /// </summary>
        public float HaloWidth { get; set; }

        /// <summary>
        /// Gets or sets the distortion.
        /// </summary>
        public float Distortion { get; set; }

        /// <summary>
        /// Gets or sets the intensity.
        /// </summary>
        public float Intensity { get; set; }

        /// <summary>
        /// Gets or sets the texture.
        /// </summary>
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
        /// Gets or sets the LensColor.
        /// </summary>
        public Texture LensColorTexture
        {
            get
            {
                return this.lensColorTexture;
            }

            set
            {
                this.lensColorTexture = value;
            }
        }

        /// <summary>
        /// Gets or sets the LensDirt.
        /// </summary>
        public Texture LensDirtTexture
        {
            get
            {
                return this.lensDirtTexture;
            }

            set
            {
                this.lensDirtTexture = value;
            }
        }

        /// <summary>
        /// Gets or sets the LensStar.
        /// </summary>
        public Texture LensStarTexture
        {
            get
            {
                return this.lensStarTexture;
            }

            set
            {
                this.lensStarTexture = value;
            }
        }

        /// <summary>
        /// Gets or sets the LensFlareTexture.
        /// </summary>
        public Texture LensFlareTexture
        {
            get
            {
                return this.lensFlareTexture;
            }

            set
            {
                this.lensFlareTexture = value;
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
        /// Initializes a new instance of the <see cref="LensFlareMaterial"/> class.
        /// </summary>
        public LensFlareMaterial()
            : base(DefaultLayers.Opaque)
        {
        }

        /// <summary>
        /// Default value for this instance.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.SamplerMode = AddressMode.LinearClamp;
            this.Bias = -0.9f;
            this.Scale = 7f;
            this.GhostDispersal = 0.37f;
            this.HaloWidth = 0.47f;
            this.Distortion = 1.5f;
            this.Intensity = 5f;
            this.shaderParameters = new LensFlareEffectParameters();
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

            // LensColor
            var assembly = ReflectionHelper.GetMemberAssembly(this);
            var currentNamespace = assembly.GetName().Name;
            var textureResourcePath = currentNamespace + "LensFlare.LensColor.wpk";
            var textureStream = ResourceLoader.GetEmbeddedResourceStream(assembly, textureResourcePath);
            this.lensColorTexture = assets.LoadAsset<Texture2D>(textureResourcePath, textureStream);

            // LensDirt
            textureResourcePath = currentNamespace + "LensFlare.LensDirt.wpk";
            textureStream = ResourceLoader.GetEmbeddedResourceStream(assembly, textureResourcePath);
            this.lensDirtTexture = assets.LoadAsset<Texture2D>(textureResourcePath, textureStream);

            // LensStar
            textureResourcePath = currentNamespace + "LensFlare.LensStar.wpk";
            textureStream = ResourceLoader.GetEmbeddedResourceStream(assembly, textureResourcePath);
            this.lensStarTexture = assets.LoadAsset<Texture2D>(textureResourcePath, textureStream);
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
                Camera camera = this.renderManager.CurrentDrawingCamera;

                this.shaderParameters.Bias = new Vector3(this.Bias);
                this.shaderParameters.Scale = new Vector3(this.Scale);
                this.shaderParameters.GhostDispersal = this.GhostDispersal;
                this.shaderParameters.HaloWidth = this.HaloWidth;
                this.shaderParameters.Distortion = this.Distortion;
                this.shaderParameters.TexcoordOffset = this.TexcoordOffset;
                this.shaderParameters.Intensity = this.Intensity;
                float rotation = Vector3.Dot(camera.View.Left, new Vector3(0, 0, 1)) + Vector3.Dot(camera.View.Forward, Vector3.Up);
                this.shaderParameters.StarRotation = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));
                this.Parameters = this.shaderParameters;

                if (this.texture != null)
                {
                    this.graphicsDevice.SetTexture(this.texture, 0);
                }

                if (this.lensColorTexture != null)
                {
                    this.graphicsDevice.SetTexture(this.lensColorTexture, 1);
                }

                if (this.lensDirtTexture != null)
                {
                    this.graphicsDevice.SetTexture(this.lensDirtTexture, 2);
                }

                if (this.lensStarTexture != null)
                {
                    this.graphicsDevice.SetTexture(this.lensStarTexture, 3);
                }

                if (this.lensFlareTexture != null)
                {
                    this.graphicsDevice.SetTexture(this.lensFlareTexture, 4);
                }
            }
        }
        #endregion
    }
}
