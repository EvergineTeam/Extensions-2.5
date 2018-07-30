// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System.Runtime.CompilerServices;
using WaveEngine.Common.Input;

namespace WaveEngine.OculusRift.Helpers
{
    /// <summary>
    /// Helpers function for Oculus VR
    /// </summary>
    public static class OculusVRHelper
    {
        /// <summary>
        /// Convert bool to button state
        /// </summary>
        /// <param name="b">The boolean value</param>
        /// <returns>The button state</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ButtonState ToButtonState(this bool b)
        {
            return b ? ButtonState.Pressed : ButtonState.Released;
        }
    }
}
