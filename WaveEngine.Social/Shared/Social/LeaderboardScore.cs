#region File Description
//-----------------------------------------------------------------------------
// LeaderboardScore
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements

#endregion

namespace WaveEngine.Social
{
    /// <summary>
    /// LeaderboardScore class
    /// </summary>
    public class LeaderboardScore
    {
        /// <summary>
        /// Gets or sets the display rank.
        /// </summary>
        /// <value>
        /// The display rank.
        /// </value>
        public string DisplayRank { get; set; }

        /// <summary>
        /// Gets or sets the display score.
        /// </summary>
        /// <value>
        /// The display score.
        /// </value>
        public string DisplayScore { get; set; }

        /// <summary>
        /// Gets or sets the rank.
        /// </summary>
        /// <value>
        /// The rank.
        /// </value>
        public long Rank { get; set; }

        /// <summary>
        /// Gets or sets the raw score.
        /// </summary>
        /// <value>
        /// The raw score.
        /// </value>
        public long RawScore { get; set; }

        /// <summary>
        /// Gets or sets the score holder.
        /// </summary>
        /// <value>
        /// The score holder.
        /// </value>
        public Player ScoreHolder { get; set; }
        
        /// <summary>
        /// Gets or sets the score tag.
        /// </summary>
        /// <value>
        /// The score tag.
        /// </value>
        public string ScoreTag { get; set; }

        /// <summary>
        /// Gets or sets the timestamp millis.
        /// </summary>
        /// <value>
        /// The timestamp millis.
        /// </value>
        public long TimestampMillis { get; set; }
    }
}
