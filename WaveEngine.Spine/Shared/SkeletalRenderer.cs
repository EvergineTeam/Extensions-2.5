#region File Description
//-----------------------------------------------------------------------------
// SkeletalRenderer
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using statements
using System;
using System.Collections.Generic;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Graphics.VertexFormats;
using WaveEngine.Common.Math;
using WaveEngine.Spine;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Materials;
using Spine;
using System.Runtime.Serialization;
#endregion

namespace WaveEngine.Spine
{
    /// <summary>
    /// Render a 2D skeletal on screen
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Spine")]
    public class SkeletalRenderer : Drawable2D
    {
        /// <summary>
        /// Number of instances of this component created.
        /// </summary>
        private static int instances;

        /// <summary>
        /// Transform of skeletal model/>.
        /// </summary>
        [RequiredComponent]
        private Transform2D Transform2D = null;

        /// <summary>
        /// The skeletal data
        /// </summary>
        [RequiredComponent]
        private SkeletalData SkeletalData = null;

        /// <summary>
        /// The skeletal animation
        /// </summary>
        [RequiredComponent]
        private SkeletalAnimation SkeletalAnimation = null;

        /// <summary>
        /// The draw order
        /// </summary>
        private ExposedList<Slot> drawOrder;

        /// <summary>
        /// The indices
        /// </summary>
        private ushort[] indices;

        /// <summary>
        /// The vertices
        /// </summary>
        private VertexPositionColorTexture[] vertices;

        /// <summary>
        /// The temporary vertices
        /// </summary>
        private VertexPositionColorTexture tempVertice;
        /// <summary>
        /// The mesh
        /// </summary>
        private Mesh[] spineMeshes;

        /// <summary>
        /// The quad indices
        /// </summary>
        /// 3----2
        /// |    |
        /// 0----1
        private ushort[] quadIndices;

        /// <summary>
        /// The minimum vertex position
        /// </summary>
        private Vector2 minVertexPosition;

        /// <summary>
        /// The maximum vertex position
        /// </summary>
        private Vector2 maxVertexPosition;

        #region Cached fields
        #endregion

        /// <summary>
        /// Debug modes
        /// </summary>
        [Flags]
        public enum DebugMode
        {
            /// <summary>
            /// The none
            /// </summary>
            None = 1,

            /// <summary>
            /// The bones
            /// </summary>
            Bones = 2,

            /// <summary>
            /// The quads
            /// </summary>
            Quads = 4
        }

        #region Properties
        /// <summary>
        /// Gets or sets the sampler mode
        /// </summary>
        [DataMember]
        public AddressMode SamplerMode { get; set; }

        /// <summary>
        /// Gets or sets the color of the debug bones.
        /// </summary>
        /// <value>
        /// The color of the debug bones.
        /// </value>
        [DataMember]
        public Color DebugBonesColor { get; set; }

        /// <summary>
        /// Gets or sets the color of the debug quad.
        /// </summary>
        /// <value>
        /// The color of the debug quad.
        /// </value>
        [DataMember]
        public Color DebugQuadColor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [debug mode].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [debug mode]; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public DebugMode ActualDebugMode { get; set; }

        /// <summary>
        /// Gets or sets the distance between slots.
        /// </summary>
        public float ZOrderBias { get; set; }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="SkeletalRenderer" /> class.
        /// </summary>
        /// <remarks>
        /// This constructor uses Alpha layer.
        /// </remarks>
        public SkeletalRenderer()
            : this(DefaultLayers.Alpha)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SkeletalRenderer" /> class.
        /// </summary>
        /// <param name="layerType">Type of the layer.</param>
        /// <param name="samplerMode">The sampler mode.</param>
        public SkeletalRenderer(Type layerType, AddressMode samplerMode = AddressMode.LinearClamp)
            : base("SkeletalRenderer" + instances++, layerType)
        {
            this.SamplerMode = samplerMode;
        }

