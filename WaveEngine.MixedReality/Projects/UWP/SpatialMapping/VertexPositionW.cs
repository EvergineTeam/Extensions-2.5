// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;

#endregion

namespace WaveEngine.MixedReality.SpatialMapping
{
    /// <summary>
    /// Represents a vertex with position and 4th float. This is used to store Vector4
    /// </summary>
    public struct VertexPositionW : IBasicVertex
    {
        /// <summary>
        /// Vertex position.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// 4th float.
        /// </summary>
        public float W;

        /// <summary>
        /// Vertex format.
        /// </summary>
        public static readonly VertexBufferFormat VertexFormat;

        #region Properties

        /// <summary>
        /// Gets the vertex format.
        /// </summary>
        VertexBufferFormat IBasicVertex.VertexFormat
        {
            get
            {
                return VertexFormat;
            }
        }
        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="VertexPositionW"/> struct.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="w">The tex coord.</param>
        public VertexPositionW(Vector3 position, float w)
        {
            this.Position = position;
            this.W = w;
        }

        /// <summary>
        /// Initializes static members of the <see cref="VertexPositionW"/> struct.
        /// </summary>
        static VertexPositionW()
        {
            VertexFormat = new VertexBufferFormat(new VertexElementProperties[]
                {
                    new VertexElementProperties(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                    new VertexElementProperties(12, VertexElementFormat.Single, VertexElementUsage.Normal, 0)
                });
        }
        #endregion
    }
}
