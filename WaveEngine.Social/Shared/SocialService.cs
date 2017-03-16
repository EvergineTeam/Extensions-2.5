#region File Description
//-----------------------------------------------------------------------------
// SocialService
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WaveEngine.Common;
using WaveEngine.Social;

#if IOS
using WaveEngineiOS.Social.Social;
#endif

#if ANDROID
using WaveEngineAndroid.Social.Social;
#endif


#endregion

namespace WaveEngine.Framework.Services
{
    /// <summary>
    /// SocialService service
    /// </summary>
    public class SocialService : Service
    {
        /// <summary>
        /// The platform specific Social Service implementation
        /// </summary>
        private readonly ISocial platformSocial;

        /// <summary>
        /// The simulation Social Service
        /// </summary>
        private readonly SocialSimulationHelper simulationSocial;

        /// <summary>
        /// The current Social Service
        /// </summary>
        private ISocial currentSocial;

        /// <summary>
        /// Gets or sets a value indicating whether simulation mode is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if simulation mode is active; otherwise, <c>false</c>.
        /// </value>
        private bool simulationMode;

        /// <summary>
        /// Gets or sets a value indicating whether simulation mode is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if simulation mode is active; otherwise, <c>false</c>.
        /// </value>
        public bool SimulationMode
        {
            get
            {
                return this.simulationMode;
            }

            set
            {
                this.simulationMode = value;
                this.currentSocial = this.simulationMode ? this.simulationSocial : this.platformSocial;
            }
        }

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
                return this.currentSocial.IsAuthenticated;
            }
        }

        /// <summary>
        /// Gets the local player.
        /// </summary>
        /// <remarks>
        /// Until the user logs in or authenticates themself, it will return <c>null</c>.
        /// </remarks>
        /// <value>
        /// The local player.
        /// </value>
        public Player LocalUser { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SocialService"/> class.
        /// </summary>
        public SocialService()
        {
            this.simulationSocial = new SocialSimulationHelper();

#if ANDROID
            this.platformSocial = new GooglePlayGameService();
#elif IOS
            this.platformSocial = new GameCenterService();
#else
            this.platformSocial = new SocialFakeHelper();
#endif

            this.currentSocial = this.platformSocial;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SocialService"/> class.
        /// </summary>
        /// <param name="socialService">The platform specific social service implementation.</param>
        /// <exception cref="System.ArgumentNullException">socialService</exception>
        public SocialService(ISocial socialService)
        {
            if (socialService == null)
            {
                throw new ArgumentNullException("socialService");
            }

            this.platformSocial = socialService;
            this.currentSocial = socialService;

        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Initialize()
        {
            this.platformSocial.Init();
        }

        /// <summary>
        /// Terminates this instance.
        /// </summary>
        protected override void Terminate()
        {
        }

        /// <summary>
        /// Logins local user.
        /// </summary>
        /// <returns>
        /// <c>true</c> if login was successful; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> Login()
        {
            this.LocalUser = await this.currentSocial.Login().ConfigureAwait(false);

            return this.LocalUser != null;
        }

        /// <summary>
        /// Logouts local user.
        /// </summary>
        /// <returns>
        /// <c>true</c> if logout was successful; otherwise, <c>false</c>.
        /// </returns>
        public Task<bool> Logout()
        {
            this.LocalUser = null;
            return this.currentSocial.Logout();
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
            return this.currentSocial.ShowLeaderboard(leaderboardCode);
        }

        /// <summary>
        /// Shows a default/system view of the games leaderboards.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the view has been shown; otherwise, <c>false</c>.
        /// </returns>
        public Task<bool> ShowAllLeaderboards()
        {
            return this.currentSocial.ShowAllLeaderboards();
        }

        /// <summary>
        /// Submit a score to a leaderboard for the currently signed in player.
        /// </summary>
        /// <param name="leaderboardCode">The leaderboard code.</param>
        /// <param name="score">The score.</param>
        /// <returns>
        /// <c>true</c> if the score has been added; otherwise, <c>false</c>.
        /// </returns>
        public Task<bool> AddNewScore(string leaderboardCode, long score)
        {
            return this.currentSocial.AddNewScore(leaderboardCode, score);
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
        public Task<IEnumerable<LeaderboardScore>> GetTopScores(string leaderboardCode, int count, bool socialOnly = false, bool forceReload = false)
        {
            if (count < 1 || count > 25)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            return this.currentSocial.GetTopScores(leaderboardCode, count, socialOnly, forceReload);
        }

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
        public Task<IEnumerable<LeaderboardScore>> GetPlayerCenteredScores(string leaderboardCode, int count, bool socialOnly = false, bool forceReload = false)
        {
            if (count < 1 || count > 25)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            return this.currentSocial.GetPlayerCenteredScores(leaderboardCode, count, socialOnly, forceReload);
        }

        /// <summary>
        /// Shows a default/system view of the games achievements.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the view has been shown; otherwise, <c>false</c>.
        /// </returns>
        public Task<bool> ShowAchievements()
        {
            return this.currentSocial.ShowAchievements();
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
            return this.currentSocial.UnlockAchievement(achievementCode);
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
            return this.currentSocial.IncrementAchievement(achievementCode, progress);
        }

        /// <summary>
        /// Gets the achievements.
        /// </summary>
        /// <returns>The achievements.</returns>
        public Task<IEnumerable<Achievement>> GetAchievements()
        {
            return this.currentSocial.GetAchievements();
        }
    }
}
