#region File Description
//-----------------------------------------------------------------------------
// HMDDesc
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Runtime.InteropServices;
using WaveEngine.Common.Math;
#endregion

namespace WaveEngine.OculusRift
{
    /// <summary>
    /// HMD description
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct HMDDesc
    {
        /// <summary>
        /// Handle of this HMD.
        /// </summary>
        public IntPtr Handle;

        /// <summary>
        /// The type
        /// </summary>
        public HMDType Type;

        /// <summary>
        /// Name string describing the product: "Oculus Rift DK1", etc.
        /// </summary>
        public IntPtr ProductName;

        /// <summary>
        /// The manufacturer
        /// </summary>
        public IntPtr Manufacturer;

        /// <summary>
        /// HID Vendor and ProductId of the device.
        /// </summary>
        public short VendorId;

        /// <summary>
        /// The product identifier
        /// </summary>
        public short ProductId;

        /// <summary>
        /// Sensor (and display) serial number.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x18)]
        public string SerialNumber;

        /// <summary>
        /// Sensor firmware
        /// </summary>
        public short FirmwareMajor;

        /// <summary>
        /// The firmware minor
        /// </summary>
        public short FirmwareMinor;

        /// <summary>
        /// Fixed camera frustum dimensions, if present
        /// </summary>
        public float CameraFrustumHFovInRadians;

        /// <summary>
        /// The camera frustum v fov in radians
        /// </summary>
        public float CameraFrustumVFovInRadians;

        /// <summary>
        /// The camera frustum near z in meters
        /// </summary>
        public float CameraFrustumNearZInMeters;

        /// <summary>
        /// The camera frustum far z in meters
        /// </summary>
        public float CameraFrustumFarZInMeters;

        /// <summary>
        /// Capability bits described by ovrHmdCaps.
        /// </summary>
        public HMDCapabilities HmdCaps;

        /// <summary>
        /// Capability bits described by ovrTrackingCaps.
        /// </summary>
        public TrackingCapabilities TrackingCaps;

        /// <summary>
        /// Capability bits described by ovrDistortionCaps.
        /// </summary>
        public DistortionCapabilities DistortionCaps;

        /// <summary>
        /// These define the recommended and maximum optical FOVs for the HMD.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.LPStruct)]
        public FovPort[] DefaultEyeFov;

        /// <summary>
        /// The maximum eye fov
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.LPStruct)]
        public FovPort[] MaxEyeFov;

        /// <summary>
        /// The eye render order
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.LPStruct)]
        public EyeType[] EyeRenderOrder;

        /// <summary>
        /// Resolution of the entire HMD screen (for both eyes) in pixels.
        /// </summary>
         public Size2 Resolution;

        /// <summary>
         /// Where monitor window should be on screen or (0,0).
        /// </summary>
        public Point WindowsPos;

        /// <summary>
        /// Display that HMD should present on.
        /// TBD: It may be good to remove this information relying on WidowPos instead.
        /// Ultimately, we may need to come up with a more convenient alternative,
        /// such as a API-specific functions that return adapter, or something that will
        /// work with our monitor driver.
        /// Windows: "\\\\.\\DISPLAY3", etc. Can be used in EnumDisplaySettings/CreateDC.        
        /// </summary>
        public IntPtr DisplayDeviceName;

        /// <summary>
        /// The display identifier
        /// </summary>
        public int DisplayId;
    }
}
