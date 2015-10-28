#region File Description
//-----------------------------------------------------------------------------
// ARServiceIOS
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Runtime.InteropServices;
using WaveEngine.Vuforia;
using UIKit;
using WaveEngine.Common.Math;
using Foundation;
using WaveEngine.Framework.Services;
using WaveEngine.Common.Graphics;
using WaveEngine.Vuforia.QCAR;
#endregion

namespace WaveEngine.Vuforia
{
	public class ARServiceIOS : IVuforiaService
	{
		#region P/Invoke
		[DllImport("__Internal")]
		private extern static bool QCAR_init(string licenseKey);

		[DllImport("__Internal")]
		private extern static bool QCAR_shutDown();

		[DllImport("__Internal")]
		private extern static ARState QCAR_getState();

		[DllImport("__Internal")]
		private extern static void QCAR_setOrientation(AROrientation orientation);

		[DllImport("__Internal")]
		private extern static int QCAR_initialize (string dataSetPath, bool extendedTracking);

		[DllImport("__Internal")]
		private extern static bool QCAR_startTrack(int frameWidth, int frameHeight);

		[DllImport("__Internal")]
		private extern static bool QCAR_stopTrack();

		[DllImport("__Internal")]
		private extern static QCAR_TrackResult QCAR_update();

		[DllImport("__Internal")]
		private extern static QCAR_Matrix4x4 QCAR_getCameraProjection(float nearPlane, float farPlane);
		#endregion

		#region Variables
		private Matrix correctionRotationMatrix;
		private NSObject notificationToken;
		private string currentTrackName;
		#endregion

		#region Properties
		public ARState State {
			get 
			{
				return QCAR_getState();
			}
		}

		public string CurrentTrackName 
		{
			get {
				return this.currentTrackName;
			}
			set{
				bool notify = this.currentTrackName != value;

				this.currentTrackName = value;
				if (notify && this.trackNameChanged != null) 
				{
					this.trackNameChanged (this.currentTrackName);
				}
			}
		}

		public Matrix Pose {
			get;
			private set;
		}

		public Matrix PoseInv {
			get;
			private set;
		}

		public Matrix Projection {
			get;
			private set;
		}
		#endregion

		#region Events
		public TrackNameChangedHandler trackNameChanged;

		public event TrackNameChangedHandler TrackNameChanged
		{
			add {this.trackNameChanged += value;}
			remove {this.trackNameChanged -= value;}
		}
		#endregion

		#region Initialize
		public ARServiceIOS ()
		{
			this.correctionRotationMatrix = Matrix.CreateRotationX (MathHelper.PiOver2);
		}
        #endregion

        public void Initialize(string licenseKey)
        {
			bool res = QCAR_init (licenseKey);

			if(res)
			{
				// Capture orientation change notifiication
				notificationToken = NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.DidChangeStatusBarOrientationNotification, this.OrientationChangeNotification, null);
			}
		}

		public bool ShutDown ()
		{
			NSNotificationCenter.DefaultCenter.RemoveObserver(notificationToken);

			return QCAR_shutDown ();
		}

		public bool LoadDataSet (string dataSetPath)
		{
			return QCAR_initialize(dataSetPath, true) == 0;
		}

		public Matrix GetCameraProjection (float nearPlane, float farPlane)
		{
			return QCAR_getCameraProjection(nearPlane, farPlane).ToEngineMatrix();
		}

		public bool StartTrack (int width, int height)
		{
			this.UpdateOrientation ();
			bool res = QCAR_startTrack (width, height);

			if (res) 
			{
				this.UpdateOrientation();
				return true;
			}
			return false;
		}

		public bool StopTrack ()
		{
			return QCAR_stopTrack ();
		}

		public void Update (TimeSpan gameTime)
		{
			if (this.State != ARState.TRACKING) 
			{
				return;
			}

			QCAR_TrackResult result = QCAR_update ();

			this.CurrentTrackName = result.IsTracking ? result.TrackName : null;

			if (result.IsTracking) 
			{
				this.Pose = this.correctionRotationMatrix * result.TrackPose.ToEngineMatrix ();
				this.PoseInv = Matrix.Invert (this.Pose);
			}
		}

		/// <summary>
		/// Notification method called when the device orientation is changed
		/// Is used to update capture orientaiton
		/// </summary>
		/// <param name="notification">notification object.</param>
		private void OrientationChangeNotification(NSNotification notification)
		{
			this.UpdateOrientation ();
		}

		/// <summary>
		/// Update QCAR orientation
		/// </summary>
		private void UpdateOrientation()
		{
			AROrientation qcarOrientation;

			switch (UIApplication.SharedApplication.StatusBarOrientation) 
			{
			case UIInterfaceOrientation.LandscapeLeft:
				qcarOrientation = AROrientation.ORIENTATION_LANDSCAPE_LEFT;
				break;

			case UIInterfaceOrientation.LandscapeRight:
				qcarOrientation = AROrientation.ORIENTATION_LANDSCAPE_RIGHT;
				break;

			case UIInterfaceOrientation.PortraitUpsideDown:
				qcarOrientation = AROrientation.ORIENTATION_PORTRAIT_UPSIDEDOWN;
				break;

			default:
				qcarOrientation = AROrientation.ORIENTATION_PORTRAIT;
				break;
			}

			QCAR_setOrientation (qcarOrientation);
		}
	}
}