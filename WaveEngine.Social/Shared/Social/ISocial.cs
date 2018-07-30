// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace WaveEngine.Social
{
    /// <summary>
    /// Platform specific Social Service interface
    /// </summary>
    public interface ISocial
    {
        /// <summary>
        /// Gets a value indicating whether the local user is authenticated.
        /// </summary>
        /// <value>
        /// <c>true</c> if the local user is authenticated; otherwise, <c>false</c>.
        /// </value>
        bool IsAuthenticated { get; }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        void Init();

        /// <summary>
        /// Logins local user.
        /// </summary>
        /// <returns>
        /// The logged in user.
        /// </returns>
        Task<Player> Login();

        /// <summary>
        /// Logouts local user.
        /// </summary>
        /// <returns>
        /// <c>true</c> if logout was successful; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> Logout();

        /// <summary>
        /// Shows a default/system view of the games leaderboards.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the view has been shown; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> ShowAllLeaderboards();

        /// <summary>
        /// Shows a default/system view of the given game leaderboard.
        /// </summary>
        /// <param name="leaderboardCode">The leaderboard code.</param>
        /// <returns>
        /// <c>true</c> if the view has been shown; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> ShowLeaderboard(string leaderboardCode);

        /// <summary>
        /// Adds a new leaderboard score.
        /// </summary>
        /// <param name="leaderboardCode">The leaderboard code.</param>
        /// <param name="score">The score.</param>
        /// <returns>
        /// <c>true</c> if the score has been added; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> AddNewScore(string leaderboardCode, long score);

        /// <summary>
        /// Gets the top page of scores for a given leaderboard.
        /// </summary>
        /// <param name="leaderboardCode">The leaderboard code.</param>
        /// <param name="count">The maximum number of scores. Must be between 1 and 25.</param>
        /// <param name="socialOnly">If <c>true</c>, the result will only contain the scores of players in the viewing player's circles.</param>
        /// <param name="forceReload">If <c>true</c>, this call will clear any locally cached data and attempt to fetch the latest data from the server. This would commonly be used for something like a user-initiated refresh. Normally, this should be set to false to gain advantages of data caching.</param>
        /// <returns>
        /// The scores.
        /// </returns>
        Task<IEnumerable<LeaderboardScore>> GetTopScores(string leaderboardCode, int count, bool socialOnly, bool forceReload);

        /// <summary>
        /// Gets the the player-centered page of scores for a given leaderboard.
        /// </summary>
        /// <param name="leaderboardCode">The leaderboard code.</param>
        /// <param name="count">The maximum number of scores. Must be between 1 and 25.</param>
        /// <param name="socialOnly">If <c>true</c>, the result will only contain the scores of players in the viewing player's circles.</param>
        /// <param name="forceReload">If <c>true</c>, this call will clear any locally cached data and attempt to fetch the latest data from the server. This would commonly be used for something like a user-initiated refresh. Normally, this should be set to false to gain advantages of data caching.</param>
        /// <returns>
        /// The scores.
        /// </returns>
        Task<IEnumerable<LeaderboardScore>> GetPlayerCenteredScores(string leaderboardCode, int count, bool socialOnly, bool forceReload);

        /// <summary>
        /// Shows a default/system view of the games achievements.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the view has been shown; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> ShowAchievements();

        /// <summary>
        /// Unlocks the achievement.
        /// </summary>
        /// <param name="achievementCode">The achievement code.</param>
        /// <returns>
        /// <c>true</c> if the achievement has been unlocked; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> UnlockAchievement(string achievementCode);

        /// <summary>
        /// Increments the achievement progress.
        /// </summary>
        /// <param name="achievementCode">The achievement code.</param>
        /// <param name="progress">The progress.</param>
        /// <returns>
        /// <c>true</c> if the achievement has been incremented; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> IncrementAchievement(string achievementCode, int progress);

        /// <summary>
        /// Gets the achievements.
        /// </summary>
        /// <returns>
        /// The achievements.
        /// </returns>
        Task<IEnumerable<Achievement>> GetAchievements();
    }
}
