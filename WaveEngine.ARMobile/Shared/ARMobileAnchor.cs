// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using WaveEngine.Common.Math;

namespace WaveEngine.ARMobile
{
    /// <summary>
    ///  The AR mobile anchor
    /// </summary>
    public class ARMobileAnchor
    {
        /// <summary>
        /// The anchor's universal unique identifier
        /// </summary>
        public Guid Id;

        /// <summary>
        /// The anchor's transform
        /// </summary>
        public Matrix Transform;

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return this.Id.Equals(obj);
        }
    }
}
