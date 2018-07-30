// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using WaveEngine.Common.Input;
#endregion

namespace WaveEngine.OculusRift.Input
{
    /// <summary>
    /// The touch state of an Oculus Touch controller
    /// </summary>
    public struct TouchState
    {
        /// <summary>
        /// A button on the right Touch controller
        /// </summary>
        public ButtonState A;

        /// <summary>
        /// B button on the right Touch controller
        /// </summary>
        public ButtonState B;

        /// <summary>
        /// Thumb rest button on the right Touch controller
        /// </summary>
        public ButtonState RThumb;

        /// <summary>
        /// Index trigger on the right Touch controller
        /// </summary>
        public ButtonState RIndexTrigger;

        /// <summary>
        /// X button on the left Touch controller
        /// </summary>
        public ButtonState X;

        /// <summary>
        /// Y button on the left Touch controller
        /// </summary>
        public ButtonState Y;

        /// <summary>
        /// Thumb rest button on the left Touch controller
        /// </summary>
        public ButtonState LThumb;

        /// <summary>
        /// Index trigger on the left Touch controller
        /// </summary>
        public ButtonState LIndexTrigger;

        /// <summary>
        /// Index finger pointing on the right Touch controller
        /// </summary>
        public ButtonState RIndexPointing;

        /// <summary>
        /// Thumb up on the right Touch controller
        /// </summary>
        public ButtonState RThumbUp;

        /// <summary>
        /// Index finger pointing on the left Touch controller
        /// </summary>
        public ButtonState LIndexPointing;

        /// <summary>
        /// Thumb up on the left Touch controller
        /// </summary>
        public ButtonState LThumbUp;
    }
}
