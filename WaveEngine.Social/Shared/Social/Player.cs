#region File Description
//-----------------------------------------------------------------------------
// Player
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements

#endregion

namespace WaveEngine.Social
{
    /// <summary>
    /// Player class
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the player identifier.
        /// </summary>
        /// <value>
        /// The player identifier.
        /// </value>
        public string PlayerId { get; set; }

        /// <summary>
        /// Gets or sets the alias.
        /// </summary>
        /// <value>
        /// The alias.
        /// </value>
        public string Alias { get; set; }

        /// <summary>
        /// Gets or sets the retrieved timestamp.
        /// </summary>
        /// <value>
        /// The retrieved timestamp.
        /// </value>
        public long RetrievedTimestamp { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has hi resource image.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has hi resource image; otherwise, <c>false</c>.
        /// </value>
        public bool HasHiResImage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has icon image.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has icon image; otherwise, <c>false</c>.
        /// </value>
        public bool HasIconImage { get; set; }

        /// <summary>
        /// Gets or sets the hi resource image URL.
        /// </summary>
        /// <value>
        /// The hi resource image URL.
        /// </value>
        public string HiResImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the icon image URL.
        /// </summary>
        /// <value>
        /// The icon image URL.
        /// </value>
        public string IconImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the last played with timestamp.
        /// </summary>
        /// <value>
        /// The last played with timestamp.
        /// </value>
        public long LastPlayedWithTimestamp { get; set; }
    }
}
