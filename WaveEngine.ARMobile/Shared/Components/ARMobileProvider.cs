// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.AR;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;

namespace WaveEngine.ARMobile.Components
{
    /// <summary>
    /// The AR Mobile provider class
    /// </summary>
    [DataContract]
    public class ARMobileProvider : ARProvider
    {
        private ARMobileService service;

        [DataMember]
        private PlaneDetectionType planeDetection;

        [DataMember]
        private bool pointCloudEnabled;

        [DataMember]
        private bool lightEstimationEnabled;

        [DataMember]
        private bool trackPosition;

        [DataMember]
        private ARMobileWorldAlignment worldAlignment;

        /// <summary>
        /// Gets or sets a value indicating whether the tracking will start automatically
        /// </summary>
        [DataMember]
        [RenderProperty(
            CustomPropertyName = "Auto Start",
            Tooltip = "Indicates whether the tracking will start automatically")]
        public bool AutoStart { get; set; }

        /// <summary>
        /// Gets or sets the world alignment mode that indicates how a scene coordinate system is constructed based on real-world device motion.
        /// On ARCore only <see cref="ARMobileWorldAlignment.Gravity"/> mode is supported
        /// </summary>
        [RenderProperty(
            CustomPropertyName = "World Alignment",
            Tooltip = "Indicates how a scene coordinate system is constructed based on real-world device motion. On ARCore only Gravity mode is supported")]
        public ARMobileWorldAlignment WorldAlignment
        {
            get
            {
                return this.worldAlignment;
            }

            set
            {
                this.worldAlignment = value;

                if (this.service != null)
                {
                    this.service.WorldAlignment = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the position tracking is enabled. On ARCore position tracking cannot be disabled
        /// </summary>
        [RenderProperty(
            CustomPropertyName = "Track Position",
            Tooltip = "Indicates whether the position tracking is enabled. On ARCore position tracking cannot be disabled")]
        public bool TrackPosition
        {
            get
            {
                return this.trackPosition;
            }

            set
            {
                this.trackPosition = value;

                if (this.service != null)
                {
                    this.service.TrackPosition = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets how flat surfaces are detected in captured images
        /// </summary>
        [RenderProperty(
            CustomPropertyName = "Plane Detection",
            Tooltip = "Indicates how flat surfaces are detected in captured images")]
        public PlaneDetectionType PlaneDetection
        {
            get
            {
                return this.planeDetection;
            }

            set
            {
                this.planeDetection = value;

                if (this.service != null)
                {
                    this.service.PlaneDetection = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the point cloud is available
        /// </summary>
        [RenderProperty(
            CustomPropertyName = "Point Cloud Enabled",
            Tooltip = "Indicates whether the point cloud is available")]
        public bool PointCloudEnabled
        {
            get
            {
                return this.pointCloudEnabled;
            }

            set
            {
                this.pointCloudEnabled = value;

                if (this.service != null)
                {
                    this.service.PointCloudEnabled = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the light estimation is available
        /// </summary>
        [RenderProperty(
            CustomPropertyName = "Light Estimation Enabled",
            Tooltip = "Indicates whether the light estimation is available")]
        public bool LightEstimationEnabled
        {
            get
            {
                return this.lightEstimationEnabled;
            }

            set
            {
                this.lightEstimationEnabled = value;

                if (this.service != null)
                {
                    this.service.LightEstimationEnabled = value;
                }
            }
        }

        /// <summary>
        /// Gets a list with the detected anchors
        /// </summary>
        public IEnumerable<ARMobileAnchor> Anchors
        {
            get
            {
                return this.service?.Anchors;
            }
        }

        /// <inheritdoc />
        public override Vector3[] PointCloud
        {
            get
            {
                return this.service?.PointCloud;
            }
        }

        /// <inheritdoc />
        public override Camera3D ActiveCamera
        {
            get
            {
                return this.service?.ActiveCamera;
            }

            set
            {
                if (this.service != null)
                {
                    this.service.ActiveCamera = value;
                }
            }
        }

        /// <inheritdoc />
        public override Matrix? CameraTransform
        {
            get
            {
                return this.service?.CameraTransform;
            }
        }

        /// <inheritdoc />
        public override Matrix CameraProjection
        {
            get
            {
                return this.service?.CameraProjection ?? Matrix.Identity;
            }
        }

        /// <inheritdoc />
        public override Mesh BackgroundCameraMesh
        {
            get
            {
                return this.service?.BackgroundCameraMesh;
            }
        }

        /// <inheritdoc />
        public override Material BackgroundCameraMaterial
        {
            get
            {
                return this.service?.BackgroundCameraMaterial;
            }
        }

        /// <inheritdoc />
        public override bool IsSupported
        {
            get
            {
                return this.service?.IsSupported ?? false;
            }
        }

        /// <inheritdoc />
        public override ARTrackingState TrackingState
        {
            get
            {
                return this.service?.TrackingState ?? ARTrackingState.NotAvailable;
            }
        }

        /// <inheritdoc />
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.trackPosition = true;
            this.planeDetection = PlaneDetectionType.Horizontal;
            this.pointCloudEnabled = false;
            this.worldAlignment = ARMobileWorldAlignment.Gravity;
            this.lightEstimationEnabled = true;
            this.AutoStart = true;
        }

        /// <inheritdoc />
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            if (ARMobileService.GetService(out this.service))
            {
                this.service.WorldAlignment = this.worldAlignment;
                this.service.TrackPosition = this.trackPosition;
                this.service.PlaneDetection = this.planeDetection;
                this.service.PointCloudEnabled = this.pointCloudEnabled;
                this.service.LightEstimationEnabled = this.lightEstimationEnabled;
            }
        }

        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();

            if (!WaveServices.Platform.IsEditor &&
                this.AutoStart)
            {
                this.StartTracking(ARMobileStartOptions.None);
            }
        }

        /// <summary>
        /// Starts the tracking process
        /// </summary>
        /// <param name="startOptions">
        /// Options affecting how existing session state (if any) transitions to the new configuration.
        /// If the session is starting for the first time, this parameter has no effect.
        /// </param>
        /// <returns>
        ///   <c>true</c>, if tracking was started, <c>false</c> otherwise.
        /// </returns>
        public Task<bool> StartTracking(ARMobileStartOptions startOptions)
        {
            return this.service?.StartTracking(startOptions);
        }

        /// <summary>
        /// Pauses the AR tracking process
        /// </summary>
        public void PauseTracking()
        {
            this.service?.PauseTracking();
        }

        /// <summary>
        /// Performs a ray cast from the user's device in the direction of the given location in the camera view.
        /// Intersections with detected scene geometry are returned, sorted by distance from the device; the nearest intersection is returned first.
        /// </summary>
        /// <param name="screenPosition">The position coordinates of the screen.</param>
        /// <param name="hitType">The types of hit-test result to search for.</param>
        /// <param name="results">The list of results, sorted from nearest to farthest (in distance from the camera).</param>
        /// <returns><c>true</c> if any hit-test has been found; otherwise, <c>false</c>.</returns>
        public bool HitTest(Vector2 screenPosition, ARMobileHitType hitType, out ARMobileHitTestResult[] results)
        {
            if (this.service == null)
            {
                results = null;
                return false;
            }

            return this.service.HitTest(screenPosition, hitType, out results);
        }

        /// <summary>
        /// Looks for an <see cref="ARMobileAnchor"/> detected in the current frame.
        /// </summary>
        /// <param name="anchorId">The <see cref="ARMobileAnchor.Id"/> of the anchor</param>
        /// <returns>
        /// The <see cref="ARMobileAnchor" /> or null if no <see cref="ARMobileAnchor" /> with the specified id was found.
        /// </returns>
        public ARMobileAnchor FindAnchor(Guid anchorId)
        {
            return this.service?.FindAnchor(anchorId);
        }
    }
}
