#region File Description
//-----------------------------------------------------------------------------
// GazeBehavior
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Hololens.Interaction;
#endregion

namespace WaveEngine.Hololens.Toolkit
{
    /// <summary>
    /// Gaze behavior class.
    /// </summary>
    [DataContract]
    public class GazeBehavior : Behavior
    {
        /// <summary>
        /// Gaze states
        /// </summary>
        public enum States
        {
            /// <summary>
            /// Undetected state
            /// </summary>
            Undetected,

            /// <summary>
            /// Ready state
            /// </summary>
            Ready,

            /// <summary>
            /// Airtap state
            /// </summary>
            AirTap
        };

        /// <summary>
        /// Undetected state event
        /// </summary>
        public event EventHandler UndetectedEvent;

        /// <summary>
        /// Ready state event
        /// </summary>
        public event EventHandler ReadyEvent;

        /// <summary>
        /// AirTap state event
        /// </summary>
        public event EventHandler AirTapEvent;

        [RequiredComponent]
        private Transform3D cursorTransform = null;

        private SpatialInputService spatialInputService;        

        private GazeCollision collisioner;
        private GazeStabilizer stabilizer;
        private GazeIndicator indicator;

        #region Cached
        private Ray ray;
        private Vector3 cursorPosition, cursorTarget;
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets Undetected prefab
        /// </summary>
        [DataMember]
        [RenderPropertyAsAsset(AssetType.Prefab, Tooltip = "Entity shown on Undetected cursor state.")]
        public string UndetectedPrefab { get; set; }

        /// <summary>
        /// Gets or sets Ready prefab
        /// </summary>
        [DataMember]
        [RenderPropertyAsAsset(AssetType.Prefab, Tooltip = "Entity shown on Ready cursor state.")]
        public string ReadyPrefab { get; set; }

        /// <summary>
        /// Gets or sets AirTap prefab
        /// </summary>
        [DataMember]
        [RenderPropertyAsAsset(AssetType.Prefab, Tooltip = "Entity shown on AirTap cursor state.")]
        public string AirTapPrefab { get; set; }

        /// <summary>
        /// Gets or sets cursor distance (meters).
        /// </summary>
        [DataMember]
        [RenderPropertyAsFInput(0.1f, 20, Tooltip = "Maximum gaze distance, in meters, for calculating a hit.")]
        public float Distance { get; set; }

        /// <summary>
        /// Gets or sets cursor keep scale behavior
        /// </summary>
        [DataMember]
        [RenderProperty(Tooltip = "Always maintain cursor scale.")]
        public bool keepScale { get; set; }

        /// <summary>
        /// Gets undetected prefab instance
        /// </summary>
        [DontRenderProperty]
        public Entity UndetectedEntity { get; private set; }

        /// <summary>
        /// Gets ready prefab instance
        /// </summary>
        [DontRenderProperty]
        public Entity ReadyEntity { get; private set; }

        /// <summary>
        /// Gets AirTap prefab instance
        /// </summary>
        [DontRenderProperty]
        public Entity AirTapEntity { get; private set; }

        /// <summary>
        /// Gets cursor current state
        /// </summary>
        [DontRenderProperty]
        public States CurrentState { get; private set; }

        /// <summary>
        /// Gets gaze ray
        /// </summary>
        [DontRenderProperty]
        public Ray GazeRay
        {
            get
            {
                return this.ray;
            }
        }

        #endregion

        /// <summary>
        ///  Default values
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.Distance = 2.0f;
            this.keepScale = false;
        }

        /// <summary>
        /// Resolved dependencies method
        /// </summary>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.Owner.RemoveChild("Undetected");
            this.Owner.RemoveChild("Ready");
            this.Owner.RemoveChild("AirTap");

            if (!string.IsNullOrEmpty(this.UndetectedPrefab))
            {
                this.UndetectedEntity = this.EntityManager.Instantiate(this.UndetectedPrefab);
                this.UndetectedEntity.IsSerializable = false;
                this.Owner.AddChild(this.UndetectedEntity);
            }

