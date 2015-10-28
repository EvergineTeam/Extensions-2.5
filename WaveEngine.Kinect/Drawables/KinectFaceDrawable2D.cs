#region File Description
// -----------------------------------------------------------------------------
// KinectFaceDrawable2D
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
using WaveEngine.Framework.Resources;
using WaveEngine.Kinect.Behaviors;
#endregion

namespace WaveEngine.Kinect.Drawables
{
    /// <summary>
    /// Kinect Face Drawable2D class
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Kinect.Drawables")]
    public class KinectFaceDrawable2D : Drawable2D
    {
        /// <summary>
        /// The behavior
        /// </summary>
        [RequiredComponent]
        private KinectFaceBehavior behavior = null;

        /// <summary>
        /// The point 0
        /// </summary>
        private Vector2 point0;

        /// <summary>
        /// The point 1
        /// </summary>
        private Vector2 point1;

        /// <summary>
        /// The line color
        /// </summary>        
        private Color lineColor;

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

        #endregion

        /// <summary>
        /// The default values.
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.lineColor = Color.LightGreen;
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
                this.behavior.DrawPoints == null ||
                this.behavior.DrawLines == null ||
                this.behavior.DrawTexts == null)
            {
                return;
            }

            // Draw points
            foreach (Vector2 p in this.behavior.DrawPoints)
            {
                this.point0 = p;
                this.layer.LineBatch2D.DrawPointVM(ref this.point0, 10, ref this.lineColor, 0);
            }

            // Draw lines
            foreach (Line l in this.behavior.DrawLines)
            {
                this.point0.X = l.StartPoint.X;
                this.point0.Y = l.StartPoint.Y;
                this.point1.X = l.EndPoint.X;
                this.point1.Y = l.EndPoint.Y;
                this.layer.LineBatch2D.DrawLineVM(ref this.point0, ref this.point1, ref this.lineColor, 0);
            }

            // Draw texts
            foreach (var textStruct in this.behavior.DrawTexts)
            {
                this.layer.SpriteBatch.DrawStringVM(StaticResources.DefaultSpriteFont, textStruct.Text, textStruct.Position, Color.LightGreen);
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
