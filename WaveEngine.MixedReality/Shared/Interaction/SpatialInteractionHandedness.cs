// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

namespace WaveEngine.MixedReality.Interaction
{
    /// <summary>
    /// Specifies whether the interaction source represents the user's left hand or right hand.
    /// </summary>
    public enum SpatialInteractionHandedness
    {
        /// <summary>
        /// The interaction source represents the user's left hand.
        /// </summary>
        Left = 1,

        /// <summary>
        ///  The interaction source represents the user's right hand.
        /// </summary>
        Right = 2,

        /// <summary>
        ///  The interaction source does not represent a specific hand.
        /// </summary>
        Unspecified = 3
    }
}