        /// <summary>
        /// Default values
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.SamplerMode = AddressMode.LinearClamp;
            this.Transform2D = null;
            this.ActualDebugMode = DebugMode.Bones;
            this.quadIndices = new ushort[6] { 0, 3, 1, 1, 3, 2 };
            this.ZOrderBias = 0.0001f;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Allows to perform custom drawing.
        /// </summary>
        /// <param name="gameTime">The elapsed game time.</param>
        public override void Draw(TimeSpan gameTime)
        {
            if (this.SkeletalData.Atlas == null || this.SkeletalAnimation.Skeleton == null)
            {
                return;
            }

            if (this.SkeletalAnimation.Skeleton != null
                && (this.drawOrder == null || (this.drawOrder != this.SkeletalAnimation.Skeleton.DrawOrder)))
            {
                this.RefreshMeshes();
            }

            float opacity = this.RenderManager.DebugLines ? DebugAlpha : this.Transform2D.GlobalOpacity;

            int numVertices = 0;
            int numPrimitives = 0;

            var rectangle = this.Transform2D.Rectangle;
            float originOffsetX = this.Transform2D.Origin.X * rectangle.Width;
            float originOffsetY = this.Transform2D.Origin.Y * rectangle.Height;

            Material material = null;

            this.ResetBoundingBox();

            // Process Mesh                        
            for (int i = 0; i < this.drawOrder.Count; i++)
            {
                Slot slot = this.drawOrder.Items[i];
                Attachment attachment = slot.Attachment;

                float alpha = this.SkeletalAnimation.Skeleton.A * slot.A * opacity;
                byte r = (byte)(this.SkeletalAnimation.Skeleton.R * slot.R * 255 * alpha);
                byte g = (byte)(this.SkeletalAnimation.Skeleton.G * slot.G * 255 * alpha);
                byte b = (byte)(this.SkeletalAnimation.Skeleton.B * slot.B * 255 * alpha);
                byte a = (byte)(alpha * 255);
                Color color = new Color(r, g, b, a);


                if (attachment is RegionAttachment)
                {
                    RegionAttachment regionAttachment = attachment as RegionAttachment;

                    float[] spineVertices = new float[8];
                    float[] uvs = regionAttachment.UVs;

                    AtlasRegion region = (AtlasRegion)regionAttachment.RendererObject;
                    material = (Material)region.page.rendererObject;

                    regionAttachment.ComputeWorldVertices(slot.Bone, spineVertices);

                    this.vertices = new VertexPositionColorTexture[4];

                    // Vertex TL
                    this.tempVertice.Position.X = originOffsetX + spineVertices[RegionAttachment.X1];
                    this.tempVertice.Position.Y = originOffsetY - spineVertices[RegionAttachment.Y1];
                    this.tempVertice.Position.Z = 0;
                    this.tempVertice.Color = color;
                    this.tempVertice.TexCoord.X = uvs[RegionAttachment.X1];
                    this.tempVertice.TexCoord.Y = uvs[RegionAttachment.Y1];
                    this.vertices[0] = this.tempVertice;
                    this.ComputeBoundingBox(ref this.tempVertice.Position);

                    // Vertex TR
                    this.tempVertice.Position.X = originOffsetX + spineVertices[RegionAttachment.X4];
                    this.tempVertice.Position.Y = originOffsetY - spineVertices[RegionAttachment.Y4];
                    this.tempVertice.Position.Z = 0;
                    this.tempVertice.Color = color;
                    this.tempVertice.TexCoord.X = uvs[RegionAttachment.X4];
                    this.tempVertice.TexCoord.Y = uvs[RegionAttachment.Y4];
                    this.vertices[1] = this.tempVertice;
                    this.ComputeBoundingBox(ref this.tempVertice.Position);

                    // Vertex BR
                    this.tempVertice.Position.X = originOffsetX + spineVertices[RegionAttachment.X3];
                    this.tempVertice.Position.Y = originOffsetY - spineVertices[RegionAttachment.Y3];
                    this.tempVertice.Position.Z = 0;
                    this.tempVertice.Color = color;
                    this.tempVertice.TexCoord.X = uvs[RegionAttachment.X3];
                    this.tempVertice.TexCoord.Y = uvs[RegionAttachment.Y3];
                    this.vertices[2] = this.tempVertice;
                    this.ComputeBoundingBox(ref this.tempVertice.Position);

                    // Vertex BL
                    this.tempVertice.Position.X = originOffsetX + spineVertices[RegionAttachment.X2];
                    this.tempVertice.Position.Y = originOffsetY - spineVertices[RegionAttachment.Y2];
                    this.tempVertice.Position.Z = 0;
                    this.tempVertice.Color = color;
                    this.tempVertice.TexCoord.X = uvs[RegionAttachment.X2];
                    this.tempVertice.TexCoord.Y = uvs[RegionAttachment.Y2];
                    this.vertices[3] = this.tempVertice;
                    this.ComputeBoundingBox(ref this.tempVertice.Position);

                    numVertices = 4;
                    numPrimitives = 2;
                    this.indices = quadIndices;
                }
                else if (attachment is MeshAttachment)
                {
                    MeshAttachment mesh = (MeshAttachment)attachment;

                    numVertices = mesh.Vertices.Length;
                    indices = CopyIndices(mesh.Triangles);
                    numPrimitives = indices.Length / 3;

                    float[] spineVertices = new float[numVertices];
                    mesh.ComputeWorldVertices(slot, spineVertices);

                    AtlasRegion region = (AtlasRegion)mesh.RendererObject;
                    material = (Material)region.page.rendererObject;

                    this.vertices = new VertexPositionColorTexture[numVertices / 2];

                    float[] uvs = mesh.UVs;
                    for (int v = 0, j = 0; v < numVertices; v += 2, j++)
                    {
                        this.tempVertice.Color = color;
                        this.tempVertice.Position.X = originOffsetX + spineVertices[v];
                        this.tempVertice.Position.Y = originOffsetY - spineVertices[v + 1];
                        this.tempVertice.Position.Z = 0;
                        this.tempVertice.TexCoord.X = uvs[v];
                        this.tempVertice.TexCoord.Y = uvs[v + 1];
                        this.vertices[j] = this.tempVertice;
                        this.ComputeBoundingBox(ref this.tempVertice.Position);
                    }
                }
                else if (attachment is SkinnedMeshAttachment)
                {
                    SkinnedMeshAttachment mesh = (SkinnedMeshAttachment)attachment;

                    numVertices = mesh.UVs.Length;
                    indices = CopyIndices(mesh.Triangles);
                    numPrimitives = indices.Length / 3;

                    float[] spineVertices = new float[numVertices];
                    mesh.ComputeWorldVertices(slot, spineVertices);

                    AtlasRegion region = (AtlasRegion)mesh.RendererObject;
                    material = (Material)region.page.rendererObject;

                    this.vertices = new VertexPositionColorTexture[numVertices / 2];

                    float[] uvs = mesh.UVs;
                    for (int v = 0, j = 0; v < numVertices; v += 2, j++)
                    {
                        this.tempVertice.Color = color;
                        this.tempVertice.Position.X = originOffsetX + spineVertices[v];
                        this.tempVertice.Position.Y = originOffsetY - spineVertices[v + 1];
                        this.tempVertice.Position.Z = 0;
                        this.tempVertice.TexCoord.X = uvs[v];
                        this.tempVertice.TexCoord.Y = uvs[v + 1];
                        this.vertices[j] = this.tempVertice;
                        this.ComputeBoundingBox(ref this.tempVertice.Position);
                    }
                }

                if (attachment != null && material != null)
                {
                    bool reset = false;

                    if (this.spineMeshes[i] != null)
                    {
                        if (this.spineMeshes[i].VertexBuffer.VertexCount != vertices.Length ||
                            this.spineMeshes[i].IndexBuffer.Data.Length != indices.Length)
                        {
                            Mesh toDispose = this.spineMeshes[i];
                            this.GraphicsDevice.DestroyIndexBuffer(toDispose.IndexBuffer);
                            this.GraphicsDevice.DestroyVertexBuffer(toDispose.VertexBuffer);

                            reset = true;
                        }
                    }

                    if (this.spineMeshes[i] == null || reset)
                    {
                        Mesh newMesh = new Mesh(
                            0,
                            numVertices,
                            0,
                            numPrimitives,
                            new DynamicVertexBuffer(VertexPositionColorTexture.VertexFormat),
                            new DynamicIndexBuffer(indices),
                            PrimitiveType.TriangleList);

                        this.spineMeshes[i] = newMesh;
                    }

                    Mesh mesh = this.spineMeshes[i];
                    mesh.IndexBuffer.SetData(this.indices);
                    this.GraphicsDevice.BindIndexBuffer(mesh.IndexBuffer);
                    mesh.VertexBuffer.SetData(this.vertices);
                    this.GraphicsDevice.BindVertexBuffer(mesh.VertexBuffer);
                    mesh.ZOrder = this.Transform2D.DrawOrder - (i * this.ZOrderBias);

                    material.LayerType = this.LayerType;
                    material.SamplerMode = this.SamplerMode;

                    Matrix worldTransform = this.Transform2D.WorldTransform;
                    this.RenderManager.DrawMesh(mesh, material, ref worldTransform, false);
                }
            }

            this.UpdateBoundingBox();
        }

