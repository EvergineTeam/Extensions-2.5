#region File Description
//-----------------------------------------------------------------------------
// SocialFakeHelper
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements

using System.Collections.Generic;
using System.Threading.Tasks;

#endregion

namespace WaveEngine.Social
{
    /// <summary>
    /// SocialFakeHelper class
    /// </summary>
    public class SocialFakeHelper : ISocial
    {
        /// <summary>
        /// Init method
        /// </summary>
        public void Init()
        {
        }

        /// <summary>
        /// Logins game services
        /// </summary>
        /// <returns>
        /// Task task
        /// </returns>
        public Task<bool> Login()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Logouts game service
        /// </summary>
        /// <returns>Task task</returns>
        public Task<bool> Logout()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Shows all leaderboards.
        /// </summary>
        public Task<bool> ShowAllLeaderboards()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Shows the leaderboards.
        /// </summary>
        /// <param name="leaderboardCode">The leaderboard code.</param>
        public Task<bool> ShowLeaderboard(string leaderboardCode)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Adds the new score.
        /// </summary>
        /// <param name="leaderboardCode">The leaderboard code.</param>
        /// <param name="score">The score.</param>
        /// <returns>
        /// Task task
        /// </returns>
        public Task<bool> AddNewScore(string leaderboardCode, long score)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Shows the archievements.
        /// </summary>
        /// <returns>
        /// Task task
        /// </returns>
        public Task<bool> ShowAchievements()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Unlocks the archievement.
        /// </summary>
        /// <param name="achievementCode">The achievement code.</param>
        /// <returns>
        /// Task task
        /// </returns>
        public Task<bool> UnlockAchievement(string achievementCode)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Increments the achievement.
        /// </summary>
        /// <param name="achievementCode">The achievement code.</param>
        /// <param name="progress">The progress.</param>
        /// <returns>
        /// Task task
        /// </returns>
        public Task<bool> IncrementAchievement(string achievementCode, int progress)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Gets the achievements.
        /// </summary>
        /// <returns>
        /// Task task
        /// </returns>
        public Task<List<Achievement>> GetAchievements()
        {
            return Task.FromResult(new List<Achievement>());
        }

        /// <summary>
        /// Gets the top scores from leaderboard.
        /// </summary>
        /// <param name="leaderboardCode">The leaderboard code.</param>
        /// <returns>
        /// Task task
        /// </returns>
        public Task<Dictionary<string, List<LeaderboardScore>>> GetTopScoresFromLeaderboard(string leaderboardCode)
        {
            return Task.FromResult(new Dictionary<string, List<LeaderboardScore>>());
        }
    }
}
