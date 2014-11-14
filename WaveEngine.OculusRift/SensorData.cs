using System.Runtime.InteropServices;
using WaveEngine.Common.Math;

namespace WaveEngine.OculusRift
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SensorData
    {
        public Vector3 Accelerometer;   // Acceleration reading in in m/s^2.
        public Vector3 Gyro;    // Rotation rate in rad/s.
        public Vector3 Magnetometer;    // Magnetic field in Gauss.
        public float Temperature;   // Temperature of sensor in degrees Celsius.
        public float TimeInSeconds; // Time when reported IMU reading took place, in seconds.
    }
}
