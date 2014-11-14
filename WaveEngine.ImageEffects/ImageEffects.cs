#region File Description
//-----------------------------------------------------------------------------
// ImageEffects
//
// Copyright © 2014 Wave Corporation
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Usings Statements

#endregion

namespace WaveEngine.ImageEffects
{
    /// <summary>
    /// Factory to all default image effect materials.
    /// </summary>
    public static class ImageEffects
    {
        /// <summary>
        /// Gets a new <see cref="AntialiasingLens"/>.
        /// </summary>
        /// <returns>A instance of AntialiasingLens.</returns>
        public static AntialiasingLens Antialiasing()
        {
            return new AntialiasingLens();
        }

        /// <summary>
        /// Gets a new <see cref="BloomLens"/>.
        /// </summary>
        /// <returns>A instance of BloomLens.</returns>
        public static BloomLens Bloom()
        {
            return new BloomLens();
        }

        /// <summary>
        /// Gets a new <see cref="ChromaticAberrationLens"/>.
        /// </summary>
        /// <returns>A instance of ChromaticAberrationLens.</returns>
        public static ChromaticAberrationLens ChromaticAberration()
        {
            return new ChromaticAberrationLens();
        }

        /// <summary>
        /// Gets a new <see cref="ConvolutionLens" />.
        /// </summary>
        /// <param name="filter">The filter type.</param>
        /// <returns>
        /// A instance of ConvolutionLens.
        /// </returns>
        public static ConvolutionLens ConvolutionMatrix(ConvolutionMaterial.FilterType filter)
        {
            ConvolutionLens convolution = new ConvolutionLens() { Filter = filter};
            return convolution;
        }

        /// <summary>
        /// Gets a new <see cref="DistortionLens" />.
        /// </summary>
        /// <param name="normalTexturePath">Texture use to apply the distortion effect.</param>
        /// <returns>
        /// A instance of DistortionLens.
        /// </returns>
        public static DistortionLens Distortion(string normalTexturePath = null)
        {
            DistortionLens distortion = new DistortionLens(normalTexturePath);

            return distortion;
        }

        /// <summary>
        /// Gets a new <see cref="FastBlurLens"/>.
        /// </summary>
        /// <returns>A instance of FastBlurLens.</returns>
        public static FastBlurLens FastBlur()
        {
            return new FastBlurLens();
        }

        /// <summary>
        /// Gets a new <see cref="FilmGrainLens"/>.
        /// </summary>
        /// <returns>A instance of FilmGrainLens.</returns>
        public static FilmGrainLens FilmGrain()
        {
            return new FilmGrainLens();
        }

        /// <summary>
        /// Gets a new <see cref="FishEyeLens"/>.
        /// </summary>
        /// <returns>A instance of FishEyeLens.</returns>
        public static FishEyeLens FishEye()
        {
            return new FishEyeLens();
        }

        /// <summary>
        /// Gets a new <see cref="GlowLens"/>.
        /// </summary>
        /// <returns>A instance of GlowLens.</returns>
        public static GlowLens Glow()
        {
            return new GlowLens();
        }

        /// <summary>
        /// Gets a new <see cref="GrayScaleLens"/>.
        /// </summary>
        /// <returns>A instance of GrayScaleLens.</returns>
        public static GrayScaleLens GrayScale()
        {
            return new GrayScaleLens();
        }

        /// <summary>
        /// Gets a new <see cref="InvertLens"/>.
        /// </summary>
        /// <returns>A instance of InvertLens.</returns>
        public static InvertLens Invert()
        {
            return new InvertLens();
        }

        /// <summary>
        /// Gets a new <see cref="PixelateLens"/>.
        /// </summary>
        /// <returns>A instance of PixelateLens.</returns>
        public static PixelateLens Pixelate()
        {
            return new PixelateLens();
        }

        /// <summary>
        /// Gets a new <see cref="PosterizeLens"/>.
        /// </summary>
        /// <returns>A instance of PosterizeLens.</returns>
        public static PosterizeLens Posterize()
        {
            return new PosterizeLens();
        }

        /// <summary>
        /// Gets a new <see cref="RadialBlurLens"/>.
        /// </summary>
        /// <returns>A instance of RadialBblurLens.</returns>
        public static RadialBlurLens RadialBlur()
        {
            return new RadialBlurLens();
        }

        /// <summary>
        /// Gets a new <see cref="ScanlinesLens"/>.
        /// </summary>
        /// <returns>A instance of ScanlinesLens.</returns>
        public static ScanlinesLens ScanLines()
        {
            return new ScanlinesLens();
        }

        /// <summary>
        /// Gets a new <see cref="ScreenOverlayLens"/>.
        /// </summary>
        /// <param name="normalTexturePath">Texture use to apply the ScreenOverlay effect.</param>
        /// <returns>A instance of ScreenOverlayLens.</returns>
        public static ScreenOverlayLens ScreenOverlay(string overlayTexturePath = null)
        {
            return new ScreenOverlayLens(overlayTexturePath);
        }

        /// <summary>
        /// Gets a new <see cref="SepiaLens"/>.
        /// </summary>
        /// <returns>A instance of SepiaLens.</returns>
        public static SepiaLens Sepia()
        {
            return new SepiaLens();
        }

        /// <summary>
        /// Gets a new <see cref="SobelLens"/>.
        /// </summary>
        /// <returns>A instance of SobelLens.</returns>
        public static SobelLens Sobel()
        {
            return new SobelLens();
        }

        /// <summary>
        /// Gets a new <see cref="TilingLens"/>.
        /// </summary>
        /// <returns>A instance of TilingLens.</returns>
        public static TilingLens Tiling()
        {
            return new TilingLens();
        }

        /// <summary>
        /// Gets a new <see cref="TiltShiftLens"/>.
        /// </summary>
        /// <returns>A instance of TiltShiftLens.</returns>
        public static TiltShiftLens TiltShift()
        {
            return new TiltShiftLens();
        }

        /// <summary>
        /// Gets a new <see cref="ToneMappingLens"/>.
        /// </summary>
        /// <returns>A instance of ToneMappingLens.</returns>
        public static ToneMappingLens ToneMapping()
        {
            return new ToneMappingLens();
        }

        /// <summary>
        /// Gets a new <see cref="VignetteLens"/>.
        /// </summary>
        /// <returns>A instance of VignetteLens.</returns>
        public static VignetteLens Vignette()
        {
            return new VignetteLens();
        }
    }
}
