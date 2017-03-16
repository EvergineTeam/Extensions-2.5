#region File Description
//-----------------------------------------------------------------------------
// LightShaftMaterial
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Runtime.InteropServices;
using System.Linq;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Graphics.VertexFormats;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;

#endregion

namespace WaveEngine.ImageEffects
{
    /// <summary>
    /// Light Shaft effect.
    /// </summary>
    public class LightShaftMaterial : Material
    {
        /// <summary>
        /// Light Shaft quality
        /// </summary>
        public enum EffectQuality
        {
            /// <summary>
            /// Only 32 samples
            /// </summary>
            Low = 1,

            /// <summary>
            /// 64 samples
            /// </summary>
            Medium = 2,

            /// <summary>
            /// 96 samples
            /// </summary>
            High = 3
        }

        /// <summary>
        /// Effect passes
        /// </summary>
        public enum Passes
        {
            /// <summary>
            /// Combine
            /// </summary>
            Combine = 0,

            /// <summary>
            /// Down sampler
            /// </summary>
            BlackMask = 1,

            /// <summary>
            /// The blur
            /// </summary>
            LightShaft = 2,
        }

        /// <summary>
        /// The platform service
        /// </summary>
        private static Platform platform = WaveServices.Platform;

        /// <summary>
        /// Steps of this effects
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
            new ShaderTechnique("Combine", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "psLSCombine", VertexPositionTexture.VertexFormat),
            new ShaderTechnique("BlackMask", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "psBlackMask", VertexPositionTexture.VertexFormat),
            new ShaderTechnique("BlackMask_Halo", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "psBlackMask", VertexPositionTexture.VertexFormat, null, new string[]{"HALO"}),
            new ShaderTechnique("LightShaft_Low", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "psLightShaft", VertexPositionTexture.VertexFormat, null, new string[]{"LOW"}),
            new ShaderTechnique("LightShaft_Medium", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "psLightShaft", VertexPositionTexture.VertexFormat, null, new string[]{"MEDIUM"}),
            new ShaderTechnique("LightShaft_High", "ImageEffectMaterial", "ImageEffectvsImageEffect", string.Empty, "psLightShaft", VertexPositionTexture.VertexFormat, null, new string[]{"HIGH"}),
        };

