#region File Description
// -----------------------------------------------------------------------------
// KinectSkeletonsDrawable2D
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
// -----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Kinect.Behaviors;
#endregion

namespace WaveEngine.Kinect.Drawables
{
    /// <summary>
    /// Kinect Skeleton drawable2D class
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Kinect.Drawables")]
    public class KinectSkeletonsDrawable2D : Drawable2D
    {
        /// <summary>
        /// The behavior
        /// </summary>
        [RequiredComponent]
        private KinectSkeletonsBehavior behavior = null;

        /// <summary>
        /// The color0
        /// </summary>        
        private Color lineColor;

        /// <summary>
        /// The color1
        /// </summary>        
        private Color pointColor;

        /// <summary>
        /// The points
        /// </summary>
        private Vector2 point0, point1;

        /// <summary>
        /// If viewport manager is actived or not
        /// </summary>
        private bool existsVM;

        #region Properties

        /// <summary>
        /// Gets or sets line color
        /// </summary>    
        [DataMember]
        public Color LineColor
        {
            get
            {
                return this.lineColor;
            }

            set
            {
                this.lineColor = value;
            }
        }

        /// <summary>
        /// Gets or sets point color
        /// </summary>        
        [DataMember]
        public Color PointColor
        {
            get
            {
                return this.pointColor;
            }

            set
            {
                this.pointColor = value;
            }
        }

        #endregion

        /// <summary>
        /// Default values
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.lineColor = Color.Yellow;
            this.pointColor = Color.Red;
        }

        /// <summary>
        /// Resolve dependencies method
        /// </summary>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.existsVM = (WaveServices.ViewportManager != null && WaveServices.ViewportManager.IsActivated) ? true : false;
        }

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
            if (this.behavior == null ||
                this.behavior.DrawPoints2DProjected == null ||
                this.behavior.DrawLines == null)
            {
                return;
            }

            foreach (var p in this.behavior.DrawPoints2DProjected)
            {
                this.point0.X = p.X;
                this.point0.Y = p.Y;

                if (existsVM)
                {
                    this.layer.LineBatch2D.DrawCircleVM(ref this.point0, 10, ref this.lineColor, 0);
                    this.layer.LineBatch2D.DrawPointVM(ref this.point0, 10, ref this.pointColor, 0);
                }
                else
                {                                   
                    this.layer.LineBatch2D.DrawCircle(ref this.point0, 10, ref this.lineColor, 0);
                    this.layer.LineBatch2D.DrawPoint(ref this.point0, 10, ref this.pointColor, 0);
                }
            }

            foreach (Line l in this.behavior.DrawLines)
            {
                this.point0.X = l.StartPoint.X;
                this.point0.Y = l.StartPoint.Y;
                this.point1.X = l.EndPoint.X;
                this.point1.Y = l.EndPoint.Y;

                if (existsVM)
                {
                    this.RenderManager.LineBatch2D.DrawLineVM(ref point0, ref point1, ref pointColor, 0);
                }
                else
                {
                    this.RenderManager.LineBatch2D.DrawLine(ref point0, ref point1, ref pointColor, 0);
                }
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
