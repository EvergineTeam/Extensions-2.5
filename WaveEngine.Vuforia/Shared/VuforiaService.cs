#region File Description
//-----------------------------------------------------------------------------
// VuforiaService
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Usings Statements
using System;
using System.Threading.Tasks;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Graphics.VertexFormats;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.Vuforia
{
    /// <summary>
    /// Vuforia integration service
    /// </summary>
    public class VuforiaService : UpdatableService
    {
        /// <summary>
        /// Vuforia patters path
        /// </summary>
        private string dataSetPath;

        /// <summary>
        /// Vuforia license key
        /// </summary>
        private string licenseKey;

        /// <summary>
        /// Vuforia extended tracking
        /// </summary>
        private bool extendedTracking;

        /// <summary>
        /// Platform specific service code
        /// </summary>
        private ARServiceBase platformSpecificARService;

        #region Events
        /// <summary>
        /// Event fired when track object name is changed
        /// </summary>
        public event EventHandler<string> TrackNameChanged;
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether Vuforia integration is supported
        /// </summary>
        public bool IsSupported
        {
            get
            {
                return this.platformSpecificARService != null;
            }
        }

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <value>The state.</value>
        public ARState State
        {
            get
            {
                this.CheckIfSupported();

                return this.platformSpecificARService.State;
            }
        }

        /// <summary>
        /// Gets the name of the current track.
        /// </summary>
        /// <value>The name of the current track.</value>
        public string CurrentTrackName
        {
            get
            {
                this.CheckIfSupported();

                return this.platformSpecificARService.CurrentTrackName;
            }
        }

        /// <summary>
        /// Gets the pose.
        /// </summary>
        /// <value>The pose.</value>
        public Matrix Pose
        {
            get
            {
                this.CheckIfSupported();

                return this.platformSpecificARService.Pose;
            }
        }

        /// <summary>
        /// Gets the pose inverted.
        /// </summary>
        /// <value>The pose.</value>
        public Matrix PoseInv
        {
            get
            {
                this.CheckIfSupported();

                return this.platformSpecificARService.PoseInv;
            }
        }

        /// <summary>
        /// Gets the projection.
        /// </summary>
        /// <value>The projection.</value>
        public Matrix Projection
        {
            get
            {
                this.CheckIfSupported();

                return this.platformSpecificARService.Projection;
            }
        }

        /// <summary>
        /// Gets the camera texture.
        /// </summary>
        /// <value>The camera texture.</value>
        public Texture CameraTexture
        {
            get
            {
                this.CheckIfSupported();

                return this.platformSpecificARService.CameraTexture;
            }
        }

        /// <summary>
        /// Gets the camera projection matrix.
        /// </summary>
        /// <value>The camera projection matrix.</value>
        public Matrix CameraProjectionMatrix
        {
            get
            {
                this.CheckIfSupported();

                return this.platformSpecificARService.CameraProjectionMatrix;
            }
        }

        /// <summary>
        /// The video mesh
        /// </summary>
        public Mesh VideoMesh
        {
            get
            {
                this.CheckIfSupported();

                return this.platformSpecificARService.VoideoMesh;
            }
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="VuforiaService" /> class.
        /// </summary>
        /// <param name="dataSetPath">Vuforia patters path.</param>
        /// <param name="licenseKey">Vuforia Liscense key</param>
        /// <param name="extendedTracking">if set to <c>true</c> enables the extended tracking feature.</param>
        public VuforiaService(string dataSetPath, string licenseKey, bool extendedTracking = true)
        {
            this.dataSetPath = dataSetPath;
            this.licenseKey = licenseKey;
            this.extendedTracking = extendedTracking;

#if IOS
            this.platformSpecificARService = new ARServiceIOS();
#elif ANDROID
            this.platformSpecificARService = new ARServiceAndroid();
#elif UWP
            this.platformSpecificARService = new ARServiceUWP();
#endif

            if (this.platformSpecificARService != null)
            {
                this.platformSpecificARService.TrackNameChanged += this.PlatformSpecificARService_TrackNameChanged;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Initializes the Vuforia service
        /// </summary>
        protected async override void Initialize()
        {
            if (this.IsSupported)
            {
                await this.platformSpecificARService.Initialize(this.licenseKey, this.dataSetPath, this.extendedTracking);
            }
        }

        /// <summary>
        /// ShutDown Vuforia
        /// </summary>
        /// <returns><c>true</c>, if down was shut, <c>false</c> otherwise.</returns>
        public bool ShutDown()
        {
            this.CheckIfSupported();

            return this.platformSpecificARService.ShutDown();
        }

        /// <summary>
        /// Gets the camera projection.
        /// </summary>
        /// <returns>The camera projection.</returns>
        /// <param name="nearPlane">Near plane.</param>
        /// <param name="farPlane">Far plane.</param>
        public Matrix GetCameraProjection(float nearPlane, float farPlane)
        {
            this.CheckIfSupported();

            return this.platformSpecificARService.GetCameraProjection(nearPlane, farPlane);
        }

        /// <summary>
        /// Starts Vuforia image tracking.
        /// </summary>
        /// <param name="retrieveCameraTexture">if set to <c>true</c>, the service will update the camera video texture.</param>
        /// <returns>
        ///   <c>true</c>, if track was started, <c>false</c> otherwise.
        /// </returns>
        public Task<bool> StartTrack(bool retrieveCameraTexture)
        {
            this.CheckIfSupported();

            return this.platformSpecificARService.StartTrack(retrieveCameraTexture);
        }

        /// <summary>
        /// Stops Vuforia image tracking.
        /// </summary>
        /// <returns><c>true</c>, if track was stopped, <c>false</c> otherwise.</returns>
        public bool StopTrack()
        {
            this.CheckIfSupported();

            return this.platformSpecificARService.StopTrack();
        }

        /// <summary>
        /// Update .
        /// </summary>
        public override void Update(TimeSpan gameTime)
        {
            if (this.IsSupported)
            {
                this.platformSpecificARService.Update(gameTime);
            }
        }
        #endregion

        #region Private Methods
        private void PlatformSpecificARService_TrackNameChanged(object sender, string trackName)
        {
            if (this.TrackNameChanged != null)
            {
                this.TrackNameChanged(this, trackName);
            }
        }

        /// <summary>
        /// Checks if Vuforia is supported in the current platform.
        /// </summary>
        /// <exception cref="System.NotSupportedException"></exception>
        private void CheckIfSupported()
        {
            if (!this.IsSupported)
            {
                throw new NotSupportedException(WaveServices.Platform.PlatformType + " does not have Vuforia support.");
            }
        }

        /// <summary>
        /// Terminate the service
        /// </summary>
        protected override void Terminate()
        {
            if (this.IsSupported)
            {
                this.StopTrack();
                this.ShutDown();
            }
        }
        #endregion
    }
}

