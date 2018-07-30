// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

namespace WaveEngine.ARMobile
{
    /// <summary>
    /// Estimated scene lighting information associated with a captured video frame in an AR session.
    /// </summary>
    public class ARMobileLightEstimation
    {
        /// <summary>
        /// A constant value of light intensity, in lumens, representing a "neutral" lighting.
        /// </summary>
        public const float NeutralAmbientIntensity = 1000;

        /// <summary>
        /// A constant value of light temperature, in degrees Kelvin, representing a "neutral" (pure white) lighting.
        /// </summary>
        public const float NeutralTemperature = 6500;

        /// <summary>
        /// Gets the estimated intensity factor. Values are in the range (0.0, 1.0), with zero being black and one being white.
        /// </summary>
        public float AmbientIntensityFactor { get; private set; }

        /// <summary>
        /// Gets the estimated intensity, in lumens, of ambient light throughout the scene.
        /// A value of 1000 represents "neutral" lighting.
        /// </summary>
        public float AmbientIntensityLumens { get; private set; }

        /// <summary>
        /// Gets the estimated color temperature, in degrees Kelvin, of ambient light throughout the scene.
        /// A value of 6500 represents neutral (pure white) lighting; lower values indicate a "warmer" yellow or orange tint,
        /// and higher values indicate a "cooler" blue tint.
        /// </summary>
        public float Temperature { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ARMobileLightEstimation"/> class.
        /// </summary>
        internal ARMobileLightEstimation()
        {
            this.AmbientIntensityFactor = 1f;
            this.AmbientIntensityLumens = NeutralAmbientIntensity;
            this.Temperature = NeutralTemperature;
        }

        /// <summary>
        /// Sets the intensity factor
        /// </summary>
        /// <param name="intensityFactor">The intensity factor</param>
        internal void UpdateFactorIntensity(float intensityFactor)
        {
            this.AmbientIntensityFactor = intensityFactor;
            this.AmbientIntensityLumens = intensityFactor * NeutralAmbientIntensity;
        }

        /// <summary>
        /// Sets the intensity
        /// </summary>
        /// <param name="intensityLumens">The intensity in lumens</param>
        internal void UpdateLumensIntensity(float intensityLumens)
        {
            this.AmbientIntensityFactor = intensityLumens / NeutralAmbientIntensity;
            this.AmbientIntensityLumens = intensityLumens;
        }

        /// <summary>
        /// Sets the temperature
        /// </summary>
        /// <param name="temperature">The temperature in Kelvin degrees</param>
        internal void UpdateTemperature(float temperature)
        {
            this.Temperature = temperature;
        }
    }
}
