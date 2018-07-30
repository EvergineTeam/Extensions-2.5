// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using OculusWrap;
using WaveEngine.Common.Input;
using WaveEngine.Common.Math;
using WaveEngine.Common.VR;
using WaveEngine.OculusRift.Helpers;
#endregion

namespace WaveEngine.OculusRift.Input
{
    /// <summary>
    /// The Oculus Touch controller state
    /// </summary>
    public class OculusTouchControllerState : IController
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
        /// Thumbstick button on the right Touch controller
        /// </summary>
        public ButtonState RThumb;

        /// <summary>
        /// X button on the left Touch controller
        /// </summary>
        public ButtonState X;

        /// <summary>
        /// Y button on the left Touch controller
        /// </summary>
        public ButtonState Y;

        /// <summary>
        /// Thumbstick button on the left Touch controller
        /// </summary>
        public ButtonState LThumb;

        /// <summary>
        /// Start button on the left Touch controller
        /// </summary>
        public ButtonState Start;

        /// <summary>
        /// Touch state for the buttons
        /// </summary>
        public TouchState TouchState;

        /// <summary>
        /// Left index trigger value, in the range 0.0 to 1.0f.
        /// </summary>
        public float LeftIndexTrigger;

        /// <summary>
        /// Right index trigger value, in the range 0.0 to 1.0f.
        /// </summary>
        public float RightIndexTrigger;

        /// <summary>
        /// Left hand trigger value, in the range 0.0 to 1.0f.
        /// </summary>
        public float LeftHandTrigger;

        /// <summary>
        /// Right hand trigger value, in the range 0.0 to 1.0f.
        /// </summary>
        public float RightHandTrigger;

        /// <summary>
        /// Left thumbstick axis values, in the range -1.0f to 1.0f.
        /// </summary>
        public Vector2 LeftThumbstick;

        /// <summary>
        /// Right thumbstick axis values, in the range -1.0f to 1.0f.
        /// </summary>
        public Vector2 RightThumbstick;

        /// <summary>
        /// The controller state
        /// </summary>
        private OVRTypes.ControllerType controllerType;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OculusTouchControllerState" /> class.
        /// </summary>
        public OculusTouchControllerState()
        {
        }

        /// <summary>
        /// Updates the state of the buttons and poses in the Oculus Touch controllers
        /// </summary>
        /// <param name="hmd">The HMD handler</param>
        internal void Update(Hmd hmd)
        {
            OVRTypes.InputState inputState = hmd.GetInputState(OVRTypes.ControllerType.Touch);

            this.controllerType = inputState.ControllerType;
            this.IsConnected = this.controllerType == OVRTypes.ControllerType.Touch;

            // Update button state
            this.A = (inputState.Buttons & (uint)OVRTypes.Button.A) != 0 ? ButtonState.Pressed : ButtonState.Released;
            this.B = (inputState.Buttons & (uint)OVRTypes.Button.B) != 0 ? ButtonState.Pressed : ButtonState.Released;
            this.RThumb = (inputState.Buttons & (uint)OVRTypes.Button.RThumb) != 0 ? ButtonState.Pressed : ButtonState.Released;
            this.X = (inputState.Buttons & (uint)OVRTypes.Button.X) != 0 ? ButtonState.Pressed : ButtonState.Released;
            this.Y = (inputState.Buttons & (uint)OVRTypes.Button.Y) != 0 ? ButtonState.Pressed : ButtonState.Released;
            this.LThumb = (inputState.Buttons & (uint)OVRTypes.Button.LThumb) != 0 ? ButtonState.Pressed : ButtonState.Released;
            this.Start = (inputState.Buttons & (uint)OVRTypes.Button.Enter) != 0 ? ButtonState.Pressed : ButtonState.Released;

            // Update touch state
            this.TouchState.A = (inputState.Touches & (uint)OVRTypes.Touch.A) != 0 ? ButtonState.Pressed : ButtonState.Released;
            this.TouchState.B = (inputState.Touches & (uint)OVRTypes.Touch.B) != 0 ? ButtonState.Pressed : ButtonState.Released;
            this.TouchState.RThumb = (inputState.Touches & (uint)OVRTypes.Touch.RThumb) != 0 ? ButtonState.Pressed : ButtonState.Released;
            this.TouchState.RIndexTrigger = (inputState.Touches & (uint)OVRTypes.Touch.RIndexTrigger) != 0 ? ButtonState.Pressed : ButtonState.Released;
            this.TouchState.X = (inputState.Touches & (uint)OVRTypes.Touch.X) != 0 ? ButtonState.Pressed : ButtonState.Released;
            this.TouchState.Y = (inputState.Touches & (uint)OVRTypes.Touch.Y) != 0 ? ButtonState.Pressed : ButtonState.Released;
            this.TouchState.LThumb = (inputState.Touches & (uint)OVRTypes.Touch.LThumb) != 0 ? ButtonState.Pressed : ButtonState.Released;
            this.TouchState.LIndexTrigger = (inputState.Touches & (uint)OVRTypes.Touch.LIndexTrigger) != 0 ? ButtonState.Pressed : ButtonState.Released;
            this.TouchState.RIndexPointing = (inputState.Touches & (uint)OVRTypes.Touch.RIndexPointing) != 0 ? ButtonState.Pressed : ButtonState.Released;
            this.TouchState.RThumbUp = (inputState.Touches & (uint)OVRTypes.Touch.RThumbUp) != 0 ? ButtonState.Pressed : ButtonState.Released;
            this.TouchState.LIndexPointing = (inputState.Touches & (uint)OVRTypes.Touch.LIndexPointing) != 0 ? ButtonState.Pressed : ButtonState.Released;
            this.TouchState.LThumbUp = (inputState.Touches & (uint)OVRTypes.Touch.LThumbUp) != 0 ? ButtonState.Pressed : ButtonState.Released;

            // Update axes
            this.LeftIndexTrigger = inputState.IndexTrigger[(int)OVRTypes.HandType.Left];
            this.RightIndexTrigger = inputState.IndexTrigger[(int)OVRTypes.HandType.Right];

            this.LeftHandTrigger = inputState.HandTrigger[(int)OVRTypes.HandType.Left];
            this.RightHandTrigger = inputState.HandTrigger[(int)OVRTypes.HandType.Right];

            this.LeftThumbstick.X = inputState.Thumbstick[(int)OVRTypes.HandType.Left].X;
            this.LeftThumbstick.Y = inputState.Thumbstick[(int)OVRTypes.HandType.Left].Y;
            this.RightThumbstick.X = inputState.Thumbstick[(int)OVRTypes.HandType.Right].X;
            this.RightThumbstick.Y = inputState.Thumbstick[(int)OVRTypes.HandType.Right].Y;
        }

