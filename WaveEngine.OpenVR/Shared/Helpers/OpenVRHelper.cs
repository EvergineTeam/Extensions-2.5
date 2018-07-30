// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Valve.VR;
using WaveEngine.Common.Input;
using WaveEngine.Common.Math;
using WaveEngine.Common.VR;
using ValveOpenVR = Valve.VR.OpenVR;

namespace WaveEngine.OpenVR.Helpers
{
    public static class OpenVRHelper
    {
        /// <summary>
        /// Convert an ovrMatrix34f to a Wave Matrix.
        /// </summary>
        /// <param name="ovrMatrix34f">ovrMatrix34f to convert to a Wave Matrix.</param>
        /// <param name="matrix">Wave Matrix, based on the ovrMatrix4f.</param>
        public static void ToMatrix(this HmdMatrix34_t ovrMatrix34f, out Matrix matrix)
        {
            matrix.M11 = ovrMatrix34f.m0;
            matrix.M12 = ovrMatrix34f.m4;
            matrix.M13 = ovrMatrix34f.m8;
            matrix.M14 = 0;

            matrix.M21 = ovrMatrix34f.m1;
            matrix.M22 = ovrMatrix34f.m5;
            matrix.M23 = ovrMatrix34f.m9;
            matrix.M24 = 0;

            matrix.M31 = ovrMatrix34f.m2;
            matrix.M32 = ovrMatrix34f.m6;
            matrix.M33 = ovrMatrix34f.m10;
            matrix.M34 = 0;

            matrix.M41 = ovrMatrix34f.m3;
            matrix.M42 = ovrMatrix34f.m7;
            matrix.M43 = ovrMatrix34f.m11;
            matrix.M44 = 1f;
        }

        /// <summary>
        /// Convert an ovrMatrix44f to a Wave Matrix.
        /// </summary>
        /// <param name="ovrMatrix34f">ovrMatrix44f to convert to a Wave Matrix.</param>
        /// <param name="matrix">Wave Matrix, based on the ovrMatrix4f.</param>
        public static void ToMatrix(this HmdMatrix44_t ovrMatrix34f, out Matrix matrix)
        {
            matrix.M11 = ovrMatrix34f.m0;
            matrix.M12 = ovrMatrix34f.m4;
            matrix.M13 = ovrMatrix34f.m8;
            matrix.M14 = ovrMatrix34f.m12;

            matrix.M21 = ovrMatrix34f.m1;
            matrix.M22 = ovrMatrix34f.m5;
            matrix.M23 = ovrMatrix34f.m9;
            matrix.M24 = ovrMatrix34f.m13;

            matrix.M31 = ovrMatrix34f.m2;
            matrix.M32 = ovrMatrix34f.m6;
            matrix.M33 = ovrMatrix34f.m10;
            matrix.M34 = ovrMatrix34f.m14;

            matrix.M41 = ovrMatrix34f.m3;
            matrix.M42 = ovrMatrix34f.m7;
            matrix.M43 = ovrMatrix34f.m11;
            matrix.M44 = ovrMatrix34f.m15;
        }

        public static void ToMatrix(this TrackedDevicePose_t trackedPose, out Matrix matrix)
        {
            trackedPose.mDeviceToAbsoluteTracking.ToMatrix(out matrix);
        }

        public static void ToVRPose(this Matrix matrix, out VRPose pose)
        {
            pose.Position = matrix.Translation;
            pose.Orientation = matrix.Orientation;
        }

        public static void ToVRPose(this TrackedDevicePose_t trackedPose, out VRPose pose)
        {
            Matrix poseMatrix;
            trackedPose.mDeviceToAbsoluteTracking.ToMatrix(out poseMatrix);

            pose.Position = poseMatrix.Translation;
            pose.Orientation = poseMatrix.Orientation;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ButtonState ToButtonState(this bool b)
        {
            return b ? ButtonState.Pressed : ButtonState.Released;
        }

        public static bool GetButtonPressed(this VRControllerState_t state, EVRButtonId buttonId)
        {
            return (state.ulButtonPressed & (1ul << (int)buttonId)) != 0;
        }

        public static bool GetButtonTouched(this VRControllerState_t state, EVRButtonId buttonId)
        {
            return (state.ulButtonTouched & (1ul << (int)buttonId)) != 0;
        }

        public static void ReportInitError(EVRInitError error)
        {
            switch (error)
            {
                case EVRInitError.None:
                    break;
                case EVRInitError.VendorSpecific_UnableToConnectToOculusRuntime:
                    Debug.WriteLine("OpenVR Initialization Failed! Make sure device is on, Oculus runtime is installed, and OVRService_*.exe is running.");
                    break;
                case EVRInitError.Init_VRClientDLLNotFound:
                    Debug.WriteLine("OpenVR drivers not found! They can be installed via Steam under Library > Tools. Visit http://steampowered.com to install Steam.");
                    break;
                case EVRInitError.Driver_RuntimeOutOfDate:
                    Debug.WriteLine("OpenVR Initialization Failed! Make sure device's runtime is up to date.");
                    break;
                default:
                    Debug.WriteLine(ValveOpenVR.GetStringForHmdError(error));
                    break;
            }
        }

        public static void ReportCompositeError(EVRCompositorError error)
        {
            switch (error)
            {
                case EVRCompositorError.None:
                    break;
                default:
                    Debug.WriteLine("OpenVR Render error! {0}", error);
                    break;
            }
        }
    }
}
