#region File Description
//-----------------------------------------------------------------------------
// HMD
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
#endregion

namespace WaveEngine.OculusRift
{
    /// <summary>
    /// HMD represent the mainly class.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is Ok here.", Scope = "type")]
    public class HMD : IDisposable
    {
        #region DllImport
        [SuppressUnmanagedCodeSecurity, DllImport("libovr.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe bool ovrHmd_AttachToWindow(IntPtr hmd, IntPtr hwnd, Rect* destMirrorRect, Rectangle* sourceRenderTargetRect);

        [SuppressUnmanagedCodeSecurity, DllImport("libovr.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern FrameTiming_Raw ovrHmd_BeginFrame(IntPtr hmd, uint frameIndex);

        [SuppressUnmanagedCodeSecurity, DllImport("libovr.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern FrameTiming_Raw ovrHmd_BeginFrameTiming(IntPtr hmd, uint frameIndex);

        [SuppressUnmanagedCodeSecurity, DllImport("libovr.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe bool ovrHmd_ConfigureRendering(IntPtr hmd, D3D11ConfigData* apiConfig, DistortionCapabilities distortionCaps, FovPort[] eyeFovIn, EyeRenderDesc* eyeRenderDescOut);

        [SuppressUnmanagedCodeSecurity, DllImport("libovr.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool ovrHmd_ConfigureTracking(IntPtr hmd, TrackingCapabilities supportedCaps, TrackingCapabilities requiredCaps);

        [SuppressUnmanagedCodeSecurity, DllImport("libovr.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void ovrHmd_Destroy(IntPtr pHmd);

        [SuppressUnmanagedCodeSecurity, DllImport("libovr.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool ovrHmd_DismissHSWDisplay(IntPtr hmd);

        [SuppressUnmanagedCodeSecurity, DllImport("libovr.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool ovrhmd_EnableHSWDisplaySDKRender(IntPtr hmd, bool enabled);

        [SuppressUnmanagedCodeSecurity, DllImport("libovr.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe void ovrHmd_EndFrame(IntPtr hmd, PoseF* renderPose, D3D11TextureData* eyeTexture);

        [SuppressUnmanagedCodeSecurity, DllImport("libovr.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void ovrHmd_EndFrameTiming(IntPtr hmd);

        [SuppressUnmanagedCodeSecurity, DllImport("libovr.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void ovrHmd_GetDesc(IntPtr pHmd, out HMDDesc pDesc);

        [SuppressUnmanagedCodeSecurity, DllImport("libovr.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern HMDCapabilities ovrHmd_GetEnabledCaps(IntPtr hmd);

        [SuppressUnmanagedCodeSecurity, DllImport("libovr.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern PoseF ovrHmd_GetEyePose(IntPtr hmd, EyeType eye);

        [SuppressUnmanagedCodeSecurity, DllImport("libovr.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern Size2 ovrHmd_GetFovTextureSize(IntPtr hmd, EyeType eye, FovPort fov, float pixelsPerDisplayPixel);

        [SuppressUnmanagedCodeSecurity, DllImport("libovr.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe void ovrHmd_GetHSWDisplayState(IntPtr hmd, HSWDisplayState* state);

        [SuppressUnmanagedCodeSecurity, DllImport("libovr.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern EyeRenderDesc ovrHmd_GetRenderDesc(IntPtr hmd, EyeType eye, FovPort fov);

        [SuppressUnmanagedCodeSecurity, DllImport("libovr.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern TrackingState ovrHmd_GetTrackingState(IntPtr hmd, double absTime);

        [SuppressUnmanagedCodeSecurity, DllImport("libovr.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void ovrHmd_RecenterPose(IntPtr hmd);

        [SuppressUnmanagedCodeSecurity, DllImport("libovr.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void ovrHmd_SetEnabledCaps(IntPtr hmd, HMDCapabilities hmdCaps);

        [SuppressUnmanagedCodeSecurity, DllImport("libovr.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void ovrHmd_StopSensor(IntPtr hmd);
        #endregion

        /// <summary>
        /// The desc
        /// </summary>
        private HMDDesc desc;

        /// <summary>
        /// The device name
        /// </summary>
        private string deviceName;

        /// <summary>
        /// The HMD
        /// </summary>
        private IntPtr hmd;

        /// <summary>
        /// The manufacturer
        /// </summary>
        private string manufacturer;

        /// <summary>
        /// The product name
        /// </summary>
        private string productName;

        /// <summary>
        /// The profile
        /// </summary>
        private Profile profile;

        #region Properties

        /// <summary>
        /// Gets the default eye fov.
        /// </summary>
        /// <value>
        /// The default eye fov.
        /// </value>
        public FovPort[] DefaultEyeFov
        {
            get
            {
                return this.desc.DefaultEyeFov;
            }
        }

        /// <summary>
        /// Gets the name of the device.
        /// </summary>
        /// <value>
        /// The name of the device.
        /// </value>
        public string DeviceName
        {
            get
            {
                if (this.deviceName == null)
                {
                    this.deviceName = Marshal.PtrToStringAnsi(this.desc.DisplayDeviceName) ?? string.Empty;
                }

                return this.deviceName;
            }
        }

        /// <summary>
        /// Gets the display identifier.
        /// </summary>
        /// <value>
        /// The display identifier.
        /// </value>
        public long DisplayId
        {
            get
            {
                return (long)this.desc.DisplayId;
            }
        }

        /// <summary>
        /// Gets the distortion caps.
        /// </summary>
        /// <value>
        /// The distortion caps.
        /// </value>
        public DistortionCapabilities DistortionCaps
        {
            get
            {
                return this.desc.DistortionCaps;
            }
        }

        /// <summary>
        /// Gets or sets the enabled caps.
        /// </summary>
        /// <value>
        /// The enabled caps.
        /// </value>
        public HMDCapabilities EnabledCaps
        {
            get
            {
                return ovrHmd_GetEnabledCaps(this.hmd);
            }

            set
            {
                ovrHmd_SetEnabledCaps(this.hmd, value);
            }
        }

        /// <summary>
        /// Gets the eye render order.
        /// </summary>
        /// <value>
        /// The eye render order.
        /// </value>
        public EyeType[] EyeRenderOrder
        {
            get
            {
                return this.desc.EyeRenderOrder;
            }
        }

        /// <summary>
        /// Gets the HMD capabilities.
        /// </summary>
        /// <value>
        /// The HMD caps.
        /// </value>
        public HMDCapabilities HMDCaps
        {
            get
            {
                return this.desc.HmdCaps;
            }
        }

        /// <summary>
        /// Gets the display state of the HSW.
        /// </summary>
        /// <value>
        /// The display state of the HSW.
        /// </value>
        public unsafe HSWDisplayState HSWDisplayState
        {
            get
            {
                HSWDisplayState state = new HSWDisplayState();
                ovrHmd_GetHSWDisplayState(this.hmd, &state);
                return state;
            }
        }

        /// <summary>
        /// Gets the manufacturer.
        /// </summary>
        /// <value>
        /// The manufacturer.
        /// </value>
        public string Manufacturer
        {
            get
            {
                return this.manufacturer ?? (this.manufacturer = Marshal.PtrToStringAnsi(this.desc.Manufacturer));
            }
        }

        /// <summary>
        /// Gets the maximum eye fov.
        /// </summary>
        /// <value>
        /// The maximum eye fov.
        /// </value>
        public FovPort[] MaxEyeFov
        {
            get
            {
                return this.desc.MaxEyeFov;
            }
        }

        /// <summary>
        /// Gets the product identifier.
        /// </summary>
        /// <value>
        /// The product identifier.
        /// </value>
        public int ProductId
        {
            get
            {
                return this.desc.ProductId;
            }
        }

        /// <summary>
        /// Gets the name of the product.
        /// </summary>
        /// <value>
        /// The name of the product.
        /// </value>
        public string ProductName
        {
            get
            {
                return this.productName ?? (this.productName = Marshal.PtrToStringAnsi(this.desc.ProductName));
            }
        }

        /// <summary>
        /// Gets the profile.
        /// </summary>
        /// <value>
        /// The profile.
        /// </value>
        public Profile Profile
        {
            get
            {
                return this.profile;
            }
        }

        /// <summary>
        /// Gets the resolution.
        /// </summary>
        /// <value>
        /// The resolution.
        /// </value>
        public Size2 Resolution
        {
            get
            {
                return this.desc.Resolution;
            }
        }

        /// <summary>
        /// Gets the serial number.
        /// </summary>
        /// <value>
        /// The serial number.
        /// </value>
        public string SerialNumber
        {
            get
            {
                return this.desc.SerialNumber;
            }
        }

        /// <summary>
        /// Gets the tracking caps.
        /// </summary>
        /// <value>
        /// The tracking caps.
        /// </value>
        public TrackingCapabilities TrackingCaps
        {
            get
            {
                return this.desc.TrackingCaps;
            }
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public HMDType Type
        {
            get
            {
                return this.desc.Type;
            }
        }

        /// <summary>
        /// Gets the vendor identifier.
        /// </summary>
        /// <value>
        /// The vendor identifier.
        /// </value>
        public int VendorId
        {
            get
            {
                return this.desc.VendorId;
            }
        }

        /// <summary>
        /// Gets the window position.
        /// </summary>
        /// <value>
        /// The window position.
        /// </value>
        public Point WindowPos
        {
            get
            {
                return this.desc.WindowsPos;
            }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="HMD"/> class.
        /// </summary>
        /// <param name="hmd">The HMD.</param>
        internal HMD(IntPtr hmd)
        {
            this.hmd = hmd;
            this.desc = (HMDDesc)Marshal.PtrToStructure(hmd, typeof(HMDDesc));
            this.profile = new Profile(hmd);
        }

        /// <summary>
        /// Attaches to window.
        /// </summary>
        /// <param name="hwnd">The HWND.</param>
        /// <returns>True if everything was ok!.</returns>
        public unsafe bool AttachToWindow(IntPtr hwnd)
        {
            return ovrHmd_AttachToWindow(this.hmd, hwnd, null, null);
        }

        /// <summary>
        /// Begins the frame.
        /// </summary>
        /// <param name="frameIndex">Index of the frame.</param>
        /// <returns>Frame timing.</returns>
        public FrameTiming BeginFrame(uint frameIndex)
        {
            FrameTiming_Raw raw = ovrHmd_BeginFrame(this.hmd, frameIndex);
            return new FrameTiming(raw);
        }

        /// <summary>
        /// Begins the frame timing.
        /// </summary>
        /// <param name="frameIndex">Index of the frame.</param>
        /// <returns>Frame timing.</returns>
        public FrameTiming BeginFrameTiming(uint frameIndex)
        {
            FrameTiming_Raw raw = ovrHmd_BeginFrameTiming(this.hmd, frameIndex);
            return new FrameTiming(raw);
        }

        /// <summary>
        /// Configures the rendering.
        /// </summary>
        /// <param name="apiConfig">The API configuration.</param>
        /// <param name="distortionCaps">The distortion caps.</param>
        /// <param name="eyeFovIn">The eye fov in.</param>
        /// <param name="eyeRenderDescOut">The eye render desc out.</param>
        /// <returns>True if everything ok!.</returns>
        public unsafe bool ConfigureRendering(D3D11ConfigData apiConfig, DistortionCapabilities distortionCaps, FovPort[] eyeFovIn, EyeRenderDesc[] eyeRenderDescOut)
        {
            fixed (EyeRenderDesc* descRef = eyeRenderDescOut)
            {
                return ovrHmd_ConfigureRendering(this.hmd, &apiConfig, distortionCaps, eyeFovIn, descRef);
            }
        }

        /// <summary>
        /// Configures the tracking.
        /// </summary>
        /// <param name="supportedCaps">The supported caps.</param>
        /// <param name="requiredCaps">The required caps.</param>
        /// <returns>True if everything ok.</returns>
        public bool ConfigureTracking(TrackingCapabilities supportedCaps, TrackingCapabilities requiredCaps)
        {
            return ovrHmd_ConfigureTracking(this.hmd, supportedCaps, requiredCaps);
        }

        /// <summary>
        /// Dismisses the HSW display.
        /// </summary>
        /// <returns>True if everything ok.</returns>
        public bool DismissHSWDisplay()
        {
            return ovrHmd_DismissHSWDisplay(this.hmd);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.hmd != IntPtr.Zero)
            {
                ovrHmd_Destroy(this.hmd);
                this.hmd = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Enables the HSW display SDK render.
        /// </summary>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        public void EnableHSWDisplaySDKRender(bool enabled)
        {
            ovrhmd_EnableHSWDisplaySDKRender(this.hmd, enabled);
        }

        /// <summary>
        /// Ends the frame.
        /// </summary>
        /// <param name="renderPose">The render pose.</param>
        /// <param name="eyeTexture">The eye texture.</param>
        public unsafe void EndFrame(PoseF[] renderPose, D3D11TextureData[] eyeTexture)
        {
            fixed (PoseF* efRef = renderPose)
            {
                fixed (D3D11TextureData* dataRef = eyeTexture)
                {
                    ovrHmd_EndFrame(this.hmd, efRef, dataRef);
                }
            }
        }

        /// <summary>
        /// Ends the frame timing.
        /// </summary>
        public void EndFrameTiming()
        {
            ovrHmd_EndFrameTiming(this.hmd);
        }

        /// <summary>
        /// Gets the default size of the render target.
        /// </summary>
        /// <param name="pixelsPerDisplayPixel">The pixels per display pixel.</param>
        /// <returns>The size of the rendertarget.</returns>
        public Size2 GetDefaultRenderTargetSize(float pixelsPerDisplayPixel = 1f)
        {
            Size2 size = this.GetFovTextureSize(EyeType.Left, this.desc.DefaultEyeFov[0], pixelsPerDisplayPixel);
            Size2 size2 = this.GetFovTextureSize(EyeType.Right, this.desc.DefaultEyeFov[1], pixelsPerDisplayPixel);
            return new Size2(size.Width + size2.Width, Math.Max(size.Height, size2.Height));
        }

        /// <summary>
        /// Gets the eye pose.
        /// </summary>
        /// <param name="eye">The eye.</param>
        /// <returns>The Eye pose.</returns>
        public PoseF GetEyePose(EyeType eye)
        {
            return ovrHmd_GetEyePose(this.hmd, eye);
        }

        /// <summary>
        /// Gets the size of the fov texture.
        /// </summary>
        /// <param name="eye">The eye.</param>
        /// <param name="fov">The fov.</param>
        /// <param name="pixelsPerDisplayPixel">The pixels per display pixel.</param>
        /// <returns>The texture size.</returns>
        public Size2 GetFovTextureSize(EyeType eye, FovPort fov, float pixelsPerDisplayPixel)
        {
            return ovrHmd_GetFovTextureSize(this.hmd, eye, fov, pixelsPerDisplayPixel);
        }

        /// <summary>
        /// Gets the maximum size of the render target.
        /// </summary>
        /// <param name="pixelsPerDisplayPixel">The pixels per display pixel.</param>
        /// <returns>The render target size.</returns>
        public Size2 GetMaxRenderTargetSize(float pixelsPerDisplayPixel = 1f)
        {
            Size2 size = this.GetFovTextureSize(EyeType.Left, this.desc.MaxEyeFov[0], pixelsPerDisplayPixel);
            Size2 size2 = this.GetFovTextureSize(EyeType.Right, this.desc.MaxEyeFov[1], pixelsPerDisplayPixel);
            return new Size2(size.Width + size2.Width, Math.Max(size.Height, size2.Height));
        }

        /// <summary>
        /// Gets the render description.
        /// </summary>
        /// <param name="eye">The eye.</param>
        /// <param name="fov">The fov.</param>
        /// <returns>The render description.</returns>
        public EyeRenderDesc GetRenderDesc(EyeType eye, FovPort fov)
        {
            return ovrHmd_GetRenderDesc(this.hmd, eye, fov);
        }

        /// <summary>
        /// Gets the state of the tracking.
        /// </summary>
        /// <param name="absTime">The abs time.</param>
        /// <returns>The tracking state.</returns>
        public TrackingState GetTrackingState(double absTime)
        {
            return ovrHmd_GetTrackingState(this.hmd, absTime);
        }

        /// <summary>
        /// Recenters the pose.
        /// </summary>
        public void RecenterPose()
        {
            ovrHmd_RecenterPose(this.hmd);
        }

        /// <summary>
        /// Shutdowns the rendering.
        /// </summary>
        /// <returns>True if everything ok.</returns>
        public unsafe bool ShutdownRendering()
        {
            return ovrHmd_ConfigureRendering(this.hmd, null, DistortionCapabilities.None, null, null);
        }

        #endregion
    }
}
