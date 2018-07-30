// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Usings Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using WaveEngine.Common;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Materials;
#endregion

namespace WaveEngine.Vuforia
{
    /// <summary>
    /// Vuforia integration service
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Vuforia")]
    public class VuforiaService : UpdatableService
    {
        /// <summary>
        /// Vuforia license key
        /// </summary>
        [DataMember]
        private string licenseKey;

        /// <summary>
        /// The dataSet path
        /// </summary>
        [DataMember]
        private string dataSetPath;

        /// <summary>
        /// The max simultaneous tracked images
        /// </summary>
        [DataMember]
        private int maxSimultaneousImageTargets;

        /// <summary>
        /// The max simultaneous tracked objects
        /// </summary>
        [DataMember]
        private int maxSimultaneousObjectTargets;

        /// <summary>
        /// If uses extended tracking
        /// </summary>
        [DataMember]
        private bool extendedTracking;

        /// <summary>
        /// The parsed trackables
        /// </summary>
        private Dictionary<string, TargetTypes> parsedTrackables;

        /// <summary>
        /// Platform specific service code
        /// </summary>
        private ARServiceBase platformSpecificARService;

        /// <summary>
        /// A task indicating that there is a pending initialization request
        /// </summary>
        private Task<bool> initializationTask;

        /// <summary>
        /// Indicates that there is a pending start tracking request
        /// </summary>
        private bool pendingStartTrack;

        /// <summary>
        /// Backing field for <see cref="CameraTransform"/> property
        /// </summary>
        private Matrix? cameraTransform;

        /// <summary>
        /// Backing field for <see cref="CameraProjection"/> property
        /// </summary>
        private Matrix cameraProjection;

        /// <summary>
        /// Backing field for <see cref="BackgroundCameraMaterial"/> property
        /// </summary>
        private StandardMaterial backgroundCameraMaterial;

        #region Properties

        /// <summary>
        /// Gets or sets the Vuforia license key that is used for this app.
        /// </summary>
        /// <exception cref="InvalidOperationException">License key cannot be changed while Vuforia tracking is active</exception>
        [RenderProperty(
            CustomPropertyName = "License Key",
            Tooltip = "The Vuforia license key that is used for this app")]
        public string LicenseKey
        {
            get
            {
                return this.licenseKey;
            }

            set
            {
                if (this.licenseKey != value)
                {
                    this.licenseKey = value;

                    this.Reset();
                }
            }
        }

