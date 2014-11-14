using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;

namespace WaveEngine.OculusRift
{
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

        private HMDDesc _desc;
        private string _deviceName;
        private IntPtr _hmd;
        private string _manufacturer;
        private string _productName;
        private Profile _profile;

        #region Properties
        // Properties
        public FovPort[] DefaultEyeFov
        {
            get
            {
                return this._desc.DefaultEyeFov;
            }
        }

        public string DeviceName
        {
            get
            {
                if (this._deviceName == null)
                {
                    this._deviceName = Marshal.PtrToStringAnsi(this._desc.DisplayDeviceName) ?? string.Empty;
                }
                return this._deviceName;
            }
        }

        public long DisplayId
        {
            get
            {
                return (long)this._desc.DisplayId;
            }
        }

        public DistortionCapabilities DistortionCaps
        {
            get
            {
                return this._desc.DistortionCaps;
            }
        }

        public HMDCapabilities EnabledCaps
        {
            get
            {
                return ovrHmd_GetEnabledCaps(this._hmd);
            }
            set
            {
                ovrHmd_SetEnabledCaps(this._hmd, value);
            }
        }

        public EyeType[] EyeRenderOrder
        {
            get
            {
                return this._desc.EyeRenderOrder;
            }
        }

        public HMDCapabilities HMDCaps
        {
            get
            {
                return this._desc.HmdCaps;
            }
        }

        public unsafe HSWDisplayState HSWDisplayState
        {
            get
            {
                HSWDisplayState state = new HSWDisplayState();
                ovrHmd_GetHSWDisplayState(this._hmd, &state);
                return state;
            }
        }

        public string Manufacturer
        {
            get
            {
                return (this._manufacturer ?? (this._manufacturer = Marshal.PtrToStringAnsi(this._desc.Manufacturer)));
            }
        }

        public FovPort[] MaxEyeFov
        {
            get
            {
                return this._desc.MaxEyeFov;
            }
        }

        public int ProductId
        {
            get
            {
                return this._desc.ProductId;
            }
        }

        public string ProductName
        {
            get
            {
                return (this._productName ?? (this._productName = Marshal.PtrToStringAnsi(this._desc.ProductName)));
            }
        }

        public Profile Profile
        {
            get
            {
                return this._profile;
            }
        }

        public Size2 Resolution
        {
            get
            {
                return this._desc.Resolution;
            }
        }

        public string SerialNumber
        {
            get
            {
                return this._desc.SerialNumber;
            }
        }

        public TrackingCapabilities TrackingCaps
        {
            get
            {
                return this._desc.TrackingCaps;
            }
        }

        public HMDType Type
        {
            get
            {
                return this._desc.Type;
            }
        }

        public int VendorId
        {
            get
            {
                return this._desc.VendorId;
            }
        }

        public Point WindowPos
        {
            get
            {
                return this._desc.WindowsPos;
            }
        }
        #endregion

        internal HMD(IntPtr hmd)
        {
            this._hmd = hmd;
            this._desc = (HMDDesc)Marshal.PtrToStructure(hmd, typeof(HMDDesc));
            this._profile = new Profile(hmd);
        }

        public unsafe bool AttachToWindow(IntPtr hwnd)
        {
            return ovrHmd_AttachToWindow(this._hmd, hwnd, null, null);
        }

        public FrameTiming BeginFrame(uint frameIndex)
        {
            FrameTiming_Raw raw = ovrHmd_BeginFrame(this._hmd, frameIndex);
            return new FrameTiming(raw);
        }

        public FrameTiming BeginFrameTiming(uint frameIndex)
        {
            FrameTiming_Raw raw = ovrHmd_BeginFrameTiming(this._hmd, frameIndex);
            return new FrameTiming(raw);
        }

        public unsafe bool ConfigureRendering(D3D11ConfigData apiConfig, DistortionCapabilities distortionCaps, FovPort[] eyeFovIn, EyeRenderDesc[] eyeRenderDescOut)
        {
            fixed (EyeRenderDesc* descRef = eyeRenderDescOut)
            {
                return ovrHmd_ConfigureRendering(this._hmd, &apiConfig, distortionCaps, eyeFovIn, descRef);
            }
        }

        public bool ConfigureTracking(TrackingCapabilities supportedCaps, TrackingCapabilities requiredCaps)
        {
            return ovrHmd_ConfigureTracking(this._hmd, supportedCaps, requiredCaps);
        }

        public bool DismissHSWDisplay()
        {
            return ovrHmd_DismissHSWDisplay(this._hmd);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this._hmd != IntPtr.Zero)
            {
                ovrHmd_Destroy(this._hmd);
                this._hmd = IntPtr.Zero;
            }
        }

        public void EnableHSWDisplaySDKRender(bool enabled)
        {
            ovrhmd_EnableHSWDisplaySDKRender(this._hmd, enabled);
        }

        public unsafe void EndFrame(PoseF[] renderPose, D3D11TextureData[] eyeTexture)
        {
            fixed (PoseF* efRef = renderPose)
            {
                fixed (D3D11TextureData* dataRef = eyeTexture)
                {
                    ovrHmd_EndFrame(this._hmd, efRef, dataRef);
                }
            }
        }

        public void EndFrameTiming()
        {
            ovrHmd_EndFrameTiming(this._hmd);
        }

        public Size2 GetDefaultRenderTargetSize(float pixelsPerDisplayPixel = 1f)
        {
            Size2 size = this.GetFovTextureSize(EyeType.Left, this._desc.DefaultEyeFov[0], pixelsPerDisplayPixel);
            Size2 size2 = this.GetFovTextureSize(EyeType.Right, this._desc.DefaultEyeFov[1], pixelsPerDisplayPixel);
            return new Size2(size.Width + size2.Width, Math.Max(size.Height, size2.Height));
        }

        public PoseF GetEyePose(EyeType eye)
        {
            return ovrHmd_GetEyePose(this._hmd, eye);
        }

        public Size2 GetFovTextureSize(EyeType eye, FovPort fov, float pixelsPerDisplayPixel)
        {
            return ovrHmd_GetFovTextureSize(this._hmd, eye, fov, pixelsPerDisplayPixel);
        }

        public Size2 GetMaxRenderTargetSize(float pixelsPerDisplayPixel = 1f)
        {
            Size2 size = this.GetFovTextureSize(EyeType.Left, this._desc.MaxEyeFov[0], pixelsPerDisplayPixel);
            Size2 size2 = this.GetFovTextureSize(EyeType.Right, this._desc.MaxEyeFov[1], pixelsPerDisplayPixel);
            return new Size2(size.Width + size2.Width, Math.Max(size.Height, size2.Height));
        }

        public EyeRenderDesc GetRenderDesc(EyeType eye, FovPort fov)
        {
            return ovrHmd_GetRenderDesc(this._hmd, eye, fov);
        }

        public TrackingState GetTrackingState(double absTime)
        {
            return ovrHmd_GetTrackingState(this._hmd, absTime);
        }

        public void RecenterPose()
        {
            ovrHmd_RecenterPose(this._hmd);
        }

        public unsafe bool ShutdownRendering()
        {
            return ovrHmd_ConfigureRendering(this._hmd, null, DistortionCapabilities.None, null, null);
        }
    }
}
