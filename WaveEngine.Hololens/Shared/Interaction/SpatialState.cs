#region File Description
//-----------------------------------------------------------------------------
// SpatialState
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
#endregion

using WaveEngine.Common.Math;

namespace WaveEngine.Hololens.Interaction
{
    /// <summary>
    /// This class represent the current state of the input system.
    /// </summary>
    public struct SpatialState
    {
        /// <summary>
        /// Gets a value indicating whether the input source is detected
        /// </summary>
        public bool IsDetected { get; internal set; }

        /// <summary>
        /// Gest a value indicating whether user is making the air tap gesture.
        /// </summary>
        public bool IsSelected { get; internal set; }

        /// <summary>
        /// Gets the kind of an interaction source.
        /// </summary>
        public SpatialSource Source { get; internal set; }

        /// <summary>
        /// Gets the recognition confident as a value from 0.0 to 1.0.
        /// </summary>
        public float SourceLossRisk { get; internal set; }

        /// <summary>
        ///  Gets the position of a hand or controller. 
        /// </summary>
        public Vector3 Position;

        /// <summary>
        ///  Gets the velocity of a hand or controller.
        /// </summary>
        public Vector3 Velocity;
    }
}
