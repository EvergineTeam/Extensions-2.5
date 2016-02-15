#region File Description
//-----------------------------------------------------------------------------
// OculusVRHelpers
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using OculusWrap;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
#endregion

namespace WaveEngine.OculusRift
{
    /// <summary>
    /// OVR utilities
    /// </summary>
    internal static class OculusVRHelpers
    {
        /// <summary>
        /// Convert an ovrVector3f to Wave Vector3.
        /// </summary>
        /// <param name="ovrVector3f">ovrVector3f to convert to a SharpDX Vector3.</param>
        /// <returns>SharpDX Vector3, based on the ovrVector3f.</returns>
        public static Vector3 ToVector3(this OVR.Vector3f ovrVector3f)
        {
            return new Vector3(ovrVector3f.X, ovrVector3f.Y, ovrVector3f.Z);
        }

        /// <summary>
        /// Convert an ovrVector3f to Wave Vector3.
        /// </summary>
        /// <param name="ovrVector3f">ovrVector3f to convert to a SharpDX Vector3.</param>
        /// <param name="vector3">Wave Vector3, based on the ovrVector3f.</param>        
        public static void ToVector3(this OVR.Vector3f ovrVector3f, out Vector3 vector3)
        {
            vector3.X = ovrVector3f.X;
            vector3.Y = ovrVector3f.Y;
            vector3.Z = ovrVector3f.Z;
        }

        /// <summary>
        /// Convert an ovrMatrix4f to a Wave Matrix.
        /// </summary>
        /// <param name="ovrMatrix4f">ovrMatrix4f to convert to a SharpDX Matrix.</param>
        /// <returns>SharpDX Matrix, based on the ovrMatrix4f.</returns>
        public static Matrix ToMatrix(this OculusWrap.OVR.Matrix4f ovrMatrix4f)
        {
            return new Matrix(ovrMatrix4f.M11, ovrMatrix4f.M12, ovrMatrix4f.M13, ovrMatrix4f.M14, ovrMatrix4f.M21, ovrMatrix4f.M22, ovrMatrix4f.M23, ovrMatrix4f.M24, ovrMatrix4f.M31, ovrMatrix4f.M32, ovrMatrix4f.M33, ovrMatrix4f.M34, ovrMatrix4f.M41, ovrMatrix4f.M42, ovrMatrix4f.M43, ovrMatrix4f.M44);
        }

        /// <summary>
        /// Convert an ovrMatrix4f to a Wave Matrix.
        /// </summary>
        /// <param name="ovrMatrix4f">ovrMatrix4f to convert to a SharpDX Matrix.</param>
        /// <param name="matrix">Wave Matrix, based on the ovrMatrix4f.</param>
        public static void ToMatrix(this OculusWrap.OVR.Matrix4f ovrMatrix4f, out Matrix matrix)
        {
            matrix.M11 = ovrMatrix4f.M11;
            matrix.M12 = ovrMatrix4f.M21;
            matrix.M13 = ovrMatrix4f.M31;
            matrix.M14 = ovrMatrix4f.M41;

            matrix.M21 = ovrMatrix4f.M12;
            matrix.M22 = ovrMatrix4f.M22;
            matrix.M23 = ovrMatrix4f.M32;
            matrix.M24 = ovrMatrix4f.M42;

            matrix.M31 = ovrMatrix4f.M13;
            matrix.M32 = ovrMatrix4f.M23;
            matrix.M33 = ovrMatrix4f.M33;
            matrix.M34 = ovrMatrix4f.M43;

            matrix.M41 = ovrMatrix4f.M14;
            matrix.M42 = ovrMatrix4f.M24;
            matrix.M43 = ovrMatrix4f.M34;
            matrix.M44 = ovrMatrix4f.M44;
        }

        /// <summary>
        /// Converts an ovrQuatf to a Wave Quaternion.
        /// </summary>
        /// <param name="ovrQuatf">The ovr quaternion</param>
        /// <returns>The Wave Quaternion</returns>
        public static Quaternion ToQuaternion(OVR.Quaternionf ovrQuatf)
        {
            return new Quaternion(ovrQuatf.X, ovrQuatf.Y, ovrQuatf.Z, ovrQuatf.W);
        }

