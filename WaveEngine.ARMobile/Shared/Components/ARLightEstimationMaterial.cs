// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Materials;

namespace WaveEngine.ARMobile.Components
{
    /// <summary>
    /// Class that changes the material ambient parameters based on the AR mobile light estimation
    /// </summary>
    [DataContract]
    public class ARLightEstimationMaterial : Behavior
    {
        /// <summary>
        /// The material component
        /// </summary>
        [RequiredComponent]
        protected MaterialComponent materialComponent;

        /// <summary>
        /// The AR service
        /// </summary>
        private ARMobileService arService;

        /// <summary>
        /// The material
        /// </summary>
        private Material material;

        /// <summary>
        /// The initial intensity of the environment channel
        /// </summary>
        private float initEnv;

        /// <summary>
        /// The initial intensity of the IBL channel
        /// </summary>
        private float initIBL;

        /// <summary>
        /// The initial color of the diffuse channel
        /// </summary>
        private Color initDiffuse;

        /// <summary>
        /// The initial color of the ambient channel
        /// </summary>
        private Color initAmbient;

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the material must be affected in the diffuse color
        /// </summary>
        [DataMember]
        [RenderProperty(Tooltip = "Indicates if the material must be affected in the diffuse color")]
        public bool UseDiffuse { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the material must be affected in the ambient color
        /// </summary>
        [DataMember]
        [RenderProperty(Tooltip = "Indicates if the material must be affected in the ambient color")]
        public bool UseAmbient { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the material must be affected in the environment
        /// </summary>
        [DataMember]
        [RenderProperty(Tooltip = "Indicates if the material must be affected in the environment")]
        public bool UseEnvironment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the material must be affected in the IBL intensity
        /// </summary>
        [DataMember]
        [RenderProperty(Tooltip = "Indicates if the material must be affected in the IBL intensity")]
        public bool UseIBL { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the estimated light temperature will affect the material
        /// </summary>
        [DataMember]
        [RenderProperty(Tooltip = "Indicates if the estimated light temperature will affect the material")]
        public bool ApplyTemperature { get; set; }

        #endregion

        /// <inheritdoc />
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.UseDiffuse = true;
        }

        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();

            this.material = this.materialComponent.Material;

            if (!ARMobileService.GetService(out this.arService))
            {
                return;
            }

            if (this.material != null)
            {
                if (this.material is StandardMaterial)
                {
                    var standard = this.material as StandardMaterial;

                    this.initDiffuse = standard.DiffuseColor;
                    this.initAmbient = standard.AmbientColor;
                    this.initEnv = standard.EnvironmentAmount;
                    this.initIBL = standard.IBLFactor;
                }
                else if (this.material is ForwardMaterial)
                {
                    var forward = this.material as ForwardMaterial;

                    this.initDiffuse = forward.DiffuseColor;
                    this.initAmbient = forward.AmbientColor;
                }
                else if (this.material is EnvironmentMaterial)
                {
                    var env = this.material as EnvironmentMaterial;

                    this.initDiffuse = env.DiffuseColor;
                    this.initAmbient = env.AmbientColor;
                    this.initEnv = env.EnvironmentAmount;
                }
            }
        }

        /// <inheritdoc />
        protected override void Update(TimeSpan gameTime)
        {
            if (this.material == null ||
                this.arService?.LightEstimation == null)
            {
                return;
            }

            var lightEstimation = this.arService.LightEstimation;
            var lightIntensity = lightEstimation.AmbientIntensityFactor;
            var colorTemprerature = this.ApplyTemperature ? Color.FromTemperature(lightEstimation.Temperature) : Color.White;

            if (this.material is StandardMaterial)
            {
                var standard = this.material as StandardMaterial;

                lightIntensity = standard.LightingEnabled ? lightIntensity : 1;

                if (this.UseDiffuse)
                {
                    standard.DiffuseColor = this.initDiffuse * colorTemprerature * lightIntensity;
                }

                if (this.UseAmbient)
                {
                    standard.AmbientColor = this.initAmbient * colorTemprerature * lightIntensity;
                }

                if (this.UseEnvironment)
                {
                    standard.EnvironmentAmount = this.initEnv * lightIntensity;
                }

                if (this.UseIBL)
                {
                    standard.IBLFactor = this.initIBL * lightIntensity;
                }
            }
            else if (this.material is ForwardMaterial)
            {
                var forward = this.material as ForwardMaterial;

                lightIntensity = forward.LightingEnabled ? lightIntensity : 1;

                if (this.UseDiffuse)
                {
                    forward.DiffuseColor = this.initDiffuse * colorTemprerature * lightIntensity;
                }

                if (this.UseAmbient)
                {
                    forward.AmbientColor = this.initAmbient * colorTemprerature * lightIntensity;
                }
            }
            else if (this.material is EnvironmentMaterial)
            {
                var env = this.material as EnvironmentMaterial;

                lightIntensity = env.LightingEnabled ? lightIntensity : 1;

                if (this.UseDiffuse)
                {
                    env.DiffuseColor = this.initDiffuse * colorTemprerature * lightIntensity;
                }

                if (this.UseAmbient)
                {
                    env.AmbientColor = this.initAmbient * colorTemprerature * lightIntensity;
                }

                if (this.UseEnvironment)
                {
                    env.EnvironmentAmount = this.initEnv * lightIntensity;
                }
            }
        }
    }
}
