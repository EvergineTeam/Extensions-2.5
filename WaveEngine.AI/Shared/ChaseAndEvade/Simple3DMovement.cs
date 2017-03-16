#region File Description
//-----------------------------------------------------------------------------
// Simple3DMovement
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Materials;

namespace WaveEngine.AI.ChaseAndEvade
{
    /// <summary>
    /// Simple implementation of MovementBase for 3D
    /// </summary>
    public class Simple3DMovement : MovementBase
    {
        /// <summary>
        /// The transform
        /// </summary>
        [RequiredComponent]
        private Transform3D transform;

        /// <summary>
        /// The time
        /// </summary>
        private TimeSpan time = TimeSpan.Zero;

        /// <summary>
        /// The color
        /// </summary>
        private Color color;

        /// <summary>
        /// The random service
        /// </summary>
        private WaveEngine.Framework.Services.Random rndService;

        /// <summary>
        /// The normal velocity
        /// </summary>
        public float NormalVelocity;

        /// <summary>
        /// The direction
        /// </summary>
        public Vector3 Direction;

        /// <summary>
        /// Resolves the dependencies.
        /// </summary>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();
            this.rndService = WaveServices.Random;
            this.Direction.X = (float)Math.Cos(this.rndService.NextInt());
            this.Direction.Y = (float)Math.Cos(this.rndService.NextInt());
            this.Direction.Z = (float)Math.Cos(this.rndService.NextInt());
            this.color = this.GetRandomColor();
        }

        /// <summary>
        /// Gets a random color.
        /// </summary>
        /// <returns>A random color</returns>
        private Color GetRandomColor()
        {
            Color color = new Color();

            color.R = (byte)this.rndService.Next(255);
            color.G = (byte)this.rndService.Next(255);
            color.B = (byte)this.rndService.Next(255);
            return color;
        }

        /// <summary>
        /// Executes a simple movement.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public override void Move(TimeSpan gameTime)
        {
            var color = this.Owner.FindComponent<MaterialsMap>();
            ((StandardMaterial)color.DefaultMaterial).DiffuseColor = this.color;

            this.time += gameTime;
            if (this.time.Seconds > 1)
            {
                this.time = TimeSpan.Zero;
                this.Direction.X +=
                    MathHelper.Lerp(-.55f, .55f, (float)this.rndService.NextDouble());
                this.Direction.Y +=
                 MathHelper.Lerp(-.55f, .55f, (float)this.rndService.NextDouble());
                this.Direction.Z +=
                    MathHelper.Lerp(-.55f, .55f, (float)this.rndService.NextDouble());

                if (this.Direction != Vector3.Zero)
                {
                    this.Direction.Normalize();
                }
            }

            this.transform.Position += this.Direction * this.NormalVelocity;
        }        
    }
}