        /// <summary>
        /// Converts an ovrQuatf to a Wave Quaternion.
        /// </summary>
        /// <param name="ovrQuatf">ovrVector3f to convert to a SharpDX Vector3.</param>
        /// <param name="quaternion">Wave Vector3, based on the ovrVector3f.</param>        
        public static void ToQuaternion(this OVR.Quaternionf ovrQuatf, out Quaternion quaternion)
        {
            quaternion.X = ovrQuatf.X;
            quaternion.Y = ovrQuatf.Y;
            quaternion.Z = ovrQuatf.Z;
            quaternion.W = ovrQuatf.W;
        }

        /// <summary>
        /// Creates a Direct3D texture description, based on the SharpDX texture description.
        /// </summary>
        /// <param name="texture2DDescription">SharpDX texture description.</param>
        /// <returns>Direct3D texture description, based on the SharpDX texture description.</returns>
        public static OVR.D3D11.D3D11_TEXTURE2D_DESC CreateTexture2DDescription(Texture2DDescription texture2DDescription)
        {
            OVR.D3D11.D3D11_TEXTURE2D_DESC d3d11DTexture = new OVR.D3D11.D3D11_TEXTURE2D_DESC();
            d3d11DTexture.Width = (uint)texture2DDescription.Width;
            d3d11DTexture.Height = (uint)texture2DDescription.Height;
            d3d11DTexture.MipLevels = (uint)texture2DDescription.MipLevels;
            d3d11DTexture.ArraySize = (uint)texture2DDescription.ArraySize;
            d3d11DTexture.Format = (OVR.D3D11.DXGI_FORMAT)texture2DDescription.Format;
            d3d11DTexture.SampleDesc.Count = (uint)texture2DDescription.SampleDescription.Count;
            d3d11DTexture.SampleDesc.Quality = (uint)texture2DDescription.SampleDescription.Quality;
            d3d11DTexture.Usage = (OVR.D3D11.D3D11_USAGE)texture2DDescription.Usage;
            d3d11DTexture.BindFlags = (uint)texture2DDescription.BindFlags;
            d3d11DTexture.CPUAccessFlags = (uint)texture2DDescription.CpuAccessFlags;
            d3d11DTexture.MiscFlags = (uint)texture2DDescription.OptionFlags;

            return d3d11DTexture;
        }

        /// <summary>
        /// Write out any error details received from the Oculus SDK, into the debug output window.
        /// Please note that writing text to the debug output window is a slow operation and will affect performance,
        /// if too many messages are written in a short timespan.
        /// </summary>
        /// <param name="oculus">OculusWrap object for which the error occurred.</param>
        /// <param name="result">Error code to write in the debug text.</param>
        /// <param name="message">Error message to include in the debug text.</param>
        public static void WriteErrorDetails(Wrap oculus, OVR.ovrResult result, string message)
        {
            if (result >= OVR.ovrResult.Success)
            {
                return;
            }

            // Retrieve the error message from the last occurring error.
            OVR.ovrErrorInfo errorInformation = oculus.GetLastError();

            string formattedMessage = string.Format("{0}. \nMessage: {1} (Error code={2})", message, errorInformation.ErrorString, errorInformation.Result);
            throw new Exception(formattedMessage);
        }

        /// <summary>
        /// Write out any error details received from the Oculus SDK, into the debug output window.
        /// Please note that writing text to the debug output window is a slow operation and will affect performance,
        /// if too many messages are written in a short timespan.
        /// </summary>
        /// <param name="oculus">OculusWrap object for which the error occurred.</param>        
        /// <param name="message">Error message to include in the debug text.</param>
        public static void WriteErrorDetails(Wrap oculus, string message)
        {
            // Retrieve the error message from the last occurring error.
            OVR.ovrErrorInfo errorInformation = oculus.GetLastError();            

            string formattedMessage = string.Format("{0}. \nMessage: {1} (Error code={2})", message, errorInformation.ErrorString, errorInformation.Result);
            Console.WriteLine(formattedMessage);
        }
    }
}
