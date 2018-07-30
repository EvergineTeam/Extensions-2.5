// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

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
using WaveEngine.Common.Attributes;
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

        /// <summary>
        /// The color factor x
        /// </summary>
        private float colorFactorX;

        /// <summary>
        /// The color factor y
        /// </summary>
        private float colorFactorY;

        /// <summary>
        /// The depth factor x
        /// </summary>
        private float depthFactorX;

        /// <summary>
        /// The depth factor y
        /// </summary>
        private float depthFactorY;

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

        #region Properties

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
        /// Gets or sets a value indicating whether you can set the size to virtualscreen size automatically
        /// </summary>
        [RenderProperty(Tag = 1)]
        [DataMember]
        public bool UseVirtualScreenSize { get; set; }

        /// <summary>
        /// Gets or sets space size (Default 1920x180)
        /// </summary>
        [RenderProperty(AttatchToTag = 1, AttachToValue = false)]
        [DataMember]
        public Vector2 Size { get; set; }

        #endregion

        /// <summary>
        /// Initialize default values
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.Size = new Vector2(1920, 1080);
            this.UseVirtualScreenSize = false;

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
        /// Initialize method
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            if (this.UseVirtualScreenSize)
            {
                var virtualScreenManager = this.RenderManager.ActiveCamera2D.UsedVirtualScreen;
                this.Size = new Vector2(virtualScreenManager.VirtualWidth, virtualScreenManager.VirtualHeight);
            }

            if (this.kinectService != null)
            {
                this.colorFactorX = this.Size.X / (float)this.kinectService.ColorTexture.Width;
                this.colorFactorY = this.Size.Y / (float)this.kinectService.ColorTexture.Height;
                this.depthFactorX = this.Size.X / (float)this.kinectService.DepthTexture.Width;
                this.depthFactorY = this.Size.Y / (float)this.kinectService.DepthTexture.Height;
            }
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

                                this.textureFactorX = this.Size.X / (float)this.kinectService.ColorTexture.Width;
                                this.textureFactorY = this.Size.Y / (float)this.kinectService.ColorTexture.Height;

                                break;
                            case KinectSources.Depth:
                            case KinectSources.Infrared:
                                points = face.FacePointsInInfraredSpace.Values;
                                rectangle = face.FaceBoundingBoxInInfraredSpace;

                                this.textureFactorX = this.Size.X / (float)this.kinectService.InfraredTexture.Width;
                                this.textureFactorY = this.Size.Y / (float)this.kinectService.InfraredTexture.Height;

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
