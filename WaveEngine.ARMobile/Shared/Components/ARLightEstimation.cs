// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace WaveEngine.ARMobile.Components
{
    /// <summary>
    /// Class that changes the light intensity based on the AR mobile light estimation
    /// </summary>
    [DataContract]
    public class ARLightEstimation : Behavior
    {
        /// <summary>
        /// The light
        /// </summary>
        [RequiredComponent(false)]
        protected LightProperties light;

        /// <summary>
        /// The AR service
        /// </summary>
        private ARMobileService arService;

        /// <summary>
        /// The initial intensity of the light
        /// </summary>
        private float initIntensity;

        /// <summary>
        /// The initial color of the light
        /// </summary>
        private Color initColor;

        /// <summary>
        /// Gets or sets a value indicating whether the estimated light temperature will affect the light
        /// </summary>
        [DataMember]
        [RenderProperty(Tooltip = "Indicates if the estimated light temperature will affect the light")]
        public bool ApplyTemperature { get; set; }

        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();

            this.initIntensity = this.light.Intensity;
            this.initColor = this.light.Color;

            ARMobileService.GetService(out this.arService);
        }

        /// <inheritdoc />
        protected override void Update(TimeSpan gameTime)
        {
            var lightEstimation = this.arService?.LightEstimation;

            if (lightEstimation == null)
            {
                return;
            }

            this.light.Intensity = this.initIntensity * lightEstimation.AmbientIntensityFactor;

            if (this.ApplyTemperature)
            {
                this.light.Color = this.initColor * Color.FromTemperature(lightEstimation.Temperature);
            }
        }
    }
}
