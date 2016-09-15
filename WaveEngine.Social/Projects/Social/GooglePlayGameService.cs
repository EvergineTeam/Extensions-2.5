#region Using Statements

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using WaveEngine.Social;
using WaveEngine.Framework;
using System.Diagnostics;

#endregion

namespace WaveEngineAndroid.Social.Social
{
    /// <summary>
    /// Google Play Utils class
    /// </summary>
    public class GooglePlayGameService : ISocial
    {
        private Activity activity;
        private WaveEngine.Adapter.Adapter adapter;
        private GooglePlayGameHelper helper;

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
                var result = false;

                if (this.helper != null)
                {
                    result = !this.helper.SignedOut;
                }

                return result;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GooglePlayGameService"/> class.
        /// </summary>
        public GooglePlayGameService()
        {
            this.adapter = (Game.Current).Application.Adapter as WaveEngine.Adapter.Adapter;
            this.activity = adapter.Activity;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Init()
        {
            this.helper = new GooglePlayGameHelper(activity, adapter);
            this.helper.ViewForPopups = activity.CurrentFocus;
            this.helper.Initialize();
        }

        /// <summary>
        /// Logins local user.
        /// </summary>
        /// <returns>
        /// The logged in user.
        /// </returns>
        public Task<Player> Login()
        {
            var tcsLogin = new TaskCompletionSource<Player>();

            this.helper.OnSignedIn += (s, e) =>
            {
                var localPlayer = this.helper.GetLocalPlayer();
                tcsLogin.TrySetResult(localPlayer);
            };

            this.helper.OnSignInFailed += (s, e) =>
            {
                tcsLogin.TrySetResult(null);
            };

            try
            {
                this.helper.SignIn();
            }
            catch(Exception e)
            {
                Debug.WriteLine(e);
            }

            return tcsLogin.Task;
        }

        /// <summary>
        /// Logouts local user.
        /// </summary>
        /// <returns>
        /// <c>true</c> if logout was successful; otherwise, <c>false</c>.
        /// </returns>
        public Task<bool> Logout()
        {
            this.ThrowIfNotInitialized();

            var tcsLogOut = new TaskCompletionSource<bool>();

            this.helper.OnSignedOut += (s, e) =>
            {
                var signedOut = this.helper.SignedOut;
                tcsLogOut.TrySetResult(signedOut);
            };

            this.helper.SignOut();

            return tcsLogOut.Task;
        }

        /// <summary>
        /// Shows a default/system view of the games leaderboards.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the view has been shown; otherwise, <c>false</c>.
        /// </returns>
        public Task<bool> ShowAllLeaderboards()
        {
            this.ThrowIfNotInitialized();
            this.ThrowIfNotLoggedIn();

            var tcs = new TaskCompletionSource<bool>();
            this.helper.ShowAllLeaderBoardsIntent(tcs);

            return tcs.Task;
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
            this.ThrowIfNotInitialized();
            this.ThrowIfNotLoggedIn();

            var tcs = new TaskCompletionSource<bool>();
            this.helper.ShowLeaderBoardIntentForLeaderboard(leaderboardCode, tcs);

            return tcs.Task;
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
            this.ThrowIfNotInitialized();
            this.ThrowIfNotLoggedIn();

            var result = true;
            try
            {
                this.helper.SubmitScore(leaderboardCode, score);
            }
            catch
            {
                result = false;
            }

            return Task.FromResult(result);
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
            this.ThrowIfNotInitialized();
            this.ThrowIfNotLoggedIn();

            var tcs = new TaskCompletionSource<IEnumerable<LeaderboardScore>>();

            this.helper.LoadTopScores(leaderboardCode, count, socialOnly, forceReload, tcs);

            return tcs.Task;
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
            this.ThrowIfNotInitialized();
            this.ThrowIfNotLoggedIn();

            var tcs = new TaskCompletionSource<IEnumerable<LeaderboardScore>>();

            this.helper.LoadPlayerCenteredScores(leaderboardCode, count, socialOnly, forceReload, tcs);

            return tcs.Task;
        }

        /// <summary>
        /// Shows a default/system view of the games achievements.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the view has been shown; otherwise, <c>false</c>.
        /// </returns>
        public Task<bool> ShowAchievements()
        {
            this.ThrowIfNotInitialized();
            this.ThrowIfNotLoggedIn();

            var tcs = new TaskCompletionSource<bool>();

            this.helper.ShowAchievements(tcs);

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
            this.ThrowIfNotInitialized();
            this.ThrowIfNotLoggedIn();

            this.helper.UnlockAchievement(achievementCode);

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
            this.ThrowIfNotInitialized();
            this.ThrowIfNotLoggedIn();

            this.helper.IncrementAchievement(achievementCode, progress);

            return Task.FromResult(true);
        }

        /// <summary>
        /// Gets the achievements.
        /// </summary>
        /// <returns>The achievements.</returns>
        public Task<IEnumerable<Achievement>> GetAchievements()
        {
            this.ThrowIfNotInitialized();
            this.ThrowIfNotLoggedIn();

            var tcs = new TaskCompletionSource<IEnumerable<Achievement>>();

            this.helper.LoadAchievements(tcs);

            return tcs.Task;
        }

        private void ThrowIfNotInitialized()
        {
            if (this.helper == null)
            {
                throw new InvalidOperationException("Initialize the social service first");
            }
        }

        private void ThrowIfNotLoggedIn()
        {
            if (this.helper != null && this.helper.SignedOut)
            {
                throw new InvalidOperationException("Log into the social service first");
            }
        }
    }
}
