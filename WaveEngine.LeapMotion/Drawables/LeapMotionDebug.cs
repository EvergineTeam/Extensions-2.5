#region File Description
//-----------------------------------------------------------------------------
// LeapMotionDebug
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Usings Statements
using Leap;
using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.LeapMotion.Drawables
{
    /// <summary>
    /// A basic drawable3D to draw the hands capture with leap motion.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.LeapMotion.Drawables")]
    public class LeapMotionDebug : Drawable3D
    {
        /// <summary>
        /// Reference to leap motion service.
        /// </summary>
        private LeapMotionService leapService;

        /// <summary>
        /// The linebatch used to draw lines
        /// </summary>
        private LineBatch3D lineBatch;

        /// <summary>
        /// The transform
        /// </summary>
        [RequiredComponent]
        private Transform3D transform = null;

        #region Initialize
        /// <summary>
        /// Default values
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
        }

        /// <summary>
        /// Initialize this instance.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            this.leapService = WaveServices.GetService<LeapMotionService>();

            if (this.leapService == null)
            {
                Console.WriteLine("You need to register the LeapMotion service.");
            }

            if (!this.leapService.IsReady)
            {
                Console.WriteLine("You need to call to start method before.");
            }

            this.lineBatch = this.RenderManager.FindLayer(DefaultLayers.Opaque).LineBatch3D;
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Draw the left and right hands capture with leap motion.
        /// </summary>
        /// <param name="gameTime">the current game time.</param>
        public override void Draw(TimeSpan gameTime)
        {
            if (this.leapService != null && this.leapService.IsReady)
            {
                if (this.leapService.CurrentFeatures.HasFlag(LeapFeatures.Hands))
                {
                    this.DrawHand(this.leapService.RightHand);
                    this.DrawHand(this.leapService.LeftHand);
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Draw debug lines of each hand.
        /// </summary>
        /// <param name="hand">the hand to draw.</param>
        private void DrawHand(Hand hand)
        {
            if (hand != null && hand.IsValid)
            {
                Color red = Color.Red;
                Color green = Color.Green;
                Color yellow = Color.Yellow;
                Color white = Color.White;

                // Plam
                var plamPosition = hand.PalmPosition.ToPositionVector3();
                this.DrawLine(plamPosition, plamPosition + (hand.PalmNormal.ToPositionVector3() * 40), green);
                this.DrawLine(plamPosition, plamPosition + (hand.Direction.ToPositionVector3() * 40), red);

                // Arm
                this.DrawLine(hand.Arm.ElbowPosition.ToPositionVector3(), hand.Arm.WristPosition.ToPositionVector3(), white);

                // Fingers
                foreach (Finger finger in hand.Fingers)
                {
                    // Get finger bones
                    Leap.Bone bone;
                    foreach (Leap.Bone.BoneType boneType in (Leap.Bone.BoneType[])Enum.GetValues(typeof(Leap.Bone.BoneType)))
                    {
                        bone = finger.Bone(boneType);
                        this.DrawLine(bone.PrevJoint.ToPositionVector3(), bone.NextJoint.ToPositionVector3(), yellow);
                    }
                }
            }
        }

        /// <summary>
        /// Draw debug line
        /// </summary>
        /// <param name="start">Start line</param>
        /// <param name="end">End line</param>
        /// <param name="color">The color of the line</param>
        private void DrawLine(Vector3 start, Vector3 end, Color color)
        {
            var worldTransform = this.transform.WorldTransform;

            Vector3.Transform(ref start, ref worldTransform, out start);
            Vector3.Transform(ref end, ref worldTransform, out end);
            this.lineBatch.DrawLine(ref start, ref end, ref color);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
        }

        #endregion
    }
}
