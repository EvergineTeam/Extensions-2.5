// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using WaveEngine.Common.Math;
#endregion

namespace WaveEngine.MixedReality.SpatialMapping
{
    /// <summary>
    /// Interface for the spatial input manager implementation
    /// </summary>
    internal interface ISpatialMapping : IDisposable
    {
        /// <summary>
        /// Gets or sets the extents of the observation volume
        /// </summary>
        Vector3 Extents
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the number of triangles to calculate per cubic meter.
        /// </summary>
        float TrianglesPerCubicMeter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the normal information must be captured.
        /// </summary>
        bool ObtainNormals
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the surfaces dictionary
        /// </summary>
        IDictionary<Guid, SpatialMappingSurface> Surfaces
        {
            get;
        }

        /// <summary>
        /// Process all spatial surfaces
        /// </summary>
        /// <param name="handler">The handler to receive the surface information</param>
        /// <param name="ignorePrevious">Ignore previous surfaces</param>
        void UpdateSurfaces(SpatialMappingService.OnSurfaceChangedHandler handler, bool ignorePrevious);

        /// <summary>
        /// Initialize all resources used by this instance.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Clean all the resources used by this instance.
        /// </summary>
        void Terminate();
    }
}
