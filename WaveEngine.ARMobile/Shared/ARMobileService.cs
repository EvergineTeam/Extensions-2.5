// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.AR;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;

namespace WaveEngine.ARMobile
{
    /// <summary>
    /// The AR mobile base service class
    /// </summary>
    public abstract class ARMobileService : UpdatableService, IDisposable
    {
        private PlaneDetectionType planeDetection;
        private bool pointCloudEnabled;
        private bool lightEstimationEnabled;
        private bool trackPosition;
        private ARMobileWorldAlignment worldAlignment;

        /// <summary>
        /// Backing field for <see cref="PointCloud"/> property
        /// </summary>
        protected Vector3[] internalPointCloud;

        /// <summary>
        /// Backing field for <see cref="Anchors"/> property
        /// </summary>
        protected ConcurrentDictionary<Guid, ARMobileAnchor> anchorsById;

        /// <summary>
        /// Backing field for <see cref="BackgroundCameraMesh"/> property
        /// </summary>
        protected Mesh backgroundCameraMesh;

        /// <summary>
        /// Backing field for <see cref="BackgroundCameraMaterial"/> property
        /// </summary>
        protected Material backgroundCameraMaterial;

        /// <summary>
        /// Backing field for <see cref="CameraTransform"/> property
        /// </summary>
        protected Matrix cameraTransform;

        /// <summary>
        /// Backing field for <see cref="CameraProjection"/> property
        /// </summary>
        protected Matrix cameraProjection;

        /// <summary>
        /// Backing field for <see cref="LightEstimation"/> property
        /// </summary>
        protected ARMobileLightEstimation lightEstimation;

        /// <summary>
        /// Backing field for <see cref="IsSupported"/> property
        /// </summary>
        protected bool isSupported;

        /// <summary>
        /// Gets the AR Mobile anchors
        /// </summary>
        public IEnumerable<ARMobileAnchor> Anchors
        {
            get { return this.anchorsById.Values; }
        }

        /// <summary>
        /// Gets the mesh for the background camera
        /// </summary>
        public Mesh BackgroundCameraMesh
        {
            get { return this.backgroundCameraMesh; }
        }

        /// <summary>
        /// Gets the material for the background camera
        /// </summary>
        public Material BackgroundCameraMaterial
        {
            get { return this.backgroundCameraMaterial; }
        }

        /// <summary>
        /// Gets the camera transform matrix
        /// </summary>
        public Matrix CameraTransform
        {
            get { return this.cameraTransform; }
        }

        /// <summary>
        /// Gets the camera projection matrix
        /// </summary>
        public Matrix CameraProjection
        {
            get { return this.cameraProjection; }
        }

        /// <summary>
        /// Gets the estimated scene lighting information based on a captured video frame in an AR session
        /// </summary>
        public ARMobileLightEstimation LightEstimation
        {
            get { return this.lightEstimation; }
        }

        /// <summary>
        /// Gets a value indicating whether the AR is supported
        /// </summary>
        public bool IsSupported
        {
            get { return this.isSupported; }
        }

