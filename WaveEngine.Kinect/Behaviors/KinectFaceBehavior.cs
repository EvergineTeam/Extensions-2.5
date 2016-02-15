#region File Description
// -----------------------------------------------------------------------------
// KinectFaceBehavior
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
// -----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Kinect;
using Microsoft.Kinect.Face;
using WaveEngine.Framework;
using WaveEngine.Common.Math;
using WaveEngine.Kinect.Helpers;
using WaveEngine.Common.Graphics;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Kinect.Enums;
using System.Runtime.Serialization;
#endregion

namespace WaveEngine.Kinect.Behaviors
{
    /// <summary>
    /// Kinect Face Behavior
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Kinect.Behaviors")]
    public class KinectFaceBehavior : Behavior
    {
        /// <summary>
        /// The kinect service
        /// </summary>
        private KinectService kinectService;

        /// <summary>
        /// The draw points
        /// </summary>
        public List<Vector2> DrawPoints;

        /// <summary>
        /// The draw lines
        /// </summary>
        public List<Line> DrawLines;

        /// <summary>
        /// The draw texts
        /// </summary>
        public List<TextStruct> DrawTexts;

        /// <summary>
        /// The color factor x
        /// </summary>
        private float textureFactorX;

        /// <summary>
        /// The color factor y
        /// </summary>
        private float textureFactorY;

        #region Cache
        /// <summary>
        /// The left top  position
        /// </summary>
        private Vector3 leftTop;

        /// <summary>
        /// The right top  position
        /// </summary>
        private Vector3 rightTop;

        /// <summary>
        /// The left bottom  position
        /// </summary>
        private Vector3 leftBottom;

        /// <summary>
        /// The right bottom position
        /// </summary>
        private Vector3 rightBottom;
        #endregion

        /// <summary>
        /// The current source
        /// </summary>
        private KinectSources currentSource;

        /// <summary>
        /// Gets or sets the current source.
        /// </summary>
        /// <value>
        /// The current source.
        /// </value>
        [DataMember]
        public KinectSources CurrentSource
        {
            get
            {
                return this.currentSource;
            }

            set
            {
                this.currentSource = value;
            }
        }
        
        /// <summary>
        /// Initialize default values
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.DrawPoints = new List<Vector2>();
            this.DrawLines = new List<Line>(4);
            this.DrawTexts = new List<TextStruct>();
        }

        /// <summary>
        /// Resolves the dependencies needed for this instance to work.
        /// </summary>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.kinectService = WaveServices.GetService<KinectService>();            
        }

        /// <summary>
        /// Allows this instance to execute custom logic during its <c>Update</c>.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <remarks>
        /// This method will not be executed if the <see cref="T:WaveEngine.Framework.Component" />, or the <see cref="T:WaveEngine.Framework.Entity" />
        /// owning it are not <c>Active</c>.
        /// </remarks>
        protected override void Update(TimeSpan gameTime)
        {
            if (this.kinectService == null)
            {
                return;
            }

            FaceFrameResult[] faces = this.kinectService.FaceFrameResults;
            FaceFrameSource[] sources = this.kinectService.FaceFrameSources;

            this.DrawPoints.Clear();
            this.DrawLines.Clear();
            this.DrawTexts.Clear();
            this.Owner.IsVisible = false;

            for (int i = 0; i < faces.Length; i++)
            {
                if (sources[i] != null && sources[i].IsTrackingIdValid)
                {
                    FaceFrameResult face = faces[i];
                    if (face != null && this.kinectService.CurrentSource.HasFlag(this.CurrentSource))
                    {
                        this.Owner.IsVisible = true;

                        IEnumerable<PointF> points = null;
                        RectI rectangle = new RectI();

                        switch (this.CurrentSource)
                        {
                            case KinectSources.Color:
                                points = face.FacePointsInColorSpace.Values;
                                rectangle = face.FaceBoundingBoxInColorSpace;

                                this.textureFactorX = 1;
                                this.textureFactorY = 1;

                                ////if (WaveServices.ViewportManager != null && WaveServices.ViewportManager.IsActivated)
                                ////{
                                ////    this.textureFactorX = (float)WaveServices.ViewportManager.VirtualWidth / (float)this.kinectService.ColorTexture.Width;
                                ////    this.textureFactorY = (float)WaveServices.ViewportManager.VirtualHeight / (float)this.kinectService.ColorTexture.Height;
                                ////}
                                ////else
                                ////{
                                ////    this.textureFactorX = (float)WaveServices.Platform.ScreenWidth / (float)this.kinectService.ColorTexture.Width;
                                ////    this.textureFactorY = (float)WaveServices.Platform.ScreenHeight / (float)this.kinectService.ColorTexture.Height;
                                ////}
                                
                                break;
                            case KinectSources.Depth:
                            case KinectSources.Infrared:
                                points = face.FacePointsInInfraredSpace.Values;
                                rectangle = face.FaceBoundingBoxInInfraredSpace;

                                ////if (WaveServices.ViewportManager != null && WaveServices.ViewportManager.IsActivated)
                                ////{
                                ////    this.textureFactorX = (float)WaveServices.ViewportManager.VirtualWidth / (float)this.kinectService.InfraredTexture.Width;
                                ////    this.textureFactorY = (float)WaveServices.ViewportManager.VirtualHeight / (float)this.kinectService.InfraredTexture.Height;
                                ////}
                                ////else
                                ////{
                                ////    this.textureFactorX = (float)WaveServices.Platform.ScreenWidth / (float)this.kinectService.InfraredTexture.Width;
                                ////    this.textureFactorY = (float)WaveServices.Platform.ScreenHeight / (float)this.kinectService.InfraredTexture.Height;
                                ////}

                                this.textureFactorX = 1;
                                this.textureFactorY = 1;

                                break;
                            default:
                                return;
                                break;
                        }

                        // Draw face points
                        foreach (PointF point in points)
                        {
                            Vector2 position = new Vector2(point.X * this.textureFactorX, point.Y * this.textureFactorY);
                            this.DrawPoints.Add(position);
                        }

                        // Draw rectangle
                        this.leftTop.X = rectangle.Left * this.textureFactorX;
                        this.leftTop.Y = rectangle.Top * this.textureFactorY;
                        this.rightBottom.X = rectangle.Right * this.textureFactorX;
                        this.rightBottom.Y = rectangle.Bottom * this.textureFactorY;

                        this.leftBottom.X = this.leftTop.X;
                        this.leftBottom.Y = this.rightBottom.Y;
                        this.rightTop.X = this.rightBottom.X;
                        this.rightTop.Y = this.leftTop.Y;
                        this.DrawLines.Add(new Line(this.leftTop, this.rightTop, Color.White));
                        this.DrawLines.Add(new Line(this.rightTop, this.rightBottom, Color.White));
                        this.DrawLines.Add(new Line(this.rightBottom, this.leftBottom, Color.White));
                        this.DrawLines.Add(new Line(this.leftBottom, this.leftTop, Color.White));

                        // Draw texts
                        if (face.FaceProperties != null)
                        {
                            var position = new Vector2(this.leftBottom.X, this.leftBottom.Y + 10);

                            foreach (var property in face.FaceProperties)
                            {
                                var text = string.Format("{0} : {1}", property.Key.ToString(), property.Value.ToString());
                                this.DrawTexts.Add(new TextStruct()
                                                {
                                                    Text = text,
                                                    Position = position,
                                                });
                                position.Y += 20;
                            }
                        }
                    }
                }
            }
        }
    }
}