        #region Struct
        /// <summary>
        /// Shader parameters.
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Size = 64)]
        private struct LSEffectParameters
        {
            [FieldOffset(0)]
            public Vector2 LightCenter;

            [FieldOffset(8)]
            public Vector2 TexcoordOffset;

            [FieldOffset(16)]
            public float Density;

            [FieldOffset(20)]
            public float Weight;

            [FieldOffset(24)]
            public float Decay;

            [FieldOffset(28)]
            public float Exposure;

            [FieldOffset(32)]
            public float Blend;

            [FieldOffset(36)]
            public Vector3 ShaftTint;

            [FieldOffset(48)]
            public float Radius;

            [FieldOffset(52)]
            public float EdgeSharpness;

            [FieldOffset(56)]
            public float SunIntensity;

            [FieldOffset(60)]
            public float DepthThreshold;
        }
        #endregion

        /// <summary>
        /// Handle the shader parameters struct.
        /// </summary>
        private LSEffectParameters shaderParameters;

        #region Properties
        /// <summary>
        /// Light Shaft quality effect, Low by default.
        /// </summary>
        public EffectQuality Quality { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [automatic intensity].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [automatic intensity]; otherwise, <c>false</c>.
        /// </value>
        public bool AutoIntensity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [halo enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [halo enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool HaloEnabled { get; set; }

        /// <summary>
        /// Sun Radius.
        /// </summary>
        public float Radius { get; set; }

        /// <summary>
        /// Sun EdgeSharpness.
        /// </summary>
        public float EdgeSharpness { get; set; }

        /// <summary>
        /// Sun intensity.
        /// </summary>
        public float SunIntensity { get; set; }

        /// <summary>
        /// Depth Threshold
        /// </summary>
        public float DepthThreshold { get; set; }

        /// <summary>
        /// Shaft mask blend.
        /// </summary>
        public float Blend { get; set; }

        /// <summary>
        /// Shaft density.
        /// </summary>
        public float Density { get; set; }

        /// <summary>
        /// Shaft weight.
        /// </summary>
        public float Weight { get; set; }

        /// <summary>
        /// Shaft decay.
        /// </summary>
        public float Decay { get; set; }

        /// <summary>
        /// Shaft exposure.
        /// </summary>
        public float Exposure { get; set; }

        /// <summary>
        /// Focus Distance
        /// </summary>
        public Color ShaftTint { get; set; }

        /// <summary>
        /// Focus range
        /// </summary>
        public Vector2 LightCenter { get; set; }
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

        public DirectionalLightProperties DirectionalLight
        {
            get;
            set;
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

                switch (index)
                {
                    case 1:
                        if (this.HaloEnabled)
                        {
                            index++;
                        }

                        break;
                    case 2:
                        index += (int)this.Quality;

                        break;
                }

                return techniques[index].Name;
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="LightShaftMaterial"/> class.
        /// </summary>
        public LightShaftMaterial()
            : base(DefaultLayers.Opaque)
        {
        }

        /// <summary>
        /// Initialize default values for this instance-
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.Quality = EffectQuality.Low;
            this.HaloEnabled = true;
            this.AutoIntensity = true;

            this.Radius = 0.5f;
            this.EdgeSharpness = 1f;
            this.SunIntensity = 0.095f;
            this.DepthThreshold = 0.9999f;

            this.Density = 0.8f;
            this.Weight = 0.8f;
            this.Decay = 0.86f;
            this.Exposure = 1f;
            this.Blend = 1f;

            this.SamplerMode = AddressMode.LinearClamp;
            this.LightCenter = new Vector2(0.5f);
            this.ShaftTint = Color.White;
            this.shaderParameters = new LSEffectParameters();
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
                if (Pass != Passes.Combine)
                {
                    // Calcule light projection
                    Camera camera = this.renderManager.CurrentDrawingCamera;
                    Vector4 lightPosition = new Vector4(0, 0, -1, 0);

                    if (this.renderManager.Lights.Count() > 0)
                    {
                        if (this.DirectionalLight == null)
                        {
                            foreach (var light in this.renderManager.Lights)
                            {
                                if (light is DirectionalLightProperties)
                                {
                                    this.DirectionalLight = light as DirectionalLightProperties;
                                    break;
                                }
                            }
                        }

                        if (this.DirectionalLight != null)
                        {
                            lightPosition.X = -this.DirectionalLight.Direction.X;
                            lightPosition.Y = -this.DirectionalLight.Direction.Y;
                            lightPosition.Z = -this.DirectionalLight.Direction.Z;
                            lightPosition.W = 0;                         
                        }
                    }

                    Vector4 screenPosition = Vector4.Transform(lightPosition, camera.ViewProjection);
                    float signW = Math.Sign(screenPosition.W);
                    screenPosition /= screenPosition.W;

                    this.shaderParameters.LightCenter = new Vector2(screenPosition.X * 0.5f + 0.5f, 1 - (screenPosition.Y * 0.5f + 0.5f));
                    this.shaderParameters.LightCenter *= signW;

                    if (platform.AdapterType != Common.AdapterType.DirectX)
                    {
                        // Flip Y on OpenGL devices
                        this.shaderParameters.LightCenter.Y = 1 - this.shaderParameters.LightCenter.Y;
                    }

                    if (Pass == Passes.BlackMask)
                    {
                        // Black Mask PASS
                        depthTexture = this.renderManager.GraphicsDevice.RenderTargets.DefaultDepthTexture;                        
                        this.shaderParameters.TexcoordOffset = this.TexcoordOffset;


                        this.shaderParameters.SunIntensity = this.SunIntensity / (1 - this.DepthThreshold);
                        this.shaderParameters.DepthThreshold = this.DepthThreshold;
                        this.shaderParameters.Radius = this.Radius;
                        this.shaderParameters.EdgeSharpness = this.EdgeSharpness;

                        this.Parameters = this.shaderParameters;

                        if (this.depthTexture != null)
                        {
                            this.graphicsDevice.SetTexture(this.depthTexture, 2);
                        }
                    }
                    else
                    {
                        // Light Shaft PASS

                        // Compute Light Intensity
                        float intensity = 1f;

                        if (this.AutoIntensity)
                        {
                            intensity = Math.Max(0, Vector3.Dot(camera.View.Backward, this.DirectionalLight.Direction));
                        }

                        this.shaderParameters.ShaftTint = this.ShaftTint.ToVector3();
                        this.shaderParameters.Blend = this.Blend;
                        this.shaderParameters.Density = this.Density;
                        this.shaderParameters.Weight = this.Weight;
                        this.shaderParameters.Decay = this.Decay;
                        this.shaderParameters.Exposure = this.Exposure * intensity;
                        this.Parameters = this.shaderParameters;
                    }
                }
                else
                {
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
