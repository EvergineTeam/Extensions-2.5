// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using Windows.Graphics.Holographic;
using Windows.Foundation;
using Windows.Perception.Spatial;
using SharpDX.Mathematics.Interop;
using SharpDX.Direct3D11;
using System.Numerics;
using Windows.Graphics.DirectX.Direct3D11;
using System.Runtime.InteropServices;
using System;
#endregion

namespace WaveEngine.MixedReality.Internals
{
    /// <summary>
    /// Constant buffer used to send hologram position transform to the shader pipeline.
    /// </summary>
    internal struct ViewProjectionConstantBuffer
    {
        /// <summary>
        /// The view projection
        /// </summary>
        public Matrix4x4 viewProjection;
    }
}
