#region File Description
// -----------------------------------------------------------------------------
// KinectSkeletonsDrawable2D
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
    /// Kinect Skeleton drawable2D class
    /// </summary>
    public class KinectSkeletonsDrawable : Drawable2D
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
        private Vector2 point0, point1;

        /// <summary>
        /// Allows to perform custom drawing.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        /// <remarks>
        /// This method will only be called if all the following points are true:
        /// <list type="bullet"><item><description>The entity passes the culling test.</description></item><item><description>The parent of the owner <see cref="T:WaveEngine.Framework.Entity" /> of the <see cref="T:WaveEngine.Framework.Drawable" /> cascades its visibility to its children and it is visible.</description></item><item><description>The <see cref="T:WaveEngine.Framework.Drawable" /> is active.</description></item><item><description>The owner <see cref="T:WaveEngine.Framework.Entity" /> of the <see cref="T:WaveEngine.Framework.Drawable" /> is active and visible.</description></item></list>
        /// </remarks>
        public override void Draw(TimeSpan gameTime)
        {
            foreach (var p in this.behavior.DrawPoints2DProjected)
            {
                this.point0.X = p.X;
                this.point0.Y = p.Y;
                this.RenderManager.LineBatch2D.DrawCircleVM(ref this.point0, 10, ref this.color0, 0);
                this.RenderManager.LineBatch2D.DrawPointVM(ref this.point0, 10, ref this.color1, 0);
            }

            foreach (Line l in this.behavior.DrawLines)
            {
                this.point0.X = l.StartPoint.X;
                this.point0.Y = l.StartPoint.Y;
                this.point1.X = l.EndPoint.X;
                this.point1.Y = l.EndPoint.Y;
                this.RenderManager.LineBatch2D.DrawLineVM(ref point0, ref point1, ref color1, 0);
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
        }
    }
}
