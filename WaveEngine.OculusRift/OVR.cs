#region File Description
//-----------------------------------------------------------------------------
// OVR
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using WaveEngine.Common.Math;
#endregion

namespace WaveEngine.OculusRift
{
    /// <summary>
    /// OVR represent the connection with the native library.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is Ok here.", Scope = "type")]
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

        /// <summary>
        /// Version String.
        /// </summary>
        private static string versionString;

        /// <summary>
        /// Library OVR name.
        /// </summary>
        internal const string LibOVRName = "libovr.dll";

        #region Public Methods
        /// <summary>
        /// Initializes static members of the <see cref="OVR"/> class.
        /// </summary>
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

        /// <summary>
        /// Gets the time in seconds.
        /// </summary>
        /// <returns>Time in seconds.</returns>
        public static double GetTimeInSeconds()
        {
            return ovr_GetTimeInSeconds();
        }

        /// <summary>
        /// HMDs the create.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The HMD instance.</returns>
        public static HMD HmdCreate(int index)
        {
            IntPtr hmd = ovrHmd_Create(index);

            if (!(hmd != IntPtr.Zero))
            {
                return null;
            }

            return new HMD(hmd);
        }

        /// <summary>
        /// HMDs the create debug.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The HMD instance.</returns>
        public static HMD HmdCreateDebug(HMDType type)
        {
            return new HMD(ovrHmd_CreateDebug(type));
        }

        /// <summary>
        /// HMDs the detect.
        /// </summary>
        /// <returns>Number of HMD.</returns>
        public static int HmdDetect()
        {
            return ovrHmd_Detect();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <returns>true if everything ok.</returns>
        public static bool Initialize()
        {
            return ovr_Initialize();
        }

        /// <summary>
        /// Initializes the rendering shim.
        /// </summary>
        public static void InitializeRenderingShim()
        {
            ovr_InitializeRenderingShim();
        }

        /// <summary>
        /// Matrixes the ortho sub projection.
        /// </summary>
        /// <param name="projection">The projection.</param>
        /// <param name="orthoScale">The ortho scale.</param>
        /// <param name="orthoDistance">The ortho distance.</param>
        /// <param name="eyeViewAdjustX">The eye view adjust x.</param>
        /// <returns>The result matrix.</returns>
        public static Matrix MatrixOrthoSubProjection(Matrix projection, Vector2 orthoScale, float orthoDistance, float eyeViewAdjustX)
        {
            return ovrMatrix4f_OrthoSubProjection(projection, orthoScale, orthoDistance, eyeViewAdjustX);
        }

        /// <summary>
        /// Matrixes the projection.
        /// </summary>
        /// <param name="fov">The fov.</param>
        /// <param name="znear">The znear.</param>
        /// <param name="zfar">The zfar.</param>
        /// <param name="rightHanded">if set to <c>true</c> [right handed].</param>
        /// <returns>The result matrix.</returns>
        public static Matrix MatrixProjection(FovPort fov, float znear, float zfar, bool rightHanded)
        {
            return ovrMatrix4f_Projection(fov, znear, zfar, rightHanded);
        }

        /// <summary>
        /// Shutdowns this instance.
        /// </summary>
        public static void Shutdown()
        {
            ovr_Shutdown();
        }

        /// <summary>
        /// Gets the version string.
        /// </summary>
        /// <value>
        /// The version string.
        /// </value>
        public static string VersionString
        {
            get
            {
                if (versionString == null)
                {
                    versionString = ovr_GetVersionString();
                }

                return versionString;
            }
        }
        #endregion
    }
}
