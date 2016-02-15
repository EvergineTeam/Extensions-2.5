#region File Description
//-----------------------------------------------------------------------------
// GaussianBlurMaterial
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
    /// Gaussian Blur effect.
    /// </summary>
    public class GaussianBlurMaterial : Material
    {
        /// <summary>
        /// Effects passes.
        /// </summary>
        public enum Passes
        {
            /// <summary>
            /// Horizontal pass.
            /// </summary>
            Horizontal,

            /// <summary>
            /// Vertical pass.
            /// </summary>
            Vertical
        }

        /// <summary>
        /// The texture
        /// </summary>
        private Texture texture;

        /// <summary>
        /// The techniques
        /// </summary>
        private static ShaderTechnique[] techniques =
        {
            new ShaderTechnique("GaussianBlur", string.Empty, "vsGaussianBlur", string.Empty, "psGaussianBlur", VertexPositionTexture.VertexFormat),
        };

        #region Struct
        /// <summary>
        /// Shader parameters.
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Size = 176)]
        private struct BlurEffectParameters
        {
            [FieldOffset(0)]
            public Vector2 SampleOffsets0;

            [FieldOffset(8)]
            public Vector2 SampleOffsets1;

            [FieldOffset(16)]
            public Vector2 SampleOffsets2;

            [FieldOffset(24)]
            public Vector2 SampleOffsets3;

            [FieldOffset(32)]
            public Vector2 SampleOffsets4;

            [FieldOffset(40)]
            public Vector2 SampleOffsets5;

            [FieldOffset(48)]
            public Vector2 SampleOffsets6;

            [FieldOffset(56)]
            public Vector2 SampleOffsets7;

            [FieldOffset(64)]
            public Vector2 SampleOffsets8;

            [FieldOffset(72)]
            public Vector2 SampleOffsets9;

            [FieldOffset(80)]
            public Vector2 SampleOffsets10;

            [FieldOffset(88)]
            public Vector2 SampleOffsets11;

            [FieldOffset(96)]
            public Vector2 SampleOffsets12;

            [FieldOffset(104)]
            public Vector2 SampleOffsets13;

            [FieldOffset(112)]
            public float SampleWeights0;

            [FieldOffset(116)]
            public float SampleWeights1;

            [FieldOffset(120)]
            public float SampleWeights2;

            [FieldOffset(124)]
            public float SampleWeights3;

            [FieldOffset(128)]
            public float SampleWeights4;

            [FieldOffset(132)]
            public float SampleWeights5;

            [FieldOffset(136)]
            public float SampleWeights6;

            [FieldOffset(140)]
            public float SampleWeights7;

            [FieldOffset(144)]
            public float SampleWeights8;

            [FieldOffset(148)]
            public float SampleWeights9;

            [FieldOffset(152)]
            public float SampleWeights10;

            [FieldOffset(156)]
            public float SampleWeights11;

            [FieldOffset(160)]
            public float SampleWeights12;

            [FieldOffset(164)]
            public float SampleWeights13;

        }
        #endregion

        /// <summary>
        /// Handle the shader parameters struct.
        /// </summary>
        private BlurEffectParameters shaderParameters;

        #region Properties
        /// <summary>
        /// Effect pass.
        /// </summary>
        public Passes Pass { get; set; }

        /// <summary>
        /// Gaussian Blur factor.
        /// </summary>
        public float Factor { get; set; }

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
        /// Initializes a new instance of the <see cref="GaussianBlurMaterial"/> class.
        /// </summary>
        public GaussianBlurMaterial()
            : base(DefaultLayers.Opaque)
        {
        }

        /// <summary>
        /// Default value for this instance.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.Factor = 1.0f;
            this.SamplerMode = AddressMode.LinearClamp;
            this.shaderParameters = new BlurEffectParameters();
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
            if (this.Pass == Passes.Horizontal)
            {
                ComputeGaussianBlur(this.Factor / this.texture.Width, 0);
            }
            else
            {
                ComputeGaussianBlur(0, this.Factor / this.texture.Height);
            }
                  
            this.Parameters = this.shaderParameters;

            if (this.texture != null)
            {
                this.graphicsDevice.SetTexture(this.texture, 0);
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        private void ComputeGaussianBlur(float dx, float dy)
        {
            // Look up how many samples our gaussian blur effect supports.
            int sampleCount = 15;

            // Create temporary arrays for computing our filter settings.
            float[] sampleWeights = new float[sampleCount];
            Vector2[] sampleOffsets = new Vector2[sampleCount];

            // The first sample always has a zero offset.
            sampleWeights[0] = ComputeGaussian(0);
            sampleOffsets[0] = new Vector2(0);

            // Maintain a sum of all the weighting values.
            float totalWeights = sampleWeights[0];

            // Add pairs of additional sample taps, positioned
            // along a line in both directions from the center.
            for (int i = 0; i < sampleCount / 2; i++)
            {
                // Store weights for the positive and negative taps.
                float weight = ComputeGaussian(i + 1);

                sampleWeights[i * 2 + 1] = weight;
                sampleWeights[i * 2 + 2] = weight;

                totalWeights += weight * 2;

                // To get the maximum amount of blurring from a limited number of
                // pixel shader samples, we take advantage of the bilinear filtering
                // hardware inside the texture fetch unit. If we position our texture
                // coordinates exactly halfway between two texels, the filtering unit
                // will average them for us, giving two samples for the price of one.
                // This allows us to step in units of two texels per sample, rather
                // than just one at a time. The 1.5 offset kicks things off by
                // positioning us nicely in between two texels.
                float sampleOffset = i * 2 + 1.5f;

                Vector2 delta = new Vector2(dx, dy) * sampleOffset;

                // Store texture coordinate offsets for the positive and negative taps.
                sampleOffsets[i * 2 + 1] = delta;
                sampleOffsets[i * 2 + 2] = -delta;
            }

            // Normalize the list of sample weightings, so they will always sum to one.
            for (int i = 0; i < sampleWeights.Length; i++)
            {
                sampleWeights[i] /= totalWeights;
            }

            // Tell the effect about our new filter settings.
            this.shaderParameters.SampleOffsets0 = sampleOffsets[0];
            this.shaderParameters.SampleOffsets1 = sampleOffsets[1];
            this.shaderParameters.SampleOffsets2 = sampleOffsets[2];
            this.shaderParameters.SampleOffsets3 = sampleOffsets[3];
            this.shaderParameters.SampleOffsets4 = sampleOffsets[4];
            this.shaderParameters.SampleOffsets5 = sampleOffsets[5];
            this.shaderParameters.SampleOffsets6 = sampleOffsets[6];
            this.shaderParameters.SampleOffsets7 = sampleOffsets[7];
            this.shaderParameters.SampleOffsets8 = sampleOffsets[8];
            this.shaderParameters.SampleOffsets9 = sampleOffsets[9];
            this.shaderParameters.SampleOffsets10 = sampleOffsets[10];
            this.shaderParameters.SampleOffsets11 = sampleOffsets[11];
            this.shaderParameters.SampleOffsets12 = sampleOffsets[12];
            this.shaderParameters.SampleOffsets13 = sampleOffsets[13];
            
            this.shaderParameters.SampleWeights0 = sampleWeights[0];
            this.shaderParameters.SampleWeights1 = sampleWeights[1];
            this.shaderParameters.SampleWeights2 = sampleWeights[2];
            this.shaderParameters.SampleWeights3 = sampleWeights[3];
            this.shaderParameters.SampleWeights4 = sampleWeights[4];
            this.shaderParameters.SampleWeights5 = sampleWeights[5];
            this.shaderParameters.SampleWeights6 = sampleWeights[6];
            this.shaderParameters.SampleWeights7 = sampleWeights[7];
            this.shaderParameters.SampleWeights8 = sampleWeights[8];
            this.shaderParameters.SampleWeights9 = sampleWeights[9];
            this.shaderParameters.SampleWeights10 = sampleWeights[10];
            this.shaderParameters.SampleWeights11 = sampleWeights[11];
            this.shaderParameters.SampleWeights12 = sampleWeights[12];
            this.shaderParameters.SampleWeights13 = sampleWeights[13];
        }

        /// <summary>
        /// Computes the gaussian.
        /// </summary>
        /// <param name="n">The n.</param>
        /// <returns></returns>
        private float ComputeGaussian(float n)
        {
            float BlurAmount = 4;
            float theta = BlurAmount;

            return (float)((1.0 / Math.Sqrt(2 * Math.PI * theta)) *
                           Math.Exp(-(n * n) / (2 * theta * theta)));
        }
        #endregion
    }
}
