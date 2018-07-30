// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using WaveEngine.Common.Graphics.VertexFormats;
using WaveEngine.Common.Math;
using WaveEngine.Vuforia.QCAR;
#endregion

namespace WaveEngine.Vuforia
{
    /// <summary>
    /// A TrackableResult is an object that represents the state of a Trackable
    /// which was found in a given frame. Every TrackableResult has a corresponding
    /// Trackable, a type, a 6DOF pose and a status(e.g.tracked).
    /// </summary>
    public class TrackableResult
    {
        private static readonly Matrix poseObjectCorrectionMatrix = Matrix.CreateRotationX(MathHelper.PiOver2);

        private static readonly Matrix poseCameraCorrectionMatrix = Matrix.CreateRotationX(MathHelper.Pi);

        /// <summary>
        /// Gets the runtime Id of the Trackable
        /// </summary>
        public int Id
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the trackable status.
        /// </summary>
        public TrackableStatus Status
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets gets the corresponding Trackable that this result represents
        /// </summary>
        public Trackable Trackable
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the current pose matrix.
        /// </summary>
        public Matrix Pose
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackableResult"/> class.
        /// </summary>
        /// <param name="trackableResult">The trackable result.</param>
        /// <param name="trackable">The trackable.</param>
        internal TrackableResult(QCAR_TrackableResult trackableResult, Trackable trackable)
        {
            this.Trackable = trackable;
            this.Refresh(trackableResult);
        }

        /// <summary>
        /// Refreshes the specified trackable result.
        /// </summary>
        /// <param name="trackableResult">The trackable result.</param>
        internal virtual void Refresh(QCAR_TrackableResult trackableResult)
        {
            this.Id = trackableResult.Id;
            this.Status = trackableResult.Status;
            this.Pose = poseObjectCorrectionMatrix * trackableResult.TrackPose.ToEngineMatrix() * poseCameraCorrectionMatrix;
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is TrackableResult &&
                this.Id.Equals(((TrackableResult)obj).Id);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}
