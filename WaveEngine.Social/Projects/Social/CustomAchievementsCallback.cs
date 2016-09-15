#region Using Statements

using System.Collections.Generic;
using Android.Gms.Games.Achievement;
using System.Threading.Tasks;
using Android.Gms.Common.Apis;
using Java.Interop;

#endregion

namespace WaveEngineAndroid.Social.Social
{
    internal class CustomAchievementsCallback : Java.Lang.Object, IResultCallback
    {
        private TaskCompletionSource<IEnumerable<WaveEngine.Social.Achievement>> tcs;
        
        public CustomAchievementsCallback(TaskCompletionSource<IEnumerable<WaveEngine.Social.Achievement>> tcs)
            : base()
        {
            this.tcs = tcs;
        }

        #region IResultCallback implementation
        public void OnResult(Java.Lang.Object result)
        {
            var ar = JavaObjectExtensions.JavaCast<IAchievementsLoadAchievementsResult>(result);
            var waveAchievements = new List<WaveEngine.Social.Achievement>();

            if (ar != null)
            {
                var achievements = new List<IAchievement>();

                achievements.Clear();
                var count = ar.Achievements.Count;
                for (int i = 0; i < count; i++)
                {
                    var item = ar.Achievements.Get(i);
                    var a = JavaObjectExtensions.JavaCast<IAchievement>(item);
                    achievements.Add(a);
                }

                waveAchievements = AndroidMapper.MapAchievements(achievements);
            }

            this.tcs.TrySetResult(waveAchievements);
        }


        #endregion
    }
}
