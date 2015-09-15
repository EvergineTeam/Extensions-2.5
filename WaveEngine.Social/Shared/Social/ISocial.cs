#region File Description
//-----------------------------------------------------------------------------
// ISocial
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
    /// Social Service interface
    /// </summary>
    public interface ISocial
    {
        /// <summary>
        /// Init method
        /// </summary>
        void Init();

        /// <summary>
        /// Logins game services
        /// </summary>
        /// <returns>Task task</returns>
        Task<bool> Login();

        /// <summary>
        /// Logouts game service
        /// </summary>
        /// <returns>Task task</returns>
        Task<bool> Logout();

        /// <summary>
        /// Shows all leaderboards.
        /// </summary>
        Task<bool> ShowAllLeaderboards();

        /// <summary>
        /// Shows the leaderboards.
        /// </summary>
        /// <param name="leaderboardCode">The leaderboard code.</param>
        Task<bool> ShowLeaderboard(string leaderboardCode);

        /// <summary>
        /// Adds the new score.
        /// </summary>
        /// <param name="leaderboardCode">The leaderboard code.</param>
        /// <param name="score">The score.</param>
        /// <returns>Task task</returns>
        Task<bool> AddNewScore(string leaderboardCode, long score);

        /// <summary>
        /// Shows the archievements.
        /// </summary>
        /// <returns>Task task</returns>
        Task<bool> ShowAchievements();

        /// <summary>
        /// Unlocks the archievement.
        /// </summary>
        /// <param name="achievementCode">The achievement code.</param>
        /// <returns>Task task</returns>
        Task<bool> UnlockAchievement(string achievementCode);

        /// <summary>
        /// Increments the achievement.
        /// </summary>
        /// <param name="achievementCode">The achievement code.</param>
        /// <param name="progress">The progress.</param>
        /// <returns>Task task</returns>
        Task<bool> IncrementAchievement(string achievementCode, int progress);

        /// <summary>
        /// Gets the achievements.
        /// </summary>
        /// <returns>Task task</returns>
        Task<List<WaveEngine.Social.Achievement>> GetAchievements();

        /// <summary>
        /// Gets the top scores from leaderboard.
        /// </summary>
        /// <param name="leaderboardCode">The leaderboard code.</param>
        /// <returns>Task task</returns>
        Task<Dictionary<string, List<WaveEngine.Social.LeaderboardScore>>> GetTopScoresFromLeaderboard(string leaderboardCode);
    }
}
