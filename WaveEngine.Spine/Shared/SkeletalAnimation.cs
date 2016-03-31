#region File Description
//-----------------------------------------------------------------------------
// SkeletalAnimation
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Spine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using WaveEngine.Common.Attributes;
using WaveEngine.Common.Helpers;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.Spine
{
    /// <summary>
    /// Behavior to control skeletal 2D animations
    /// </summary>
    [DataContract(Namespace = "WaveEngine.Spine")]
    public class SkeletalAnimation : Behavior
    {
        /// <summary>
        /// Event raised when an animation raise a Spine event.
        /// </summary>
        public event EventHandler<SpineEvent> EventAnimation;

        /// <summary>
        /// Event raised when an animation has finalized.
        /// </summary>
        public event AnimationState.StartEndDelegate EndAnimation;

        /// <summary>
        /// The skeletal data
        /// </summary>
        [RequiredComponent]
        private SkeletalData SkeletalData = null;

        /// <summary>
        /// The animation path
        /// </summary>
        [DataMember]
        private string animationPath;

        /// <summary>
        /// The current skin
        /// </summary>
        [DataMember]
        private string currentSkin;

        /// <summary>
        /// The state
        /// </summary>
        private AnimationState state;

        /// <summary>
        /// The current animation
        /// </summary>
        private string currentAnimation;

        /// <summary>
        /// Whether animation need a refresh.
        /// </summary>
        private bool animationRefreshFlag;

        #region Properties

        /// <summary>
        /// The skeleton
        /// </summary>
        [DontRenderProperty]
        public Skeleton Skeleton { get; private set; }

        /// <summary>
        /// Gets or sets the speed.
        /// </summary>
        /// <value>
        /// The speed.
        /// </value>
        [DataMember]
        public float Speed { get; set; }

        /// <summary>
        /// Gets or sets the current animation.
        /// </summary>
        /// <value>
        /// The current animation.
        /// </value>
        [RenderPropertyAsSelector("AnimationNames")]
        [DataMember]
        public string CurrentAnimation
        {
            get
            {
                return this.currentAnimation;
            }

            set
            {
                this.currentAnimation = value;

                if (this.state != null && this.PlayAutomatically)
                {
                    this.Play(this.Loop);
                }

            }
        }

        /// <summary>
        /// Gets or sets the Path of Spine animation file (.json)
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        [RenderPropertyAsAsset(AssetType.Unknown)]
        public string AnimationPath
        {
            get
            {
                return this.animationPath;
            }
            set
            {
                this.animationPath = value;
                this.animationRefreshFlag = true;

                if (this.isInitialized)
                {
                    this.RefreshAnimation();
                }
            }
        }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        [DontRenderProperty]
        public AnimationState State
        {
            get
            {
                return this.state;
            }

            set
            {
                if (this.state != null)
                {
                    this.state.End -= this.OnEndAnimation;
                }

                this.state = value;

                if (this.state != null)
                {
                    this.state.End -= this.OnEndAnimation;
                    this.state.End += this.OnEndAnimation;
                }
            }
        }

        /// <summary>
        /// Gets or sets the skin.
        /// </summary>
        /// <value>
        /// The skin.
        /// </value>
        [RenderPropertyAsSelector("SkinNames")]
        public string Skin
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    this.currentSkin = value;

                    if (this.Skeleton != null)
                    {
                        try
                        {
                            this.Skeleton.SetSkin(value);
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(string.Format("The Skin [{0}] is not valid: {2}", value, e.Message));
                        }
                    }
                }
            }

            get
            {
                return this.currentSkin;
            }
        }

        /// <summary>
        /// Gets the names of the different skins.
        /// </summary>
        [DontRenderProperty]
        public IEnumerable<string> SkinNames
        {
            get
            {
                if (this.Skeleton != null)
                {
                    foreach (var skin in this.Skeleton.Data.Skins)
                    {
                        yield return skin.Name;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the names of the different animations.
        /// </summary>
        [DontRenderProperty]
        public IEnumerable<string> AnimationNames
        {
            get
            {
                if (this.Skeleton != null)
                {
                    foreach (var animation in this.Skeleton.Data.Animations)
                    {
                        yield return animation.Name;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the animation is played automatically when the CurrentAnimation is changed
        /// </summary>
        /// <value>
        ///   <c>true</c> if [play automatically]; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool PlayAutomatically { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the animation is looped
        /// </summary>
        /// <value>
        ///   <c>true</c> if [play automatically]; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool Loop { get; set; }

        /// <summary>
        /// A new atlas has been loaded
        /// </summary>
        internal event EventHandler OnAnimationRefresh;
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="SkeletalAnimation" /> class.
        /// </summary>

        public SkeletalAnimation()
            : base("SkeletalAnimation")
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SkeletalAnimation" /> class.
        /// </summary>
        /// <param name="animationPath">The animation path.</param>
        public SkeletalAnimation(string animationPath)
            : base("SkeletalAnimation")
        {
            this.animationPath = animationPath;
        }

        /// <summary>
        /// Sets the default values
        /// </summary>
        protected override void DefaultValues()
        {
            base.DefaultValues();
            this.Speed = 1;
            this.PlayAutomatically = false;
            this.Loop = false;

            this.animationRefreshFlag = true;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Plays this instance.
        /// </summary>
        public void Play()
        {
            this.Play(false);
        }

        /// <summary>
        /// Plays the specified loop.
        /// </summary>
        /// <param name="loop">if set to <c>true</c> [loop].</param>
        public void Play(bool loop)
        {
            if (this.state != null && !string.IsNullOrEmpty(this.currentAnimation))
            {
                this.state.SetAnimation(0, this.currentAnimation, loop);
            }
        }

        /// <summary>
        /// Search if the skeletal animation contains 
        /// </summary>
        /// <param name="animation">Animation name</param>
        /// <returns>Returns true if the skeletal animation contains the animation. False otherwise.</returns>
        public bool ContainsAnimation(string animation)
        {
            if (this.state != null)
            {
                return this.state.Data.SkeletonData.FindAnimation(animation) != null;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Stops the current animation.
        /// </summary>
        public void Stop()
        {
            if (this.state != null)
            {
                this.state.ClearTracks();
                this.Skeleton.Update(0);
            }
        }

        /// <summary>
        /// Set the duration of mix between the two specified animations
        /// </summary>
        /// <param name="fromAnimation">The from animation.</param>
        /// <param name="toAnimation">The to animation.</param>
        /// <param name="duration">The mix duration</param>
        public void SetAnimationMixDuration(string fromAnimation, string toAnimation, float duration)
        {
            if (this.state != null)
            {
                this.state.Data.SetMix(fromAnimation, toAnimation, duration);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Resolves the dependencies.
        /// </summary>
        protected override void ResolveDependencies()
        {
            base.ResolveDependencies();

            this.SkeletalData.OnAtlasRefresh -= this.OnAtlasRefresh;
            this.SkeletalData.OnAtlasRefresh += this.OnAtlasRefresh;
        }

        /// <summary>
        /// Deletes the dependencies.
        /// </summary>
        protected override void DeleteDependencies()
        {
            this.SkeletalData.OnAtlasRefresh -= this.OnAtlasRefresh;
            
            base.DeleteDependencies();
        }

        /// <summary>
        /// Performs further custom initialization for this instance.
        /// </summary>
        /// <remarks>
        /// By default this method does nothing.
        /// </remarks>
        protected override void Initialize()
        {
            base.Initialize();

            this.RefreshAnimation();
            this.CurrentAnimation = this.currentAnimation;
        }

        /// <summary>
        /// Event handler of the animation event.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="trackIndex">Index of the track.</param>
        /// <param name="e">event data.</param>
        private void OnEventAnimation(AnimationState state, int trackIndex, Event e)
        {
            if (this.EventAnimation != null)
            {
                this.EventAnimation(this, new SpineEvent(e));
            }
        }

        /// <summary>
        /// Event handler of the end animation event.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="trackIndex">Index of the track.</param>
        private void OnEndAnimation(AnimationState state, int trackIndex)
        {
            if (this.EndAnimation != null)
            {
                this.EndAnimation(state, trackIndex);
            }
        }

        /// <summary>
        /// Allows this instance to execute custom logic during its <c>Update</c>.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <remarks>
        /// This method will not be executed if the <see cref="T:WaveEngine.Framework.Component" />, or the <see cref="T:WaveEngine.Framework.Entity" />
        /// owning it are not <c>Active</c>.
        /// </remarks>
        protected override void Update(TimeSpan gameTime)
        {
            if (this.state != null)
            {
                this.state.Update((float)gameTime.TotalSeconds * this.Speed);
                this.state.Apply(this.Skeleton);
                this.Skeleton.UpdateWorldTransform();
            }
        }

        /// <summary>
        /// A new atlas has been loaded
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void OnAtlasRefresh(object sender, EventArgs e)
        {
            this.animationRefreshFlag = true;
            this.RefreshAnimation();
        }

        /// <summary>
        /// Refresh the animation
        /// </summary>
        private void RefreshAnimation()
        {
            if (this.SkeletalData.Atlas == null
             || !this.animationRefreshFlag)
            {
                return;
            }

            if (this.Skeleton != null)
            {
                this.Skeleton = null;
            }

            if (this.State != null)
            {
                this.state.Event -= this.OnEventAnimation;
                this.state.End -= this.OnEndAnimation;
                this.state = null;
            }

            try
            {
                if (!string.IsNullOrEmpty(this.animationPath))
                {
                    using (var fileStream = WaveServices.Storage.OpenContentFile(this.animationPath))
                    {
                        SkeletonData skeletonData = null;
                        var pathExtension = Path.GetExtension(this.animationPath.ToLowerInvariant());

                        if (pathExtension == ".skel")
                        {
                            SkeletonBinary binary = new SkeletonBinary(this.SkeletalData.Atlas);
                            skeletonData = binary.ReadSkeletonData(fileStream);
                        }
                        else
                        {
                            using (var streamReader = new StreamReader(fileStream))
                            {
                                SkeletonJson json = new SkeletonJson(this.SkeletalData.Atlas);
                                skeletonData = json.ReadSkeletonData(streamReader);
                            }
                        }

                        this.Skeleton = new Skeleton(skeletonData);
                    }

                    if (string.IsNullOrEmpty(this.currentAnimation)
                     || !this.AnimationNames.Any(animation => animation == this.currentAnimation))
                    {
                        this.currentAnimation = string.Empty;
                    }

                    if (string.IsNullOrEmpty(this.currentSkin)
                     || !this.SkinNames.Any(skin => skin == this.currentSkin))
                    {
                        this.currentSkin = this.Skeleton.Data.DefaultSkin.Name;
                    }

                    this.Skeleton.SetSkin(this.currentSkin);

                    if (this.OnAnimationRefresh != null)
                    {
                        this.OnAnimationRefresh(this, EventArgs.Empty);
                    }

                    AnimationStateData stateData = new AnimationStateData(this.Skeleton.Data);
                    this.state = new AnimationState(stateData);
                    this.state.Event += this.OnEventAnimation;
                    this.state.End += this.OnEndAnimation;

                    this.Update(TimeSpan.Zero);
                    this.animationRefreshFlag = false;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("The atlas file is not valid: " + e.Message);
                this.Skeleton = null;
                this.state = null;
            }
        }
        #endregion
    }
}
