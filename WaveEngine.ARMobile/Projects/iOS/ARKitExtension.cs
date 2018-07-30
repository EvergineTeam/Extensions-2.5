// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

using ARKit;
using CoreGraphics;
using Foundation;
using System;
using WaveEngine.Common.Input;
using WaveEngine.Common.Math;

namespace WaveEngine.ARMobile
{
    /// <summary>
    /// ARKit extension class to do types conversions
    /// </summary>
    internal static class ARKitExtension
    {
        /// <summary>
        /// Converts an <see cref="ARMobileWorldAlignment"/> into a <see cref="ARWorldAlignment"/>.
        /// </summary>
        /// <param name="worldAlignment">The world alignment value to be converted</param>
        /// <returns>Returns an <see cref="ARWorldAlignment"/></returns>
        internal static ARWorldAlignment ToARKit(this ARMobileWorldAlignment worldAlignment)
        {
            return (ARWorldAlignment)worldAlignment;
        }

        /// <summary>
        /// Converts an <see cref="PlaneDetectionType"/> into a <see cref="ARPlaneDetection"/>.
        /// </summary>
        /// <param name="planeDetection">The plane detection type value to be converted</param>
        /// <returns>Returns an <see cref="ARPlaneDetection"/></returns>
        internal static ARPlaneDetection ToARKit(this PlaneDetectionType planeDetection)
        {
            return (ARPlaneDetection)planeDetection;
        }

        /// <summary>
        /// Converts an <see cref="ARPlaneDetection"/> into a <see cref="PlaneDetectionType"/>.
        /// </summary>
        /// <param name="planeDetection">The plane detection type value to be converted</param>
        /// <returns>Returns an <see cref="PlaneDetectionType"/></returns>
        internal static PlaneDetectionType ToWave(this ARPlaneDetection planeDetection)
        {
            return (PlaneDetectionType)planeDetection;
        }

        /// <summary>
        /// Converts an <see cref="NSUuid"/> into a <see cref="Guid"/>.
        /// </summary>
        /// <param name="id">The universal unique idenfier to be converted</param>
        /// <returns>Returns an <see cref="Guid"/></returns>
        internal static Guid ToWave(this NSUuid id)
        {
            return new Guid(id.GetBytes());
        }

        /// <summary>
        /// Converts an <see cref="ARMobileHitType"/> into a <see cref="ARHitTestResultType"/>.
        /// </summary>
        /// <param name="hitType">The hit type value to be converted</param>
        /// <returns>Returns an <see cref="ARHitTestResultType"/></returns>
        internal static ARHitTestResultType ToARKit(this ARMobileHitType hitType)
        {
            return (ARHitTestResultType)hitType;
        }

        /// <summary>
        /// Converts an <see cref="ARHitTestResultType"/> into a <see cref="ARMobileHitType"/>.
        /// </summary>
        /// <param name="hitTestResult">The hit test result value to be converted</param>
        /// <returns>Returns an <see cref="ARMobileHitType"/></returns>
        internal static ARMobileHitType ToWave(this ARHitTestResultType hitTestResult)
        {
            return (ARMobileHitType)hitTestResult;
        }

        /// <summary>
        /// Converts an <see cref="ARMobileStartOptions"/> into a <see cref="ARSessionRunOptions"/>.
        /// </summary>
        /// <param name="startOptions">The start options value to be converted</param>
        /// <returns>Returns an <see cref="ARSessionRunOptions"/></returns>
        internal static ARSessionRunOptions ToARKit(this ARMobileStartOptions startOptions)
        {
            return (ARSessionRunOptions)startOptions;
        }

        /// <summary>
        /// Converts an <see cref="ARPlaneAnchorAlignment"/> into a <see cref="PlaneAnchorType"/>.
        /// </summary>
        /// <param name="alignment">The plane alignment value to be converted</param>
        /// <returns>Returns an <see cref="PlaneAnchorType"/></returns>
        internal static PlaneAnchorType ToWave(this ARPlaneAnchorAlignment alignment)
        {
            switch (alignment)
            {
                default:
                case ARPlaneAnchorAlignment.Horizontal:
                    return PlaneAnchorType.HorizontalUpwardFacing;
                case ARPlaneAnchorAlignment.Vertical:
                    return PlaneAnchorType.Vertical;
            }
        }

        /// <summary>
        /// Converts an <see cref="DisplayOrientation"/> into a <see cref="UIKit.UIInterfaceOrientation"/>.
        /// </summary>
        /// <param name="orientation">The orientation value to be converted</param>
        /// <returns>Returns an <see cref="UIKit.UIInterfaceOrientation"/></returns>
        internal static UIKit.UIInterfaceOrientation ToUIKit(this DisplayOrientation orientation)
        {
            switch (orientation)
            {
                case DisplayOrientation.LandscapeLeft:
                    return UIKit.UIInterfaceOrientation.LandscapeLeft;
                case DisplayOrientation.PortraitFlipped:
                case DisplayOrientation.Portrait:
                    return UIKit.UIInterfaceOrientation.Portrait;
                case DisplayOrientation.LandscapeRight:
                case DisplayOrientation.Default:
                default:
                    return UIKit.UIInterfaceOrientation.LandscapeRight;
            }
        }

