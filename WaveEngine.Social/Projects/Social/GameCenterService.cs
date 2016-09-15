#region Using Statements

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameKit;
using UIKit;
using WaveEngine.Social;

#endregion

namespace WaveEngineiOS.Social.Social
{
    /// <summary>
    /// Game Center Service class
    /// </summary>
    public class GameCenterService : ISocial
    {
        private UIViewController view;

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
                return this.InternalIsPlayerAuthenticated().Result;
            }
        }
        
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <exception cref="NotSupportedException">The current device does not support Game Center</exception>
        public void Init()
        {
            if (!UIDevice.CurrentDevice.CheckSystemVersion(4, 1))
            {
#if DEBUG
                new UIAlertView("Game Center Support Required", "The current device does not support Game Center, which this service requires.", null, "OK", null).Show();
#endif
                throw new NotSupportedException("The current device does not support Game Center");
            }

            this.view = UIApplication.SharedApplication.KeyWindow.RootViewController;
        }

        /// <summary>
        /// Logins local user.
        /// </summary>
        /// <returns>
        /// The logged in user.
        /// </returns>
        public Task<Player> Login()
        {
            var tcs = new TaskCompletionSource<Player>();

            GKLocalPlayer.LocalPlayer.Authenticate(async (error) =>
            {
                var localPlayer = GKLocalPlayer.LocalPlayer;
                Player result = null;

                if (error != null)
                {
#if DEBUG
                    new UIAlertView("Score submittion failed", "Submittion failed because: " + error, null, "OK", null).Show();
#endif
                }
                else if (localPlayer != null &&
                        localPlayer.Authenticated)
                {
                    result = await IOSMapper.MapPlayer(localPlayer);
                }

                tcs.TrySetResult(result);

            });

            return tcs.Task;
        }

        /// <summary>
        /// Logouts local user.
        /// </summary>
        /// <returns>
        /// <c>true</c> if logout was successful; otherwise, <c>false</c>.
        /// </returns>
        public Task<bool> Logout()
        {
            return Task.FromResult<bool>(true);
        }

        /// <summary>
        /// Shows a default/system view of the games leaderboards.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the view has been shown; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> ShowAllLeaderboards()
        {
            var gameCenterController = new GKGameCenterViewController();
            gameCenterController.ViewState = GKGameCenterViewControllerState.Leaderboards;
            gameCenterController.Finished += (sender, e) =>
            {
                gameCenterController.DismissViewController(true, delegate { });
            };

            await this.view.PresentViewControllerAsync(gameCenterController, true);

            return true;
        }

        /// <summary>
        /// Shows a default/system view of the given game leaderboard.
        /// </summary>
        /// <param name="leaderboardCode">The leaderboard code.</param>
        /// <returns>
        /// <c>true</c> if the view has been shown; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> ShowLeaderboard(string leaderboardCode)
        {
            var gameCenterController = new GKGameCenterViewController();
            gameCenterController.ViewState = GKGameCenterViewControllerState.Leaderboards;
            gameCenterController.LeaderboardCategory = leaderboardCode;
            gameCenterController.LeaderboardIdentifier = leaderboardCode;
            gameCenterController.Finished += (sender, e) =>
            {
                gameCenterController.DismissViewController(true, delegate { });
            };

            await this.view.PresentViewControllerAsync(gameCenterController, true);

            return true;
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
            var tcs = new TaskCompletionSource<bool>();

            UIKit.UIApplication.SharedApplication.InvokeOnMainThread(async () =>
            {
                var submitScore = new GKScore(leaderboardCode);
                submitScore.Value = score;

                bool result = true;
                try
                {
                    await GKScore.ReportScoresAsync(new GKScore[] { submitScore });
                }
                catch
                {
                    result = false;
                }

                tcs.TrySetResult(result);
            });

            return tcs.Task;
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
            return this.InternalGetScores(leaderboardCode, count, socialOnly, false);
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
        public Task<IEnumerable<LeaderboardScore>> GetPlayerCenteredScores(string leaderboardCode, int count, bool socialOnly, bool forceReload)
        {
            return this.InternalGetScores(leaderboardCode, count, socialOnly, true);
        }

        /// <summary>
        /// Shows a default/system view of the games achievements.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the view has been shown; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> ShowAchievements()
        {
            var gameCenterController = new GKGameCenterViewController();
            gameCenterController.ViewState = GKGameCenterViewControllerState.Achievements;
            gameCenterController.Finished += (sender, e) =>
            {
                gameCenterController.DismissViewController(true, delegate { });
            };

            await this.view.PresentViewControllerAsync(gameCenterController, true);

            return true;
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
            var tcs = new TaskCompletionSource<bool>();

            UIKit.UIApplication.SharedApplication.InvokeOnMainThread(async () =>
            {
                var currentAchievement = new GKAchievement(achievementCode);
                var percentComplete = 100.0d;

                var result = await InternalUpdateAchievement(currentAchievement, percentComplete);
                tcs.TrySetResult(result);
            });

            return tcs.Task;
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
            var tcs = new TaskCompletionSource<bool>();

            UIKit.UIApplication.SharedApplication.InvokeOnMainThread(async () =>
            {
                var currentAchievement = new GKAchievement(achievementCode);

                var finalPercent = currentAchievement.PercentComplete + progress;

                var result = await InternalUpdateAchievement(currentAchievement, finalPercent);
                tcs.TrySetResult(result);
            });

            return tcs.Task;
        }

        /// <summary>
        /// Gets the achievements.
        /// </summary>
        /// <returns>
        /// The achievements.
        /// </returns>
        public Task<IEnumerable<Achievement>> GetAchievements()
        {
            var tcs = new TaskCompletionSource<IEnumerable<Achievement>>();

            UIKit.UIApplication.SharedApplication.InvokeOnMainThread(async () =>
            {
                var achievements = await GKAchievement.LoadAchievementsAsync() ?? new GKAchievement[] { };
                var achievementsList = achievements.ToList();

                var result = await IOSMapper.MapAchievements(achievementsList);
                tcs.TrySetResult(result);
            });

            return tcs.Task;
        }

        private Task<bool> InternalIsPlayerAuthenticated()
        {
            var tcs = new TaskCompletionSource<bool>();

            UIKit.UIApplication.SharedApplication.InvokeOnMainThread(() =>
            {
                var result = false;

                if (GKLocalPlayer.LocalPlayer != null)
                {
                    result = GKLocalPlayer.LocalPlayer.Authenticated;
                }

                tcs.TrySetResult(result);
            });

            return tcs.Task;
        }

        private Task<IEnumerable<LeaderboardScore>> InternalGetScores(string leaderboardCode, int count, bool socialOnly, bool centered)
        {
            var tcs = new TaskCompletionSource<IEnumerable<LeaderboardScore>>();

            UIKit.UIApplication.SharedApplication.InvokeOnMainThread(async () =>
            {
                var result = Enumerable.Empty<LeaderboardScore>();

                var leaderboards = await GKLeaderboard.LoadLeaderboardsAsync() ?? new GKLeaderboard[] { };
                var selectedLeaderboard = leaderboards.FirstOrDefault(l => l.Identifier == leaderboardCode);

                if (selectedLeaderboard != null)
                {
                    selectedLeaderboard.PlayerScope = socialOnly ? GKLeaderboardPlayerScope.FriendsOnly : GKLeaderboardPlayerScope.Global;
                    selectedLeaderboard.Range = new Foundation.NSRange(1, count);
                    var scores = await selectedLeaderboard.LoadScoresAsync();

                    if (centered && selectedLeaderboard.LocalPlayerScore != null)
                    {
                        var playerRank = selectedLeaderboard.LocalPlayerScore.Rank;
                        var fromIndex = playerRank - (count / 2);
                        fromIndex = fromIndex < 1 ? 1 : fromIndex;

                        selectedLeaderboard.Range = new Foundation.NSRange(fromIndex, count);
                        scores = await selectedLeaderboard.LoadScoresAsync();
                    }

                    result = await IOSMapper.MapLeaderBoards(scores);
                }

                tcs.SetResult(result);
            });

            return tcs.Task;
        }

        private Task<bool> InternalUpdateAchievement(GKAchievement achievement, double value)
        {
            var tcs = new TaskCompletionSource<bool>();

            achievement.PercentComplete = value;
            achievement.ReportAchievement((error) =>
            {
                var result = true;

                if (error != null)
                {
                    result = false;
#if DEBUG
                    new UIAlertView("Achievement submittion failed", "Submittion failed because: " + error, null, "OK", null).Show();
#endif
                }

                tcs.TrySetResult(result);
            });

            return tcs.Task;
        }
    }
}