        /// <summary>
        /// Gets or sets the world alignment mode that indicates how a scene coordinate system is constructed based on real-world device motion.
        /// On ARCore only <see cref="ARMobileWorldAlignment.Gravity"/> mode is supported
        /// </summary>
        public ARMobileWorldAlignment WorldAlignment
        {
            get
            {
                return this.worldAlignment;
            }

            set
            {
                this.worldAlignment = value;
                this.RefreshConfiguration();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the position tracking is enabled. On ARCore position tracking cannot be disabled
        /// </summary>
        public bool TrackPosition
        {
            get
            {
                return this.trackPosition;
            }

            set
            {
                this.trackPosition = value;
                this.RefreshConfiguration();
            }
        }

        /// <summary>
        /// Gets or sets how flat surfaces are detected in captured images
        /// </summary>
        public PlaneDetectionType PlaneDetection
        {
            get
            {
                return this.planeDetection;
            }

            set
            {
                this.planeDetection = value;
                this.RefreshConfiguration();
            }
        }

        /// <summary>
        /// Gets an array with the current intermediate results of the scene analysis that is used to perform world tracking
        /// </summary>
        public Vector3[] PointCloud
        {
            get
            {
                return this.internalPointCloud;
            }
        }

        /// <summary>
        /// Gets or sets the active camera
        /// </summary>
        public Camera3D ActiveCamera
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the point cloud is available
        /// </summary>
        public bool PointCloudEnabled
        {
            get
            {
                return this.pointCloudEnabled;
            }

            set
            {
                this.pointCloudEnabled = value;
                this.RefreshConfiguration();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the light estimation is available
        /// </summary>
        public bool LightEstimationEnabled
        {
            get
            {
                return this.lightEstimationEnabled;
            }

            set
            {
                this.lightEstimationEnabled = value;
                this.RefreshConfiguration();
            }
        }

        /// <summary>
        /// Gets the service tracking state
        /// </summary>
        public ARTrackingState TrackingState
        {
            get;
            internal set;
        }

        /// <summary>
        /// Event launched when one or more anchors have been added
        /// </summary>
        public event EventHandler<IEnumerable<ARMobileAnchor>> AddedAnchor;

        /// <summary>
        /// Event launched when one or more anchors have been updated
        /// </summary>
        public event EventHandler<IEnumerable<ARMobileAnchor>> UpdatedAnchor;

        /// <summary>
        /// Event launched when one or more anchors have been removed
        /// </summary>
        public event EventHandler<IEnumerable<ARMobileAnchor>> RemovedAnchor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ARMobileService"/> class.
        /// </summary>
        protected ARMobileService()
        {
            this.trackPosition = true;
            this.planeDetection = PlaneDetectionType.None;
            this.lightEstimationEnabled = false;
            this.pointCloudEnabled = false;
            this.TrackingState = ARTrackingState.NotAvailable;
            this.anchorsById = new ConcurrentDictionary<Guid, ARMobileAnchor>();
        }

        /// <summary>
        /// Adds anchors to the internal collection and raise the <see cref="AddedAnchor"/> event
        /// </summary>
        /// <param name="anchors">The added anchors</param>
        internal void AddAnchors(IEnumerable<ARMobileAnchor> anchors)
        {
            if (anchors.Count() == 0)
            {
                return;
            }

            foreach (var anchor in anchors)
            {
                this.anchorsById[anchor.Id] = anchor;
            }

            this.AddedAnchor?.Invoke(this, anchors);
        }

        /// <summary>
        /// Raise the <see cref="UpdatedAnchor"/> event
        /// </summary>
        /// <param name="anchors">The updated anchors</param>
        internal void UpdatedAnchors(IEnumerable<ARMobileAnchor> anchors)
        {
            if (anchors.Count() > 0)
            {
                this.UpdatedAnchor?.Invoke(this, anchors);
            }
        }

        /// <summary>
        /// Removes anchors from the internal collection and raise the <see cref="RemovedAnchor"/> event
        /// </summary>
        /// <param name="anchorIds">The identifiers of the removed anchors</param>
        internal void RemoveAnchors(IEnumerable<Guid> anchorIds)
        {
            var removedAnchors = new List<ARMobileAnchor>();
            foreach (var id in anchorIds)
            {
                ARMobileAnchor anchor;
                if (this.anchorsById.TryRemove(id, out anchor))
                {
                    removedAnchors.Add(anchor);
                }
            }

            if (removedAnchors.Count > 0)
            {
                this.RemovedAnchor?.Invoke(this, removedAnchors);
            }
        }

        /// <summary>
        /// Removes all existing anchors from the internal collection and raise the <see cref="RemovedAnchor"/> event
        /// </summary>
        protected void ClearAllAnchors()
        {
            var removedAnchors = this.anchorsById.Values.ToArray();

            if (removedAnchors != null &&
                removedAnchors.Length > 0)
            {
                this.anchorsById.Clear();
                this.RemovedAnchor?.Invoke(this, removedAnchors);
            }
        }

        /// <inheritdoc />
        public override void Update(TimeSpan gameTime)
        {
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
            ARMobileAnchor anchor;
            this.anchorsById.TryGetValue(anchorId, out anchor);

            return anchor;
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
        public abstract Task<bool> StartTracking(ARMobileStartOptions startOptions);

        /// <summary>
        /// Pauses the tracking process
        /// </summary>
        public abstract void PauseTracking();

        /// <summary>
        /// Disposes the entity
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// Performs a ray cast from the user's device in the direction of the given location in the camera view.
        /// Intersections with detected scene geometry are returned, sorted by distance from the device; the nearest intersection is returned first.
        /// </summary>
        /// <param name="screenPosition">The position coordinates of the screen.</param>
        /// <param name="hitType">The types of hit-test result to search for.</param>
        /// <param name="results">The list of results, sorted from nearest to farthest (in distance from the camera).</param>
        /// <returns><c>true</c> if any hit-test has been found; otherwise, <c>false</c>.</returns>
        public abstract bool HitTest(Vector2 screenPosition, ARMobileHitType hitType, out ARMobileHitTestResult[] results);

        /// <summary>
        /// Refreshes the configuration
        /// </summary>
        internal abstract void RefreshConfiguration();

        /// <summary>
        /// Resets the session tracking
        /// </summary>
        internal abstract void Reset();

        /// <summary>
        /// Gets the platform specific AR service
        /// </summary>
        /// <param name="service">The found service if defined</param>
        /// <returns><c>true</c> if the service has been found; otherwise, <c>false</c>.</returns>
        internal static bool GetService(out ARMobileService service)
        {
            service = WaveServices.GetService<ARMobileService>(false);

            if (service == null)
            {
#if IOS
                service = new ARKitService();
                WaveServices.RegisterService(service);
#elif ANDROID
                service = new ARCoreService();
                WaveServices.RegisterService(service);
#endif
            }

            return service != null;
        }
    }
}