        /// <summary>
        /// Refresh Spine meshes
        /// </summary>
        private void RefreshMeshes()
        {
            this.DisposeMeshes();

            this.drawOrder = this.SkeletalAnimation.Skeleton.DrawOrder;
            this.spineMeshes = new Mesh[this.drawOrder.Count];
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Performs further custom initialization for this instance.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Helper method that draws debug lines.
        /// </summary>
        /// <remarks>
        /// This method will only work on debug mode and if RenderManager.DebugLines /&gt;
        /// is set to <c>true</c>.
        /// </remarks>
        protected override void DrawDebugLines()
        {
            base.DrawDebugLines();

            var platform = WaveServices.Platform;

            Vector2 start = new Vector2();
            Vector2 end = new Vector2();
            Color color = Color.Red;
            Matrix worldTransform = this.Transform2D.WorldTransform;

            var rectangle = this.Transform2D.Rectangle;
            float originOffsetX = this.Transform2D.Origin.X * rectangle.Width;
            float originOffsetY = this.Transform2D.Origin.Y * rectangle.Height;

            // Draw bones
            if ((this.ActualDebugMode & DebugMode.Bones) == DebugMode.Bones)
            {
                foreach (var bone in this.SkeletalAnimation.Skeleton.Bones)
                {
                    if (bone.Parent != null)
                    {
                        start.X = originOffsetX + bone.WorldX;
                        start.Y = originOffsetY - bone.WorldY;
                        end.X = originOffsetX + (bone.Data.Length * bone.M00) + bone.WorldX;
                        end.Y = originOffsetY - ((bone.Data.Length * bone.M10) + bone.WorldY);

                        Vector2.Transform(ref start, ref worldTransform, out start);
                        Vector2.Transform(ref end, ref worldTransform, out end);

                        RenderManager.LineBatch2D.DrawLine(ref start, ref end, ref color, this.Transform2D.DrawOrder);
                    }
                }
            }

            // Draw quads
            if ((this.ActualDebugMode & DebugMode.Quads) == DebugMode.Quads)
            {
                color = Color.Yellow;
                for (int i = 0; i < this.drawOrder.Count; i++)
                {
                    Slot slot = this.drawOrder.Items[i];
                    Attachment attachment = slot.Attachment;

                    if (attachment is RegionAttachment)
                    {
                        float[] spineVertices = new float[8];

                        RegionAttachment mesh = (RegionAttachment)attachment;
                        mesh.ComputeWorldVertices(slot.Bone, spineVertices);

                        // Edge1
                        start.X = originOffsetX + spineVertices[RegionAttachment.X1];
                        start.Y = originOffsetY - spineVertices[RegionAttachment.Y1];
                        end.X = originOffsetX + spineVertices[RegionAttachment.X2];
                        end.Y = originOffsetY - spineVertices[RegionAttachment.Y2];

                        Vector2.Transform(ref start, ref worldTransform, out start);
                        Vector2.Transform(ref end, ref worldTransform, out end);

                        RenderManager.LineBatch2D.DrawLine(ref start, ref end, ref color, this.Transform2D.DrawOrder);

                        // Edge2
                        start.X = originOffsetX + spineVertices[RegionAttachment.X2];
                        start.Y = originOffsetY - spineVertices[RegionAttachment.Y2];
                        end.X = originOffsetX + spineVertices[RegionAttachment.X3];
                        end.Y = originOffsetY - spineVertices[RegionAttachment.Y3];

                        Vector2.Transform(ref start, ref worldTransform, out start);
                        Vector2.Transform(ref end, ref worldTransform, out end);

                        RenderManager.LineBatch2D.DrawLine(ref start, ref end, ref color, this.Transform2D.DrawOrder);

                        // Edge3
                        start.X = originOffsetX + spineVertices[RegionAttachment.X3];
                        start.Y = originOffsetY - spineVertices[RegionAttachment.Y3];
                        end.X = originOffsetX + spineVertices[RegionAttachment.X4];
                        end.Y = originOffsetY - spineVertices[RegionAttachment.Y4];

                        Vector2.Transform(ref start, ref worldTransform, out start);
                        Vector2.Transform(ref end, ref worldTransform, out end);

                        RenderManager.LineBatch2D.DrawLine(ref start, ref end, ref color, this.Transform2D.DrawOrder);

                        // Edge4
                        start.X = originOffsetX + spineVertices[RegionAttachment.X4];
                        start.Y = originOffsetY - spineVertices[RegionAttachment.Y4];
                        end.X = originOffsetX + spineVertices[RegionAttachment.X1];
                        end.Y = originOffsetY - spineVertices[RegionAttachment.Y1];

                        Vector2.Transform(ref start, ref worldTransform, out start);
                        Vector2.Transform(ref end, ref worldTransform, out end);

                        RenderManager.LineBatch2D.DrawLine(ref start, ref end, ref color, this.Transform2D.DrawOrder);
                    }
                    else if (attachment is MeshAttachment)
                    {
                        MeshAttachment mesh = (MeshAttachment)attachment;
                        int vertexCount = mesh.Vertices.Length;
                        float[] spineVertices = new float[vertexCount];
                        mesh.ComputeWorldVertices(slot, spineVertices);

                        for (int j = 0; j < vertexCount; j += 2)
                        {
                            start.X = originOffsetX + spineVertices[j];
                            start.Y = originOffsetY - spineVertices[j + 1];

                            if (j < vertexCount - 2)
                            {
                                end.X = originOffsetX + spineVertices[j + 2];
                                end.Y = originOffsetY - spineVertices[j + 3];
                            }
                            else
                            {
                                end.X = originOffsetX + spineVertices[0];
                                end.Y = originOffsetY - spineVertices[1];
                            }

                            Vector2.Transform(ref start, ref worldTransform, out start);
                            Vector2.Transform(ref end, ref worldTransform, out end);

                            RenderManager.LineBatch2D.DrawLine(ref start, ref end, ref color, this.Transform2D.DrawOrder);
                        }
                    }
                    else if (attachment is SkinnedMeshAttachment)
                    {
                        SkinnedMeshAttachment mesh = (SkinnedMeshAttachment)attachment;
                        int vertexCount = mesh.UVs.Length;
                        float[] spineVertices = new float[vertexCount];
                        mesh.ComputeWorldVertices(slot, spineVertices);

                        for (int j = 0; j < vertexCount; j += 2)
                        {
                            start.X = originOffsetX + spineVertices[j];
                            start.Y = originOffsetY - spineVertices[j + 1];

                            if (j < vertexCount - 2)
                            {
                                end.X = originOffsetX + spineVertices[j + 2];
                                end.Y = originOffsetY - spineVertices[j + 3];
                            }
                            else
                            {
                                end.X = originOffsetX + spineVertices[0];
                                end.Y = originOffsetY - spineVertices[1];
                            }

                            Vector2.Transform(ref start, ref worldTransform, out start);
                            Vector2.Transform(ref end, ref worldTransform, out end);

                            RenderManager.LineBatch2D.DrawLine(ref start, ref end, ref color, this.Transform2D.DrawOrder);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.DisposeMeshes();
            }
        }

        /// <summary>
        /// Dispose meshes
        /// </summary>
        private void DisposeMeshes()
        {
            if (this.spineMeshes != null)
            {
                foreach (Mesh spineMesh in this.spineMeshes)
                {
                    if (spineMesh != null)
                    {
                        this.GraphicsDevice.DestroyIndexBuffer(spineMesh.IndexBuffer);
                        this.GraphicsDevice.DestroyVertexBuffer(spineMesh.VertexBuffer);
                    }
                }
            }
        }

        /// <summary>
        /// Convert int[] to ushort[].
        /// </summary>
        /// <param name="fromIndices">int indices array.</param>
        /// <returns>ushort indices array.</returns>
        private ushort[] CopyIndices(int[] fromIndices)
        {
            ushort[] indices = new ushort[fromIndices.Length];

            for (int i = 0; i < indices.Length; i++)
            {
                indices[i] = (ushort)fromIndices[i];
            }

            return indices;
        }

        /// <summary>
        /// Resets the bounding box.
        /// </summary>
        private void ResetBoundingBox()
        {
            this.minVertexPosition = new Vector2(float.MaxValue);
            this.maxVertexPosition = new Vector2(float.MinValue);
        }

        /// <summary>
        /// Computes the bounding box.
        /// </summary>
        /// <param name="vertexPosition">The vertex position.</param>
        private void ComputeBoundingBox(ref Vector3 vertexPosition)
        {
            this.minVertexPosition.X = MathHelper.Min(this.minVertexPosition.X, vertexPosition.X);
            this.minVertexPosition.Y = MathHelper.Min(this.minVertexPosition.Y, vertexPosition.Y);
            this.maxVertexPosition.X = MathHelper.Max(this.maxVertexPosition.X, vertexPosition.X);
            this.maxVertexPosition.Y = MathHelper.Max(this.maxVertexPosition.Y, vertexPosition.Y);
        }

        /// <summary>
        /// Updates the bounding box.
        /// </summary>
        private void UpdateBoundingBox()
        {
            this.Transform2D.Rectangle.X = this.minVertexPosition.X;
            this.Transform2D.Rectangle.Y = this.minVertexPosition.Y;
            this.Transform2D.Rectangle.Width = this.maxVertexPosition.X - this.minVertexPosition.X;
            this.Transform2D.Rectangle.Height = this.maxVertexPosition.Y - this.minVertexPosition.Y;
        }
        #endregion
    }
}
