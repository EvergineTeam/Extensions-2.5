// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers;
using WaveEngine.Framework.Services;

#endregion

namespace WaveEngine.Cardboard
{
    /// <summary>
    /// The cardboard device info
    /// </summary>
    public class CardboardDeviceInfo
    {
        /// <summary>
        /// Gets the viewer
        /// </summary>
        internal CardboardViewer Viewer;

        /// <summary>
        /// Gets the device
        /// </summary>
        internal CardboardDevice Device;

        /// <summary>
        /// Gets the distortion
        /// </summary>
        internal CardboardDistortion Distortion;

        /// <summary>
        /// Initializes a new instance of the <see cref="CardboardDeviceInfo" /> class.
        /// </summary>
        public CardboardDeviceInfo()
        {
            this.Viewer = CardboardViewer.CardboardV2;
            this.UpdateDeviceParams();
            this.Distortion = new CardboardDistortion(this.Viewer.DistortionCoefficients);
        }

        /// <summary>
        /// Calculates field of view for the left eye
        /// </summary>
        /// <returns>The field of view of the left eye</returns>
        public float[] GetDistortedFieldOfViewLeftEye()
        {
            var viewer = this.Viewer;
            var device = this.Device;
            var distortion = this.Distortion;

            // Device.height and device.width for device in portrait mode, so transpose.
            var eyeToScreenDistance = viewer.ScreenLensDistance;

            var outerDist = (device.WidthMeters - viewer.InterLensDistance) / 2;
            var innerDist = viewer.InterLensDistance / 2;
            var bottomDist = viewer.BaselineLensDistance - device.BevelMeters;
            var topDist = device.HeightMeters - bottomDist;

            var outerAngle = MathHelper.ToDegrees(Math.Atan(distortion.Distort(outerDist / eyeToScreenDistance)));
            var innerAngle = MathHelper.ToDegrees(Math.Atan(distortion.Distort(innerDist / eyeToScreenDistance)));
            var bottomAngle = MathHelper.ToDegrees(Math.Atan(distortion.Distort(bottomDist / eyeToScreenDistance)));
            var topAngle = MathHelper.ToDegrees(Math.Atan(distortion.Distort(topDist / eyeToScreenDistance)));

            return new float[]
            {
                Math.Min(outerAngle, viewer.FoV),
                Math.Min(innerAngle, viewer.FoV),
                Math.Min(bottomAngle, viewer.FoV),
                Math.Min(topAngle, viewer.FoV)
            };
        }

        /// <summary>
        /// Calculates field of view for the left eye
        /// </summary>
        /// <returns>The field of view of the left eye</returns>
        public float[] GetLeftEyeVisibleTanAngles()
        {
            var viewer = this.Viewer;
            var device = this.Device;
            var distortion = this.Distortion;

            // Tan-angles from the max FOV.
            var fovLeft = Math.Tan(-MathHelper.ToRadians(viewer.FoV));
            var fovTop = Math.Tan(MathHelper.ToRadians(viewer.FoV));
            var fovRight = Math.Tan(MathHelper.ToRadians(viewer.FoV));
            var fovBottom = Math.Tan(-MathHelper.ToRadians(viewer.FoV));

            // Viewport size.
            var halfWidth = device.WidthMeters / 4;
            var halfHeight = device.HeightMeters / 2;

            // Viewport center, measured from left lens position.
            var verticalLensOffset = viewer.BaselineLensDistance - device.BevelMeters - halfHeight;
            var centerX = (viewer.InterLensDistance / 2) - halfWidth;
            var centerY = -verticalLensOffset;
            var centerZ = viewer.ScreenLensDistance;

            // Tan-angles of the viewport edges, as seen through the lens.
            var screenLeft = distortion.Distort((centerX - halfWidth) / centerZ);
            var screenTop = distortion.Distort((centerY + halfHeight) / centerZ);
            var screenRight = distortion.Distort((centerX + halfWidth) / centerZ);
            var screenBottom = distortion.Distort((centerY - halfHeight) / centerZ);

            // Compare the two sets of tan-angles and take the value closer to zero on each side.
            var result = new float[4];
            result[0] = (float)Math.Max(fovLeft, screenLeft);
            result[1] = (float)Math.Min(fovTop, screenTop);
            result[2] = (float)Math.Min(fovRight, screenRight);
            result[3] = (float)Math.Max(fovBottom, screenBottom);
            return result;
        }

