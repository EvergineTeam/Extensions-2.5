// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

using Valve.VR;
using WaveEngine.Common.Input;
using WaveEngine.Common.Math;
using WaveEngine.Common.VR;
using WaveEngine.OpenVR.Helpers;

namespace WaveEngine.OpenVR.Input
{
    /// <summary>
    /// Open VR tracking reference
    /// </summary>
    public class OpenVRTrackingReference : IController
    {
        /// <summary>
        /// Gets or sets the tracking reference index
        /// </summary>
        public int Id;

        /// <summary>
        /// Gets or sets the pose of the controller
        /// </summary>
        public VRPose Pose
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this pose is valid
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected
        {
            get;
            set;
        }

        /// <summary>
        /// Update this instance
        /// </summary>
        /// <param name="id">The Id</param>
        /// <param name="pose">The pose</param>
        internal void Update(int id, VRPose pose)
        {
            this.Id = id;
            this.IsConnected = true;
            this.Pose = pose;
        }
    }
}
