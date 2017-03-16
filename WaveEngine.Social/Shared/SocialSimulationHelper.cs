#region File Description
//-----------------------------------------------------------------------------
// SocialFakeHelper
//
// Copyright © 2017 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using WaveEngine.Framework.Services;
using NetTask = System.Threading.Tasks.Task;

#endregion

namespace WaveEngine.Social
{
    /// <summary>
    /// SocialFakeHelper class
    /// </summary>
    public class SocialSimulationHelper : ISocial
    {
        /// <summary>
        /// The maximum progress in simulation
        /// </summary>
        private readonly long maxProgressInSimulation = 100;

        /// <summary>
        /// The simulated achievements
        /// </summary>
        private readonly List<Achievement> simulatedAchievements = new List<Achievement>();

        /// <summary>
        /// The simulated leaderboards
        /// </summary>
        private readonly Dictionary<string, List<LeaderboardScore>> simulatedLeaderboards = new Dictionary<string, List<LeaderboardScore>>();

        /// <summary>
        /// The local user
        /// </summary>
        private Player localUser;

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
                return this.localUser != null;
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
            if (this.localUser == null)
            {
                this.localUser = new Player();
            }

            return NetTask.FromResult(this.localUser);
        }

        /// <summary>
        /// Logouts local user.
        /// </summary>
        /// <returns>
        /// <c>true</c> if logout was successful; otherwise, <c>false</c>.
        /// </returns>
        public Task<bool> Logout()
        {
            this.localUser = null;

            return NetTask.FromResult(true);
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
            List<LeaderboardScore> scoreLists;
            var leaderBoardFound = this.simulatedLeaderboards.TryGetValue(leaderboardCode, out scoreLists);
            if (!leaderBoardFound)
            {
                throw new ArgumentException("leaderboardCode not found");
            }

            var xml = scoreLists.Count > 0 ? "Leaderboard Content: " + this.Serialize(scoreLists) : string.Empty;

            return WaveServices.Platform.ShowMessageBoxAsync("ShowLeaderboard Simulation Mode", xml);
        }

        /// <summary>
        /// Shows a default/system view of the games leaderboards.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the view has been shown; otherwise, <c>false</c>.
        /// </returns>
        public Task<bool> ShowAllLeaderboards()
        {
            var sb = new StringBuilder();
            foreach (var leaderboardKeyPair in this.simulatedLeaderboards)
            {
                sb.AppendLine();
                sb.AppendLine("Leaderboard Code: " + leaderboardKeyPair.Key);
                var xml = this.Serialize(leaderboardKeyPair.Value);
                sb.AppendLine("Leaderboard Content: " + xml);
            }

            var result = sb.ToString();

            return WaveServices.Platform.ShowMessageBoxAsync("ShowAllLeaderboards Simulation Mode", result);
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
            List<LeaderboardScore> scoreLists;

            if (!this.simulatedLeaderboards.TryGetValue(leaderboardCode, out scoreLists))
            {
                scoreLists = new List<LeaderboardScore>();
                this.simulatedLeaderboards[leaderboardCode] = scoreLists;
            }

            scoreLists.Add(new LeaderboardScore()
            {
                RawScore = score,
            });

            scoreLists = scoreLists.OrderByDescending(s => s.RawScore).ToList();

            for (int i = 0; i < scoreLists.Count; i++)
            {
                scoreLists[i].Rank = i + 1;
            }

            return NetTask.FromResult(true);
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
            var result = Enumerable.Empty<LeaderboardScore>();
            List<LeaderboardScore> selectedLeaderboard;

            if (this.simulatedLeaderboards.TryGetValue(leaderboardCode, out selectedLeaderboard))
            {
                result = selectedLeaderboard.Take(count);
            }

            return NetTask.FromResult(result);
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
            var result = Enumerable.Empty<LeaderboardScore>();
            List<LeaderboardScore> selectedLeaderboard;

            if (this.simulatedLeaderboards.TryGetValue(leaderboardCode, out selectedLeaderboard))
            {
                var playerScoreIndex = selectedLeaderboard.FindIndex(l => l.ScoreHolder == this.localUser);

                if (playerScoreIndex >= 0)
                {
                    var fromIndex = playerScoreIndex - (count / 2);
                    fromIndex = Math.Max(0, fromIndex);

                    result = selectedLeaderboard.Skip(fromIndex).Take(count);
                }
                else
                {
                    result = selectedLeaderboard.Take(count);
                }
            }

            return NetTask.FromResult(result);
        }
        
        /// <summary>
        /// Shows a default/system view of the games achievements.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the view has been shown; otherwise, <c>false</c>.
        /// </returns>
        public Task<bool> ShowAchievements()
        {
            var xml = this.simulatedAchievements.Count > 0 ? "Achievements Content: " + this.Serialize(this.simulatedAchievements) : string.Empty;

            return WaveServices.Platform.ShowMessageBoxAsync("ShowAchievements Simulation Mode", xml);
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
            var achievement = this.simulatedAchievements.FirstOrDefault(a => a.AchievementCode == achievementCode);
            if (achievement != null)
            {
                achievement.TotalSteps = this.maxProgressInSimulation;
            }
            else
            {
                this.simulatedAchievements.Add(new Achievement()
                {
                    AchievementCode = achievementCode,
                    TotalSteps = this.maxProgressInSimulation,
                });
            }

            return NetTask.FromResult(true);
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
            var achievement = this.simulatedAchievements.FirstOrDefault(a => a.AchievementCode == achievementCode);
            if (achievement == null)
            {
                throw new ArgumentException("achievementCode not found");
            }

            achievement.TotalSteps = Math.Min(this.maxProgressInSimulation, achievement.TotalSteps + progress);

            return NetTask.FromResult(true);
        }

        /// <summary>
        /// Gets the achievements.
        /// </summary>
        /// <returns>
        /// The achievements.
        /// </returns>
        public Task<IEnumerable<Achievement>> GetAchievements()
        {
            return NetTask.FromResult(this.simulatedAchievements.Cast<Achievement>());
        }
        
        /// <summary>
        /// Serializes the specified value.
        /// </summary>
        /// <typeparam name="T">T param</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>
        /// Xml representation of the value.
        /// </returns>
        /// <exception cref="System.Exception">An error occurred</exception>
        private string Serialize<T>(T value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            try
            {
                var xmlserializer = new XmlSerializer(typeof(T));
                var stringWriter = new StringWriter();
                using (var writer = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true }))
                {
                    xmlserializer.Serialize(writer, value);
                    return stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred in Serialize method", ex);
            }
        }
    }
}
