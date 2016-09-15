#region File Description
//-----------------------------------------------------------------------------
// SpatialMappingSurface
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using statements
using System;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.Hololens.SpatialMapping
{
    /// <summary>
    /// The SpatialMappingObserver class encapsulates the SurfaceObserver into an easy to use
    /// object that handles managing the observed surfaces and the rendering of surface geometry.
    /// </summary>
    public class SpatialMappingSurface : IDisposable
    {
        /// <summary>
        /// The surface is processing at this moment
        /// </summary>
        internal bool IsProcessing;

        #region Properties
        /// <summary>
        /// The updated time
        /// </summary>
        public DateTimeOffset UpdateTime { get; internal set; }

        /// <summary>
        /// Gets or sets the mesh Id
        /// </summary>
        public Guid Id { get; internal set; }

        /// <summary>
        /// Gets or sets the surface mesh
        /// </summary>
        public Mesh Mesh { get; internal set; }
        
        /// <summary>
        /// Gets or sets the Bounds of the model
        /// </summary>
        public BoundingBox Bounds { get; internal set; }

        /// <summary>
        /// Gets or sets the Mesh position
        /// </summary>
        public Vector3 Position { get; internal set; }

        /// <summary>
        /// Gets or sets the Mesh scale
        /// </summary>
        public Vector3 Scale { get; internal set; }

        /// <summary>
        /// Gets or sets the Mesh orientation
        /// </summary>
        public Quaternion Orientation { get; internal set; }
        #endregion

        #region Initialization
        #endregion

        #region Public Methods 
        /// <summary>
        /// Reset the VertexBuffer and IndexBuffer of the mesh
        /// </summary>
        internal void ResetMesh()
        {
            if (this.Mesh != null)
            {
                if (this.Mesh.VertexBuffer != null)
                {
                    WaveServices.GraphicsDevice.DestroyVertexBuffer(this.Mesh.VertexBuffer);
                }

                if (this.Mesh.IndexBuffer != null)
                {
                    WaveServices.GraphicsDevice.DestroyIndexBuffer(this.Mesh.IndexBuffer);
                }

                // Reset mesh
                this.Mesh.VertexOffset = 0;
                this.Mesh.NumVertices = 0;
                this.Mesh.IndexOffset = 0;
                this.Mesh.NumPrimitives = 0;
                this.Mesh.VertexBuffer = null;
                this.Mesh.IndexBuffer = null;

                this.Mesh = null;
            }
        }

        /// <summary>
        /// Dispose all unmanaged resources
        /// </summary>
        public void Dispose()
        {
            this.ResetMesh();
            this.Mesh = null;
        }
        #endregion

        #region Private Methods
        #endregion
    }
}
