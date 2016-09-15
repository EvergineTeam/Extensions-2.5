#region Using Statements

using GameKit;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;

#endregion

namespace WaveEngineiOS.Social.Social
{
    internal static class IOSMapper
    {
        internal static async Task<WaveEngine.Social.Player> MapPlayer(GKPlayer player)
        {
            WaveEngine.Social.Player wavePlayer = null;

            if (player != null)
            {
                var iconPath = await GetPlayerPhotoPath(player, GKPhotoSize.Small);
                
                wavePlayer = new WaveEngine.Social.Player()
                {
                    DisplayName = player.DisplayName,
                    PlayerId = player.PlayerID,
                    Alias = player.Alias,
                    HiResImageUrl = null,
                    IconImageUrl = iconPath,
                    LastPlayedWithTimestamp = 0,
                    RetrievedTimestamp = 0,
                };
            }

            return wavePlayer;
        }

        private static async Task<string> GetPlayerPhotoPath(GKPlayer player, GKPhotoSize size)
        {
            try
            {
                var icon = await player.LoadPhotoAsync(size);
                var iconJPEG = icon.AsJPEG();
                var iconPath = Path.Combine(Path.GetTempPath(), player.PlayerID + size);
                iconJPEG.Save(iconPath, atomically: false);
                return iconPath;
            }
            catch
            {
                return null;
            }
        }

        internal static async Task<IEnumerable<WaveEngine.Social.Achievement>> MapAchievements(IEnumerable<GKAchievement> achievements)
        {
            List<WaveEngine.Social.Achievement> waveAchievements = null;

            if (achievements != null)
            {
                waveAchievements = new List<WaveEngine.Social.Achievement>();

                foreach (var achievement in achievements)
                {
                    var waveAchievement = new WaveEngine.Social.Achievement()
                    {
                        AchievementCode = achievement.Identifier,
                        CurrentSteps = achievement.PercentComplete,
                        Description = achievement.Description,
                        Player = await IOSMapper.MapPlayer(achievement.Player),
                        RevealedImageUri = null,
                        UnlockedImageUri = null,
                        State = 0,
                        TotalSteps = 0,
                        Type = 0,
                        LastUpdatedTimestamp = 0,
                    };

                    waveAchievements.Add(waveAchievement);
                }
            }

            return waveAchievements;
        }

        internal static async Task<IEnumerable<WaveEngine.Social.LeaderboardScore>> MapLeaderBoards(IEnumerable<GKScore> scores)
        {
            List<WaveEngine.Social.LeaderboardScore> waveScores = null;

            if (scores != null)
            {
                waveScores = new List<WaveEngine.Social.LeaderboardScore>();

                foreach (var item in scores)
                {
                    var scoreHolder = await IOSMapper.MapPlayer(item.Player);

                    var waveLeaderBoardScore = new WaveEngine.Social.LeaderboardScore()
                    {
                        DisplayRank = null,
                        DisplayScore = item.FormattedValue,
                        Rank = item.Rank,
                        RawScore = item.Value,
                        ScoreHolder = scoreHolder,
                        ScoreTag = null,
                    };

                    waveScores.Add(waveLeaderBoardScore);
                }
            }

            return waveScores;
        }
    }
}