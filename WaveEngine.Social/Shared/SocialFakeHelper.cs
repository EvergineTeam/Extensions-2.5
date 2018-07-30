// Copyright © 2018 Wave Engine S.L. All rights reserved. Use is subject to license terms.

#region Using Statements

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#endregion

namespace WaveEngine.Social
{
    /// <summary>
    /// SocialFakeHelper class
    /// </summary>
    public class SocialFakeHelper : ISocial
    {
        private bool isAuthenticated;

        /// <summary>
        /// Gets a value indicating whether the local user is authenticated.
        /// </summary>
        /// <value>
        /// <c>true</c> if the local user is authenticated; otherwise, <c>false</c>.
        /// </value>
        public bool IsAuthenticated
        {
            get
            {
                return this.isAuthenticated;
            }
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Init()
        {
        }

        /// <summary>
        /// Logins local user.
        /// </summary>
        /// <returns>
        /// The logged in user.
        /// </returns>
        public Task<Player> Login()
        {
            this.isAuthenticated = true;
            return Task.FromResult(new Player());
        }

        /// <summary>
        /// Logouts local user.
        /// </summary>
        /// <returns>
        /// <c>true</c> if logout was successful; otherwise, <c>false</c>.
        /// </returns>
        public Task<bool> Logout()
        {
            this.isAuthenticated = false;
            return Task.FromResult(true);
        }

        /// <summary>
        /// Shows a default/system view of the games leaderboards.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the view has been shown; otherwise, <c>false</c>.
        /// </returns>
        public Task<bool> ShowAllLeaderboards()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Shows a default/system view of the given game leaderboard.
        /// </summary>
        /// <param name="leaderboardCode">The leaderboard code.</param>
        /// <returns>
        /// <c>true</c> if the view has been shown; otherwise, <c>false</c>.
        /// </returns>
        public Task<bool> ShowLeaderboard(string leaderboardCode)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Adds a new leaderboard score.
        /// </summary>
        /// <param name="leaderboardCode">The leaderboard code.</param>
        /// <param name="score">The score.</param>
        /// <returns>
        /// <c>true</c> if the score has been added; otherwise, <c>false</c>.
        /// </returns>
        public Task<bool> AddNewScore(string leaderboardCode, long score)
        {
            return Task.FromResult(true);
        }

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
        public Task<IEnumerable<LeaderboardScore>> GetTopScores(string leaderboardCode, int count, bool socialOnly, bool forceReload)
        {
            return Task.FromResult(Enumerable.Empty<LeaderboardScore>());
        }

        /// <summary>
        /// Gets the the player-centered page of scores for a given leaderboard.
        /// </summary>
        /// <param name="leaderboardCode">The leaderboard code.</param>
        /// <param name="count">The maximum number of scores.</param>
        /// <param name="socialOnly">If <c>true</c>, the result will only contain the scores of players in the viewing player's circles.</param>
        /// <param name="forceReload">If <c>true</c>, this call will clear any locally cached data and attempt to fetch the latest data from the server. This would commonly be used for something like a user-initiated refresh. Normally, this should be set to false to gain advantages of data caching.</param>
        /// <returns>
        /// The scores.
        /// </returns>
        public Task<IEnumerable<LeaderboardScore>> GetPlayerCenteredScores(string leaderboardCode, int count, bool socialOnly, bool forceReload)
        {
            return Task.FromResult(Enumerable.Empty<LeaderboardScore>());
        }

        /// <summary>
        /// Shows a default/system view of the games achievements.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the view has been shown; otherwise, <c>false</c>.
        /// </returns>
        public Task<bool> ShowAchievements()
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Unlocks the achievement.
        /// </summary>
        /// <param name="achievementCode">The achievement code.</param>
        /// <returns>
        /// <c>true</c> if the achievement has been unlocked; otherwise, <c>false</c>.
        /// </returns>
        public Task<bool> UnlockAchievement(string achievementCode)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Increments the achievement progress.
        /// </summary>
        /// <param name="achievementCode">The achievement code.</param>
        /// <param name="progress">The progress.</param>
        /// <returns>
        /// <c>true</c> if the achievement has been incremented; otherwise, <c>false</c>.
        /// </returns>
        public Task<bool> IncrementAchievement(string achievementCode, int progress)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Gets the achievements.
        /// </summary>
        /// <returns>The achievements.</returns>
        public Task<IEnumerable<Achievement>> GetAchievements()
        {
            return Task.FromResult(Enumerable.Empty<Achievement>());
        }
    }
}
