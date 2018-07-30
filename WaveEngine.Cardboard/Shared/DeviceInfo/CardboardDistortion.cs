// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers;
using WaveEngine.Framework.Services;

#endregion

namespace WaveEngine.Cardboard
{
    /// <summary>
    /// The cardboard device
    /// </summary>
    public class CardboardDistortion
    {
        /// <summary>
        /// The coefficients
        /// </summary>
        private float[] coefficients;

        /// <summary>
        /// Initializes a new instance of the <see cref="CardboardDistortion" /> class.
        /// </summary>
        /// <param name="coefficients">The coefficients</param>
        public CardboardDistortion(float[] coefficients)
        {
            this.coefficients = coefficients;
        }

        /// <summary>
        /// Calculates the inverse distortion for a radius.
        /// </summary>
        /// <param name="radius">Distorted radius from the lens center in tan-angle units</param>
        /// <returns>The undistorted radius in tan-angle units</returns>
        public float DistortInverse(float radius)
        {
            // Secant method.
            float r0 = 0;
            float r1 = 1;
            float dr0 = radius - this.Distort(r0);
            while (Math.Abs(r1 - r0) > 0.0001)
            {
                float dr1 = radius - this.Distort(r1);
                float r2 = r1 - (dr1 * ((r1 - r0) / (dr1 - dr0)));
                r0 = r1;
                r1 = r2;
                dr0 = dr1;
            }

            return r1;
        }

        /// <summary>
        /// Distorts a radius by its distortion factor from the center of the lenses
        /// </summary>
        /// <param name="radius">Radius from the lens center in tan-angle units</param>
        /// <returns>The distorted radius in tan-angle units</returns>
        public float Distort(float radius)
        {
            float r2 = radius * radius;
            float ret = 0;
            for (var i = 0; i < this.coefficients.Length; i++)
            {
                ret = r2 * (ret + this.coefficients[i]);
            }

            return (ret + 1) * radius;
        }
    }
}
