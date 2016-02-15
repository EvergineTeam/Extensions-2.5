#region File Description
//-----------------------------------------------------------------------------
// HandRig
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
    /// Hierachy of entity that represents a hand.
    /// </summary>
    [DataContract(Namespace = "WaveEngine.LeapMotion.Behaviors")]
    public class HandRig : Behavior
    {
        /// <summary>
        /// Reference to leap motion service.
        /// </summary>
        private LeapMotionService leapService;

        #region Properties
        /// <summary>
        /// Gets the anchor entity in the Bow position.
        /// </summary>
        [DontRenderProperty]
        public Entity ElBowAnchor { get; private set; }

        /// <summary>
        /// Gets the bow transform.
        /// </summary>
        [DontRenderProperty]
        public Transform3D ElBowTransfom { get; private set; }

        /// <summary>
        /// Gets the anchor entity in the wrist position.
        /// </summary>
        [DontRenderProperty]
        public Entity WristAnchor { get; private set; }

        /// <summary>
        /// Gets the wrist transform.
        /// </summary>
        [DontRenderProperty]
        public Transform3D WristTransfom { get; private set; }

        /// <summary>
        /// Gets the anchor entity in the plam position.
        /// </summary>
        [DontRenderProperty]
        public Entity PalmAnchor { get; private set; }

        /// <summary>
        /// Gets the plam transform.
        /// </summary>
        [DontRenderProperty]
        public Transform3D PlamTransfom { get; private set; }

        /// <summary>
        /// Gets the anchor entity in the thumb position.
        /// </summary>
        [DontRenderProperty]
        public Entity ThumbAnchor { get; private set; }

        /// <summary>
        /// Gets the thumb transform.
        /// </summary>
        [DontRenderProperty]
        public Transform3D ThumbTransform { get; private set; }

        /// <summary>
        /// Gets the anchor entity in the index position.
        /// </summary>
        [DontRenderProperty]
        public Entity IndexAnchor { get; private set; }

        /// <summary>
        /// Gets the index transform.
        /// </summary>
        [DontRenderProperty]
        public Transform3D IndexTransform { get; private set; }

        /// <summary>
        /// Gets the anchor entity in the middle position.
        /// </summary>
        [DontRenderProperty]
        public Entity MiddleAnchor { get; private set; }

        /// <summary>
        /// Gets the middle transform.
        /// </summary>
        [DontRenderProperty]
        public Transform3D MiddleTransform { get; private set; }

        /// <summary>
        /// Gets the anchor entity in the ring position.
        /// </summary>
        [DontRenderProperty]
        public Entity RingAnchor { get; private set; }

        /// <summary>
        /// Gets the rig transform.
        /// </summary>
        [DontRenderProperty]
        public Transform3D RingTransform { get; private set; }

        /// <summary>
        /// Gets the anchor entity in the pinky position.
        /// </summary>
        [DontRenderProperty]
        public Entity PinkyAnchor { get; private set; }

        /// <summary>
        /// Gets the pinky transform.
        /// </summary>
        [DontRenderProperty]
        public Transform3D PinkyTransform { get; private set; }

        /// <summary>
        /// Gets the leap hand reference.
        /// </summary>
        [DontRenderProperty]
        public Leap.Hand LeapHand
        {
            get
            {
                if (this.Owner.Name == "LeftHand")
                {
                    return this.leapService.LeftHand;
                }
                else
                {
                    return this.leapService.RightHand;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this hand is active
        /// </summary>
        [DontRenderProperty]
        public bool IsHandActive
        {
            get;
            internal set;
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Default values
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.IsHandActive = false;
        }

        /// <summary>
        /// Initialize this instance.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            this.leapService = WaveServices.GetService<LeapMotionService>();

            if (this.leapService == null)
            {
                throw new InvalidOperationException("You need to register the LeapMotion service.");
            }

            if (!this.leapService.IsReady)
            {
                throw new InvalidOperationException("You need to call to start method before.");
            }

            this.InitializeHierachy();
        }

        #endregion

        #region Public Methods
        #endregion

        #region Private Methods
        /// <summary>
        /// Update the local position of each anchor.
        /// </summary>
        /// <param name="gameTime">the current game time</param>
        protected override void Update(TimeSpan gameTime)
        {
            if (this.LeapHand != null)
            {
                this.ElBowTransfom.LocalPosition = this.LeapHand.Arm.ElbowPosition.ToPositionVector3();
                this.WristTransfom.LocalPosition = this.LeapHand.Arm.WristPosition.ToPositionVector3();
                this.PlamTransfom.LocalPosition = this.LeapHand.PalmPosition.ToPositionVector3();
                this.ThumbTransform.LocalPosition = this.LeapHand.Fingers[0].TipPosition.ToPositionVector3();
                this.IndexTransform.LocalPosition = this.LeapHand.Fingers[1].TipPosition.ToPositionVector3();
                this.MiddleTransform.LocalPosition = this.LeapHand.Fingers[2].TipPosition.ToPositionVector3();
                this.RingTransform.LocalPosition = this.LeapHand.Fingers[3].TipPosition.ToPositionVector3();
                this.PinkyTransform.LocalPosition = this.LeapHand.Fingers[4].TipPosition.ToPositionVector3();
            }
        }

        /// <summary>
        /// Initialize the hierachy of entities.
        /// </summary>
        private void InitializeHierachy()
        {
            if (this.ElBowAnchor == null)
            {
                this.ElBowAnchor = this.ConfigureAnchor(this.Owner, "ElBow");
                this.ElBowTransfom = this.ElBowAnchor.FindComponent<Transform3D>();
            }

            if (this.WristAnchor == null)
            {
                this.WristAnchor = this.ConfigureAnchor(this.Owner, "Wrist");
                this.WristTransfom = this.WristAnchor.FindComponent<Transform3D>();
            }

            if (this.PalmAnchor == null)
            {
                this.PalmAnchor = this.ConfigureAnchor(this.Owner, "Plam");
                this.PlamTransfom = this.PalmAnchor.FindComponent<Transform3D>();
            }

            if (this.ThumbAnchor == null)
            {
                this.ThumbAnchor = this.ConfigureAnchor(this.Owner, "Thumb");
                this.ThumbTransform = this.ThumbAnchor.FindComponent<Transform3D>();
            }

            if (this.IndexAnchor == null)
            {
                this.IndexAnchor = this.ConfigureAnchor(this.Owner, "Index");
                this.IndexTransform = this.IndexAnchor.FindComponent<Transform3D>();
            }

            if (this.MiddleAnchor == null)
            {
                this.MiddleAnchor = this.ConfigureAnchor(this.Owner, "Middle");
                this.MiddleTransform = this.MiddleAnchor.FindComponent<Transform3D>();
            }

            if (this.RingAnchor == null)
            {
                this.RingAnchor = this.ConfigureAnchor(this.Owner, "Ring");
                this.RingTransform = this.RingAnchor.FindComponent<Transform3D>();
            }

            if (this.PinkyAnchor == null)
            {
                this.PinkyAnchor = this.ConfigureAnchor(this.Owner, "Pinky");
                this.PinkyTransform = this.PinkyAnchor.FindComponent<Transform3D>();
            }
        }

        /// <summary>
        /// Configure an anchor entity.
        /// </summary>
        /// <param name="root">The root entity of the new entity.</param>
        /// <param name="name">The name of the new entity.</param>
        /// <returns>The new entity initialized.</returns>
        private Entity ConfigureAnchor(Entity root, string name)
        {
            Entity anchor = root.FindChild(name);

            if (anchor == null)
            {
                anchor = new Entity(name)
                .AddComponent(new Transform3D());

                root.AddChild(anchor);
            }

            return anchor;
        }

        #endregion
    }
}
