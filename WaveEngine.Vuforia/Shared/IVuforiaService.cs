#region File Description
//-----------------------------------------------------------------------------
// IVuforiaService
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Usings Statements
using System;
using WaveEngine.Common.Math;
#endregion

namespace WaveEngine.Vuforia
{
    public delegate void TrackNameChangedHandler(string newTrackName);

    /// <summary>
    /// AR interface service
    /// </summary>
    internal interface IVuforiaService
    {
        #region Properties
        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <value>The state.</value>
        ARState State { get; }

        /// <summary>
        /// Gets the name of the current track.
        /// </summary>
        /// <value>The name of the current track.</value>
        string CurrentTrackName { get; }

        /// <summary>
        /// Gets the pose.
        /// </summary>
        /// <value>The pose.</value>
        Matrix Pose { get; }

        /// <summary>
        /// Gets the pose inverted.
        /// </summary>
        /// <value>The pose.</value>
        Matrix PoseInv { get; }


        /// <summary>
        /// Gets the projection.
        /// </summary>
        /// <value>The projection.</value>
        Matrix Projection { get; }
        #endregion

        #region Events
        event TrackNameChangedHandler TrackNameChanged;
        #endregion

        /// <summary>
        /// Init this instance.
        /// </summary>
        void Initialize(string licenseKey);

        /// <summary>
        /// ShutDown AR
        /// </summary>
        /// <returns><c>true</c>, if down was shut, <c>false</c> otherwise.</returns>
        bool ShutDown();

        /// <summary>
        /// Loads the AR data set.
        /// </summary>
        /// <returns><c>true</c>, if data set was loaded, <c>false</c> otherwise.</returns>
        /// <param name="dataSetPath">Data set path.</param>
        bool LoadDataSet(string dataSetPath);

        /// <summary>
        /// Gets the camera projection.
        /// </summary>
        /// <returns>The camera projection.</returns>
        /// <param name="nearPlane">Near plane.</param>
        /// <param name="farPlane">Far plane.</param>
        Matrix GetCameraProjection(float nearPlane, float farPlane);

        /// <summary>
        /// Starts the track.
        /// </summary>
        /// <returns><c>true</c>, if track was started, <c>false</c> otherwise.</returns>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        bool StartTrack(int width, int height);

        /// <summary>
        /// Stops the track.
        /// </summary>
        /// <returns><c>true</c>, if track was stoped, <c>false</c> otherwise.</returns>
        bool StopTrack();

        /// <summary>
        /// Update the specified gameTime.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        void Update(TimeSpan gameTime);
    }
}

