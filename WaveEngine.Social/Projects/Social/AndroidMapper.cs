#region Using Statements

using Android.Gms.Games;
using Android.Gms.Games.Achievement;
using Android.Gms.Games.LeaderBoard;
using System.Collections.Generic;

#endregion

namespace WaveEngineAndroid.Social.Social
{
    internal static class AndroidMapper
    {
        internal static WaveEngine.Social.Player MapPlayer(IPlayer player)
        {
            WaveEngine.Social.Player wavePlayer = null;

            if (player != null)
            {
                wavePlayer = new WaveEngine.Social.Player()
                {
                    DisplayName = player.DisplayName,
                    Alias = player.DisplayName,
                    HiResImageUrl = player.HiResImageUrl,
                    IconImageUrl = player.IconImageUrl,
                    PlayerId = player.PlayerId,
                    RetrievedTimestamp = player.RetrievedTimestamp,
                    LastPlayedWithTimestamp = player.LastPlayedWithTimestamp
                };
            }

            return wavePlayer;
        }

        internal static List<WaveEngine.Social.Achievement> MapAchievements(List<IAchievement> achievements)
        {
            List<WaveEngine.Social.Achievement> waveAchievements = null;

            if (achievements != null)
            {
                waveAchievements = new List<WaveEngine.Social.Achievement>();

                foreach (var achievement in achievements)
                {
                    var waveAchievement = new WaveEngine.Social.Achievement()
                    {
                        AchievementCode = achievement.AchievementId,
                        Description = achievement.Description,
                        Name = achievement.Name,
                        State = achievement.State,
                        Type = achievement.Type,
                        Player = AndroidMapper.MapPlayer(achievement.Player),
                    };

                    waveAchievements.Add(waveAchievement);
                }
            }

            return waveAchievements;
        }

        internal static List<WaveEngine.Social.LeaderboardScore> MapLeaderBoards(List<ILeaderboardScore> scores)
        {
            List<WaveEngine.Social.LeaderboardScore> waveScores = null;

            if (scores != null)
            {
                waveScores = new List<WaveEngine.Social.LeaderboardScore>();

                foreach (var leaderboardScore in scores)
                {
                    var waveLeaderBoardScore = new WaveEngine.Social.LeaderboardScore()
                    {
                        DisplayRank = leaderboardScore.DisplayRank,
                        DisplayScore = leaderboardScore.DisplayScore,
                        Rank = leaderboardScore.Rank,
                        RawScore = leaderboardScore.RawScore,
                        ScoreHolder = AndroidMapper.MapPlayer(leaderboardScore.ScoreHolder),
                        ScoreTag = leaderboardScore.ScoreTag,
                        TimestampMillis = leaderboardScore.TimestampMillis,
                    };

                    waveScores.Add(waveLeaderBoardScore);
                }
            }

            return waveScores;
        }
    }

}
