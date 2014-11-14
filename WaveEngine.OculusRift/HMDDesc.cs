using System;
using System.Runtime.InteropServices;
using WaveEngine.Common.Math;

namespace WaveEngine.OculusRift
{
    [StructLayout(LayoutKind.Sequential)]
    public struct HMDDesc
    {
        public IntPtr Handle;    // Handle of this HMD.
        public HMDType Type;

        // Name string describing the product: "Oculus Rift DK1", etc.
        public IntPtr ProductName;
        public IntPtr Manufacturer;

        // HID Vendor and ProductId of the device.
        public short VendorId;
        public short ProductId;

        // Sensor (and display) serial number.
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x18)]
        public string SerialNumber;

        // Sensor firmware
        public short FirmwareMajor;
        public short FirmwareMinor;

        // Fixed camera frustum dimensions, if present
        public float CameraFrustumHFovInRadians;
        public float CameraFrustumVFovInRadians;
        public float CameraFrustumNearZInMeters;
        public float CameraFrustumFarZInMeters;

        // Capability bits described by ovrHmdCaps.
        public HMDCapabilities HmdCaps;

        // Capability bits described by ovrTrackingCaps.
        public TrackingCapabilities TrackingCaps;

        // Capability bits described by ovrDistortionCaps.
        public DistortionCapabilities DistortionCaps;

        // These define the recommended and maximum optical FOVs for the HMD.
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.LPStruct)]
        public FovPort[] DefaultEyeFov;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.LPStruct)]
        public FovPort[] MaxEyeFov;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.LPStruct)]
        public EyeType[] EyeRenderOrder;

        // Resolution of the entire HMD screen (for both eyes) in pixels.
        public Size2 Resolution;

        // Where monitor window should be on screen or (0,0).
        public Point WindowsPos;

        // Display that HMD should present on.
        // TBD: It may be good to remove this information relying on WidowPos instead.
        // Ultimately, we may need to come up with a more convenient alternative,
        // such as a API-specific functions that return adapter, or something that will
        // work with our monitor driver.

        // Windows: "\\\\.\\DISPLAY3", etc. Can be used in EnumDisplaySettings/CreateDC.
        public IntPtr DisplayDeviceName;

        // MacOS
        public int DisplayId;
    }
}
