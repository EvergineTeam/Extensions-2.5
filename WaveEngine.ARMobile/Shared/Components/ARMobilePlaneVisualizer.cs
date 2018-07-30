// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace WaveEngine.ARMobile.Components
{
    /// <summary>
    /// Component that visualizes the AR mobile planes
    /// </summary>
    [DataContract]
    public class ARMobilePlaneVisualizer : Component
    {
        private const string SubplaneEntityName = "subplane";

        private ARMobileService arService;

        /// <summary>
        /// Gets or sets the prefab path for the plane
        /// </summary>
        [RenderPropertyAsAsset(AssetType.Prefab)]
        [DataMember]
        public string PrefabPath { get; set; }

        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();

            if (ARMobileService.GetService(out this.arService))
            {
                this.arService.AddedAnchor += this.OnAddedAnchor;
                this.arService.UpdatedAnchor += this.OnUpdatedAnchor;
                this.arService.RemovedAnchor += this.OnRemovedAnchor;
            }
        }

        private void OnAddedAnchor(object sender, IEnumerable<ARMobileAnchor> addedAnchors)
        {
            foreach (var anchor in addedAnchors)
            {
                if (!(anchor is ARMobilePlaneAnchor))
                {
                    continue;
                }

                var name = anchor.Id.ToString();

                var entity = new Entity(name)
                    .AddComponent(new Transform3D());

                var plane = this.EntityManager.Instantiate(this.PrefabPath);
                plane.Name = SubplaneEntityName;
                entity.AddChild(plane);

                this.UpdateAnchorEntity(entity, anchor);

                this.Owner.AddChild(entity);
            }
        }

        private void OnUpdatedAnchor(object sender, IEnumerable<ARMobileAnchor> updatedAnchors)
        {
            foreach (var anchor in updatedAnchors)
            {
                if (!(anchor is ARMobilePlaneAnchor))
                {
                    continue;
                }

                var name = anchor.Id.ToString();

                var entity = this.Owner.FindChild(name);
                if (entity == null)
                {
                    continue;
                }

                this.UpdateAnchorEntity(entity, anchor);
            }
        }

        private void OnRemovedAnchor(object sender, IEnumerable<ARMobileAnchor> e)
        {
            foreach (var anchor in e)
            {
                var name = anchor.Id.ToString();
                this.Owner.RemoveChild(name);
            }
        }

        private void UpdateAnchorEntity(Entity entity, ARMobileAnchor anchor)
        {
            var transform = entity.FindComponent<Transform3D>();
            transform.LocalPosition = anchor.Transform.Translation;
            transform.LocalOrientation = anchor.Transform.Orientation;
            transform.LocalScale = anchor.Transform.Scale;

            var planeAnchor = anchor as ARMobilePlaneAnchor;
            if (planeAnchor == null)
            {
                return;
            }

            var plane = entity.FindChild(SubplaneEntityName);

            if (plane == null)
            {
                return;
            }

            var planeTransform = plane.FindComponent<Transform3D>();
            planeTransform.LocalPosition = planeAnchor.Center;
            planeTransform.LocalScale = planeAnchor.Size;
        }
    }
}
