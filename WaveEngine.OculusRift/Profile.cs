using System;
using System.Runtime.InteropServices;
using System.Security;

namespace WaveEngine.OculusRift
{
    public class Profile
    {
        [SuppressUnmanagedCodeSecurity, DllImport("libovr.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern float ovrHmd_GetFloat(IntPtr hmd, string propertyName, float defaultValue);
        [SuppressUnmanagedCodeSecurity, DllImport("libovr.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern string ovrHmd_GetString(IntPtr hmd, string propertyName, string defaultValue);

        private IntPtr _hmd;

        #region Properties

        public float EyeHeight
        {
            get
            {
                return ovrHmd_GetFloat(this._hmd, "EyeHeight", 1.675f);
            }
        }

        public string Gender
        {
            get
            {
                return ovrHmd_GetString(this._hmd, "Gender", "Male");
            }
        }

        public float IPD
        {
            get
            {
                return ovrHmd_GetFloat(this._hmd, "IPD", 0.064f);
            }
        }

        public string Name
        {
            get
            {
                return ovrHmd_GetString(this._hmd, "Name", null);
            }
        }

        public float NeckEyeHori
        {
            get
            {
                return ovrHmd_GetFloat(this._hmd, "NeckEyeHori", 0.12f);
            }
        }

        public float NeckEyeVert
        {
            get
            {
                return ovrHmd_GetFloat(this._hmd, "NeckEyeVert", 0.12f);
            }
        }

        public float PlayerHeight
        {
            get
            {
                return ovrHmd_GetFloat(this._hmd, "PlayerHeight", 1.778f);
            }
        }

        public string User
        {
            get
            {
                return ovrHmd_GetString(this._hmd, "User", null);
            }
        }

        #endregion

        internal Profile(IntPtr hmd)
        {
            this._hmd = hmd;
        }
    }
}
