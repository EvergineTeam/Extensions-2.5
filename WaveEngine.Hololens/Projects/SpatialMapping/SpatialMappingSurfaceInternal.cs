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
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Graphics.VertexFormats;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Services;
using WaveEngine.Hololens.Utilities;
using Windows.Perception.Spatial.Surfaces;
#endregion

namespace WaveEngine.Hololens.SpatialMapping
{
    /// <summary>
    /// The hololens surface
    /// </summary>
    public class SpatialMappingSurfaceInternal : SpatialMappingSurface
    {
        #region Properties
        /// <summary>
        /// The raw hololens provided surface.
        /// </summary>
        internal SpatialSurfaceMesh InternalSurface;

        #endregion

        #region Private Methods
        /// <summary>
        /// Update surface mesh
        /// </summary>        
        /// <param name="spatialSurfaceMesh">The mesh</param>
        internal void UpdateSurfaceMesh(SpatialSurfaceMesh spatialSurfaceMesh)
        {
            this.InternalSurface = spatialSurfaceMesh;

            // Creates the index buffer
            IndexBuffer indexBuffer = this.ComputeIndexbuffer(spatialSurfaceMesh);

            // Creates the VertexBuffer
            VertexBuffer vertexBuffer;
            if (spatialSurfaceMesh.VertexNormals == null)
            {
                vertexBuffer = this.ComputePositionMesh(spatialSurfaceMesh);
            }
            else
            {
                vertexBuffer = this.ComputePositionNormalMesh(spatialSurfaceMesh);
            }

            this.ResetMesh();

            this.Mesh = new Mesh(vertexBuffer, indexBuffer, PrimitiveType.TriangleList)
            {
                DisableBatch = true
            };

            this.UpdateTime = spatialSurfaceMesh.SurfaceInfo.UpdateTime;

            // Update the surface transform
            Vector3 scale = spatialSurfaceMesh.VertexPositionScale.ToWave();
            Quaternion orientation = Quaternion.Identity;
            Vector3 position = Vector3.One;

            var referenceTransform = spatialSurfaceMesh.CoordinateSystem.TryGetTransformTo(WaveServices.GetService<HololensService>().ReferenceFrame.CoordinateSystem);
            if (referenceTransform.HasValue)
            {
                Matrix transform;
                referenceTransform.Value.ToWave(out transform);
                position = transform.Translation;
                orientation = Quaternion.CreateFromRotationMatrix(transform);
            }

            this.Position = position;
            this.Orientation = orientation;
            this.Scale = scale;
        }

        /// <summary>
        /// Compute index buffer
        /// </summary>
        /// <param name="surfaceMesh">The mesh</param>
        private IndexBuffer ComputeIndexbuffer(SpatialSurfaceMesh surfaceMesh)
        {
            var surfaceIndices = surfaceMesh.TriangleIndices;
            var indices = new ushort[surfaceIndices.ElementCount];

            using (var stream = surfaceIndices.Data.AsStream())
            using (var dataReader = new BinaryReader(stream))
            {
                for (int i = 0; i < indices.Length; i++)
                {
                    indices[i] = (ushort)dataReader.ReadInt16();
                }
            }

            return new IndexBuffer(indices);
        }

        /// <summary>
        /// Compute position mesh
        /// </summary>
        /// <param name="surfaceMesh">The position mesh</param>
        private VertexBuffer ComputePositionMesh(SpatialSurfaceMesh surfaceMesh)
        {
            var surfacePositions = surfaceMesh.VertexPositions;
            VertexBuffer vertexBuffer = new VertexBuffer(VertexPositionW.VertexFormat);
            vertexBuffer.SetData(surfacePositions.Data.ToArray(), (int)surfacePositions.ElementCount);

            return vertexBuffer;
        }

        /// <summary>
        /// Compute position normal mesh
        /// </summary>
        /// <param name="surfaceMesh">The position mesh</param>
        private VertexBuffer ComputePositionNormalMesh(SpatialSurfaceMesh surfaceMesh)
        {
            var surfacePositions = surfaceMesh.VertexPositions;
            var surfaceNormals = surfaceMesh.VertexNormals;
            VertexBuffer vertexBuffer = new VertexBuffer(VertexPositionNormal.VertexFormat);

            VertexPositionNormal[] vertices = new VertexPositionNormal[surfacePositions.ElementCount];

            using (var positionStream = surfacePositions.Data.AsStream())
            using (var normalStream = surfaceNormals.Data.AsStream())
            using (var positionReader = new BinaryReader(positionStream))
            using (var normalReader = new BinaryReader(normalStream))
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    // Read position
                    vertices[i].Position.X = positionReader.ReadSingle();
                    vertices[i].Position.Y = positionReader.ReadSingle();
                    vertices[i].Position.Z = positionReader.ReadSingle();
                    positionReader.ReadSingle(); // Unused 4th float

                    // Read normal
                    vertices[i].Normal.X = normalReader.ReadSingle();
                    vertices[i].Normal.Y = normalReader.ReadSingle();
                    vertices[i].Normal.Z = normalReader.ReadSingle();
                    normalReader.ReadSingle(); // Unused 4th float                        
                }
            }

            vertexBuffer.SetData(vertices);

            return vertexBuffer;
        }
        #endregion
    }
}
