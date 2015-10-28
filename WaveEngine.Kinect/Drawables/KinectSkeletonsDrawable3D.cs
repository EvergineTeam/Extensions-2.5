#region File Description
// -----------------------------------------------------------------------------
// KinectSkeletonsDrawable3D
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
using WaveEngine.Kinect.Behaviors;
#endregion

namespace WaveEngine.Kinect.Drawables
{
    /// <summary>
    /// Kinect Skeleton Drawable 3D
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Kinect.Drawables")]
    public class KinectSkeletonsDrawable3D : Drawable3D
    {
        /// <summary>
        /// The behavior
        /// </summary>
        [RequiredComponent]
        private KinectSkeletonsBehavior behavior = null;

        /// <summary>
        /// The color0
        /// </summary>
        [DataMember]
        private Color lineColor;

        /// <summary>
        /// The color1
        /// </summary>        
        private Color pointColor;

        /// <summary>
        /// The points
        /// </summary>
        private Vector3 point0, point1;

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
        /// Draws the current frame
        /// </summary>
        /// <param name="gameTime">Current Gametime</param>
        public override void Draw(TimeSpan gameTime)
        {            
            if (this.behavior == null ||
                this.behavior.DrawPoints3D == null ||
                this.behavior.DrawOrientations == null)
            {
                return;
            }

            foreach (var p in this.behavior.DrawPoints3D)
            {
                this.point0 = p;
         
                this.RenderManager.LineBatch3D.DrawPoint(ref this.point0, 0.02f, ref this.lineColor);
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
