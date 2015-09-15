#region File Description
//-----------------------------------------------------------------------------
// SensorData
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Runtime.InteropServices;
using WaveEngine.Common.Math;
#endregion

namespace WaveEngine.OculusRift
{
    /// <summary>
    /// All sensor info.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SensorData
    {
        /// <summary>
        /// Acceleration reading in in m/s^2.
        /// </summary>
        public Vector3 Accelerometer;   

        /// <summary>
        /// Rotation rate in rad/s.
        /// </summary>
        public Vector3 Gyro;           
 
        /// <summary>
        /// Magnetic field in Gauss.
        /// </summary>
        public Vector3 Magnetometer;   

        /// <summary>
        /// Temperature of sensor in degrees Celsius.
        /// </summary>
        public float Temperature;       

        /// <summary>
        /// Time when reported IMU reading took place, in seconds.
        /// </summary>
        public float TimeInSeconds;     
    }
}
