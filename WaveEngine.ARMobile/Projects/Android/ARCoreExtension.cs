// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

using Android.Views;
using Google.AR.Core;
using Java.Nio;
using System;
using WaveEngine.Common.Input;
using WaveEngine.Common.Math;
using ARConfig = Google.AR.Core.Config;

namespace WaveEngine.ARMobile
{
    /// <summary>
    /// ARCore extension class to do types conversions
    /// </summary>
    internal static class ARCoreExtension
    {
        /// <summary>
        /// Converts a <see cref="Google.AR.Core.Pose"/> into a <see cref="Matrix"/>.
        /// </summary>
        /// <param name="pose">The ARCore pose to be converted</param>
        /// <param name="waveMatrix">The converted matrix</param>
        public static void ToWave(this Pose pose, out Matrix waveMatrix)
        {
            var values = new float[16];
            pose.ToMatrix(values, 0);
            values.ToWave(out waveMatrix);
        }

        /// <summary>
        /// Extracts the translation from a <see cref="Google.AR.Core.Pose"/> and store it into a <see cref="Vector3"/>.
        /// </summary>
        /// <param name="pose">The ARCore pose to be converted</param>
        /// <param name="translation">The extracted position</param>
        public static void ToWave(this Pose pose, out Vector3 translation)
        {
            var values = new float[3];
            pose.GetTranslation(values, 0);
            values.ToWave(out translation);
        }

        /// <summary>
        /// Converts a <see cref="float"/> array into a <see cref="Vector3"/>.
        /// </summary>
        /// <param name="values">The float array containing pose values</param>
        /// <param name="waveVector">The converted vector</param>
        public static void ToWave(this float[] values, out Vector3 waveVector)
        {
            waveVector.X = values[0];
            waveVector.Y = values[1];
            waveVector.Z = values[2];
        }

        /// <summary>
        /// Converts a plane polygon from ARCore into a <see cref="Vector3"/> array.
        /// </summary>
        /// <param name="buffer">The float buffer containing 2D vertices of the polygon</param>
        /// <param name="waveVectorArray">The <see cref="Vector3"/> array with the 3D vertices of the polygon</param>
        public static void ToWave(this FloatBuffer buffer, ref Vector3[] waveVectorArray)
        {
            buffer.Rewind();

            var boundaryVertices = buffer.Limit() / 2;

            if (waveVectorArray == null)
            {
                waveVectorArray = new Vector3[boundaryVertices];
            }
            else if (waveVectorArray.Length != boundaryVertices)
            {
                Array.Resize(ref waveVectorArray, boundaryVertices);
            }

            for (int i = 0; i < boundaryVertices; i++)
            {
                waveVectorArray[i].X = buffer.Get();
                waveVectorArray[i].Z = buffer.Get();
            }
        }

        /// <summary>
        /// Converts a <see cref="float"/> array into a <see cref="Matrix"/>.
        /// </summary>
        /// <param name="values">The float array containing pose values</param>
        /// <param name="waveMatrix">The converted matrix</param>
        public static void ToWave(this float[] values, out Matrix waveMatrix)
        {
            waveMatrix.M11 = values[0];
            waveMatrix.M12 = values[1];
            waveMatrix.M13 = values[2];
            waveMatrix.M14 = values[3];

            waveMatrix.M21 = values[4];
            waveMatrix.M22 = values[5];
            waveMatrix.M23 = values[6];
            waveMatrix.M24 = values[7];

            waveMatrix.M31 = values[8];
            waveMatrix.M32 = values[9];
            waveMatrix.M33 = values[10];
            waveMatrix.M34 = values[11];

            waveMatrix.M41 = values[12];
            waveMatrix.M42 = values[13];
            waveMatrix.M43 = values[14];
            waveMatrix.M44 = values[15];
        }

        /// <summary>
        /// Extracts an universal identifier from a <see cref="Google.AR.Core.ITrackable"/>.
        /// </summary>
        /// <param name="trackable">The trackable object</param>
        /// <returns>An universal identifier for the trackable</returns>
        public static Guid GetGuid(this Google.AR.Core.ITrackable trackable)
        {
            byte[] guidData = new byte[16];
            Array.Copy(BitConverter.GetBytes(trackable.GetHashCode()), guidData, sizeof(int));
            return new Guid(guidData);
        }

        public static PlaneAnchorType GetPlaneAnchorType(this Google.AR.Core.Plane plane)
        {
            var planeType = plane.GetType();
            if (planeType == Google.AR.Core.Plane.Type.HorizontalDownwardFacing)
            {
                return PlaneAnchorType.HorizontalDownwardFacing;
            }
            else
            {
                return PlaneAnchorType.HorizontalUpwardFacing;
            }
        }

        /// <summary>
        /// Converts a <see cref="PlaneDetectionType"/> into a <see cref="ARConfig.PlaneFindingMode"/>.
        /// </summary>
        /// <param name="planeType">The plane detection type from ARMobile</param>
        /// <param name="planeFindingMode">The plane finding mode from ARCore</param>
        public static void ToPlaneFindingMode(this PlaneDetectionType planeType, out ARConfig.PlaneFindingMode planeFindingMode)
        {
            if (planeType == PlaneDetectionType.None)
            {
                planeFindingMode = ARConfig.PlaneFindingMode.Disabled;
            }
            else
            {
                planeFindingMode = ARConfig.PlaneFindingMode.Horizontal;
            }
        }

        /// <summary>
        /// Converts a <see cref="DisplayOrientation"/> into a <see cref="SurfaceOrientation"/>.
        /// </summary>
        /// <param name="displayOrientation">The display orientation type from Wave Engine</param>
        /// <param name="surfaceOrientation">The surface orientation type from Android</param>
        public static void ToSurfaceOrientation(this DisplayOrientation displayOrientation, out SurfaceOrientation surfaceOrientation)
        {
            switch (displayOrientation)
            {
                default:
                case DisplayOrientation.Default:
                case DisplayOrientation.Portrait:
                    surfaceOrientation = SurfaceOrientation.Rotation0;
                    break;
                case DisplayOrientation.LandscapeLeft:
                    surfaceOrientation = SurfaceOrientation.Rotation90;
                    break;
                case DisplayOrientation.LandscapeRight:
                    surfaceOrientation = SurfaceOrientation.Rotation270;
                    break;
                case DisplayOrientation.PortraitFlipped:
                    surfaceOrientation = SurfaceOrientation.Rotation180;
                    break;
            }
        }
    }
}
