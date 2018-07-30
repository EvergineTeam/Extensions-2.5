// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Usings Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.Vuforia
{
    /// <summary>
    /// A component that binds a generic Vuforia target with an entity
    /// </summary>
    [DataContract]
    public class ARTrackableBehavior : Behavior
    {
        /// <summary>
        /// The vuforia service
        /// </summary>
        protected VuforiaService vuforiaService;

        /// <summary>
        /// The transform 3D
        /// </summary>
        [RequiredComponent]
        protected Transform3D transform;

        private bool isStaticDirty;

        [DataMember]
        private bool isStatic;

        /// <summary>
        /// Gets or sets the trackable name that match with this entity.
        /// </summary>
        [DataMember]
        [RenderPropertyAsSelector(
            "AvailableTrackables",
            CustomPropertyName = "Trackable Name",
            Tooltip = "The trackable name that match with this entity")]
        public string TrackableName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity pose will only be set the first time the target is tracked.
        /// This is only used when <see cref="WorldCenterMode.Camera"/> tracking mode is active."/>.
        /// </summary>
        /// <value>
        /// <c>true</c> if the entity pose will only be set the first time the target is tracked; otherwise, <c>false</c>.
        /// </value>
        [RenderProperty(
            CustomPropertyName = "Is Static",
            Tooltip = "Indicates whether the entity pose will only be set the first time the target is tracked. This is only used when 'WorldCenterMode.Camera' tracking mode is active")]
        public bool IsStatic
        {
            get
            {
                return this.isStatic;
            }

            set
            {
                if (this.isStatic != value)
                {
                    this.isStatic = value;
                    this.isStaticDirty = true;
                }
            }
        }

        /// <summary>
        /// Gets enumerable that contains the names of the trackables described by the dataset.
        /// </summary>
        [DontRenderProperty]
        public IEnumerable<string> AvailableTrackables
        {
            get
            {
                return this.vuforiaService?.ParseTrackableNames().Keys;
            }
        }

        /// <summary>
        /// Gets the TrackableResult that match with this entity.
        /// </summary>
        [DontRenderProperty]
        public TrackableResult MatchedTrackableResult
        {
            get
            {
                var detectedResults = this.vuforiaService.TrackableResults;
                if (detectedResults != null)
                {
                    return this.FindMatchedTrackable(detectedResults);
                }

                return null;
            }
        }

        /// <summary>
        /// Set the default values for the components members
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();

            this.isStatic = false;
            this.isStaticDirty = true;
        }

        /// <summary>
        /// Resolves the dependencies needed for this instance to work.
        /// </summary>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.vuforiaService = WaveServices.GetService<VuforiaService>();
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <remarks>
        /// This is only executed if the instance is active.
        /// </remarks>
        protected override void Update(TimeSpan gameTime)
        {
            if (this.vuforiaService != null &&
                this.vuforiaService.IsSupported &&
                this.vuforiaService.State == ARState.Tracking)
            {
                this.UpdateTransform();
            }
        }

        private void UpdateTransform()
        {
            if (this.vuforiaService.WorldCenterMode == WorldCenterMode.Camera &&
                (!this.isStatic || this.isStaticDirty))
            {
                var activeARCamera = this.vuforiaService.ActiveCamera;
                var matchedTrackable = this.MatchedTrackableResult;

                if (activeARCamera != null &&
                    matchedTrackable != null)
                {
                    var cameraPose = activeARCamera.Transform.WorldTransform;
                    var trackablePose = matchedTrackable.Pose * cameraPose;

                    this.transform.Position = trackablePose.Translation;
                    this.transform.Scale = trackablePose.Scale;
                    this.transform.Orientation = trackablePose.Orientation;

                    this.isStaticDirty = false;
                }
            }
        }

        /// <summary>
        /// Returns the TrackableResult that match this trackable.
        /// </summary>
        /// <param name="detectedTrackables">The detected trackables.</param>
        /// <returns>The TrackableResult that match this trackable.</returns>
        internal virtual TrackableResult FindMatchedTrackable(IEnumerable<TrackableResult> detectedTrackables)
        {
            return detectedTrackables.FirstOrDefault(tr => tr.Trackable.Name == this.TrackableName);
        }
    }
}
