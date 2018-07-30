// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using System.Collections.Generic;
using System.Linq;
using ARKit;
using UIKit;
using WaveEngine.Common.Math;

namespace WaveEngine.ARMobile
{
    /// <summary>
    /// Delegate object for the ARKit.ARSession object used to respond
    /// to events relating to the augmented-reality session.
    /// </summary>
    internal class ARKitSessionDelegate : ARSessionDelegate
    {
        private ARKitService service;

        private bool supportARKitOnePointFive;

        /// <summary>
        /// Initializes a new instance of the <see cref="ARKitSessionDelegate"/> class.
        /// </summary>
        /// <param name="session">The <see cref="ARKitService"/> instance</param>
        public ARKitSessionDelegate(ARKitService session)
        {
            this.service = session;
            this.supportARKitOnePointFive = UIDevice.CurrentDevice.CheckSystemVersion(11, 3);
        }

        /// <inheritdoc />
        public override void CameraDidChangeTrackingState(ARSession session, ARCamera camera)
        {
            this.service.TrackingState = (WaveEngine.Components.AR.ARTrackingState)camera.TrackingState;
        }

        /// <inheritdoc />
        public override void DidUpdateFrame(ARSession session, ARFrame frame)
        {
            this.service.ProcessFrame(frame);
        }

        /// <inheritdoc />
        public override void DidAddAnchors(ARSession session, ARAnchor[] anchors)
        {
            var newAnchors = new List<ARMobileAnchor>();

            foreach (var anchor in anchors)
            {
                var id = new Guid(anchor.Identifier.GetBytes());

                Matrix transform;
                anchor.Transform.ToWave(out transform);

                ARMobileAnchor newAnchor;

                var planeAnchor = anchor as ARPlaneAnchor;
                if (planeAnchor != null)
                {
                    var plane = new ARMobilePlaneAnchor();
                    newAnchor = plane;

                    planeAnchor.Transform.ToWave(out plane.Transform);
                    planeAnchor.Center.ToWave(out plane.Center);
                    planeAnchor.Extent.ToWave(out plane.Size);
                    plane.Type = planeAnchor.Alignment.ToWave();

                    if (this.supportARKitOnePointFive &&
                        planeAnchor.Geometry != null)
                    {
                        planeAnchor.Geometry.GetBoundaryVertices().ToWave(ref plane.BoundaryPolygon);
                    }
                }
                else
                {
                    newAnchor = new ARMobileAnchor();
                }

                newAnchor.Id = id;
                newAnchor.Transform = transform;

                newAnchors.Add(newAnchor);
            }

            this.service.AddAnchors(newAnchors);
        }

        /// <inheritdoc />
        public override void DidUpdateAnchors(ARSession session, ARAnchor[] anchors)
        {
            var updatedAnchors = new List<ARMobileAnchor>();

            foreach (var anchor in anchors)
            {
                var planeAnchor = anchor as ARPlaneAnchor;
                if (planeAnchor != null)
                {
                    var id = new Guid(anchor.Identifier.GetBytes());

                    var plane = this.service.FindAnchor(id) as ARMobilePlaneAnchor;
                    if (plane != null)
                    {
                        planeAnchor.Transform.ToWave(out plane.Transform);
                        planeAnchor.Center.ToWave(out plane.Center);
                        planeAnchor.Extent.ToWave(out plane.Size);
                        plane.Type = planeAnchor.Alignment.ToWave();

                        if (this.supportARKitOnePointFive &&
                            planeAnchor.Geometry != null)
                        {
                            planeAnchor.Geometry.GetBoundaryVertices().ToWave(ref plane.BoundaryPolygon);
                        }
                    }

                    updatedAnchors.Add(plane);
                }
            }

            this.service.UpdatedAnchors(updatedAnchors);
        }

        /// <inheritdoc />
        public override void DidRemoveAnchors(ARSession session, ARAnchor[] anchors)
        {
            var removedAnchorIds = new List<Guid>();

            foreach (var anchor in anchors)
            {
                var id = new Guid(anchor.Identifier.GetBytes());
                removedAnchorIds.Add(id);
            }

            this.service.RemoveAnchors(removedAnchorIds);
        }
    }
}
