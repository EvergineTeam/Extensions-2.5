// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.AR;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.Vuforia
{
    /// <summary>
    /// The Vuforia AR provider class
    /// </summary>
    [DataContract]
    public class VuforiaProvider : ARProvider
    {
        /// <summary>
        /// The Vuforia service
        /// </summary>
        [RequiredService]
        protected VuforiaService vuforiaService;

        [DataMember]
        private bool renderVideoCameraBackground;

        [DataMember]
        private WorldCenterMode worldCenterMode;

        [DataMember]
        private string worldCenterEntityPath;

        /// <summary>
        /// Gets or sets a value indicating whether the tracking will start automatically
        /// </summary>
        [DataMember]
        [RenderProperty(
            CustomPropertyName = "Auto Start",
            Tooltip = "Indicates whether the tracking will start automatically")]
        public bool AutoStart { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the camera video should be rendered in the background.
        /// </summary>
        /// <value>
        /// <c>true</c> if the camera video should be rendered in the background; otherwise, <c>false</c>.
        /// </value>
        [RenderProperty(
            CustomPropertyName = "Render Video Camera",
            Tooltip = "Indicates whether the camera video should be rendered in the background")]
        public bool RenderVideoCameraBackground
        {
            get
            {
                return this.renderVideoCameraBackground;
            }

            set
            {
                this.renderVideoCameraBackground = value;
            }
        }

        /// <summary>
        /// Gets or sets defines how the relative transformation that is returned by the service is applied.
        /// Either the camera is moved in the scene with respect to a "world center" or all the targets are moved with
        /// respect to the camera.
        /// </summary>
        [RenderProperty(
            Tag = 1,
            CustomPropertyName = "World Center Mode",
            Tooltip = "Either the camera is moved in the scene with respect to a 'world center' entity or all the targets are moved with respect to the camera")]
        public WorldCenterMode WorldCenterMode
        {
            get
            {
                return this.worldCenterMode;
            }

            set
            {
                this.worldCenterMode = value;
                this.UpdateVuforiaWorldCenterProperties();
            }
        }

        /// <summary>
        /// Gets or sets the path of entity that will be used as world center when
        /// <see cref="VuforiaProvider.WorldCenterMode"/> is set to <see cref="WorldCenterMode.SpecificTarget"/>.
        /// </summary>
        [RenderPropertyAsEntity(
            new string[] { "WaveEngine.Vuforia.ARTrackableBehavior" },
            AttatchToTag = 1,
            AttachToValue = WorldCenterMode.SpecificTarget,
            CustomPropertyName = "World Center Entity",
            Tooltip = "Indicates the entity that will be used as world center")]
        public string WorldCenterEntityPath
        {
            get
            {
                return this.worldCenterEntityPath;
            }

            set
            {
                this.worldCenterEntityPath = value;
                this.UpdateVuforiaWorldCenterProperties();
            }
        }

        /// <inheritdoc />
        public override Vector3[] PointCloud
        {
            get
            {
                return null;
            }
        }

        /// <inheritdoc />
        public override Camera3D ActiveCamera
        {
            get
            {
                return this.vuforiaService?.ActiveCamera;
            }

            set
            {
                if (this.vuforiaService != null)
                {
                    this.vuforiaService.ActiveCamera = value;
                }
            }
        }

        /// <inheritdoc />
        public override Matrix? CameraTransform
        {
            get
            {
                return this.vuforiaService?.CameraTransform;
            }
        }

        /// <inheritdoc />
        public override Matrix CameraProjection
        {
            get
            {
                return this.vuforiaService?.CameraProjection ?? Matrix.Identity;
            }
        }

        /// <inheritdoc />
        public override Mesh BackgroundCameraMesh
        {
            get
            {
                if (this.renderVideoCameraBackground)
                {
                    return this.vuforiaService?.BackgroundCameraMesh;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <inheritdoc />
        public override Material BackgroundCameraMaterial
        {
            get
            {
                if (this.renderVideoCameraBackground)
                {
                    return this.vuforiaService?.BackgroundCameraMaterial;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <inheritdoc />
        public override bool IsSupported
        {
            get
            {
                return this.vuforiaService?.IsSupported == true;
            }
        }

        /// <inheritdoc />
        public override ARTrackingState TrackingState
        {
            get
            {
                return this.vuforiaService?.State != ARState.Stopped ? ARTrackingState.Normal : ARTrackingState.NotAvailable;
            }
        }

        /// <inheritdoc />
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.AutoStart = true;
        }

        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();

            this.UpdateVuforiaWorldCenterProperties();

            if (!WaveServices.Platform.IsEditor &&
                this.AutoStart &&
                this.vuforiaService != null &&
                this.vuforiaService.IsSupported)
            {
                this.StartTracking();
            }
        }

        /// <summary>
        /// Starts the Vuforia target tracking.
        /// </summary>
        /// <returns>
        ///   <c>true</c>, if tracking was started, <c>false</c> otherwise.
        /// </returns>
        public Task<bool> StartTracking()
        {
            return this.vuforiaService?.StartTracking(this.renderVideoCameraBackground);
        }

        /// <summary>
        /// Pauses the AR tracking process
        /// </summary>
        public void StopTracking()
        {
            this.vuforiaService?.StopTracking();
        }

        private void UpdateVuforiaWorldCenterProperties()
        {
            if (this.vuforiaService != null &&
                this.vuforiaService.IsSupported)
            {
                ARTrackableBehavior worldCenterTrackable = null;
                if (this.worldCenterMode == WorldCenterMode.SpecificTarget &&
                    !string.IsNullOrEmpty(this.worldCenterEntityPath))
                {
                    var worldCenterEntity = this.EntityManager.Find(this.worldCenterEntityPath);
                    worldCenterTrackable = worldCenterEntity?.FindComponent<ARTrackableBehavior>(false);
                }

                this.vuforiaService.WorldCenterMode = this.worldCenterMode;
                this.vuforiaService.WorldCenterTrackable = worldCenterTrackable;
            }
        }
    }
}
