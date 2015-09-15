#region File Description
// -----------------------------------------------------------------------------
// KinectSkeletonsDrawable3D
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
// -----------------------------------------------------------------------------
#endregion

#region using
using System;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Kinect.Behaviors;
#endregion

namespace WaveEngine.Kinect.Drawables
{
    /// <summary>
    /// Kinect Skeleton Drawable 3D
    /// </summary>
    public class KinectSkeletonsDrawable3D : Drawable3D
    {
        /// <summary>
        /// The behavior
        /// </summary>
        [RequiredComponent]
        private KinectSkeletonsBehavior behavior;

        /// <summary>
        /// The color0
        /// </summary>
        private Color color0 = Color.Yellow;

        /// <summary>
        /// The color1
        /// </summary>
        private Color color1 = Color.Red;

        /// <summary>
        /// The points
        /// </summary>
        private Vector3 point0, point1;

        /// <summary>
        /// Draws the current frame
        /// </summary>
        /// <param name="gameTime">Current Gametime</param>
        public override void Draw(TimeSpan gameTime)
        {
            //// this.RenderManager.LineBatch3D.DrawCircle(Vector3.Zero, 50, this.color0);

            foreach(var p in this.behavior.DrawPoints3D)
            {
                this.point0 = p;

                // this.RenderManager.LineBatch3D.DrawCircle(ref this.point0, 0.05f, ref this.color0);
                this.RenderManager.LineBatch3D.DrawPoint(ref this.point0, 0.02f, ref this.color0);
            }

            foreach (var line in this.behavior.DrawOrientations)
            {
                this.point0 = line.StartPoint;
                this.point1 = line.EndPoint;
                var c = line.Color;
                this.RenderManager.LineBatch3D.DrawLine(ref point0, ref point1, ref c);
            }
        }

        /// <summary>
        /// Dispose method
        /// </summary>
        /// <param name="disposing">True to dispose managed content</param>
        protected override void Dispose(bool disposing)
        {
        }
    }
}
