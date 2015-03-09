#region File Description
//-----------------------------------------------------------------------------
// Simple2DMovement
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;

namespace WaveEngine.AI.ChaseAndEvade
{
    /// <summary>
    /// Simple implementation of MovementBase
    /// </summary>
    public class Simple2DMovement : MovementBase
    {
        /// <summary>
        /// The transform
        /// </summary>
        [RequiredComponent]
        private Transform2D transform;

        /// <summary>
        /// The counter
        /// </summary>
        private TimeSpan counter = TimeSpan.Zero;

        /// <summary>
        /// The random service
        /// </summary>
        private WaveEngine.Framework.Services.Random rndService;

        /// <summary>
        /// The direction
        /// </summary>
        public Vector2 Direction;

        /// <summary>
        /// Gets or sets the normal velocity.
        /// </summary>
        /// <value>
        /// The normal velocity.
        /// </value>
        public float NormalVelocity { get; set; }

        /// <summary>
        /// Gets the screen center.
        /// </summary>
        /// <value>
        /// The screen center.
        /// </value>
        public Vector2 ScreenCenter
        {
            get
            {
                Vector2 center = Vector2.Zero;
                center.X = WaveServices.ViewportManager.VirtualWidth / 2;
                center.Y = WaveServices.ViewportManager.VirtualHeight / 2;
                return center;
            }
        }

        /// <summary>
        /// Resolves the dependencies.
        /// </summary>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();
            this.rndService = WaveServices.Random;
            this.Direction.X = (float)Math.Cos(this.rndService.NextInt());
            this.Direction.Y = (float)Math.Cos(this.rndService.NextInt());
        }

        /// <summary>
        /// Executes a simple movement.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Move(TimeSpan gameTime)
        {
            this.counter += gameTime;
            if (this.counter.Seconds > 1)
            {
                this.counter = TimeSpan.Zero;
                this.Direction.X +=
                    MathHelper.Lerp(-1f, 1f, (float)this.rndService.NextDouble());
                this.Direction.Y +=
                    MathHelper.Lerp(-1f, 1f, (float)this.rndService.NextDouble());
            }

            if (this.Direction != Vector2.Zero)
            {
                this.Direction.Normalize();
            }

            this.LookTo(this.transform.Position + this.Direction, 0.15f * this.NormalVelocity);

            this.CorrectDirectionToNotToExitScreen();

            this.transform.Position += this.Direction * this.NormalVelocity;
        }

        /// <summary>
        /// Move the entity to look to the face position with a given turn speed
        /// </summary>
        /// <param name="facePosition">The face position.</param>
        /// <param name="turnSpeed">The turn speed.</param>
        public void LookTo(Vector2 facePosition, float turnSpeed)
        {
            float x = facePosition.X - this.transform.Position.X;
            float y = facePosition.Y - this.transform.Position.Y;

            float desiredAngle = (float)Math.Atan2(y, x);
            float difference = this.FixAngle(desiredAngle - this.transform.Rotation);

            difference = MathHelper.Clamp(difference, -turnSpeed, turnSpeed);
            this.transform.Rotation = this.FixAngle(this.transform.Rotation + difference);
        }

        /// <summary>
        /// Fixes rotation to a correct angle.
        /// </summary>
        /// <param name="newRotation">The new rotation.</param>
        /// <returns>The angle fixed</returns>
        private float FixAngle(float newRotation)
        {
            if (newRotation < -MathHelper.Pi)
            {
                newRotation += MathHelper.TwoPi;
            }
            else if (newRotation > MathHelper.Pi)
            {
                newRotation -= MathHelper.TwoPi;
            }

            return newRotation;
        }

        /// <summary>
        /// Corrects the direction to not to exit screen.
        /// </summary>
        private void CorrectDirectionToNotToExitScreen()
        {
            float distanceFromCenter = Vector2.Distance(this.transform.Position, this.ScreenCenter);

            float maxDistanceFromScreenCenter = Math.Min(this.ScreenCenter.X, this.ScreenCenter.Y);

            if ((this.transform.X < 0 || this.transform.X > this.ScreenCenter.X * 2)
                || this.transform.Y < 0 || this.transform.Y > this.ScreenCenter.Y * 2)
            {
                this.Direction = Vector2.Normalize(this.ScreenCenter - this.transform.Position);
            }
        }
    }
}
