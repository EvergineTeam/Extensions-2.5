// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
#endregion

using WaveEngine.Common.Math;

namespace WaveEngine.MixedReality.Interaction
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
        /// Gets a value indicating whether gest a value indicating whether user is making the air tap gesture.
        /// </summary>
        public bool IsSelected { get; internal set; }

        /// <summary>
        /// Gets the kind of an interaction source.
        /// </summary>
        public SpatialSource Kind { get; internal set; }

        /// <summary>
        /// Gets the recognition confident as a value from 0.0 to 1.0.
        /// </summary>
        public float SourceLossRisk { get; internal set; }

        /// <summary>
        /// Gets Left or Right controller
        /// </summary>
        public SpatialInteractionHandedness Handedness { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether thumbstick is pressed or not
        /// </summary>
        public bool IsThumbstickPressed { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether touchpad is pressed or not
        /// </summary>
        public bool IsTouchpadPressed { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether touchpad is touched or not
        /// </summary>
        public bool IsTouchpadTouched { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether grasp button is pressed or not
        /// </summary>
        public bool IsGraspPressed { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether menu button is pressed or not
        /// </summary>
        public bool IsMenuPressed { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether menu button is pressed or not
        /// </summary>
        public bool IsSelectTriggerPressed { get; internal set; }

        /// <summary>
        /// Gets the select trigger value
        /// </summary>
        public float SelectTriggerValue { get; internal set; }

        /// <summary>
        /// Gets Thumbstick X,Y values
        /// </summary>
        public Vector2 Thumbstick { get; internal set; }

        /// <summary>
        /// Gets Touchpad X,Y values
        /// </summary>
        public Vector2 Touchpad { get; internal set; }

        /// <summary>
        ///  Gets the position of a hand or controller.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// Gets the Orientation of a hand or controller.
        /// </summary>
        public Quaternion Orientation;

        /// <summary>
        ///  Gets the velocity of a hand or controller.
        /// </summary>
        public Vector3 Velocity;

        /// <summary>
        /// Gets the tip of controller position
        /// </summary>
        public Vector3 TipControllerPosition;

        /// <summary>
        /// Gets the tip of controller forward
        /// </summary>
        public Vector3 TipControllerForward;
    }
}
