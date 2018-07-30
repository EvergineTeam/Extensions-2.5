// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using statements
using System;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Graphics.VertexFormats;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using System.Runtime.Serialization;
using Spine;
using System.Collections.Generic;
using WaveEngine.Materials;
using SpineBlendMode = Spine.BlendMode;
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

        #region Cached fields
        #endregion

        #region Properties

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
        /// Gets or sets the distance between slots.
        /// </summary>
        [DataMember]
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
        /// <param name="layerId">Type of the layer.</param>
        public SkeletalRenderer(int layerId)
            : base("SkeletalRenderer" + instances++, layerId)
        {
        }

        /// <summary>
        /// Default values
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.Transform2D = null;
            this.quadIndices = new ushort[6] { 0, 3, 1, 1, 3, 2 };
            this.ZOrderBias = 0.0001f;
        }

        /// <summary>
        /// Resolve dependencies
        /// </summary>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.SkeletalAnimation.OnAnimationRefresh -= this.SkeletalAnimation_OnAnimationRefresh;
            this.SkeletalAnimation.OnAnimationRefresh += this.SkeletalAnimation_OnAnimationRefresh;
        }

        /// <summary>
        /// Delete dependencies
        /// </summary>
        protected override void DeleteDependencies()
        {
            this.SkeletalAnimation.OnAnimationRefresh -= this.SkeletalAnimation_OnAnimationRefresh;

            base.DeleteDependencies();
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

            float opacity = this.Transform2D.GlobalOpacity;
            if (this.RenderManager.ShouldDrawFlag(Framework.Managers.DebugLinesFlags.DebugAlphaOpacity))
            {
                opacity *= DebugAlpha;
            }

            int numVertices = 0;
            int numPrimitives = 0;

            VertexPositionColorTexture tempVertex;
            StandardMaterial material = null;

            // Process Mesh
            for (int i = 0; i < this.drawOrder.Count; i++)
            {
                var slot = this.drawOrder.Items[i];
                var attachment = slot.Attachment;
                var skeleton = this.SkeletalAnimation.Skeleton;

                if (attachment is RegionAttachment)
                {
                    RegionAttachment regionAttachment = attachment as RegionAttachment;

                    byte a = (byte)(skeleton.A * 255 * slot.A * regionAttachment.A * opacity);
                    byte r = (byte)(skeleton.R * slot.R * regionAttachment.R * a);
                    byte g = (byte)(skeleton.G * slot.G * regionAttachment.G * a);
                    byte b = (byte)(skeleton.B * slot.B * regionAttachment.B * a);

                    if (slot.Data.BlendMode == SpineBlendMode.additive)
                    {
                        a = 0;
                    }

                    Color color = new Color(r, g, b, a);

                    float[] uvs = regionAttachment.UVs;

                    var region = (AtlasRegion)regionAttachment.RendererObject;
                    material = (StandardMaterial)region.page.rendererObject;

                    var computedVertices = this.ComputeAttachmentVertices(regionAttachment, slot);
                    this.vertices = new VertexPositionColorTexture[4];

                    // Vertex TL
                    tempVertex.Position = computedVertices[0];
                    tempVertex.Color = color;
                    tempVertex.TexCoord.X = uvs[RegionAttachment.X1];
                    tempVertex.TexCoord.Y = uvs[RegionAttachment.Y1];
                    this.vertices[0] = tempVertex;

                    // Vertex TR
                    tempVertex.Position = computedVertices[1];
                    tempVertex.Color = color;
                    tempVertex.TexCoord.X = uvs[RegionAttachment.X4];
                    tempVertex.TexCoord.Y = uvs[RegionAttachment.Y4];
                    this.vertices[1] = tempVertex;

                    // Vertex BR
                    tempVertex.Position = computedVertices[2];
                    tempVertex.Color = color;
                    tempVertex.TexCoord.X = uvs[RegionAttachment.X3];
                    tempVertex.TexCoord.Y = uvs[RegionAttachment.Y3];
                    this.vertices[2] = tempVertex;

                    // Vertex BL
                    tempVertex.Position = computedVertices[3];
                    tempVertex.Color = color;
                    tempVertex.TexCoord.X = uvs[RegionAttachment.X2];
                    tempVertex.TexCoord.Y = uvs[RegionAttachment.Y2];
                    this.vertices[3] = tempVertex;

                    numVertices = 4;
                    numPrimitives = 2;
                    this.indices = this.quadIndices;
                }
                else if (attachment is MeshAttachment)
                {
                    var mesh = (MeshAttachment)attachment;

                    byte a = (byte)(skeleton.A * 255 * slot.A * mesh.A * opacity);
                    byte r = (byte)(skeleton.R * slot.R * mesh.R * a);
                    byte g = (byte)(skeleton.G * slot.G * mesh.G * a);
                    byte b = (byte)(skeleton.B * slot.B * mesh.B * a);

                    if (slot.Data.BlendMode == SpineBlendMode.additive)
                    {
                        a = 0;
                    }

                    Color color = new Color(r, g, b, a);

                    numVertices = mesh.Vertices.Length;
                    this.indices = this.CopyIndices(mesh.Triangles);
                    numPrimitives = this.indices.Length / 3;

                    var region = (AtlasRegion)mesh.RendererObject;
                    material = (StandardMaterial)region.page.rendererObject;

                    var computedVertices = this.ComputeAttachmentVertices(mesh, slot);
                    this.vertices = new VertexPositionColorTexture[numVertices / 2];

                    float[] uvs = mesh.UVs;
                    for (int v = 0, j = 0; v < numVertices; v += 2, j++)
                    {
                        tempVertex.Position = computedVertices[j];
                        tempVertex.Color = color;
                        tempVertex.TexCoord.X = uvs[v];
                        tempVertex.TexCoord.Y = uvs[v + 1];
                        this.vertices[j] = tempVertex;
                    }
                }
                else if (attachment is WeightedMeshAttachment)
                {
                    var mesh = (WeightedMeshAttachment)attachment;

                    byte a = (byte)(skeleton.A * 255 * slot.A * mesh.A * opacity);
                    byte r = (byte)(skeleton.R * slot.R * mesh.R * a);
                    byte g = (byte)(skeleton.G * slot.G * mesh.G * a);
                    byte b = (byte)(skeleton.B * slot.B * mesh.B * a);

                    if (slot.Data.BlendMode == SpineBlendMode.additive)
                    {
                        a = 0;
                    }

                    Color color = new Color(r, g, b, a);

                    numVertices = mesh.UVs.Length;
                    this.indices = this.CopyIndices(mesh.Triangles);
                    numPrimitives = this.indices.Length / 3;

                    var computedVertices = this.ComputeAttachmentVertices(mesh, slot);

                    var region = (AtlasRegion)mesh.RendererObject;
                    material = (StandardMaterial)region.page.rendererObject;

                    this.vertices = new VertexPositionColorTexture[numVertices / 2];

                    float[] uvs = mesh.UVs;
                    for (int v = 0, j = 0; v < numVertices; v += 2, j++)
                    {
                        tempVertex.Position = computedVertices[j];
                        tempVertex.Color = color;
                        tempVertex.TexCoord.X = uvs[v];
                        tempVertex.TexCoord.Y = uvs[v + 1];
                        this.vertices[j] = tempVertex;
                    }
                }

                if (attachment != null && material != null)
                {
                    bool reset = false;

                    if (this.spineMeshes[i] != null)
                    {
                        if (this.spineMeshes[i].VertexBuffer.VertexCount != this.vertices.Length ||
                            this.spineMeshes[i].IndexBuffer.Data.Length != this.indices.Length)
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
                            new DynamicIndexBuffer(this.indices),
                            PrimitiveType.TriangleList);

                        this.spineMeshes[i] = newMesh;
                    }

                    Mesh mesh = this.spineMeshes[i];
                    mesh.IndexBuffer.SetData(this.indices);
                    this.GraphicsDevice.BindIndexBuffer(mesh.IndexBuffer);
                    mesh.VertexBuffer.SetData(this.vertices);
                    this.GraphicsDevice.BindVertexBuffer(mesh.VertexBuffer);
                    mesh.ZOrder = this.Transform2D.DrawOrder - (i * this.ZOrderBias);
                    mesh.DisableBatch = true;

                    material.LayerId = this.LayerId;

                    Matrix worldTransform = this.Transform2D.WorldTransform;
                    this.RenderManager.DrawMesh(mesh, material, ref worldTransform, false);
                }
            }
        }

        /// <summary>
        /// Refresh Spine meshes
        /// </summary>
        private void RefreshMeshes()
        {
            if (this.drawOrder == null
             || this.drawOrder != this.SkeletalAnimation.Skeleton.DrawOrder)
            {
                this.DisposeMeshes();

                this.drawOrder = this.SkeletalAnimation.Skeleton.DrawOrder;
                this.spineMeshes = new Mesh[this.drawOrder.Count];
            }
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

            if (this.SkeletalData.Atlas == null
             || this.SkeletalAnimation.Skeleton == null
             || this.drawOrder == null)
            {
                return;
            }

            Vector2 start = new Vector2();
            Vector2 end = new Vector2();
            Color color = Color.Red;
            Matrix worldTransform = this.Transform2D.WorldTransform;

            // Draw bones
            if (this.RenderManager.ShouldDrawFlag(Framework.Managers.DebugLinesFlags.Bones))
            {
                foreach (var bone in this.SkeletalAnimation.Skeleton.Bones)
                {
                    if (bone.Parent != null)
                    {
                        start.X = bone.WorldX;
                        start.Y = -bone.WorldY;
                        end.X = (bone.Data.Length * bone.A) + bone.WorldX;
                        end.Y = -(bone.Data.Length * bone.C) - bone.WorldY;

                        Vector2.Transform(ref start, ref worldTransform, out start);
                        Vector2.Transform(ref end, ref worldTransform, out end);

                        this.RenderManager.LineBatch2D.DrawLine(ref start, ref end, ref color, this.Transform2D.DrawOrder);
                    }
                }
            }

            // Draw quads
            color = Color.Yellow;
            for (int i = 0; i < this.drawOrder.Count; i++)
            {
                var slot = this.drawOrder.Items[i];
                var attachment = slot.Attachment;

                var computedVertices = this.ComputeAttachmentVertices(attachment, slot);
                int vertexCount = computedVertices.Count;

                for (int j = 0; j < vertexCount; j++)
                {
                    start = computedVertices[j].ToVector2();

                    if (j < vertexCount - 1)
                    {
                        end = computedVertices[j + 1].ToVector2();
                    }
                    else
                    {
                        end = computedVertices[0].ToVector2();
                    }

                    Vector2.Transform(ref start, ref worldTransform, out start);
                    Vector2.Transform(ref end, ref worldTransform, out end);

                    this.RenderManager.LineBatch2D.DrawLine(ref start, ref end, ref color, this.Transform2D.DrawOrder);
                }
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
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

        private void SkeletalAnimation_OnAnimationRefresh(object sender, EventArgs e)
        {
            this.RefreshMeshes();

            this.UpdateBoundingBox();
        }

        private List<Vector3> ComputeAttachmentVertices(Attachment attachment, Slot slot)
        {
            var result = new List<Vector3>();

            if (attachment is RegionAttachment)
            {
                RegionAttachment regionAttachment = attachment as RegionAttachment;
                float[] spineVertices = new float[8];
                regionAttachment.ComputeWorldVertices(slot.Bone, spineVertices);

                result.Add(new Vector3(
                    spineVertices[RegionAttachment.X1],
                    -spineVertices[RegionAttachment.Y1],
                    0));

                result.Add(new Vector3(
                    spineVertices[RegionAttachment.X4],
                    -spineVertices[RegionAttachment.Y4],
                    0));

                result.Add(new Vector3(
                    spineVertices[RegionAttachment.X3],
                    -spineVertices[RegionAttachment.Y3],
                    0));

                result.Add(new Vector3(
                    spineVertices[RegionAttachment.X2],
                    -spineVertices[RegionAttachment.Y2],
                    0));
            }
            else if (attachment is MeshAttachment || attachment is WeightedMeshAttachment)
            {
                float[] spineVertices;

                if (attachment is MeshAttachment)
                {
                    var mesh = attachment as MeshAttachment;
                    var numVertices = mesh.Vertices.Length;
                    spineVertices = new float[numVertices];
                    mesh.ComputeWorldVertices(slot, spineVertices);
                }
                else
                {
                    var mesh = attachment as WeightedMeshAttachment;
                    var numVertices = mesh.UVs.Length;
                    spineVertices = new float[numVertices];
                    mesh.ComputeWorldVertices(slot, spineVertices);
                }

                for (int v = 0; v < spineVertices.Length; v += 2)
                {
                    result.Add(new Vector3(
                        spineVertices[v],
                        -spineVertices[v + 1],
                        0));
                }
            }

            return result;
        }

        /// <summary>
        /// Updates the bounding box.
        /// </summary>
        private void UpdateBoundingBox()
        {
            var minVertexPosition = new Vector3(float.MaxValue);
            var maxVertexPosition = new Vector3(float.MinValue);

            if (this.drawOrder != null)
            {
                for (int i = 0; i < this.drawOrder.Count; i++)
                {
                    var slot = this.drawOrder.Items[i];
                    var attachment = slot.Attachment;

                    var computedVertices = this.ComputeAttachmentVertices(attachment, slot);

                    for (int j = 0; j < computedVertices.Count; j++)
                    {
                        var vertexPosition = computedVertices[j];
                        Vector3.Min(ref minVertexPosition, ref vertexPosition, out minVertexPosition);
                        Vector3.Max(ref maxVertexPosition, ref vertexPosition, out maxVertexPosition);
                    }
                }

                var width = maxVertexPosition.X - minVertexPosition.X;
                var height = maxVertexPosition.Y - minVertexPosition.Y;

                this.Transform2D.Rectangle = new RectangleF(0, 0, width, height);
                this.Transform2D.Origin = new Vector2(
                    (width - maxVertexPosition.X) / width,
                    (height - maxVertexPosition.Y) / height);
            }
        }
        #endregion
    }
}
