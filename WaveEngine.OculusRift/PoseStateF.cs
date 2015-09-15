#region File Description
//-----------------------------------------------------------------------------
// PoseStateF
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Runtime.InteropServices;
using WaveEngine.Common.Math;
#endregion

namespace WaveEngine.OculusRift
{
    /// <summary>
    /// Full pose (rigid body) configuration with first and second derivatives.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct PoseStateF
    {
        /// <summary>
        /// The pose
        /// </summary>
        public PoseF Pose;

        /// <summary>
        /// The angular velocity
        /// </summary>
        public Vector3 AngularVelocity;

        /// <summary>
        /// The linear velocity
        /// </summary>
        public Vector3 LinearVelocity;

        /// <summary>
        /// The angular acceleration
        /// </summary>
        public Vector3 AngularAcceleration;

        /// <summary>
        /// The linear acceleration
        /// </summary>
        public Vector3 LinearAcceleration;

        /// <summary>
        /// The time in seconds
        /// </summary>
        public double TimeInSeconds;
    }
}
