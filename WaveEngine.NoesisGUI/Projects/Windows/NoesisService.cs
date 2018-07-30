// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Usings Statements
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using WaveEngine.Common;
#endregion

namespace WaveEngine.NoesisGUI
{
    /// <summary>
    /// Windows partial class for NoesisService. Loads the dynamic library.
    /// </summary>
    public partial class NoesisService : Service
    {
        [DllImport("kernel32")]
        [SuppressUnmanagedCodeSecurity]
        private static extern IntPtr LoadLibrary(string lpFileName);

        /// <summary>
        /// Initializes static members of the <see cref="NoesisService"/> class.
        /// </summary>
        static NoesisService()
        {
            if (Environment.Is64BitProcess)
            {
                LoadLibrary("NativeLibraries/x64/Noesis.dll");
            }
            else
            {
                LoadLibrary("NativeLibraries/x86/Noesis.dll");
            }
        }
    }
}
