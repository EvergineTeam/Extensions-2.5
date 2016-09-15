#region File Description
//-----------------------------------------------------------------------------
// ARServiceAndroid
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Runtime.InteropServices;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Vuforia.QCAR;
#endregion

namespace WaveEngine.Vuforia
{
    public class ARServiceAndroid : IVuforiaService
    {
        #region P/Invoke
        [DllImport("libVuforiaAdapter.so")]
        private extern static void QCAR_setInitState();

        [DllImport("libVuforiaAdapter.so")]
        private extern static bool QCAR_shutDown();

        [DllImport("libVuforiaAdapter.so")]
        private extern static ARState QCAR_getState();

        [DllImport("libVuforiaAdapter.so")]
        private extern static void QCAR_setOrientation(AROrientation orientation);

        [DllImport("libVuforiaAdapter.so")]
        private extern static int QCAR_initialize(string dataSetPath, bool extendedTracking);

        [DllImport("libVuforiaAdapter.so")]
        private extern static bool QCAR_startTrack(int frameWidth, int frameHeight);

        [DllImport("libVuforiaAdapter.so")]
        private extern static bool QCAR_stopTrack();

        [DllImport("libVuforiaAdapter.so")]
        private extern static QCAR_TrackResult QCAR_update();

        [DllImport("libVuforiaAdapter.so")]
        private extern static QCAR_Matrix4x4 QCAR_getCameraProjection(float nearPlane, float farPlane);
        #endregion

        #region Variables
        private Matrix correctionRotationMatrix;
        private string currentTrackName;
        #endregion

        #region Properties
        public ARState State
        {
            get
            {
                return QCAR_getState();
            }
        }

        public string CurrentTrackName
        {
            get
            {
                return this.currentTrackName;
            }
            set
            {
                bool notify = this.currentTrackName != value;

                this.currentTrackName = value;
                if (notify && this.trackNameChanged != null)
                {
                    this.trackNameChanged(this.currentTrackName);
                }
            }
        }

        public Matrix Pose
        {
            get;
            private set;
        }

        public Matrix PoseInv
        {
            get;
            private set;
        }

        public Matrix Projection
        {
            get;
            private set;
        }
        #endregion

        #region Events
        public TrackNameChangedHandler trackNameChanged;

        public event TrackNameChangedHandler TrackNameChanged
        {
            add { this.trackNameChanged += value; }
            remove { this.trackNameChanged -= value; }
        }
        #endregion

        #region Initialize
        public ARServiceAndroid()
        {
            this.correctionRotationMatrix = Matrix.CreateRotationX(MathHelper.PiOver2);
        }
        #endregion

        public void Initialize(string licenseKey)
        {
            Java.Lang.JavaSystem.LoadLibrary("Vuforia");
            Java.Lang.JavaSystem.LoadLibrary("VuforiaAdapter");

            var adapter = (Game.Current).Application.Adapter as WaveEngine.Adapter.Adapter;
            var activity = adapter.Activity;

            Com.Vuforia.Vuforia.SetInitParameters(activity, Com.Vuforia.Vuforia.Gl20, licenseKey);
            Com.Vuforia.Vuforia.Init();
            QCAR_setInitState();
        }

        public bool ShutDown()
        {
            return QCAR_shutDown();
        }

        public bool LoadDataSet(string dataSetPath)
        {
            return QCAR_initialize(dataSetPath, true) == 0;
        }

        public Matrix GetCameraProjection(float nearPlane, float farPlane)
        {
            return QCAR_getCameraProjection(nearPlane, farPlane).ToEngineMatrix();
        }

        public bool StartTrack(int width, int height)
        {
            bool res = QCAR_startTrack(width, height);

            if (res)
            {
                return true;
            }
            return false;
        }

        public bool StopTrack()
        {
            return QCAR_stopTrack();
        }

        public void Update(TimeSpan gameTime)
        {
            if (this.State != ARState.TRACKING)
            {
                return;
            }

            QCAR_TrackResult result = QCAR_update();

            this.CurrentTrackName = result.IsTracking ? result.TrackName : null;

            if (result.IsTracking)
            {
                this.Pose = this.correctionRotationMatrix * result.TrackPose.ToEngineMatrix();
                this.PoseInv = Matrix.Invert(this.Pose);
            }
        }
    }
}