        /// <summary>
        /// Convert Oculus Touch controllers to left generic controllers
        /// </summary>
        /// <param name="genericController">The generic controller to fill</param>
        public void ToLeftGenericController(out VRGenericControllerState genericController)
        {
            genericController = new VRGenericControllerState()
            {
                IsConnected = true,
                Role = VRControllerRole.LeftHand,
                Button1 = this.X,
                Button2 = this.Y,
                Grip = (this.LeftHandTrigger > 0.5).ToButtonState(),
                TriggerButton = (this.LeftIndexTrigger > 0.5).ToButtonState(),
                ThumbStickButton = this.LThumb,
                ThumbStick = this.LeftThumbstick,
                Trigger = this.LeftIndexTrigger,
            };
        }

        /// <summary>
        /// Convert Oculus Touch controllers to right generic controllers
        /// </summary>
        /// <param name="genericController">The generic controller to fill</param>
        public void ToRightGenericController(out VRGenericControllerState genericController)
        {
            genericController = new VRGenericControllerState()
            {
                IsConnected = true,
                Role = VRControllerRole.RightHand,
                Button1 = this.A,
                Button2 = this.B,
                Grip = (this.RightHandTrigger > 0.5).ToButtonState(),
                TriggerButton = (this.RightIndexTrigger > 0.5).ToButtonState(),
                ThumbStickButton = this.RThumb,
                ThumbStick = this.RightThumbstick,
                Trigger = this.RightIndexTrigger
            };
        }

        /// <summary>
        /// Updates generic controller state.
        /// </summary>
        /// <param name="gamePad">The gamepad</param>
        internal void ToGamePadState(out GamePadState gamePad)
        {
            gamePad = default(GamePadState);

            if (this.IsConnected)
            {
                gamePad.IsConnected = true;

                gamePad.ThumbSticks.Left = this.LeftThumbstick;
                gamePad.ThumbSticks.Right = this.RightThumbstick;
                gamePad.DPad.Up = this.LeftThumbstick.Y > 0.5 ? ButtonState.Pressed : ButtonState.Released;
                gamePad.DPad.Down = this.LeftThumbstick.Y < 0.5 ? ButtonState.Pressed : ButtonState.Released;
                gamePad.DPad.Left = this.LeftThumbstick.X < 0.5 ? ButtonState.Pressed : ButtonState.Released;
                gamePad.DPad.Right = this.LeftThumbstick.X > 0.5 ? ButtonState.Pressed : ButtonState.Released;

                gamePad.Triggers.Left = this.LeftIndexTrigger;
                gamePad.Triggers.Right = this.RightIndexTrigger;
                gamePad.Buttons.A = this.A;
                gamePad.Buttons.B = this.B;
                gamePad.Buttons.X = this.X;
                gamePad.Buttons.Y = this.Y;
                gamePad.Buttons.RightShoulder = this.RightHandTrigger > 0.5 ? ButtonState.Pressed : ButtonState.Released;
                gamePad.Buttons.LeftShoulder = this.LeftHandTrigger > 0.5 ? ButtonState.Pressed : ButtonState.Released;
                gamePad.Buttons.Start = this.Start;
                gamePad.Buttons.LeftStick = this.LThumb;
                gamePad.Buttons.RightStick = this.RThumb;
            }
        }
    }
}
