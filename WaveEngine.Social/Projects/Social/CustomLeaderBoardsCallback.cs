#region Using Statements

using System.Collections.Generic;
using Android.Gms.Games.LeaderBoard;
using System.Threading.Tasks;
using Android.Gms.Common.Apis;
using Java.Interop;

#endregion

namespace WaveEngineAndroid.Social.Social
{
    internal class CustomLeaderBoardsCallback : Java.Lang.Object, IResultCallback
    {
        private TaskCompletionSource<IEnumerable<WaveEngine.Social.LeaderboardScore>> tcs;
        
        public CustomLeaderBoardsCallback(TaskCompletionSource<IEnumerable<WaveEngine.Social.LeaderboardScore>> tcs)
            : base()
        {
            this.tcs = tcs;
        }

        #region IResultCallback implementation
        public void OnResult(Java.Lang.Object result)
        {
            var ar = JavaObjectExtensions.JavaCast<ILeaderboardsLoadScoresResult>(result);
            var waveScores = new List<WaveEngine.Social.LeaderboardScore>();

            if (ar != null)
            {
                var id = ar.Leaderboard.LeaderboardId;

                var scores = new List<ILeaderboardScore>();
                var count = ar.Scores.Count;
                for (int i = 0; i < count; i++)
                {
                    var item = ar.Scores.Get(i);

                    var score = JavaObjectExtensions.JavaCast<ILeaderboardScore>(item);
                    scores.Add(score);
                }

                waveScores = AndroidMapper.MapLeaderBoards(scores);
            }

            this.tcs.TrySetResult(waveScores);
        }
        #endregion
    }
}
