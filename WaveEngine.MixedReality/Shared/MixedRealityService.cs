// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using WaveEngine.Common;
using WaveEngine.Common.Math;
using WaveEngine.Common.VR;
#endregion

namespace WaveEngine.MixedReality
{
    /// <summary>
    /// Wave Engine service to make it easy to connect with the OVR framework.
    /// </summary>
    public class MixedRealityService : Service
    {
        /// <summary>
        /// The holo application
        /// </summary>
        internal IMixedRealityApplication MixedRealityApplication;

        /// <summary>
        /// The world to tracking space transform
        /// </summary>
        internal Matrix WorldToTrackingSpace;

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected
        {
            get
            {
                return this.MixedRealityApplication != null;
            }
        }

        /// <summary>
        /// Gets the eye properties
        /// </summary>
        public VREye[] EyeProperties
        {
            get
            {
                if (!this.IsConnected)
                {
                    throw new Exception("MixedReality is not available. See console output to find the reason");
                }

                return this.MixedRealityApplication.EyesProperties;
            }
        }

#if UWP
        /// <summary>
        /// Gets a reference frame attached to the holographic camera.
        /// </summary>
        internal Windows.Perception.Spatial.SpatialStationaryFrameOfReference ReferenceFrame
        {
            get
            {
                if (!this.IsConnected)
                {
                    throw new Exception("MixedReality is not available. See console output to find the reason");
                }

                return (this.MixedRealityApplication as BaseMixedRealityApplication).ReferenceFrame;
            }
        }
#endif

        /// <summary>
        /// Gets the focus position of the stabilization plane
        /// </summary>
        public Vector3? FocusPosition
        {
            get; internal set;
        }

        /// <summary>
        /// Gets the focus normal of the stabilization plane
        /// </summary>
        public Vector3? FocusNormal
        {
            get; internal set;
        }

        /// <summary>
        /// Gets the focus velocity of the stabilization plane
        /// </summary>
        public Vector3? FocusVelocity
        {
            get; internal set;
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="MixedRealityService"/> class.
        /// </summary>
        /// <param name="mixedRealityApplication">The MixedReality application.</param>
        public MixedRealityService(IMixedRealityApplication mixedRealityApplication)
        {
            this.MixedRealityApplication = mixedRealityApplication;
        }

        /// <summary>
        /// Sets the stabilization plane
        /// </summary>
        /// <param name="focusPosition">The focus position</param>
        public void SetStabilizationPlane(Vector3 focusPosition)
        {
            this.FocusPosition = Vector3.Transform(focusPosition, this.WorldToTrackingSpace);
            this.FocusNormal = null;
            this.FocusVelocity = null;
        }

        /// <summary>
        /// Sets the stabilization plane
        /// </summary>
        /// <param name="focusPosition">The focus position</param>
        /// <param name="focusNormal">The focus normal</param>
        public void SetStabilizationPlane(Vector3 focusPosition, Vector3 focusNormal)
        {
            this.FocusPosition = Vector3.Transform(focusPosition, this.WorldToTrackingSpace);
            this.FocusNormal = Vector3.TransformNormal(focusNormal, this.WorldToTrackingSpace);
            this.FocusVelocity = null;
        }

        /// <summary>
        /// Sets the stabilization plane
        /// </summary>
        /// <param name="focusPosition">The focus position</param>
        /// <param name="focusNormal">The focus normal</param>
        /// <param name="focusVelocity">The focus velocity</param>
        public void SetStabilizationPlane(Vector3 focusPosition, Vector3 focusNormal, Vector3 focusVelocity)
        {
            this.FocusPosition = Vector3.Transform(focusPosition, this.WorldToTrackingSpace);
            this.FocusNormal = Vector3.TransformNormal(focusNormal, this.WorldToTrackingSpace);
            this.FocusVelocity = Vector3.Transform(focusVelocity, this.WorldToTrackingSpace);
        }

        /// <summary>
        /// Sets the default stabilization plane
        /// </summary>
        public void SetDefaultStabilizationPlane()
        {
            this.FocusPosition = null;
            this.FocusNormal = null;
            this.FocusVelocity = null;
        }
        #endregion

        #region Public Methods
        #endregion

        #region Private Methods
        #endregion
    }
}