            if (!string.IsNullOrEmpty(this.ReadyPrefab))
            {
                this.ReadyEntity = this.EntityManager.Instantiate(this.ReadyPrefab);
                this.ReadyEntity.IsSerializable = false;
                this.Owner.AddChild(this.ReadyEntity);
            }

            if (!string.IsNullOrEmpty(this.AirTapPrefab))
            {
                this.AirTapEntity = this.EntityManager.Instantiate(this.AirTapPrefab);
                this.AirTapEntity.IsSerializable = false;
                this.Owner.AddChild(this.AirTapEntity);
            }

            this.spatialInputService = WaveServices.GetService<SpatialInputService>();
            if (this.spatialInputService == null)
            {
                Debug.WriteLine("You need to register the SpatialInputServices before to use the cursor");
            }

            this.collisioner = this.Owner.FindComponent<GazeCollision>();
            this.stabilizer = this.Owner.FindComponent<GazeStabilizer>();
            this.indicator = this.Owner.FindComponent<GazeIndicator>();
        }

        /// <summary>
        /// Update method
        /// </summary>
        /// <param name="gameTime">game time</param>
        protected override void Update(TimeSpan gameTime)
        {
            if (this.spatialInputService != null && this.spatialInputService.IsConnected)
            {
                SpatialState currentSpatialState = spatialInputService.SpatialState;

                this.UndetectedEntity.IsVisible = false;
                this.ReadyEntity.IsVisible = false;
                this.AirTapEntity.IsVisible = false;

                if (currentSpatialState.IsDetected)
                {
                    if (currentSpatialState.IsSelected)
                    {
                        // Airtap state    
                        if (this.AirTapEntity != null)
                        {
                            this.AirTapEntity.IsVisible = true;
                        }

                        if (this.CurrentState != States.AirTap)
                        {
                            this.CurrentState = States.AirTap;
                            AirTapEvent?.Invoke(this, null);
                        }
                    }
                    else
                    {
                        // Ready state
                        if (this.ReadyEntity != null)
                        {
                            this.ReadyEntity.IsVisible = true;
                        }

                        if (this.CurrentState != States.Ready)
                        {
                            this.CurrentState = States.Ready;
                            ReadyEvent?.Invoke(this, null);
                        }
                    }
                }
                else
                {
                    // Undetected state
                    if (this.UndetectedEntity != null)
                    {
                        this.UndetectedEntity.IsVisible = true;
                    }

                    if (this.CurrentState != States.Undetected)
                    {
                        this.CurrentState = States.Undetected;
                        UndetectedEvent?.Invoke(this, null);
                    }
                }
            }

            // Create ray
            var cameraTransform = this.RenderManager.ActiveCamera3D.Transform;
            this.ray.Position = cameraTransform.Position;
            this.ray.Direction = cameraTransform.WorldTransform.Forward;

            // Calculate cursor position
            this.cursorPosition = this.ray.Position + this.ray.Direction * this.Distance;
            this.cursorTarget = cameraTransform.Position;

            // Calculate collisions
            if (this.collisioner != null)
            {
                var data = this.collisioner.CheckCursorCollision(ref this.ray);
                if (data.Hit)
                {
                    this.cursorPosition = data.Position;
                    if (this.collisioner.NormalAligned)
                    {
                        this.cursorTarget = data.Target;
                    }
                }
            }

            // Stabilizer            
            if (this.stabilizer != null)
            {
                this.cursorPosition = this.stabilizer.UpdateSmoothCursorPosition(this.cursorPosition);
            }

            // Update cursor
            this.cursorTransform.Position = this.cursorPosition;
            this.cursorTransform.LookAt(this.cursorTarget, Vector3.Up);

            // Keep scale          
            if (this.keepScale)
            {
                Vector3 direction = this.cursorPosition - cameraTransform.Position;
                this.cursorTransform.Scale = Vector3.One * (direction.Length() / this.Distance);
            }

            // Indicator
            if (this.indicator != null)
            {
                this.indicator.UpdateIndicator(this.cursorPosition, this.cursorTransform.Scale);
            }
        }
    }
}
