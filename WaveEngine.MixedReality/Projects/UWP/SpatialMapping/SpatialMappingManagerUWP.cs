// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using statements
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Graphics.VertexFormats;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Services;
using WaveEngine.MixedReality.Utilities;
using Windows.Perception.Spatial;
using Windows.Perception.Spatial.Surfaces;
using static WaveEngine.MixedReality.SpatialMapping.SpatialMappingService;
#endregion

namespace WaveEngine.MixedReality.SpatialMapping
{
    /// <summary>
    /// The SpatialMappingObserver class encapsulates the SurfaceObserver into an easy to use
    /// object that handles managing the observed surfaces and the rendering of surface geometry.
    /// </summary>
    internal class SpatialMappingManagerUWP : ISpatialMapping
    {
        /// <summary>
        /// Used for gathering real-time Spatial Mapping data on the MixedReality.
        /// </summary>
        private SpatialSurfaceObserver surfaceObserver;

        /// <summary>
        /// The extents of the observation volume
        /// </summary>
        private Vector3 extents;

        /// <summary>
        /// The surface list
        /// </summary>
        private Dictionary<Guid, SpatialMappingSurface> surfaces;

        /// <summary>
        /// The options to process meshes
        /// </summary>
        private SpatialSurfaceMeshOptions surfaceMeshOptions;

        /// <summary>
        /// There are pending surface changes
        /// </summary>
        private bool pendingChanges;

        /// <summary>
        /// Task that update the surfaces
        /// </summary>
        private System.Threading.Tasks.Task updateSurfacesTask;

        #region Properties

        /// <summary>
        /// Gets gets or sets the extents of the observation volume
        /// </summary>
        public IDictionary<Guid, SpatialMappingSurface> Surfaces
        {
            get
            {
                return this.surfaces;
            }
        }

        /// <summary>
        /// Gets or sets the extents of the observation volume
        /// </summary>
        public Vector3 Extents
        {
            get
            {
                return this.extents;
            }

            set
            {
                this.extents = value;
                this.RefreshBoundingVolume();
            }
        }

