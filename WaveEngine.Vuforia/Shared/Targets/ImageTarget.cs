// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using WaveEngine.Common.Graphics.VertexFormats;
using WaveEngine.Common.Math;
using WaveEngine.Vuforia.QCAR;
#endregion

namespace WaveEngine.Vuforia
{
    /// <summary>
    /// A flat natural feature target
    /// </summary>
    public class ImageTarget : Trackable
    {
        /// <summary>
        /// Gets the type of this ImageTarget (Predefined, User Defined, Cloud Reco)
        /// </summary>
        public ImageTargetType ImageTargetType
        {
            get;
            internal set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageTarget"/> class.
        /// </summary>
        /// <param name="trackable">The trackable.</param>
        internal ImageTarget(QCAR_Trackable trackable)
            : base(trackable)
        {
            this.ImageTargetType = ImageTargetType.PREDEFINED;
        }

        /*
        /// <summary>
        /// Creates a new virtual button and adds it to the ImageTarget
        /// Returns NULL if the corresponding DataSet is currently active.
        /// </summary>
        publicVirtualButton CreateVirtualButton(string name, RectangleF area);

        /// <summary>
        /// Removes and destroys one of the ImageTarget's virtual buttons
        /// Returns false if the corresponding DataSet is currently active.
        /// </summary>
        public bool DestroyVirtualButton(VirtualButton vb);

        /// <summary>
        /// Returns a virtual button by its name
        /// Returns NULL if no virtual button with that name
        /// exists in this ImageTarget
        /// </summary>
        public VirtualButton GetVirtualButtonByName(string name);

        /// <summary>
        ///  Returns the virtual buttons that are defined for this imageTarget
        /// </summary>
        public IEnumerable<VirtualButton> GetVirtualButtons();
        */
    }
}