        /// <summary>
        /// Gets or sets the active dataSet path. Only one dataSet can be loaded and active at a time.
        /// </summary>
        [RenderPropertyAsAsset(
            AssetType.Unknown,
            ".xml",
            CustomPropertyName = "DataSet Path",
            Tooltip = "The active dataSet path. Only one dataSet can be loaded and active at a time")]
        public string DataSetPath
        {
            get
            {
                return this.dataSetPath;
            }

            set
            {
                this.dataSetPath = value;

                this.UpdateDataSet();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether extended tracking feature is enabled for all dataSet trackables.
        /// </summary>
        /// <value>
        ///   <c>true</c> if extended tracking feature is enabled for all dataSet trackables; otherwise, <c>false</c>.
        /// </value>
        [RenderProperty(
            CustomPropertyName = "Extended Tracking",
            Tooltip = "Indicates whether extended tracking feature is enabled for all dataSet trackables")]
        public bool ExtendedTracking
        {
            get
            {
                return this.extendedTracking;
            }

            set
            {
                if (this.extendedTracking != value)
                {
                    this.extendedTracking = value;
                    this.UpdateDataSet();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether Vuforia integration is supportede
        /// </summary>
        public bool IsSupported
        {
            get; private set;
        }

        /// <summary>
        /// Gets the dataset in use.
        /// </summary>
        public DataSet Dataset
        {
            get
            {
                this.CheckIfSupported();

                return this.platformSpecificARService.Dataset;
            }
        }

        /// <summary>
        /// Gets the Trackable objects currently being tracked.
        /// </summary>
        public IEnumerable<TrackableResult> TrackableResults
        {
            get
            {
                this.CheckIfSupported();

                return this.platformSpecificARService.TrackableResults;
            }
        }

        /// <summary>
        /// Gets the Vuforia service state.
        /// </summary>
        /// <value>The Vuforia service state.</value>
        public ARState State
        {
            get
            {
                if (this.IsSupported)
                {
                    return this.platformSpecificARService.State;
                }
                else
                {
                    return ARState.Stopped;
                }
            }
        }

        /// <summary>
        /// Gets the camera transform matrix
        /// </summary>
        public Matrix? CameraTransform
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
        /// Gets the material for the background camera
        /// </summary>
        public Material BackgroundCameraMaterial
        {
            get { return this.backgroundCameraMaterial; }
        }

        /// <summary>
        /// Gets the mesh for the background camera
        /// </summary>
        public Mesh BackgroundCameraMesh
        {
            get
            {
                return this.platformSpecificARService?.BackgroundCameraMesh;
            }
        }

        /// <summary>
        /// Gets the world center result if it is detected.
        /// </summary>
        public TrackableResult WorldCenterResult
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the active AR camera.
        /// </summary>
        public Camera3D ActiveCamera
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets how many image targets to detect and track at the same time
        /// </summary>
        /// <remarks>
        /// Tells the tracker how many image shall be processed
        /// at most at the same time.E.g. if an app will never require
        /// tracking more than two targets, this value should be set to 2.
        /// Default is: 1.
        /// </remarks>
        [RenderProperty(
            CustomPropertyName = "Max. Image Targets",
            Tooltip = "How many image targets to detect and track at the same time")]
        public int MaxSimultaneousImageTargets
        {
            get
            {
                return this.maxSimultaneousImageTargets;
            }

            set
            {
                this.maxSimultaneousImageTargets = value;

                if (this.platformSpecificARService != null)
                {
                    this.platformSpecificARService.MaxSimultaneousImageTargets = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets how many object targets to detect and track at the same time
        /// </summary>
        /// <remarks>
        /// Tells the tracker how many 3D objects shall be processed
        /// at most at the same time.E.g. if an app will never require
        /// tracking more than 1 target, this value should be set to 1.
        /// Default is: 1.
        /// </remarks>
        [RenderProperty(
            CustomPropertyName = "Max. Object Targets",
            Tooltip = "How many object targets to detect and track at the same time")]
        public int MaxSimultaneousObjectTargets
        {
            get
            {
                return this.maxSimultaneousObjectTargets;
            }

            set
            {
                this.maxSimultaneousObjectTargets = value;

                if (this.platformSpecificARService != null)
                {
                    this.platformSpecificARService.MaxSimultaneousObjectTargets = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets defines how the relative transformation that is returned by the service is applied.
        /// Either the camera is moved in the scene with respect to a "world center" or all the targets are moved with respect to the camera.
        /// </summary>
        [DontRenderProperty]
        public WorldCenterMode WorldCenterMode { get; set; }

        /// <summary>
        /// Gets or sets world center setting on the ARCamera
        /// </summary>
        [DontRenderProperty]
        public ARTrackableBehavior WorldCenterTrackable { get; set; }
        #endregion

        #region Initialize

        /// <summary>
        /// The default values
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.parsedTrackables = new Dictionary<string, TargetTypes>();

            this.maxSimultaneousImageTargets = 1;
            this.maxSimultaneousObjectTargets = 1;

            this.extendedTracking = true;

            this.backgroundCameraMaterial = new StandardMaterial()
            {
                LayerId = DefaultLayers.Skybox,
                LightingEnabled = false,
            };

#if IOS
            this.platformSpecificARService = new ARServiceIOS();
#elif ANDROID
            this.platformSpecificARService = new ARServiceAndroid();
#elif UWP
            this.platformSpecificARService = new ARServiceUWP();
#else
            this.platformSpecificARService = null;
#endif

            this.IsSupported = this.platformSpecificARService != null;
        }

        /// <summary>
        /// Initializes the Vuforia service
        /// </summary>
        protected async override void Initialize()
        {
            if (this.IsSupported)
            {
                var tcs = new TaskCompletionSource<bool>();
                this.initializationTask = tcs.Task;

                var result = await this.platformSpecificARService.Initialize(this.licenseKey);
                result = result && this.UpdateDataSet();

                tcs.SetResult(result);
            }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// ShutDown Vuforia Service
        /// </summary>
        /// <returns><c>true</c>, if down was shut, <c>false</c> otherwise.</returns>
        public bool ShutDown()
        {
            if (this.IsSupported)
            {
                return this.platformSpecificARService.ShutDown();
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Starts the Vuforia target tracking.
        /// </summary>
        /// <param name="retrieveCameraTexture">if set to <c>true</c>, the service will update the camera video texture.</param>
        /// <returns>
        ///   <c>true</c>, if tracking was started, <c>false</c> otherwise.
        /// </returns>
        public async Task<bool> StartTracking(bool retrieveCameraTexture)
        {
            this.CheckIfSupported();

            var initializationResult = await this.WaitForInitialization();

            if (this.pendingStartTrack ||
                !initializationResult)
            {
                return false;
            }

            this.pendingStartTrack = true;

            var result = await this.platformSpecificARService.StartTracking(retrieveCameraTexture);

            this.backgroundCameraMaterial.Diffuse1 = this.platformSpecificARService.CameraTexture;

            this.pendingStartTrack = false;

            return result;
        }

        /// <summary>
        /// Stops the Vuforia target tracking.
        /// </summary>
        /// <returns><c>true</c>, if tracking was stopped, <c>false</c> otherwise.</returns>
        public bool StopTracking()
        {
            if (this.State != ARState.Tracking)
            {
                return false;
            }

            return this.platformSpecificARService.StopTracking();
        }

        /// <summary>
        /// Update the service
        /// </summary>
        /// <param name="gameTime">The game timestan elapsed from the latest update</param>
        public override void Update(TimeSpan gameTime)
        {
            if (this.IsSupported)
            {
                this.platformSpecificARService.Update(gameTime);

                if (this.State == ARState.Tracking)
                {
                    this.WorldCenterResult = this.UpdateWorldCenterResult(out this.cameraTransform);

                    var activeCamera = this.ActiveCamera;
                    if (activeCamera != null)
                    {
                        this.cameraProjection = this.platformSpecificARService.GetCameraProjection(activeCamera.NearPlane, activeCamera.FarPlane);
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Loads a new dataSet. If any other dataSet is loaded, it will be deactivated and unloaded before load the new one.
        /// </summary>
        /// <returns><c>true</c>, if the dataset has been loaded, <c>false</c> otherwise.</returns>
        private bool UpdateDataSet()
        {
            this.parsedTrackables.Clear();

            if (this.IsSupported &&
                this.State != ARState.Tracking)
            {
                return this.platformSpecificARService.LoadDataSet(this.dataSetPath, this.extendedTracking);
            }

            return false;
        }

        private async Task<bool> WaitForInitialization()
        {
            var initResult = true;

            if (this.initializationTask != null)
            {
                initResult = await this.initializationTask;
                initResult &= this.State == ARState.Initialized;
            }

            return initResult;
        }

        /// <summary>
        /// Checks if Vuforia is supported in the current platform.
        /// </summary>
        /// <exception cref="System.NotSupportedException">NotSupportedException</exception>
        private void CheckIfSupported()
        {
            if (!this.IsSupported)
            {
                throw new NotSupportedException(WaveServices.Platform.PlatformType + " does not have Vuforia support.");
            }
        }

        private void Reset()
        {
            if (this.State == ARState.Stopped)
            {
                return;
            }

            this.Terminate();

            this.Initialize();
        }

        /// <summary>
        /// Terminate the service
        /// </summary>
        protected override void Terminate()
        {
            if (this.IsSupported)
            {
                this.StopTracking();
                this.ShutDown();
            }
        }

        private TrackableResult UpdateWorldCenterResult(out Matrix? cameraTransform)
        {
            TrackableResult result = null;
            Matrix? additionalTransform = null;
            cameraTransform = null;

            if (this.WorldCenterMode == WorldCenterMode.FirstTarget)
            {
                result = this.TrackableResults.FirstOrDefault();
            }
            else if (this.WorldCenterMode == WorldCenterMode.SpecificTarget &&
                     this.WorldCenterTrackable != null)
            {
                result = this.WorldCenterTrackable.FindMatchedTrackable(this.platformSpecificARService.TrackableResults);

                var resultTransform = this.WorldCenterTrackable.Owner.FindComponent<Transform3D>(false);
                if (resultTransform != null)
                {
                    additionalTransform = Matrix.CreateFromTRS(resultTransform.Position, resultTransform.Orientation, Vector3.One);
                }
            }

            if (result != null)
            {
                cameraTransform = Matrix.Invert(result.Pose);

                if (additionalTransform.HasValue)
                {
                    cameraTransform *= additionalTransform.Value;
                }
            }

            return result;
        }

        /// <summary>
        /// ParseTrackableNames
        /// </summary>
        /// <returns>IReadOnlyDictionary string, TargetTypes</returns>
        internal IReadOnlyDictionary<string, TargetTypes> ParseTrackableNames()
        {
            if (this.parsedTrackables.Count == 0)
            {
                using (var fileStream = WaveServices.Storage.OpenContentFile(this.dataSetPath))
                {
                    var doc = System.Xml.Linq.XDocument.Load(fileStream);

                    var trackables = doc.Descendants("Tracking").FirstOrDefault();
                    if (trackables != null)
                    {
                        foreach (var trackable in trackables.Descendants())
                        {
                            var typeStr = trackable.Name.LocalName;
                            var name = trackable.Attribute("name").Value;

                            TargetTypes targetType;
                            if (Enum.TryParse(typeStr, out targetType))
                            {
                                this.parsedTrackables.Add(name, targetType);
                            }
                        }
                    }
                }
            }

            return this.parsedTrackables;
        }
        #endregion
    }
}