        /// <summary>
        /// Converts an <see cref="CGPoint"/> into a <see cref="Vector2"/>.
        /// </summary>
        /// <param name="cGPoint">The point to be converted</param>
        /// <param name="waveVector">The converted vector</param>
        internal static void ToWave(this CGPoint cGPoint, ref Vector2 waveVector)
        {
            waveVector.X = (float)cGPoint.X;
            waveVector.Y = (float)cGPoint.Y;
        }

        /// <summary>
        /// Converts an <see cref="OpenTK.NVector3"/> array into a <see cref="Vector3"/> array.
        /// </summary>
        /// <param name="glVectorArray">The vector array to be converted</param>
        /// <param name="waveVectorArray">The converted vector array</param>
        internal static void ToWave(this OpenTK.NVector3[] glVectorArray, ref Vector3[] waveVectorArray)
        {
            if (waveVectorArray == null)
            {
                waveVectorArray = new Vector3[glVectorArray.Length];
            }
            else if (glVectorArray.Length != waveVectorArray.Length)
            {
                Array.Resize(ref waveVectorArray, glVectorArray.Length);
            }

            for (int i = 0; i < glVectorArray.Length; i++)
            {
                glVectorArray[i].ToWave(out waveVectorArray[i]);
            }
        }

        /// <summary>
        /// Converts an <see cref="OpenTK.NVector3"/> into a <see cref="Vector3"/>.
        /// </summary>
        /// <param name="glVector3">The vector to be converted</param>
        /// <param name="waveVector">The converted vector</param>
        internal static void ToWave(this OpenTK.NVector3 glVector3, out Vector3 waveVector)
        {
            waveVector.X = glVector3.X;
            waveVector.Y = glVector3.Y;
            waveVector.Z = glVector3.Z;
        }

        /// <summary>
        /// Converts an <see cref="OpenTK.NMatrix4"/> into a <see cref="Matrix"/>.
        /// </summary>
        /// <param name="glMatrix">The 4x4 matrix to be converted</param>
        /// <param name="waveMatrix">The converted matrix</param>
        internal static void ToWave(this OpenTK.NMatrix4 glMatrix, out Matrix waveMatrix)
        {
            waveMatrix.M11 = glMatrix.M11;
            waveMatrix.M12 = glMatrix.M21;
            waveMatrix.M13 = glMatrix.M31;
            waveMatrix.M14 = glMatrix.M41;
            waveMatrix.M21 = glMatrix.M12;
            waveMatrix.M22 = glMatrix.M22;
            waveMatrix.M23 = glMatrix.M32;
            waveMatrix.M24 = glMatrix.M42;
            waveMatrix.M31 = glMatrix.M13;
            waveMatrix.M32 = glMatrix.M23;
            waveMatrix.M33 = glMatrix.M33;
            waveMatrix.M34 = glMatrix.M43;
            waveMatrix.M41 = glMatrix.M14;
            waveMatrix.M42 = glMatrix.M24;
            waveMatrix.M43 = glMatrix.M34;
            waveMatrix.M44 = glMatrix.M44;
        }

        /// <summary>
        /// Converts an <see cref="OpenTK.NMatrix3"/> into a <see cref="Matrix"/>.
        /// </summary>
        /// <param name="glMatrix">The 3x3 matrix to be converted</param>
        /// <param name="waveMatrix">The converted matrix</param>
        internal static void ToWave(this OpenTK.NMatrix3 glMatrix, out Matrix waveMatrix)
        {
            waveMatrix.M11 = glMatrix.R0C0;
            waveMatrix.M12 = glMatrix.R1C0;
            waveMatrix.M13 = glMatrix.R2C0;
            waveMatrix.M14 = 0;
            waveMatrix.M21 = glMatrix.R0C1;
            waveMatrix.M22 = glMatrix.R1C1;
            waveMatrix.M23 = glMatrix.R2C1;
            waveMatrix.M24 = 0;
            waveMatrix.M31 = glMatrix.R0C2;
            waveMatrix.M32 = glMatrix.R1C2;
            waveMatrix.M33 = glMatrix.R2C2;
            waveMatrix.M34 = 0;
            waveMatrix.M41 = 0;
            waveMatrix.M42 = 0;
            waveMatrix.M43 = 0;
            waveMatrix.M44 = 0;
        }
    }
}
