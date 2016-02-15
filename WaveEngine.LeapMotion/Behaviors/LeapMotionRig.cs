#region File Description
//-----------------------------------------------------------------------------
// LeapMotionRig
//
// Copyright © 2016 Wave Coorporation. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Usings Statements
using System;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;

#endregion

namespace WaveEngine.LeapMotion.Behaviors
{
    /// <summary>
    /// Hierachy of entities to represent a root entity with two hand entities.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.LeapMotion.Behaviors")]
    public class LeapMotionRig : Behavior
    {
        /// <summary>
        /// Pointer to LeapMotion service.
        /// </summary>
        public LeapMotionService LeapService;

        #region Properties
        /// <summary>
        /// Gets the root entity of tracking space.
        /// </summary>
        [DontRenderProperty]
        public Entity TrackingSpace { get; private set; }

        /// <summary>
        /// Gets the transform3D component of TrackingSpace entity.
        /// </summary>
        [DontRenderProperty]
        public Transform3D TrackingSpaceTransform { get; private set; }

        /// <summary>
        /// Gets the entity that represent the left hand.
        /// </summary>
        [DontRenderProperty]
        public Entity LeftHandAnchor { get; private set; }

        /// <summary>
        /// Gets the transform3D of left Hand entity.
        /// </summary>
        [DontRenderProperty]
        public Transform3D LeftHandTransform { get; private set; }

        /// <summary>
        /// Gets the Left hand rig information.
        /// </summary>
        [DontRenderProperty]
        public HandRig LeftHandRig { get; private set; }

        /// <summary>
        /// Gets the entity that represent the right hand.
        /// </summary>
        [DontRenderProperty]
        public Entity RightHandAnchor { get; private set; }

        /// <summary>
        /// Gets the transform3D of right hand entity.
        /// </summary>
        [DontRenderProperty]
        public Transform3D RightHandTransform { get; private set; }

        /// <summary>
        /// Gets the Right hand rig information.
        /// </summary>
        [DontRenderProperty]
        public HandRig RightHandRig { get; private set; }

        #endregion

        #region Initialize
        /// <summary>
        /// Default values
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
        }

        /// <summary>
        /// Initialize the LeapMotionRig entity hierarchy.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            this.LeapService = WaveServices.GetService<LeapMotionService>();

            if (this.LeapService == null)
            {
                Console.WriteLine("You need to register the LeapMotion service.");
            }

            if (!this.LeapService.IsReady)
            {
                Console.WriteLine("You need to call to start method before.");
            }

            if (this.TrackingSpace == null)
            {
                this.TrackingSpace = this.ConfigureRootAnchor("TrackingSpace");                
            }

            this.TrackingSpaceTransform = this.TrackingSpace.FindComponent<Transform3D>();

            if (this.LeftHandAnchor == null)
            {
                this.LeftHandAnchor = this.ConfigureHandAnchor(this.TrackingSpace, "LeftHand");
            }

            this.LeftHandRig = this.LeftHandAnchor.FindComponent<HandRig>();
            this.LeftHandTransform = this.LeftHandAnchor.FindComponent<Transform3D>();

            if (this.RightHandAnchor == null)
            {
                this.RightHandAnchor = this.ConfigureHandAnchor(this.TrackingSpace, "RightHand");
            }

            this.RightHandRig = this.RightHandAnchor.FindComponent<HandRig>();
            this.RightHandTransform = this.RightHandAnchor.FindComponent<Transform3D>();
        }

        #endregion

        #region Public Methods
        #endregion

        #region Private methods
        /// <summary>
        /// Refresh all local positions.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        protected override void Update(TimeSpan gameTime)
        {
            this.LeftHandRig.IsHandActive = this.LeapService.LeftHand != null;
            if (this.LeftHandRig.IsHandActive)
            {
                this.LeftHandTransform.LocalPosition = this.LeapService.LeftHand.Basis.origin.ToPositionVector3();
            }

            this.RightHandRig.IsHandActive = this.LeapService.RightHand != null;
            if (this.RightHandRig.IsHandActive)
            {
                this.RightHandTransform.LocalPosition = this.LeapService.RightHand.Basis.origin.ToPositionVector3();
            }
        }

        /// <summary>
        /// Create the root anchor
        /// </summary>
        /// <param name="name">The name</param>
        /// <returns>The anchor entity</returns>
        private Entity ConfigureRootAnchor(string name)
        {
            Entity root = this.Owner.FindChild(name);

            if (root == null)
            {
                root = new Entity(name)
                    .AddComponent(new Transform3D());

                this.Owner.AddChild(root);
            }

            return root;
        }

        /// <summary>
        /// Create a hand anchor
        /// </summary>
        /// <param name="root">The  root entity.</param>
        /// <param name="name">The name of the entity.</param>
        /// <returns>the new entity.</returns>
        private Entity ConfigureHandAnchor(Entity root, string name)
        {
            Entity anchor = root.FindChild(name);

            if (anchor == null)
            {
                anchor = new Entity(name)
                .AddComponent(new Transform3D())
                .AddComponent(new HandRig());

                root.AddChild(anchor);
            }

            return anchor;
        }
        #endregion
    }
}
