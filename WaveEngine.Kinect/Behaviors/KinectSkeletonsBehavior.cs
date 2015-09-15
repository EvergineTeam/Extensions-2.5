#region File Description
//-----------------------------------------------------------------------------
// KinectSkeletonsBehavior
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using region
using System;
using System.Collections.Generic;
using Microsoft.Kinect;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.Kinect.Behaviors
{
    using WaveEngine.Kinect.Enums;

    /// <summary>
    /// Kinect Skeleton Behavior
    /// </summary>
    public class KinectSkeletonsBehavior : Behavior
    {
        /// <summary>
        /// The bones
        /// </summary>
        private List<Tuple<JointType, JointType>> bones;

        /// <summary>
        /// The kinect service
        /// </summary>
        private KinectService kinectService;

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

        /// <summary>
        /// Gets or sets the draw points.
        /// </summary>
        /// <value>
        /// The draw points.
        /// </value>
        public List<Vector3> DrawPoints3D { get; set; }

        /// <summary>
        /// Projected DrawPoints
        /// </summary>
        public List<Vector2> DrawPoints2DProjected { get; set; }

        /// <summary>
        /// Gets or sets the draw lines.
        /// </summary>
        /// <value>
        /// The draw lines.
        /// </value>
        public List<Line> DrawLines { get; set; }

        /// <summary>
        /// Orientations
        /// </summary>
        public List<Line> DrawOrientations { get; set; }

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
        /// Initializes a new instance of the <see cref="KinectSkeletonsBehavior"/> class.
        /// </summary>
        public KinectSkeletonsBehavior()
        {          
        }

        /// <summary>
        /// Initialize default values
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.CurrentSource = KinectSources.Color;

            // a bone defined as a line between two joints
            this.bones = new List<Tuple<JointType, JointType>>();

            // Torso
            this.bones.Add(new Tuple<JointType, JointType>(JointType.Head, JointType.Neck));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.Neck, JointType.SpineShoulder));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.SpineMid));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineMid, JointType.SpineBase));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipLeft));

            // Right Arm
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ShoulderRight, JointType.ElbowRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ElbowRight, JointType.WristRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristRight, JointType.HandRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HandRight, JointType.HandTipRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristRight, JointType.ThumbRight));

            // Left Arm
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ShoulderLeft, JointType.ElbowLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.ElbowLeft, JointType.WristLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.HandLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HandLeft, JointType.HandTipLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.ThumbLeft));

            // Right Leg
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HipRight, JointType.KneeRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.KneeRight, JointType.AnkleRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.AnkleRight, JointType.FootRight));

            // Left Leg
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HipLeft, JointType.KneeLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.KneeLeft, JointType.AnkleLeft));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.AnkleLeft, JointType.FootLeft));
        }

        /// <summary>
        /// Resolves the dependencies needed for this instance to work.
        /// </summary>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.kinectService = WaveServices.GetService<KinectService>();

            this.colorFactorX = WaveServices.ViewportManager.VirtualWidth / this.kinectService.ColorTexture.Width;
            this.colorFactorY = WaveServices.ViewportManager.VirtualHeight / this.kinectService.ColorTexture.Height;
            this.depthFactorX = WaveServices.ViewportManager.VirtualWidth / this.kinectService.DepthTexture.Width;
            this.depthFactorY = WaveServices.ViewportManager.VirtualHeight / this.kinectService.DepthTexture.Height;
            this.DrawPoints2DProjected = new List<Vector2>();
            this.DrawPoints3D = new List<Vector3>();
            this.DrawLines = new List<Line>();
            this.DrawOrientations = new List<Line>();
        }

        /// <summary>
        /// Allows this instance to execute custom logic during its <c>Update</c>.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <remarks>
        /// This method will not be executed if the 
        /// <see cref="T:WaveEngine.Framework.Component" />, or the 
        /// <see cref="T:WaveEngine.Framework.Entity" />
        /// owning it are not 
        /// <c>Active</c>.
        /// </remarks>
        protected override void Update(TimeSpan gameTime)
        {
            var bodies = this.kinectService.Bodies;

            this.DrawPoints2DProjected.Clear();
            this.DrawLines.Clear();
            this.DrawOrientations.Clear();
            this.DrawPoints3D.Clear();

            if (bodies != null)
            {
                foreach (var body in bodies)
                {
                    if (body.IsTracked)
                    {
                        var joints = body.Joints;
                        var orientations = body.JointOrientations;

                        foreach (var jointType in joints.Keys)
                        {
                            var v = joints[jointType].Position;
                            var position = this.TranslatePosition(v);
                            this.DrawPoints2DProjected.Add(position);

                            Vector3 pos = Vector3.Zero;
                            pos.X = v.X;
                            pos.Y = v.Y;
                            pos.Z = v.Z;
                            this.DrawPoints3D.Add(pos);

                            var orientation = orientations[jointType].Orientation;
                            Quaternion q = new Quaternion()
                            {
                                X = orientation.X,
                                Y = orientation.Y,
                                Z = orientation.Z,

                                W = orientation.W
                            };

                            Vector3 axis;
                            float angle;

                            Quaternion.ToAngleAxis(ref q, out axis, out angle);
                            
                            float aux = axis.X;
                            axis.X = axis.Y;
                            axis.Y = -aux;
                            axis.Z = axis.Z;

                            var m = Matrix.CreateFromAxisAngle(axis, angle);

                            var front = m.Forward;
                            var left = m.Left;
                            var up = m.Up;

                            float size = 0.1f;

                            front *= size;
                            left *= size;
                            up *= size;

                            this.DrawOrientations.Add(new Line(pos, pos + front, Color.Green));
                            this.DrawOrientations.Add(new Line(pos, pos + left, Color.Red));
                            this.DrawOrientations.Add(new Line(pos, pos + up, Color.Blue));
                        }

                        this.UpdateBones(joints);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the bones.
        /// </summary>
        /// <param name="joints">The joints.</param>
        private void UpdateBones(IReadOnlyDictionary<JointType, Joint> joints)
        {
            foreach (var bone in this.bones)
            {
                var joint0 = joints[bone.Item1];
                var joint1 = joints[bone.Item2];

                // If we can't find either of these joints, exit
                if (joint0.TrackingState == TrackingState.NotTracked ||
                    joint1.TrackingState == TrackingState.NotTracked)
                {
                    return;
                }

                var position0 = this.TranslatePosition(joint0.Position);
                var position1 = this.TranslatePosition(joint1.Position);
                this.DrawLines.Add(new Line(position0, position1, Color.White));
            }
        }

        /// <summary>
        /// Translates the position.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>Camera space translated point</returns>
        private Vector2 TranslatePosition(CameraSpacePoint point)
        {
            Vector2 position = Vector2.Zero;
            switch (this.currentSource)
            {
                case KinectSources.Color:
                    var colorPoint = this.kinectService.Mapper.MapCameraPointToColorSpace(point);
                    position = new Vector2(colorPoint.X * this.colorFactorX, colorPoint.Y * this.colorFactorY);
                    break;
                case KinectSources.Depth:
                    var depthPoint = this.kinectService.Mapper.MapCameraPointToDepthSpace(point);
                    position = new Vector2(depthPoint.X * this.depthFactorX, depthPoint.Y * this.depthFactorY);
                    break;
                default:
                    break;
            }

            return position;
        }
    }
}
