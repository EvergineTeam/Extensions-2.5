#region File Description
//-----------------------------------------------------------------------------
// BaseSpatialMappingManager
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using statements
using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using WaveEngine.Framework;
using WaveEngine.Framework.Physics3D;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.Hololens.SpatialMapping
{
    /// <summary>
    /// The SpatialMappingObserver class encapsulates the SurfaceObserver into an easy to use
    /// object that handles managing the observed surfaces and the rendering of surface geometry.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Hololens.SpatialMapping")]
    public abstract class BaseSpatialMapping : Component
    {
        /// <summary>
        /// The surface entity tag
        /// </summary>
        private const string SurfaceEntityTag = "Surface";

        /// <summary>
        /// The spatial mapping service
        /// </summary>
        private SpatialMappingService spatialMappingService;

        /// <summary>
        /// The update interval
        /// </summary>
        [DataMember]
        private TimeSpan updateInterval;

        /// <summary>
        /// The update surface timer
        /// </summary>
        private Timer updateTimer;

        /// <summary>
        /// Generate colliders for the surfaces
        /// </summary>
        [DataMember]
        private bool generateColliders;

        #region Properties   
        /// <summary>
        /// Gets a value indicating whether the spatial mapping is available
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return this.spatialMappingService != null && this.spatialMappingService.IsConnected;
            }
        }

        /// <summary>
        /// Gets or sets the spatial mapping update interval
        /// </summary>
        public TimeSpan UpdateInterval
        {
            get
            {
                return this.updateInterval;
            }

            set
            {
                this.updateInterval = value;

                if (this.isInitialized)
                {
                    this.RefreshTimer();
                }
            }
        }

        /// <summary>
        /// public 
        /// </summary>
        public bool GenerateColliders
        {
            get
            {
                return this.generateColliders;
            }

            set
            {
                if (value != this.generateColliders)
                {
                    this.generateColliders = value;

                    if (this.isInitialized)
                    {
                        this.RefreshColliders();
                    }
                }
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Sets the default values
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.updateInterval = TimeSpan.Zero;
            this.generateColliders = true;
        }
        #endregion

        #region Public Methods
        #endregion

        #region Private Methods
        /// <summary>
        /// Initialize the component
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            this.spatialMappingService = WaveServices.GetService<SpatialMappingService>();

            if (this.IsConnected)
            {
                // Do a first update
                this.spatialMappingService.UpdateSurfaces(this.OnSurfaceChanged, true);

                // start the timer
                this.RefreshTimer();
            }
        }

        /// <summary>
        /// Start the update timer
        /// </summary>
        private void StartTimer()
        {
            this.updateTimer = WaveServices.TimerFactory.CreateTimer(this.updateInterval, this.UpdateSurfaces, true, this.Owner.Scene);
        }

        /// <summary>
        /// Stop the update timer
        /// </summary>
        private void StopTimer()
        {
            WaveServices.TimerFactory.RemoveTimer(this.updateTimer);
            this.updateTimer = null;
        }

        /// <summary>
        /// Refresh the timer
        /// </summary>
        private void RefreshTimer()
        {
            if (this.updateTimer != null)
            {
                if (this.updateInterval == TimeSpan.Zero)
                {
                    this.StopTimer();
                }
                else
                {
                    this.updateTimer.Interval = this.updateInterval;
                }
            }
            else if (this.updateInterval > TimeSpan.Zero)
            {
                this.StartTimer();
            }
        }

        /// <summary>
        /// Update surfaces
        /// </summary>
        private void UpdateSurfaces()
        {
            this.spatialMappingService.UpdateSurfaces(this.OnSurfaceChanged);
        }

        /// <summary>
        /// Handles the SurfaceObserver's OnSurfaceChanged event. 
        /// </summary>
        /// <param name="id">The identifier assigned to the surface which has changed.</param>
        /// <param name="surface">The surface</param>
        /// <param name="changeType">The type of change that occurred on the surface.</param>
        /// <param name="updateTime">The date and time at which the change occurred.</param>
        protected virtual void OnSurfaceChanged(Guid id, SpatialMappingSurface surface, SurfaceChange changeType, DateTimeOffset updateTime)
        {
            WaveServices.Dispatcher.RunOnWaveThread(() =>
            {
                if (surface.Mesh == null)
                {
                    return;
                }

                Debug.WriteLine("OnSurfaceChanged [" + changeType + "] " + id);

                string entityId = this.GetEntityNameFromSurfaceId(id);

                switch (changeType)
                {
                    case SurfaceChange.Added:
                    case SurfaceChange.Updated:

                        var surfaceEntity = this.Owner.FindChild(entityId);
                        if (surfaceEntity == null)
                        {
                            surfaceEntity = this.CreateNewSurfaceEntity(entityId, surface);
                            surfaceEntity.Tag = SurfaceEntityTag;

                            if (surfaceEntity != null)
                            {
                                this.Owner.AddChild(surfaceEntity);
                            }
                        }
                        else
                        {
                            this.RefreshModel(surface, surfaceEntity);
                            this.UpdateSurfaceEntity(surface, surfaceEntity);
                        }

                        break;

                    case SurfaceChange.Removed:

                        // Remove the child entity
                        this.Owner.RemoveChild(entityId);

                        break;

                    default:
                        break;
                }
            });
        }

        /// <summary>
        /// Gets a surface entity from its surface Id
        /// </summary>
        /// <param name="id">The surface Id</param>
        /// <returns>The entity</returns>
        protected abstract string GetEntityNameFromSurfaceId(Guid id);

        /// <summary>
        /// Creates a new entity from a surface
        /// </summary>
        /// <param name="entityName">The entity Name</param>
        /// <param name="surface">The hololens surface information</param>
        /// <returns>The new entity</returns>
        protected abstract Entity CreateNewSurfaceEntity(string entityName, SpatialMappingSurface surface);

        /// <summary>
        /// Updates a surface entity
        /// </summary>        
        /// <param name="surface">The hololens surface information</param>
        /// <param name="surfaceEntity">The entity to update</param>
        protected abstract void UpdateSurfaceEntity(SpatialMappingSurface surface, Entity surfaceEntity);

        /// <summary>
        /// Refresh the collider of a surface entity
        /// </summary>
        /// <param name="surfaceEntity">The surface entity</param>
        protected abstract void RefreshCollider(Entity surfaceEntity);

        /// <summary>
        /// Refresh the surface mesh
        /// </summary>
        /// <param name="surface">The hololens surface information</param>
        /// <param name="surfaceEntity">The entity to update</param>
        protected abstract void RefreshModel(SpatialMappingSurface surface, Entity surfaceEntity);

        /// <summary>
        /// Refresh the collider generation
        /// </summary>
        protected void RefreshColliders()
        {
            var surfaceEntities = this.Owner.FindAllChildrenByTag(SurfaceEntityTag);
            foreach (var surfaceEntity in surfaceEntities)
            {
                this.RefreshCollider(surfaceEntity);
            }
        }
        #endregion
    }
}
