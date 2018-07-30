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
    /// The VuMarkTarget contains all static data of a VuMark which is available from the dataset.
    /// </summary>
    public class VuMarkTarget : Trackable
    {
        /*
        /// <summary>
        /// Returns the size-scaled origin of the VuMark
        /// </summary>
        Vector2 Origin
        {
            get;
        }

        /// <summary>
        /// Sets or gets whether the VuMark has a changing background per instance, signaling
        /// to SDK how to track it.
        /// </summary>
        /// Setting TrackingFromRuntimeAppearance to true indicates that the SDK
        /// should track this type of VuMark based on what is seen by the camera and
        /// not assume the template background image is useful for tracking because
        /// the background can change per instance.
        /// Setting TrackingFromRuntimeAppearance to false indicates that the SDK
        /// should track this type of VuMark based on VuMark Template used to create
        /// the dataset. This is the default behavior.
        bool TrackingFromRuntimeAppearance
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the user data associated with this template that comes from
        /// the dataset.  If there is no VuMark use data associated with this
        /// template, an empty string is returned.
        /// </summary>
        string VuMarkUserData
        {
            get;
        }
        */

        /// <summary>
        /// Initializes a new instance of the <see cref="VuMarkTarget"/> class.
        /// </summary>
        /// <param name="trackable">The trackable</param>
        internal VuMarkTarget(QCAR_Trackable trackable)
            : base(trackable)
        {
        }
    }
}
