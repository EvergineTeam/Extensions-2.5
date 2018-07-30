// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers;

#endregion

namespace WaveEngine.Cardboard
{
    /// <summary>
    /// The cardboard viewer
    /// </summary>
    public class CardboardViewer
    {
        /// <summary>
        /// Gets the cardboard Id
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets the label
        /// </summary>
        public string Label { get; private set; }

        /// <summary>
        /// Gets the Field Of View
        /// </summary>
        public float FoV { get; private set; }

        /// <summary>
        /// Gets the Inter Lens distance
        /// </summary>
        public float InterLensDistance { get; private set; }

        /// <summary>
        /// Gets the baseline lens distance
        /// </summary>
        public float BaselineLensDistance { get; private set; }

        /// <summary>
        /// Gets the screen lens distance
        /// </summary>
        public float ScreenLensDistance { get; private set; }

        /// <summary>
        /// Gets the distortion coefficients
        /// </summary>
        public float[] DistortionCoefficients { get; private set; }

        /// <summary>
        /// Gets the Inverse coefficients
        /// </summary>
        public float[] InverseCoefficients { get; private set; }

        /// <summary>
        /// Gets the carboard V1
        /// </summary>
        public static CardboardViewer CardboardV1 { get; private set; }

        /// <summary>
        /// Gets the carboard V2
        /// </summary>
        public static CardboardViewer CardboardV2 { get; private set; }

        static CardboardViewer()
        {
            CardboardV1 = new CardboardViewer
            {
                Id = "CardboardV1",
                Label = "Cardboard I/O 2014",
                FoV = 40,
                InterLensDistance = 0.060f,
                BaselineLensDistance = 0.035f,
                ScreenLensDistance = 0.042f,
                DistortionCoefficients = new float[] { 0.441f, 0.156f },
                InverseCoefficients = new float[] { -0.4410035f, 0.42756155f, -0.4804439f, 0.5460139f, -0.58821183f, 0.5733938f, -0.48303202f, 0.33299083f, -0.17573841f, 0.0651772f, -0.01488963f, 0.001559834f }
            };

            CardboardV2 = new CardboardViewer
            {
                Id = "CardboardV2",
                Label = "Cardboard I/O 2015",
                FoV = 60,
                InterLensDistance = 0.064f,
                BaselineLensDistance = 0.035f,
                ScreenLensDistance = 0.039f,
                DistortionCoefficients = new float[] { 0.34f, 0.55f },
                InverseCoefficients = new float[] { -0.33836704f, -0.18162185f, 0.862655f, -1.2462051f, 1.0560602f, -0.58208317f, 0.21609078f, -0.05444823f, 0.009177956f, -9.904169E-4f, 6.183535E-5f, -1.6981803E-6f }
            };
        }
    }
}
