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
    /// Game Center Utils class
    /// </summary>
    public class GameCenterUtils : ISocial
    {
		private UIViewController view;

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
        /// Game center login
        /// </summary>      
        public Task<bool> Login()
        {
            var tcs = new TaskCompletionSource<bool>();

            GKLocalPlayer.LocalPlayer.Authenticate((error) =>
            {
                var result = GKLocalPlayer.LocalPlayer.Authenticated;

                if (error != null)
                {
                    result = false;
#if DEBUG
                    new UIAlertView("Score submittion failed", "Submittion failed because: " + error, null, "OK", null).Show();
#endif
                }

                tcs.TrySetResult(result);

            });

            return tcs.Task;
        }

        /// <summary>
        /// Game center logout
        /// </summary>
        /// <returns>Task</returns>
        public Task<bool> Logout()
        {
            return Task.FromResult<bool>(true);
        }

        /// <summary>
        /// Shows the leaderboards.
        /// </summary>
        /// <param name="leaderboardCode">The leaderboard code.</param>
        public Task<bool> ShowLeaderboard(string leaderboardCode)
        {
            throw new NotImplementedException("iOS doesn't provide a way to provide leaderboardCode to the GKLeaderboardViewController");
        }

        /// <summary>
        /// Shows the leaderboards.
        /// </summary>        
        public async Task<bool> ShowAllLeaderboards()
        {
            var leaderboardViewController = new GKLeaderboardViewController();
            leaderboardViewController.Category = "Leaderboard";
            leaderboardViewController.DidFinish += (sender, e) =>
            {
                leaderboardViewController.DismissViewController(true, null);                
            };

            await this.view.PresentViewControllerAsync(leaderboardViewController, true);
            return true;
        }

        /// <summary>
        /// Adds the new score.
        /// </summary>
        /// <param name="score">The score.</param>        
        public Task<bool> AddNewScore(string leaderboardCode, long score)
        {
            var tcs = new TaskCompletionSource<bool>();

            var submitScore = new GKScore(leaderboardCode);
            submitScore.Value = score;
            submitScore.Context = 100;

            submitScore.ReportScore((error) =>
            {
                var result = true;

                if (error != null)
                {
                    result = false;
#if DEBUG
                    new UIAlertView("Score submittion failed", "Submittion failed because: " + error, null, "OK", null).Show();
#endif
                }

                tcs.TrySetResult(result);
            });

            return tcs.Task;
        }

        /// <summary>
        /// Shows the archievements.
        /// </summary>        
        public async Task<bool> ShowAchievements()
        {
            var achievementViewController = new GKAchievementViewController();
            achievementViewController.DidFinish += (sender, e) =>
            {
                achievementViewController.DismissViewController(true, null);
            };

            await this.view.PresentViewControllerAsync(achievementViewController, true);
            return true;
        }

        /// <summary>
        /// Unlocks the archievement.
        /// </summary>        
        public async Task<bool> UnlockAchievement(string achievementCode)
        {
            var currentAchievement = new GKAchievement(achievementCode);

            var percentComplete = 100.0d;

            return await InternalUpdateAchievement(currentAchievement, percentComplete);
        }

        /// <summary>
        /// Increments the achievement.
        /// </summary>
        /// <param name="achievementCode">The achievement code.</param>
        /// <param name="progress">The progress.</param>
        /// <returns>
        /// Task
        /// </returns>
        public async Task<bool> IncrementAchievement(string achievementCode, int progress)
        {
            var currentAchievement = new GKAchievement(achievementCode);

            var finalPercent = currentAchievement.PercentComplete + progress;

            return await InternalUpdateAchievement(currentAchievement, finalPercent);
        }

        private async Task<GKAchievement> GetAchievementByCode(string achievementCode)
        {
            var achievements = await GKAchievement.LoadAchievementsAsync() ?? new GKAchievement[] { };
            var currentAchievement = achievements.FirstOrDefault(a => a.Identifier == achievementCode);

            if (currentAchievement == null)
            {
                throw new ArgumentException("achievementCode was not found");
            }

            return currentAchievement;
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

        /// <summary>
        /// Gets the achievements.
        /// </summary>
        /// <returns>
        /// Task
        /// </returns>
        public async Task<List<WaveEngine.Social.Achievement>> GetAchievements()
        {
            var achievements = await GKAchievement.LoadAchievementsAsync() ?? new GKAchievement[] { };
            var achievementsList = achievements.ToList();

            var waveAchievements = Mapper.MapAchievements(achievementsList);

            return waveAchievements;
        }

        /// <summary>
        /// Gets the top scores from leaderboard.
        /// </summary>
        /// <param name="leaderboardCode">The leaderboard code.</param>
        /// <returns>
        /// Task
        /// </returns>
        public async Task<Dictionary<string, List<WaveEngine.Social.LeaderboardScore>>> GetTopScoresFromLeaderboard(string leaderboardCode)
        {
            var scores = new Dictionary<string, List<GKScore>>();

            var leaderboards = await GKLeaderboard.LoadLeaderboardsAsync() ?? new GKLeaderboard[] { };

            foreach (var leaderboard in leaderboards)
            {
                var leaderboardScores = await leaderboard.LoadScoresAsync() ?? new GKScore[] { };
                var leaderboardScoresList = leaderboardScores.ToList();

                var boardId = leaderboard.Identifier;

                scores.Add(boardId, leaderboardScoresList);
            }

            var waveScores = Mapper.MapLeaderBoards(scores);

            return waveScores;
        }


        internal static class Mapper
        {
            internal static WaveEngine.Social.Player MapPlayer(GKPlayer player)
            {
                WaveEngine.Social.Player wavePlayer = null;

                if (player != null)
                {
                    wavePlayer = new WaveEngine.Social.Player()
                    {
                        DisplayName = player.DisplayName,//player.Alias,
                        PlayerId = player.PlayerID,//player.PlayerId,
                        Alias = player.Alias,
                        HasHiResImage = false,//player.HasHiResImage,
                        HasIconImage = false,//player.HasIconImage,
                        HiResImageUrl = null,//player.HiResImageUrl,
                        IconImageUrl = null,//player.IconImageUrl,
                        LastPlayedWithTimestamp = 0,//player.LastPlayedWithTimestamp,
                        RetrievedTimestamp = 0,//player.RetrievedTimestamp,
                    };
                }

                return wavePlayer;
            }

            internal static List<WaveEngine.Social.Achievement> MapAchievements(List<GKAchievement> achievements)
            {
                List<WaveEngine.Social.Achievement> waveAchievements = null;

                if (achievements != null)
                {
                    waveAchievements = new List<WaveEngine.Social.Achievement>();

                    foreach (var achievement in achievements)
                    {
                        var waveAchievement = new WaveEngine.Social.Achievement()
                        {
                            AchievementCode = achievement.Identifier,//achievement.AchievementId,
                            CurrentSteps = achievement.PercentComplete,//achievement.CurrentSteps,
                            Description = achievement.Description,//achievement.Description,
                            Player = Mapper.MapPlayer(achievement.Player),
                            RevealedImageUri = null,//achievement.RevealedImageUri.Path,
                            UnlockedImageUri = null,//achievement.UnlockedImageUri.Path,
                            State = 0,//achievement.State,
                            TotalSteps = 0,//achievement.TotalSteps,
                            Type = 0,//achievement.Type,
                            LastUpdatedTimestamp = 0,//achievement.LastUpdatedTimestamp,
                        };

                        waveAchievements.Add(waveAchievement);
                    }
                }

                return waveAchievements;
            }

            internal static Dictionary<string, List<WaveEngine.Social.LeaderboardScore>> MapLeaderBoards(Dictionary<string, List<GKScore>> scores)
            {
                Dictionary<string, List<WaveEngine.Social.LeaderboardScore>> waveScores = null;

                if (scores != null)
                {
                    waveScores = new Dictionary<string, List<WaveEngine.Social.LeaderboardScore>>();

                    foreach (var item in scores)
                    {
                        var leaderboardScoreId = item.Key;
                        var leaderboardScores = item.Value;

                        var waveLeaderBoardScores = new List<WaveEngine.Social.LeaderboardScore>();

                        foreach (var leaderboardScore in leaderboardScores)
                        {
                            var waveLeaderBoardScore = new WaveEngine.Social.LeaderboardScore()
                            {
                                DisplayRank = null,//leaderboardScore.DisplayRank,
                                DisplayScore = leaderboardScore.FormattedValue,//leaderboardScore.DisplayScore,
                                Rank = leaderboardScore.Rank,//leaderboardScore.Rank,
                                RawScore = leaderboardScore.Value,//leaderboardScore.RawScore,
                                ScoreHolder = Mapper.MapPlayer(leaderboardScore.Player),
                                ScoreTag = null,//leaderboardScore.ScoreTag,
                            };

                            waveLeaderBoardScores.Add(waveLeaderBoardScore);
                        }

                        waveScores.Add(leaderboardScoreId, waveLeaderBoardScores);
                    }
                }

                return waveScores;
            }
        }

    }
}