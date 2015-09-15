#region File Description
//-----------------------------------------------------------------------------
// Profile
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;
#endregion

namespace WaveEngine.OculusRift
{
    /// <summary>
    /// OVR profile.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is Ok here.", Scope = "type")]
    public class Profile
    {
        [SuppressUnmanagedCodeSecurity, DllImport("libovr.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern float ovrHmd_GetFloat(IntPtr hmd, string propertyName, float defaultValue);
        [SuppressUnmanagedCodeSecurity, DllImport("libovr.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern string ovrHmd_GetString(IntPtr hmd, string propertyName, string defaultValue);

        /// <summary>
        /// The HMD
        /// </summary>
        private IntPtr hmd;

        #region Properties

        /// <summary>
        /// Gets the height of the eye.
        /// </summary>
        /// <value>
        /// The height of the eye.
        /// </value>
        public float EyeHeight
        {
            get
            {
                return ovrHmd_GetFloat(this.hmd, "EyeHeight", 1.675f);
            }
        }

        /// <summary>
        /// Gets the gender.
        /// </summary>
        /// <value>
        /// The gender.
        /// </value>
        public string Gender
        {
            get
            {
                return ovrHmd_GetString(this.hmd, "Gender", "Male");
            }
        }

        /// <summary>
        /// Gets the ipd.
        /// </summary>
        /// <value>
        /// The ipd.
        /// </value>
        public float IPD
        {
            get
            {
                return ovrHmd_GetFloat(this.hmd, "IPD", 0.064f);
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get
            {
                return ovrHmd_GetString(this.hmd, "Name", null);
            }
        }

        /// <summary>
        /// Gets the neck eye hori.
        /// </summary>
        /// <value>
        /// The neck eye hori.
        /// </value>
        public float NeckEyeHori
        {
            get
            {
                return ovrHmd_GetFloat(this.hmd, "NeckEyeHori", 0.12f);
            }
        }

        /// <summary>
        /// Gets the neck eye vert.
        /// </summary>
        /// <value>
        /// The neck eye vert.
        /// </value>
        public float NeckEyeVert
        {
            get
            {
                return ovrHmd_GetFloat(this.hmd, "NeckEyeVert", 0.12f);
            }
        }

        /// <summary>
        /// Gets the height of the player.
        /// </summary>
        /// <value>
        /// The height of the player.
        /// </value>
        public float PlayerHeight
        {
            get
            {
                return ovrHmd_GetFloat(this.hmd, "PlayerHeight", 1.778f);
            }
        }

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        public string User
        {
            get
            {
                return ovrHmd_GetString(this.hmd, "User", null);
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Profile"/> class.
        /// </summary>
        /// <param name="hmd">The HMD.</param>
        internal Profile(IntPtr hmd)
        {
            this.hmd = hmd;
        }
    }
}
