#region File Description
//-----------------------------------------------------------------------------
// SpatialMappingManager 
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using statements
using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Models;
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
    public class SpatialMapping : BaseSpatialMapping
    {
        #region Properties 

        /// <summary>
        /// Gets or sets a value indicating whether the component will use an individual copy of the material file, instead of sharing the material instance.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the material file will be copied otherwise, <c>false</c>.
        /// </value>
        [RenderProperty(Tooltip = "indicating whether the component will use an individual copy of the material file, instead of sharing the material instance.")]
        [DataMember]
        public bool UseMaterialCopy { get; set; }

        /// <summary>
        /// Gets or sets the material path.
        /// </summary>
        [RenderPropertyAsAsset(AssetType.Material)]
        [DataMember]
        public string MaterialPath { get; set; }

        /// <summary>
        /// Indicate whether the mesh will shown or not.
        /// </summary>
        [RenderProperty(Tooltip = "Indicate whether the mesh will shown or not.")]
        [DataMember]
        public bool IsVisible { get; set; }

        #endregion

        #region Initialization

        /// <summary>
        /// Default values method
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.IsVisible = true;
            this.MaterialPath = null;
            this.UseMaterialCopy = false;
        }

        #endregion

        #region Public Methods 
        /// <summary>
        /// Initializes the component
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Gets a surface entity from its surface Id
        /// </summary>
        /// <param name="id">The surface Id</param>
        /// <returns>The entity</returns>
        protected override string GetEntityNameFromSurfaceId(Guid id)
        {
            return id.ToString();
        }

        /// <summary>
        /// Creates a new entity from a surface
        /// </summary>
        /// <param name="entityName">The entity Name</param>
        /// <param name="surface">The hololens surface information</param>
        /// <returns>The new entity</returns>
        protected override Entity CreateNewSurfaceEntity(string entityName, SpatialMappingSurface surface)
        {
            var surfaceEntity = new Entity(entityName)
            {
                IsSerializable = false,
                IsStatic = true
            }
            .AddComponent(new Transform3D());

            if (!string.IsNullOrEmpty(this.MaterialPath))
            {
                surfaceEntity.AddComponent(new MaterialsMap()
                {
                    DefaultMaterialPath = this.MaterialPath,
                    UseMaterialCopy = this.UseMaterialCopy,
                });
            }

            this.RefreshModel(surface, surfaceEntity);
            this.UpdateSurfaceEntity(surface, surfaceEntity);
            this.RefreshCollider(surfaceEntity);

            return surfaceEntity;
        }

        /// <summary>
        /// Updates a surface entity
        /// </summary>        
        /// <param name="surface">The hololens surface information</param>
        /// <param name="surfaceEntity">The entity to update</param>
        protected override void UpdateSurfaceEntity(SpatialMappingSurface surface, Entity surfaceEntity)
        {
            Transform3D transform = surfaceEntity.FindComponent<Transform3D>();
            transform.LocalPosition = surface.Position;
            transform.LocalScale = surface.Scale;
            transform.LocalOrientation = surface.Orientation;
        }

        /// <summary>
        /// Refresh the surface mesh
        /// </summary>
        /// <param name="surface">The hololens surface information</param>
        /// <param name="surfaceEntity">The entity to update</param>
        protected override void RefreshModel(SpatialMappingSurface surface, Entity surfaceEntity)
        {
            if (!surfaceEntity.IsDisposed)
            {
                surfaceEntity.RemoveComponent<Model>()
                    .RemoveComponent<ModelRenderer>()
                    .AddComponent(Model.CreateFromMesh(surface.Mesh));

                if (this.IsVisible && !string.IsNullOrEmpty(this.MaterialPath))
                {
                    surfaceEntity.AddComponent(new ModelRenderer());
                }
            }
        }

        /// <summary>
        /// Refresh the collider of a surface entity
        /// </summary>
        /// <param name="surfaceEntity">The entity to update</param>
        protected override void RefreshCollider(Entity surfaceEntity)
        {
            if (!surfaceEntity.IsDisposed)
            {
                surfaceEntity.RemoveComponent<MeshCollider3D>();
                surfaceEntity.RemoveComponent<RigidBody3D>();

                if (this.GenerateColliders)
                {
                    surfaceEntity.AddComponent(new MeshCollider3D());
                    surfaceEntity.AddComponent(new RigidBody3D() { IsKinematic = true });

                    surfaceEntity.RefreshDependencies();
                }
            }
        }

        #endregion
    }
}
