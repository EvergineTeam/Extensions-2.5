using System;
using System.Runtime.InteropServices;
using WaveEngine.Vuforia;
using MonoTouch.UIKit;
using WaveEngine.Common.Math;
using MonoTouch.Foundation;
using WaveEngine.Framework.Services;
using WaveEngine.Common.Graphics;

namespace WaveEngine.Vuforia.iOS
{
	[StructLayout(LayoutKind.Sequential)]
	internal struct QCAR_Matrix4x4
	{
		[MarshalAs (UnmanagedType.ByValArray, SizeConst=16)]
		public float[] data;

		/// <summary>
		/// To the engine matrix.
		/// </summary>
		/// <param name="mat">The mat.</param>
		/// <returns>Converted matrix.</returns>
		public WaveEngine.Common.Math.Matrix ToEngineMatrix()
		{
			return new WaveEngine.Common.Math.Matrix()
			{
				M11 = data[0],
				M12 = data[1],
				M13 = data[2],
				M14 = data[3],
				M21 = data[4],
				M22 = data[5],
				M23 = data[6],
				M24 = data[7],
				M31 = data[8],
				M32 = data[9],
				M33 = data[10],
				M34 = data[11],
				M41 = data[12],
				M42 = data[13],
				M43 = data[14],
				M44 = data[15]
			};
		}

	};

	[StructLayout(LayoutKind.Sequential)]
	internal struct QCAR_TrackResult
	{
		public const int TRACK_NAME_SIZE = 32;

		[MarshalAs(UnmanagedType.I1)]
		public bool IsTracking;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst=TRACK_NAME_SIZE)]
		public string TrackName;

		public QCAR_Matrix4x4 TrackPose;
	};


	public class ARServiceIOS : IVuforiaService
	{
		#region P/Invoke
		[DllImport("__Internal")]
		private extern static bool QCAR_init();

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

		public void Initialize ()
		{
			bool res = QCAR_init ();

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