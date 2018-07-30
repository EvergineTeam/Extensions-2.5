// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Usings Statements
using Leap;
using WaveEngine.Common.Math;
#endregion

namespace WaveEngine.LeapMotion
{
    /// <summary>
    /// Utils of LeapMotion integration.
    /// </summary>
    public static class LeapExtensions
    {
        /// <summary>
        /// Head mounted device mode
        /// </summary>
        internal static bool HWDModeEnable;

        /// <summary>
        /// Extension method to convert from LeapMotion vector struct to Wave vector.
        /// </summary>
        /// <param name="vector">The Leap motion vector.</param>
        /// <returns>The WaveEngine vector.</returns>
        public static Vector3 ToPositionVector3(this Vector vector)
        {
            Vector3 result = Vector3.Zero;

            if (HWDModeEnable)
            {
                result = new Vector3(-vector.x / 1000f, -vector.z / 1000f, -vector.y / 1000f);
            }
            else
            {
                result = new Vector3(-vector.x / 1000f, vector.y / 1000f, -vector.z / 1000f);
            }

            return result;
        }

        /// <summary>
        /// Extension method to convert from LeapMotion vector struct to Wave vector.
        /// </summary>
        /// <param name="vector">The Leap motion vector.</param>
        /// <returns>The WaveEngine vector.</returns>
        public static Vector3 ToNormalVector3(this Vector vector)
        {
            Vector3 result = Vector3.Zero;

            if (HWDModeEnable)
            {
                result = new Vector3(-vector.x, -vector.z, -vector.y);
            }
            else
            {
                result = new Vector3(-vector.x, vector.y, -vector.z);
            }

            return result;
        }

        /// <summary>
        /// Extension method to convert from LeapMotion matrix struct to Wave matrix.
        /// </summary>
        /// <param name="matrix">The Leap motion matrix</param>
        /// <returns>The WaveEngine matrix.</returns>
        public static WaveEngine.Common.Math.Matrix ToMatrix(this Leap.Matrix matrix)
        {
            WaveEngine.Common.Math.Matrix result;
            var array4x4 = matrix.ToArray4x4();
            result.M11 = array4x4[0];
            result.M12 = array4x4[1];
            result.M13 = array4x4[2];
            result.M14 = array4x4[3];

            result.M21 = array4x4[4];
            result.M22 = array4x4[5];
            result.M23 = array4x4[6];
            result.M24 = array4x4[7];

            result.M31 = array4x4[8];
            result.M32 = array4x4[9];
            result.M33 = array4x4[10];
            result.M34 = array4x4[11];

            result.M41 = array4x4[12];
            result.M42 = array4x4[13];
            result.M43 = array4x4[14];
            result.M44 = array4x4[15];

            return result;
        }
    }
}
