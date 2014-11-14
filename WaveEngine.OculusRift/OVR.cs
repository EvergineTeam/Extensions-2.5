using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using WaveEngine.Common.Math;

namespace WaveEngine.OculusRift
{
    public static class OVR
    {
        #region DllImport
        [SuppressUnmanagedCodeSecurity, DllImport("kernel32")]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [SuppressUnmanagedCodeSecurity, DllImport("libovr.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern double ovr_GetTimeInSeconds();

        [SuppressUnmanagedCodeSecurity, DllImport("libovr.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern string ovr_GetVersionString();

        [SuppressUnmanagedCodeSecurity, DllImport("libovr.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool ovr_Initialize();

        [SuppressUnmanagedCodeSecurity, DllImport("libovr.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void ovr_InitializeRenderingShim();

        [SuppressUnmanagedCodeSecurity, DllImport("libovr.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void ovr_Shutdown();

        [SuppressUnmanagedCodeSecurity, DllImport("libovr.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr ovrHmd_Create(int index);

        [SuppressUnmanagedCodeSecurity, DllImport("libovr.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr ovrHmd_CreateDebug(HMDType type);

        [SuppressUnmanagedCodeSecurity, DllImport("libovr.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int ovrHmd_Detect();

        [SuppressUnmanagedCodeSecurity, DllImport("libovr.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern Matrix ovrMatrix4f_OrthoSubProjection(Matrix projection, Vector2 orthoScale, float orthoDistance, float eyeViewAdjustX);

        [SuppressUnmanagedCodeSecurity, DllImport("libovr.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern Matrix ovrMatrix4f_Projection(FovPort fov, float znear, float zfar, bool rightHanded);
        #endregion

        private static string _versionString;
        internal const string LibOVRName = "libovr.dll";

        static OVR()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "libovr.dll");
            if (!File.Exists(path) || (LoadLibrary(path) == IntPtr.Zero))
            {
                path = Path.Combine(Path.GetTempPath(), "libovr.dll");
                try
                {
                    string name = "WaveEngine.OculusRift.libovr.dll";
                    using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write))
                    {
                        Assembly.GetExecutingAssembly().GetManifestResourceStream(name).CopyTo(stream);
                    }
                }
                catch
                {
                }

                LoadLibrary(path);
            }
        }

        public static double GetTimeInSeconds()
        {
            return ovr_GetTimeInSeconds();
        }

        public static HMD HmdCreate(int index)
        {
            IntPtr hmd = ovrHmd_Create(index);

            if (!(hmd != IntPtr.Zero))
            {
                return null;
            }

            return new HMD(hmd);
        }

        public static HMD HmdCreateDebug(HMDType type)
        {
            return new HMD(ovrHmd_CreateDebug(type));
        }

        public static int HmdDetect()
        {
            return ovrHmd_Detect();
        }

        public static bool Initialize()
        {
            return ovr_Initialize();
        }

        public static void InitializeRenderingShim()
        {
            ovr_InitializeRenderingShim();
        }

        public static Matrix MatrixOrthoSubProjection(Matrix projection, Vector2 orthoScale, float orthoDistance, float eyeViewAdjustX)
        {
            return ovrMatrix4f_OrthoSubProjection(projection, orthoScale, orthoDistance, eyeViewAdjustX);
        }

        public static Matrix MatrixProjection(FovPort fov, float znear, float zfar, bool rightHanded)
        {
            return ovrMatrix4f_Projection(fov, znear, zfar, rightHanded);
        }

        public static void Shutdown()
        {
            ovr_Shutdown();
        }

        // Properties
        public static string VersionString
        {
            get
            {
                if (_versionString == null)
                {
                    _versionString = ovr_GetVersionString();
                }
                return _versionString;
            }
        }
    }
}
