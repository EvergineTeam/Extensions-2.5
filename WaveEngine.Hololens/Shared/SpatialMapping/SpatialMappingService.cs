#region File Description
//-----------------------------------------------------------------------------
// SpatialMappingService
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using statements
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WaveEngine.Common;
using WaveEngine.Common.Math;
#endregion

namespace WaveEngine.Hololens.SpatialMapping
{
    /// <summary>
    /// Spatial Mapping Observer states.
    /// </summary>
    public enum ObserverStates
    {
        /// <summary>
        /// The SurfaceObserver is currently running.
        /// </summary>
        Running = 0,

        /// <summary>
        /// The SurfaceObserver is currently idle.
        /// </summary>
        Stopped = 1
    }

    /// <summary>
    /// Spatial mapping surface changes
    /// </summary>
    public enum SurfaceChange
    {
        /// <summary>
        /// The surface is just added
        /// </summary>
        Added,

        /// <summary>
        /// The surface has been updated
        /// </summary>
        Updated,

        /// <summary>
        /// The surface has been removed
        /// </summary>
        Removed,
    }

    /// <summary>
    /// The spatial mapping manager service
    /// </summary>
    public class SpatialMappingService : Service, IDisposable
    {
        /// <summary>
        /// Instance of SpatialInteractionManager.
        /// </summary>
        private ISpatialMapping spatialMappingManager;

        /// <summary>
        /// Wether this instance has been disposed.
        /// </summary>
        private bool disposed;
        
        /// <summary>
        /// Handles the SurfaceObserver's OnSurfaceChanged event. 
        /// </summary>
        /// <param name="id">The identifier assigned to the surface which has changed.</param>
        /// <param name="surface">The surface</param>
        /// <param name="changeType">The type of change that occurred on the surface.</param>
        /// <param name="updateTime">The date and time at which the change occurred.</param>
        public delegate void OnSurfaceChangedHandler(Guid id, SpatialMappingSurface surface, SurfaceChange changeType, DateTimeOffset updateTime);

        #region Properties
        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected
        {
            get
            {
                return this.spatialMappingManager != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="SpatialMappingService" /> is disposed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if disposed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisposed
        {
            get { return this.disposed; }
        }

        /// <summary>
        /// Gets or sets the extents of the observation volume
        /// </summary>
        public Vector3 Extents
        {
            get
            {
                if (!this.IsConnected)
                {
                    throw new InvalidOperationException("Hololens is not available");
                }

                return this.spatialMappingManager.Extents;
            }

            set
            {
                if (!this.IsConnected)
                {
                    throw new InvalidOperationException("Hololens is not available");
                }

                this.spatialMappingManager.Extents = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of triangles to calculate per cubic meter.
        /// </summary>
        public float TrianglesPerCubicMeter
        {
            get
            {
                if (!this.IsConnected)
                {
                    throw new InvalidOperationException("Hololens is not available");
                }

                return this.spatialMappingManager.TrianglesPerCubicMeter;
            }

            set
            {
                if (!this.IsConnected)
                {
                    throw new InvalidOperationException("Hololens is not available");
                }

                this.spatialMappingManager.TrianglesPerCubicMeter = value;
            }
        }

        public bool ObtainNormals
        {
            get
            {
                if (!this.IsConnected)
                {
                    throw new InvalidOperationException("Hololens is not available");
                }

                return this.spatialMappingManager.ObtainNormals;
            }

            set
            {
                if (!this.IsConnected)
                {
                    throw new InvalidOperationException("Hololens is not available");
                }

                this.spatialMappingManager.ObtainNormals = value;
            }
        }

        /// <summary>
        /// Gets the spatial mapping surface meshes
        /// </summary>
        public IReadOnlyDictionary<Guid, SpatialMappingSurface> Surfaces
        {
            get;
            private set;
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="SpatialMappingService"/> class.
        /// </summary>
        public SpatialMappingService()
        {
#if UWP
            this.spatialMappingManager = new SpatialMappingManagerUWP();
            this.Surfaces = new ReadOnlyDictionary<Guid, SpatialMappingSurface>(this.spatialMappingManager.Surfaces);
#endif
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="SpatialMappingService"/> class.
        /// </summary>
        ~SpatialMappingService()
        {
            this.Dispose(false);
        }
        #endregion

        #region Public Methods    
        /// <summary>
        /// Process all spatial surfaces
        /// </summary>
        /// <param name="handler">The handler to receive the surface information</param>
        /// <param name="ignorePrevious">Ignore previous surfaces</param>
        public void UpdateSurfaces(OnSurfaceChangedHandler handler, bool ignorePrevious = false)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }

            if (!this.IsConnected)
            {
                throw new InvalidOperationException("Hololens is not available");
            }

            this.spatialMappingManager.UpdateSurfaces(handler, ignorePrevious);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Initialize all resources used by this instance.
        /// </summary>
        protected override void Initialize()
        {
            if (this.IsConnected)
            {
                this.spatialMappingManager.Initialize();
            }
        }

        /// <summary>
        /// Clean all the resources used by this instance.
        /// </summary>
        protected override void Terminate()
        {
            if (this.IsConnected)
            {
                this.spatialMappingManager.Terminate();
            }

            this.Dispose();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.IsConnected)
                    {
                        this.spatialMappingManager.Dispose();
                        this.spatialMappingManager = null;
                    }
                }

                this.disposed = true;
            }
        }
        #endregion
    }
}
