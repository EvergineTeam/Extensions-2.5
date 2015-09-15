#region Using Statements

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using WaveEngine.Social;
using WaveEngine.Framework;

#endregion

namespace WaveEngineAndroid.Social.Social
{
    public class GooglePlayGameServiceUtils : ISocial
    {
        private Activity activity;
        private WaveEngine.Adapter.Adapter adapter;
        private GooglePlayGameHelper helper;

        public GooglePlayGameServiceUtils()
        {
            this.adapter = (Game.Current).Application.Adapter as WaveEngine.Adapter.Adapter;
            this.activity = adapter.Activity;
        }

        public void Init()
        {
            this.helper = new GooglePlayGameHelper(activity, adapter);
            this.helper.ViewForPopups = activity.CurrentFocus;
            this.helper.Initialize();
        }

        public Task<bool> Login()
        {
            var tcsLogin = new TaskCompletionSource<bool>();

            this.helper.OnSignedIn += (s, e) =>
            {
                var signedIn = true;
                tcsLogin.TrySetResult(signedIn);
            };

            this.helper.OnSignInFailed += (s, e) =>
            {
                var signedIn = false;
                tcsLogin.TrySetResult(signedIn);
            };

            this.helper.SignIn();

            return tcsLogin.Task;
        }

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

        public Task<bool> ShowAllLeaderboards()
        {
            this.ThrowIfNotInitialized();
            this.ThrowIfNotLoggedIn();

            var tcs = new TaskCompletionSource<bool>();
            this.helper.ShowAllLeaderBoardsIntent(tcs);

            return tcs.Task;
        }

        public Task<bool> ShowLeaderboard(string leaderboardCode)
        {
            this.ThrowIfNotInitialized();
            this.ThrowIfNotLoggedIn();

            var tcs = new TaskCompletionSource<bool>();
            this.helper.ShowLeaderBoardIntentForLeaderboard(leaderboardCode, tcs);

            return tcs.Task;
        }

        public Task<bool> AddNewScore(string leaderboardCode, long score)
        {
            this.ThrowIfNotInitialized();
            this.ThrowIfNotLoggedIn();

            this.helper.SubmitScore(leaderboardCode, score);

            return Task.FromResult(true);
        }

        public Task<bool> ShowAchievements()
        {
            this.ThrowIfNotInitialized();
            this.ThrowIfNotLoggedIn();

            var tcs = new TaskCompletionSource<bool>();

            this.helper.ShowAchievements(tcs);

            return Task.FromResult(true);
        }

        public Task<bool> UnlockAchievement(string achievementCode)
        {
            this.ThrowIfNotInitialized();
            this.ThrowIfNotLoggedIn();

            this.helper.UnlockAchievement(achievementCode);

            return Task.FromResult(true);
        }

        public Task<bool> IncrementAchievement(string achievementCode, int progress)
        {
            this.ThrowIfNotInitialized();
            this.ThrowIfNotLoggedIn();

            this.helper.IncrementAchievement(achievementCode, progress);

            return Task.FromResult(true);
        }

        public Task<List<WaveEngine.Social.Achievement>> GetAchievements()
        {
            this.ThrowIfNotInitialized();
            this.ThrowIfNotLoggedIn();

            var tcs = new TaskCompletionSource<List<WaveEngine.Social.Achievement>>();

            this.helper.LoadAchievements(tcs);

            return tcs.Task;
        }

        public Task<Dictionary<string, List<WaveEngine.Social.LeaderboardScore>>> GetTopScoresFromLeaderboard(string leaderboardCode)
        {
            this.ThrowIfNotInitialized();
            this.ThrowIfNotLoggedIn();

            var tcs = new TaskCompletionSource<Dictionary<string, List<WaveEngine.Social.LeaderboardScore>>>();

            this.helper.LoadTopScores(leaderboardCode, tcs);

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
