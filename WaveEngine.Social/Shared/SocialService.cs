#region File Description
//-----------------------------------------------------------------------------
// SocialService
//
// Copyright © 2015 Wave Engine S.L. All rights reserved.
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
        /// The maximum progress in simulation
        /// </summary>
        private readonly long maxProgressInSimulation = 100;

        /// <summary>
        /// The simulated achievements
        /// </summary>
        private readonly List<WaveEngine.Social.Achievement> fakeAchievements = new List<Achievement>();

        /// <summary>
        /// The simulated leaderboards
        /// </summary>
        private readonly Dictionary<string, List<WaveEngine.Social.LeaderboardScore>> simulatedLeaderboards = new Dictionary<string, List<WaveEngine.Social.LeaderboardScore>>();

        /// <summary>
        /// Gets or sets a value indicating whether [simulation mode].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [simulation mode]; otherwise, <c>false</c>.
        /// </value>
        public bool SimulationMode { get; set; }

        /// <summary>
        /// The community
        /// </summary>
        private readonly ISocial social;

        /// <summary>
        /// Initializes a new instance of the <see cref="SocialService"/> class.
        /// </summary>
        public SocialService()
        {
#if ANDROID
            this.social = new GooglePlayGameServiceUtils();
#elif IOS
            this.social = new GameCenterUtils();
#else
            this.social = new SocialFakeHelper();
#endif
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Initialize()
        {
        }

        /// <summary>
        /// Terminates this instance.
        /// </summary>
        protected override void Terminate()
        {
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="properties">The properties.</param>
        public void Initialize(Dictionary<string, string> properties)
        {
            if (!this.SimulationMode)
            {
                this.social.Init();
            }
        }

        /// <summary>
        /// Logouts this instance.
        /// </summary>
        /// <returns>Task task</returns>
        public Task<bool> Logout()
        {
            if (!this.SimulationMode)
            {
                return this.social.Logout();
            }

            return System.Threading.Tasks.Task.FromResult(true);
        }

        /// <summary>
        /// Logins this instance.
        /// </summary>
        /// <returns>Task task</returns>
        public Task<bool> Login()
        {
            if (!this.SimulationMode)
            {
                return this.social.Login();
            }

            return System.Threading.Tasks.Task.FromResult(true);
        }

        /// <summary>
        /// Shows the leaderboard.
        /// </summary>
        /// <param name="leaderboardCode">The leaderboard code.</param>
        public Task<bool> ShowLeaderboard(string leaderboardCode)
        {
            if (!this.SimulationMode)
            {
                return this.social.ShowLeaderboard(leaderboardCode);
            }
            else
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
        }

        /// <summary>
        /// Shows the leaderboards.
        /// </summary>
        public Task<bool> ShowAllLeaderboards()
        {
            if (!this.SimulationMode)
            {
                return this.social.ShowAllLeaderboards();
            }
            else
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
        }

        /// <summary>
        /// Adds the new score.
        /// </summary>
        /// <param name="leaderboardCode">The leaderboard code.</param>
        /// <param name="score">The score.</param>
        public Task<bool> AddNewScore(string leaderboardCode, long score)
        {
            if (!this.SimulationMode)
            {
                return this.social.AddNewScore(leaderboardCode, score);
            }
            else
            {
                List<LeaderboardScore> scoreLists;
                var leaderBoardFound = this.simulatedLeaderboards.TryGetValue(leaderboardCode, out scoreLists);
                if (!leaderBoardFound)
                {
                    scoreLists = new List<LeaderboardScore>();
                    this.simulatedLeaderboards[leaderboardCode] = scoreLists;
                }

                scoreLists.Add(new LeaderboardScore()
                {
                    RawScore = score
                });

                return System.Threading.Tasks.Task.FromResult(true);
            }
        }

        /// <summary>
        /// Shows the archievements.
        /// </summary>
        public Task<bool> ShowAchievements()
        {
            if (!this.SimulationMode)
            {
                return this.social.ShowAchievements();
            }
            else
            {
                var xml = this.fakeAchievements.Count > 0 ? "Achievements Content: " + this.Serialize(this.fakeAchievements) : string.Empty;

                return WaveServices.Platform.ShowMessageBoxAsync("ShowAchievements Simulation Mode", xml);
            }
        }

        /// <summary>
        /// Unlocks the archievement.
        /// </summary>
        /// <param name="achievementCode">The achievement code.</param>
        /// <returns>Task task</returns>
        public Task<bool> UnlockAchievement(string achievementCode)
        {
            if (!this.SimulationMode)
            {
                return this.social.UnlockAchievement(achievementCode);
            }

            var achievement = this.fakeAchievements.FirstOrDefault(a => a.AchievementCode == achievementCode);
            if (achievement != null)
            {
                achievement.TotalSteps = this.maxProgressInSimulation;
            }
            else
            {
                this.fakeAchievements.Add(new Achievement()
                {
                    AchievementCode = achievementCode,
                    TotalSteps = this.maxProgressInSimulation,
                });
            }

            return System.Threading.Tasks.Task.FromResult(true);
        }

        /// <summary>
        /// Increments the achievement.
        /// </summary>
        /// <param name="achievementCode">The achievement code.</param>
        /// <param name="progress">The progress.</param>
        /// <returns>Task task</returns>
        public Task<bool> IncrementAchievement(string achievementCode, int progress)
        {
            if (!this.SimulationMode)
            {
                return this.social.IncrementAchievement(achievementCode, progress);
            }

            var achievement = this.fakeAchievements.FirstOrDefault(a => a.AchievementCode == achievementCode);
            if (achievement == null)
            {
                throw new ArgumentException("achievementCode not found");
            }

            achievement.TotalSteps = Math.Min(this.maxProgressInSimulation, achievement.TotalSteps + progress);

            return System.Threading.Tasks.Task.FromResult(true);
        }

        /// <summary>
        /// Gets the achievements.
        /// </summary>
        /// <returns>Task task</returns>
        public Task<List<WaveEngine.Social.Achievement>> GetAchievements()
        {
            if (!this.SimulationMode)
            {
                return this.social.GetAchievements();
            }

            return System.Threading.Tasks.Task.FromResult(this.fakeAchievements);
        }

        /// <summary>
        /// Gets the top scores from leaderboard.
        /// </summary>
        /// <param name="leaderboardCode">The leaderboard code.</param>
        /// <returns>Task task</returns>
        public Task<Dictionary<string, List<WaveEngine.Social.LeaderboardScore>>> GetTopScoresFromLeaderboard(string leaderboardCode)
        {
            if (!this.SimulationMode)
            {
                return this.social.GetTopScoresFromLeaderboard(leaderboardCode);
            }

            return System.Threading.Tasks.Task.FromResult(this.simulatedLeaderboards);
        }

        /// <summary>
        /// Serializes the specified value.
        /// </summary>
        /// <typeparam name="T">T param</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>string s</returns>
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
