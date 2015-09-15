#region File Description
//-----------------------------------------------------------------------------
// ARCamera
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Usings Statements
using System;
using WaveEngine.Framework.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using System.Collections.Generic;
using WaveEngine.Common.Graphics;
#endregion

namespace WaveEngine.Vuforia
{
	/// <summary>
	/// FixedCamera decorate class
	/// </summary>
	public class ARCamera : BaseDecorator
	{
		#region Properties
        /// <summary>
        /// Gets or sets the filed of view.
        /// </summary>
        /// <value>
        /// The filed of view.
        /// </value>
        public float FieldOfView
        {
            get
            {
                return this.entity.FindComponent<ARCameraComponent>().FieldOfView;
            }

            set
            {
                this.entity.FindComponent<ARCameraComponent>().FieldOfView = value;
            }
        }

        /// <summary>
        /// Gets or sets the aspect ratio.
        /// </summary>
        /// <value>
        /// The aspect ratio.
        /// </value>
        public float AspectRatio
        {
            get
            {
                return this.entity.FindComponent<ARCameraComponent>().AspectRatio;
            }

            set
            {
                this.entity.FindComponent<ARCameraComponent>().AspectRatio = value;
            }
        }

        /// <summary>
        /// Gets or sets the far plane.
        /// </summary>
        /// <value>
        /// The far plane.
        /// </value>
        public float FarPlane
        {
            get
            {
                return this.entity.FindComponent<ARCameraComponent>().FarPlane;
            }

            set
            {
                this.entity.FindComponent<ARCameraComponent>().FarPlane = value;
            }
        }

        /// <summary>
        /// Gets or sets the near plane.
        /// </summary>
        /// <value>
        /// The near plane.
        /// </value>
        public float NearPlane
        {
            get
            {
                return this.entity.FindComponent<ARCameraComponent>().NearPlane;
            }

            set
            {
                this.entity.FindComponent<ARCameraComponent>().NearPlane = value;
            }
        }

        /// <summary>
        /// Gets or sets the RenderTarget associated to the camera.
        /// </summary>
        /// <value>
        /// The render target.
        /// </value>
        public RenderTarget RenderTarget
        {
            get
            {
                return this.entity.FindComponent<ARCameraComponent>().RenderTarget;
            }

            set
            {
                this.entity.FindComponent<ARCameraComponent>().RenderTarget = value;
            }
        }

        /// <summary>
        /// Gets or sets Clear flags used for clean FrameBuffer, stencilbuffer and Zbuffer.
        /// </summary>
        /// <value>
        /// The clear flags.
        /// </value>
        /// <exception cref="System.ObjectDisposedException">RenderManager has been disposed.</exception>
        public ClearFlags ClearFlags
        {
            get
            {
                return this.entity.FindComponent<ARCameraComponent>().ClearFlags;
            }

            set
            {
                this.entity.FindComponent<ARCameraComponent>().ClearFlags = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>
        /// The background color of the camera if it was setted, or the RenderManager default background color.
        /// </value>
        public Color BackgroundColor
        {
            get
            {
                return this.entity.FindComponent<ARCameraComponent>().BackgroundColor;
            }

            set
            {
                this.entity.FindComponent<ARCameraComponent>().BackgroundColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the camera is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the camera is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive
        {
            get
            {
                return this.entity.FindComponent<ARCameraComponent>().IsActive;
            }

            set
            {
                this.entity.FindComponent<ARCameraComponent>().IsActive = value;
            }
        }

        /// <summary>
        /// Gets the layer mask.
        /// </summary>
        /// <value>
        /// The layer mask.
        /// </value>
        public IDictionary<Type, bool> LayerMask
        {
            get
            {
                return this.entity.FindComponent<ARCameraComponent>().LayerMask;
            }
        }
		#endregion

		#region Initialize

		/// <summary>
        /// Initializes a new instance of the <see cref="ARCamera" /> class.
		/// </summary>
		/// <param name="name">The name.</param>
		public ARCamera(string name)
		{
			this.entity = new Entity(name)
				.AddComponent(new ARCameraComponent());
		}
		#endregion
	}
}