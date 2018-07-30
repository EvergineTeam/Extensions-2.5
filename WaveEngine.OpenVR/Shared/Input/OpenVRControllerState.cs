// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

using Valve.VR;
using WaveEngine.Common.Input;
using WaveEngine.Common.Math;
using WaveEngine.Common.VR;
using WaveEngine.Components.VR;
using WaveEngine.OpenVR.Helpers;

namespace WaveEngine.OpenVR.Input
{
    public class OpenVRControllerState : OpenVRTrackingReference
    {
        /// <summary>
        /// Gets the controller role
        /// </summary>
        public VRControllerRole Role;

        /// <summary>
        /// Direction Pad buttons on the controller
        /// </summary>
        public GamePadDPad DPad;

        /// <summary>
        /// Grip button on the controller
        /// </summary>
        public ButtonState Grip;

        /// <summary>
        /// Button A on the controller
        /// </summary>
        public ButtonState ApplicationMenu;

        /// <summary>
        /// Menu button on the controller
        /// </summary>
        public ButtonState A;

        /// <summary>
        /// Trackpad button on the controller
        /// </summary>
        public ButtonState TrackpadButton;

        /// <summary>
        /// Trigger button on the controller
        /// </summary>
        public ButtonState TriggerButton;

        /// <summary>
        /// Gets or sets a value indicating whether the trackpad is touched.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the trackpad is touched; otherwise, <c>false</c>.
        /// </value>
        public bool TrackpadTouch;

        /// <summary>
        /// Trigger value, in the range 0.0 to 1.0f.
        /// </summary>
        public float Trigger;

        /// <summary>
        /// Trackpad axis values, in the range -1.0f to 1.0f.
        /// </summary>
        public Vector2 Trackpad;

        internal void Update(int id, VRControllerRole role, ref VRControllerState_t state, ref VRPose pose)
        {
            this.Update(id, pose);

            this.Role = role;

            // Axies
            this.Trackpad.X = state.rAxis0.x;
            this.Trackpad.Y = state.rAxis0.y;
            this.Trigger = state.rAxis1.x;

            // Buttons
            this.TrackpadButton = state.GetButtonPressed(EVRButtonId.k_EButton_SteamVR_Touchpad).ToButtonState();
            this.TriggerButton = state.GetButtonPressed(EVRButtonId.k_EButton_SteamVR_Trigger).ToButtonState();
            this.ApplicationMenu = state.GetButtonPressed(EVRButtonId.k_EButton_ApplicationMenu).ToButtonState();
            this.A = state.GetButtonPressed(EVRButtonId.k_EButton_A).ToButtonState();
            this.Grip = state.GetButtonPressed(EVRButtonId.k_EButton_Grip).ToButtonState();

            this.TrackpadTouch = state.GetButtonTouched(EVRButtonId.k_EButton_SteamVR_Touchpad);

            // DPad
            this.DPad.Up = state.GetButtonPressed(EVRButtonId.k_EButton_DPad_Up).ToButtonState();
            this.DPad.Right = state.GetButtonPressed(EVRButtonId.k_EButton_DPad_Right).ToButtonState();
            this.DPad.Left = state.GetButtonPressed(EVRButtonId.k_EButton_DPad_Left).ToButtonState();
            this.DPad.Down = state.GetButtonPressed(EVRButtonId.k_EButton_DPad_Down).ToButtonState();
        }

        internal void ToGenericController(out VRGenericControllerState genericController)
        {
            genericController = new VRGenericControllerState()
            {
                IsConnected = true,
                Role = this.Role,
                ThumbStick = this.Trackpad,
                Trigger = this.Trigger,
                TriggerButton = this.TriggerButton,
                Grip = this.Grip,
                Menu = this.ApplicationMenu,
                ThumbStickButton = this.TrackpadButton,
                Button1 = this.A,
                Button2 = ButtonState.Released,
                Pose = this.Pose
            };
        }

        internal void ToLeftGamepad(ref GamePadState gamePadState)
        {
            gamePadState.IsConnected = true;
            gamePadState.ThumbSticks.Left = this.Trackpad;
            gamePadState.Triggers.Left = this.Trigger;
            gamePadState.Buttons.X = this.A;
            gamePadState.Buttons.Y = this.ApplicationMenu;
            gamePadState.Buttons.LeftShoulder = this.Grip;
            gamePadState.Buttons.LeftStick = this.TrackpadButton;

            gamePadState.DPad = this.DPad;

            if (this.ApplicationMenu == ButtonState.Pressed)
            {
                gamePadState.Buttons.Start = ButtonState.Pressed;
            }
        }

        internal void ToRightGamepad(ref GamePadState gamePadState)
        {
            gamePadState.IsConnected = true;
            gamePadState.ThumbSticks.Right = this.Trackpad;
            gamePadState.Triggers.Right = this.Trigger;
            gamePadState.Buttons.A = this.A;
            gamePadState.Buttons.B = this.ApplicationMenu;
            gamePadState.Buttons.RightShoulder = this.Grip;
            gamePadState.Buttons.RightStick = this.TrackpadButton;

            if (this.ApplicationMenu == ButtonState.Pressed)
            {
                gamePadState.Buttons.Start = ButtonState.Pressed;
            }
        }
    }
}
