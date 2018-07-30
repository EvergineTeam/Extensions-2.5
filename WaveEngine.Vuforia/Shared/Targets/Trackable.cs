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
    /// Base class for all objects that can be tracked.
    /// </summary>
    public abstract class Trackable
    {
        /// <summary>
        /// Gets or sets the runtime Id of the Trackable
        /// </summary>
        public int Id
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the name of the Trackable
        /// </summary>
        public string Name
        {
            get;
            protected set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Trackable"/> class.
        /// </summary>
        /// <param name="trackable">The trackable.</param>
        internal Trackable(QCAR_Trackable trackable)
        {
            this.Id = trackable.Id;
            this.Name = trackable.TrackName;
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is Trackable &&
                this.Id.Equals(((Trackable)obj).Id);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}
