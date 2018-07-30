// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System.Numerics;
using WaveEngine.Common.Math;
#endregion

namespace WaveEngine.MixedReality.Utilities
{
    /// <summary>
    /// Math Utilities.
    /// </summary>
    public static class MathUtils
    {
        /// <summary>
        /// Convert a system.numerics matrix to waveengine matrix.
        /// </summary>
        /// <param name="numerics">The source matrix.</param>
        /// <param name="result">The waveengine result matrix.</param>
        public static void ToWave(this Matrix4x4 numerics, out Matrix result)
        {
            result.M11 = numerics.M11;
            result.M12 = numerics.M12;
            result.M13 = numerics.M13;
            result.M14 = numerics.M14;
            result.M21 = numerics.M21;
            result.M22 = numerics.M22;
            result.M23 = numerics.M23;
            result.M24 = numerics.M24;
            result.M31 = numerics.M31;
            result.M32 = numerics.M32;
            result.M33 = numerics.M33;
            result.M34 = numerics.M34;
            result.M41 = numerics.M41;
            result.M42 = numerics.M42;
            result.M43 = numerics.M43;
            result.M44 = numerics.M44;
        }

        /// <summary>
        /// Convert a waveengine vector3 to system.numerics vector3
        /// </summary>
        /// <param name="wave">Wave vector</param>
        /// <param name="numerics">System numerics vector</param>
        public static void ToSystemNumerics(this Common.Math.Vector3 wave, out System.Numerics.Vector3 numerics)
        {
            numerics.X = wave.X;
            numerics.Y = wave.Y;
            numerics.Z = wave.Z;
        }

        /// <summary>
        /// Convert a system.numerics vector3 to waveengine vector3
        /// </summary>
        /// <param name="wave">Wave vector</param>
        /// <returns>System numerics vector</returns>
        public static System.Numerics.Vector3 ToSystemNumerics(this Common.Math.Vector3 wave)
        {
            return new System.Numerics.Vector3(wave.X, wave.Y, wave.Z);
        }

        /// <summary>
        /// Convert a waveengine vector3 to system.numerics vector3
        /// </summary>
        /// <param name="numerics">Wave vector</param>
        /// <returns>System numerics vector</returns>
        public static Common.Math.Vector3 ToWave(this System.Numerics.Vector3 numerics)
        {
            return new Common.Math.Vector3(numerics.X, numerics.Y, numerics.Z);
        }

        /// <summary>
        /// Convert a system.numerics vector3 to waveengine vector3
        /// </summary>
        /// <param name="numerics">Wave vector</param>
        /// <param name="result">The result</param>
        public static void ToWave(this System.Numerics.Vector3 numerics, out Common.Math.Vector3 result)
        {
            result.X = numerics.X;
            result.Y = numerics.Y;
            result.Z = numerics.Z;
        }

        /// <summary>
        /// Convert a system.numerics quaternion to waveengine quaternion
        /// </summary>
        /// <param name="numerics">Numeric quaternion</param>
        /// <param name="result">Wave quaternion</param>
        public static void ToWave(this System.Numerics.Quaternion numerics, out Common.Math.Quaternion result)
        {
            result.X = numerics.X;
            result.Y = numerics.Y;
            result.Z = numerics.Z;
            result.W = numerics.W;
        }
    }
}