        /// <summary>
        /// Gets or sets the number of triangles to calculate per cubic meter.
        /// </summary>
        public float TrianglesPerCubicMeter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the surface normals should be obtained.
        /// </summary>
        public bool ObtainNormals
        {
            get;
            set;
        }
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="SpatialMappingManagerUWP"/> class.
        /// </summary>
        public SpatialMappingManagerUWP()
        {
            this.surfaces = new Dictionary<Guid, SpatialMappingSurface>();

            this.extents = Vector3.One * 10.0f;
            this.TrianglesPerCubicMeter = 500;
            this.ObtainNormals = true;
            this.pendingChanges = true;

            this.surfaceMeshOptions = new SpatialSurfaceMeshOptions()
            {
                IncludeVertexNormals = this.ObtainNormals,
                VertexPositionFormat = Windows.Graphics.DirectX.DirectXPixelFormat.R32G32B32A32Float,
                VertexNormalFormat = Windows.Graphics.DirectX.DirectXPixelFormat.R32G32B32A32Float,
                ////TriangleIndexFormat = Windows.Graphics.DirectX.DirectXPixelFormat.R32UInt
            };
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Process all spatial surfaces
        /// </summary>
        /// <param name="handler">The handler to receive the surface information</param>
        /// <param name="ignorePrevious">Ignore previous surfaces</param>
        public void UpdateSurfaces(OnSurfaceChangedHandler handler, bool ignorePrevious)
        {
            ////if (!this.pendingChanges && !ignorePrevious)
            ////{
            ////    return;
            ////}

            this.pendingChanges = false;

            lock (this)
            {
                if (this.updateSurfacesTask != null)
                {
                    return;
                }

                this.updateSurfacesTask = System.Threading.Tasks.Task.Run(() =>
                {
                    try
                    {
                        this.surfaceMeshOptions.IncludeVertexNormals = this.ObtainNormals;

                        var surfaceCollection = this.surfaceObserver.GetObservedSurfaces();

                        foreach (var pair in surfaceCollection)
                        {
                        // Gets the MixedReality surface
                        var id = pair.Key;
                            var surfaceInfo = pair.Value;
                            SpatialMappingSurface surface;
                            SurfaceChange change = SurfaceChange.Updated;

                            if (!this.surfaces.TryGetValue(id, out surface))
                            {
                                surface = new SpatialMappingSurfaceInternal()
                                {
                                    Id = id
                                };

                                this.surfaces.Add(id, surface);
                                change = SurfaceChange.Added;
                            }

                            if (surface.UpdateTime < surfaceInfo.UpdateTime)
                            {
                                this.UpdateSurface(handler, change, surface as SpatialMappingSurfaceInternal, this.TrianglesPerCubicMeter, surfaceInfo, this.surfaceMeshOptions);
                            }
                            else if (ignorePrevious)
                            {
                                if (handler != null)
                                {
                                    handler(id, surface, SurfaceChange.Removed, surface.UpdateTime);
                                }
                            }
                        }

                        var surafacesToRemove = this.surfaces.Where(id => !surfaceCollection.ContainsKey(id.Key)).ToList();
                        foreach (var pair in surafacesToRemove)
                        {
                            var id = pair.Key;
                            var surface = pair.Value;

                            this.surfaces.Remove(id);
                            surface.Dispose();

                            if (handler != null)
                            {
                                handler(id, surface, SurfaceChange.Removed, surface.UpdateTime);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                    }

                    this.updateSurfacesTask = null;
                });
            }
        }
        #endregion

        #region Private Methods

            /// <summary>
            /// Initialize all resources used by this instance.
            /// </summary>
        public async void Initialize()
        {
            // The surface mapping API reads information about the user's environment. The user must
            // grant permission to the app to use this capability of the Windows Holographic device.
            var status = await SpatialSurfaceObserver.RequestAccessAsync();

            if (status != SpatialPerceptionAccessStatus.Allowed)
            {
                return;
            }

            // If status is allowed, we can create the surface observer.
            this.surfaceObserver = new SpatialSurfaceObserver();
            this.surfaceObserver.ObservedSurfacesChanged += this.ObservedSurfacesChanged;
            this.RefreshBoundingVolume();
        }

        private void ObservedSurfacesChanged(SpatialSurfaceObserver sender, object args)
        {
            this.pendingChanges = true;
        }

        /// <summary>
        /// Clean all the resources used by this instance.
        /// </summary>
        public void Terminate()
        {
            this.Dispose();
        }

        /// <summary>
        /// Refresh the volume
        /// </summary>
        private void RefreshBoundingVolume()
        {
            // The surface observer can now be configured as needed.
            // In this example, we specify one area to be observed using an axis-aligned
            // bounding box 20 meters in width and 5 meters in height and centered at the
            // origin.
            SpatialBoundingBox aabb = new SpatialBoundingBox()
            {
                Center = System.Numerics.Vector3.Zero,
                Extents = this.extents.ToSystemNumerics()
            };

            SpatialBoundingVolume bounds = SpatialBoundingVolume.FromBox(WaveServices.GetService<MixedRealityService>().ReferenceFrame.CoordinateSystem, aabb);
            this.surfaceObserver.SetBoundingVolume(bounds);
        }

        /// <summary>
        /// Update the surface
        /// </summary>
        /// <param name="handler">The handler to receive the surface information</param>
        /// <param name="change">The surface change</param>
        /// <param name="surface">The surface to update</param>
        /// <param name="trianglesPerCubicMeter">The max triangles per cubic meter</param>
        /// <param name="surfaceInfo">The surface info</param>
        /// <param name="surfaceOptions">The mesh options</param>
        private async void UpdateSurface(OnSurfaceChangedHandler handler, SurfaceChange change, SpatialMappingSurfaceInternal surface, float trianglesPerCubicMeter, SpatialSurfaceInfo surfaceInfo, SpatialSurfaceMeshOptions surfaceOptions)
        {
            if (surface.IsProcessing)
            {
                return;
            }

            try
            {
                surface.IsProcessing = true;

                // Generate the mesh from the MixedReality surface info
                SpatialSurfaceMesh surfaceMesh = await surfaceInfo.TryComputeLatestMeshAsync(trianglesPerCubicMeter, surfaceOptions);

                if (surfaceMesh != null && surface.UpdateTime < surfaceMesh.SurfaceInfo.UpdateTime)
                {
                    // Update the surface mesh
                    surface.UpdateSurfaceMesh(surfaceMesh);

                    if (handler != null)
                    {
                        handler(surface.Id, surface, change, surface.UpdateTime);
                    }
                }
            }
            finally
            {
                surface.IsProcessing = false;
            }
        }

        /// <summary>
        /// Dispose all unmanaged resources
        /// </summary>
        public void Dispose()
        {
            this.surfaceObserver.ObservedSurfacesChanged -= this.ObservedSurfacesChanged;
            this.surfaceObserver = null;
        }
        #endregion
    }
}
