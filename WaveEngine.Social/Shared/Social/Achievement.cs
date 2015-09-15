#region File Description
//-----------------------------------------------------------------------------
// Achievement
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements

#endregion

namespace WaveEngine.Social
{
    /// <summary>
    /// Achievement class
    /// </summary>
    public class Achievement
    {
        /// <summary>
        /// Gets or sets the achievement identifier.
        /// </summary>
        /// <value>
        /// The achievement identifier.
        /// </value>
        public string AchievementCode { get; set; }

        /// <summary>
        /// Gets or sets the current steps.
        /// </summary>
        /// <value>
        /// The current steps.
        /// </value>
        public double CurrentSteps { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the last updated timestamp.
        /// </summary>
        /// <value>
        /// The last updated timestamp.
        /// </value>
        public long LastUpdatedTimestamp { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the player.
        /// </summary>
        /// <value>
        /// The player.
        /// </value>
        public Player Player { get; set; }

        /// <summary>
        /// Gets or sets the revealed image URI.
        /// </summary>
        /// <value>
        /// The revealed image URI.
        /// </value>
        public string RevealedImageUri { get; set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public int State { get; set; }

        /// <summary>
        /// Gets or sets the total steps.
        /// </summary>
        /// <value>
        /// The total steps.
        /// </value>
        public double TotalSteps { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public int Type { get; set; }

        /// <summary>
        /// Gets or sets the unlocked image URI.
        /// </summary>
        /// <value>
        /// The unlocked image URI.
        /// </value>
        public string UnlockedImageUri { get; set; }
    }
}