        /// <summary>
        /// Calculates the tan-angles from the maximum FOV for the left eye for the current device and screen parameters, assuming no lenses.
        /// </summary>
        /// <returns>The field of view of the left eye</returns>
        public float[] GetLeftEyeNoLensTanAngles()
        {
            var viewer = this.Viewer;
            var device = this.Device;
            var distortion = this.Distortion;

            var result = new float[4];

            // Tan-angles from the max FOV.
            var fovLeft = distortion.DistortInverse((float)Math.Tan(-MathHelper.ToRadians(viewer.FoV)));
            var fovTop = distortion.DistortInverse((float)Math.Tan(MathHelper.ToRadians(viewer.FoV)));
            var fovRight = distortion.DistortInverse((float)Math.Tan(MathHelper.ToRadians(viewer.FoV)));
            var fovBottom = distortion.DistortInverse((float)Math.Tan(-MathHelper.ToRadians(viewer.FoV)));

            // Viewport size.
            var halfWidth = device.WidthMeters / 4;
            var halfHeight = device.HeightMeters / 2;

            // Viewport center, measured from left lens position.
            var verticalLensOffset = viewer.BaselineLensDistance - device.BevelMeters - halfHeight;
            var centerX = (viewer.InterLensDistance / 2) - halfWidth;
            var centerY = -verticalLensOffset;
            var centerZ = viewer.ScreenLensDistance;

            // Tan-angles of the viewport edges, as seen through the lens.
            var screenLeft = (centerX - halfWidth) / centerZ;
            var screenTop = (centerY + halfHeight) / centerZ;
            var screenRight = (centerX + halfWidth) / centerZ;
            var screenBottom = (centerY - halfHeight) / centerZ;

            // Compare the two sets of tan-angles and take the value closer to zero on each side.
            result[0] = Math.Max(fovLeft, screenLeft);
            result[1] = Math.Min(fovTop, screenTop);
            result[2] = Math.Min(fovRight, screenRight);
            result[3] = Math.Max(fovBottom, screenBottom);
            return result;
        }

        /// <summary>
        /// Calculates the screen rectangle visible from the left eye for the current device and screen parameters
        /// </summary>
        /// <param name="undistortedFrustum">Undistorted frustrum</param>
        /// <returns>The screen rect</returns>
        public RectangleF GetLeftEyeVisibleScreenRect(float[] undistortedFrustum)
        {
            var viewer = this.Viewer;
            var device = this.Device;

            var dist = viewer.ScreenLensDistance;
            var eyeX = (device.WidthMeters - viewer.InterLensDistance) / 2;
            var eyeY = viewer.BaselineLensDistance - device.BevelMeters;
            var left = ((undistortedFrustum[0] * dist) + eyeX) / device.WidthMeters;
            var top = ((undistortedFrustum[1] * dist) + eyeY) / device.HeightMeters;
            var right = ((undistortedFrustum[2] * dist) + eyeX) / device.WidthMeters;
            var bottom = ((undistortedFrustum[3] * dist) + eyeY) / device.HeightMeters;

            return new RectangleF(
                left,
                bottom,
                right - left,
                top - bottom);
        }

        /// <summary>
        /// Update the device params
        /// </summary>
        private void UpdateDeviceParams()
        {
            if (WaveServices.Platform.PlatformType == Common.PlatformType.iOS)
            {
                this.Device = CardboardDevice.DefaultIOS;
            }
            else
            {
                this.Device = CardboardDevice.DefaultAndroid;
            }
        }
    }
}
