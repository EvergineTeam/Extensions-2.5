#region File Description
//-----------------------------------------------------------------------------
// VuforiaService
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Usings Statements
using System;
using System.Runtime.InteropServices;
using WaveEngine.Common.Math;
using System.Drawing;
using WaveEngine.Common.Graphics;
using System.Diagnostics;
using WaveEngine.Vuforia;
using WaveEngine.Framework.Services;
using WaveEngine.Common;
#endregion

#if IOS
using WaveEngine.Vuforia.iOS;
#endif

namespace WaveEngine.Vuforia
{
    public enum ARState
    {
        STOPPED = 0,
        INITIALIZED,
        TRACKING
    }

	public enum AROrientation
	{
		ORIENTATION_PORTRAIT = 0,
		ORIENTATION_PORTRAIT_UPSIDEDOWN,
		ORIENTATION_LANDSCAPE_LEFT,
		ORIENTATION_LANDSCAPE_RIGHT,
	};

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
        /// Platform specific service code
        /// </summary>
		private IVuforiaService platformSpecificARService;

		#region Events
		/// <summary>
		/// Event fired when track object name is changed
		/// </summary>
		public event TrackNameChangedHandler TrackNameChanged
		{
			add { this.platformSpecificARService.TrackNameChanged += value; }
			remove { this.platformSpecificARService.TrackNameChanged -= value; }
		} 
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
				return this.platformSpecificARService.CurrentTrackName;
			}
		}

		/// <summary>
		/// Gets the pose.
		/// </summary>
		/// <value>The pose.</value>
		public Matrix Pose {
			get { return this.platformSpecificARService.Pose; }
		}

		/// <summary>
		/// Gets the pose inverted.
		/// </summary>
		/// <value>The pose.</value>
		public Matrix PoseInv 
		{
			get { return this.platformSpecificARService.PoseInv; }
		}

		/// <summary>
		/// Gets the projection.
		/// </summary>
		/// <value>The projection.</value>
		public Matrix Projection 
		{
			get { return this.platformSpecificARService.Projection; }
		}
		#endregion

		#region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="VuforiaService" /> class.
        /// </summary>
        /// <param name="dataSetPath">Vuforia patters path.</param>
		public VuforiaService(string dataSetPath)
		{
            this.dataSetPath = dataSetPath;
#if IOS
            this.platformSpecificARService = new ARServiceIOS();
#endif
		}
		#endregion

		#region Public Methods
        /// <summary>
        /// Initializes the Vuforia service
        /// </summary>
		protected override void Initialize ()
		{
            if (this.IsSupported)
            {
                this.platformSpecificARService.Initialize();
                this.platformSpecificARService.LoadDataSet(this.dataSetPath);
            }
		}

		/// <summary>
		/// ShutDown Vuforia
		/// </summary>
		/// <returns><c>true</c>, if down was shut, <c>false</c> otherwise.</returns>
		public bool ShutDown()
        {
            if (!this.IsSupported)
            {
                throw new NotSupportedException(WaveServices.Platform.PlatformType + " does not have Vuforia support.");
            }

			return this.platformSpecificARService.ShutDown ();
		}

		/// <summary>
		/// Gets the camera projection.
		/// </summary>
		/// <returns>The camera projection.</returns>
		/// <param name="nearPlane">Near plane.</param>
		/// <param name="farPlane">Far plane.</param>
		public Matrix GetCameraProjection(float nearPlane, float farPlane)
		{
            if (!this.IsSupported)
            {
                throw new NotSupportedException(WaveServices.Platform.PlatformType + " does not have Vuforia support.");
            }

            return this.platformSpecificARService.GetCameraProjection(nearPlane, farPlane);
		}

		/// <summary>
		/// Starts Vuforia image tracking.
		/// </summary>
		/// <returns><c>true</c>, if track was started, <c>false</c> otherwise.</returns>
		/// <param name="frameSize">Frame size.</param>
		public bool StartTrack()
		{
            if (!this.IsSupported)
            {
                throw new NotSupportedException(WaveServices.Platform.PlatformType + " does not have Vuforia support.");
            }

            return this.platformSpecificARService.StartTrack(WaveServices.Platform.ScreenWidth, WaveServices.Platform.ScreenHeight);
		}

		/// <summary>
		/// Stops Vuforia image tracking.
		/// </summary>
		/// <returns><c>true</c>, if track was stopped, <c>false</c> otherwise.</returns>
		public bool StopTrack()
		{
            if (!this.IsSupported)
            {
                throw new NotSupportedException(WaveServices.Platform.PlatformType + " does not have Vuforia support.");
            }

			return this.platformSpecificARService.StopTrack ();
		}

		/// <summary>
		/// Update .
		/// </summary>
		public override void Update (TimeSpan gameTime)
		{
            if (this.IsSupported)
            {
                this.platformSpecificARService.Update(gameTime);
            }
		}
		#endregion

        #region Private Methods
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

