using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Games;
using Android.Gms.Games.Achievement;
using Android.Gms.Games.LeaderBoard;
using Android.OS;
using Android.Views;

namespace WaveEngineAndroid.Social.Social
{
    /// <summary>
    /// Basic wrapper for interfacing with the GooglePlayServices Game API's
    /// </summary>
    public class GooglePlayGameHelper : Java.Lang.Object, IGoogleApiClientConnectionCallbacks, IGoogleApiClientOnConnectionFailedListener
    {
        private WaveEngine.Adapter.Adapter adapter;
        private IGoogleApiClient client;
        private Activity activity;
        private bool signedOut = true;
        private bool signingin = false;
        private bool resolving = false;
        private List<IAchievement> achievments = new List<IAchievement>();
        private Dictionary<string, List<ILeaderboardScore>> scores = new Dictionary<string, List<ILeaderboardScore>>();
        private AchievementsCallback achievmentsCallback;
        private LeaderBoardsCallback leaderboardsCallback;

        private const int REQUEST_LEADERBOARD = 9002;
        private const int REQUEST_ALL_LEADERBOARDS = 9003;
        private const int REQUEST_ACHIEVEMENTS = 9004;
        private const int RC_RESOLVE = 9001;

        private TaskCompletionSource<bool> currentGPlayPanelTCS;

        private TaskCompletionSource<bool> CurrentGPlayPanelTCS
        {
            get
            {
                return this.currentGPlayPanelTCS;
            }

            set
            {
                if (this.currentGPlayPanelTCS != null)
                {
                    this.currentGPlayPanelTCS.TrySetCanceled();
                    this.currentGPlayPanelTCS = null;
                }

                this.currentGPlayPanelTCS = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user is signed out or not.
        /// </summary>
        /// <value><c>true</c> if signed out; otherwise, <c>false</c>.</value>
        public bool SignedOut
        {
            get { return signedOut; }
            set
            {
                if (signedOut != value)
                {
                    signedOut = value;
                    // Store if we Signed Out so we don't bug the player next time.
                    using (var settings = this.activity.GetSharedPreferences("googleplayservicessettings", FileCreationMode.Private))
                    {
                        using (var e = settings.Edit())
                        {
                            e.PutBoolean("SignedOut", signedOut);
                            e.Commit();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the gravity for the GooglePlay Popups. 
        /// Defaults to Bottom|Center
        /// </summary>
        /// <value>The gravity for popups.</value>
        public GravityFlags GravityForPopups { get; set; }

        /// <summary>
        /// The View on which the Popups should show
        /// </summary>
        /// <value>The view for popups.</value>
        public View ViewForPopups { get; set; }

        /// <summary>
        /// This event is fired when a user successfully signs in
        /// </summary>
        public event EventHandler OnSignedIn;

        /// <summary>
        /// This event is fired when the Sign in fails for any reason
        /// </summary>
        public event EventHandler OnSignInFailed;

        /// <summary>
        /// This event is fired when the user Signs out
        /// </summary>
        public event EventHandler OnSignedOut;

        /// <summary>
        /// List of Achievements. Populated by LoadAchievements
        /// </summary>
        /// <value>The achievements.</value>
        public List<IAchievement> Achievements
        {
            get { return achievments; }
        }

        public GooglePlayGameHelper(Activity activity, WaveEngine.Adapter.Adapter adapter)
        {
            this.adapter = adapter;
            this.activity = activity;
            this.GravityForPopups = GravityFlags.Bottom | GravityFlags.Center;
            achievmentsCallback = new AchievementsCallback(this);
            leaderboardsCallback = new LeaderBoardsCallback(this);

            this.adapter.OnActivityResult += this.OnGooglePlayPanelActivityResult;
        }

        public void Initialize()
        {

            var settings = this.activity.GetSharedPreferences("googleplayservicessettings", FileCreationMode.Private);
            signedOut = settings.GetBoolean("SignedOut", true);

            if (!signedOut)
                CreateClient();
        }

        private void CreateClient()
        {

            // did we log in with a player id already? If so we don't want to ask which account to use
            var settings = this.activity.GetSharedPreferences("googleplayservicessettings", FileCreationMode.Private);
            var id = settings.GetString("playerid", String.Empty);

            var builder = new GoogleApiClientBuilder(activity, this, this);
            builder.AddApi(GamesClass.Api);
            builder.AddScope(GamesClass.ScopeGames);
            builder.SetGravityForPopups((int)GravityForPopups);
            if (ViewForPopups != null)
                builder.SetViewForPopups(ViewForPopups);
            if (!string.IsNullOrEmpty(id))
            {
                builder.SetAccountName(id);
            }
            client = builder.Build();
        }

        /// <summary>
        /// Start the GooglePlayClient. This should be called from your Activity Start
        /// </summary>
        public void Start()
        {

            if (SignedOut && !signingin)
                return;

            if (client != null && !client.IsConnected)
            {
                client.Connect();
            }
        }

        /// <summary>
        /// Disconnects from the GooglePlayClient. This should be called from your Activity Stop
        /// </summary>
        public void Stop()
        {

            if (client != null && client.IsConnected)
            {
                client.Disconnect();
            }
        }

        /// <summary>
        /// Reconnect to google play.
        /// </summary>
        public void Reconnect()
        {
            if (client != null)
                client.Reconnect();
        }

        /// <summary>
        /// Sign out of Google Play and make sure we don't try to auto sign in on the next startup
        /// </summary>
        public void SignOut()
        {

            SignedOut = true;
            if (client.IsConnected)
            {
                GamesClass.SignOut(client);
                Stop();
                using (var settings = this.activity.GetSharedPreferences("googleplayservicessettings", FileCreationMode.Private))
                {
                    using (var e = settings.Edit())
                    {
                        e.PutString("playerid", String.Empty);
                        e.Commit();
                    }
                }
                client.Dispose();
                client = null;
                if (OnSignedOut != null)
                    OnSignedOut(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Attempt to Sign in to Google Play
        /// </summary>
        public void SignIn()
        {

            signingin = true;
            if (client == null)
                CreateClient();

            if (client.IsConnected)
                return;

            if (client.IsConnecting)
                return;

            var result = GooglePlayServicesUtil.IsGooglePlayServicesAvailable(activity);
            if (result != ConnectionResult.Success)
            {
                return;
            }

            Start();

        }

        /// <summary>
        /// Unlocks the achievement.
        /// </summary>
        /// <param name="achievementCode">Achievement code from you applications Google Play Game Services Achievements Page</param>
        public void UnlockAchievement(string achievementCode)
        {
            GamesClass.Achievements.Unlock(client, achievementCode);
        }

        public void IncrementAchievement(string achievementCode, int progress)
        {
            GamesClass.Achievements.Increment(client, achievementCode, progress);
        }

        /// <summary>
        /// Show the built in google Achievements Activity. This will cause your application to go into a Paused State
        /// </summary>
        /// <param name="tcs">The task completion source</param>
        public void ShowAchievements(TaskCompletionSource<bool> tcs)
        {
            var intent = GamesClass.Achievements.GetAchievementsIntent(client);
            activity.StartActivityForResult(intent, REQUEST_ACHIEVEMENTS);
        }

        /// <summary>
        /// Submit a score to google play. The score will only be updated if it is greater than the existing score. 
        /// This is not immediate but will occur at the next sync of the google play client.
        /// </summary>
        /// <param name="leaderboardCode">Leaderboard code from you applications Google Play Game Services Leaderboards Page</param>
        /// <param name="value">The value of the score</param>
        public void SubmitScore(string leaderboardCode, long value)
        {
            GamesClass.Leaderboards.SubmitScore(client, leaderboardCode, value);
        }

        /// <summary>
        /// Submit a score to google play. The score will only be updated if it is greater than the existing score. 
        /// This is not immediate but will occur at the next sync of the google play client.
        /// </summary>
        /// <param name="leaderboardCode">Leaderboard code from you applications Google Play Game Services Leaderboards Page</param>
        /// <param name="value">The value of the score</param>
        /// <param name="metadata">Additional MetaData to attach. Must be a URI safe string with a max length of 64 characters</param>
        public void SubmitScore(string leaderboardCode, long value, string metadata)
        {
            GamesClass.Leaderboards.SubmitScore(client, leaderboardCode, value, metadata);
        }

        /// <summary>
        /// Show the built in leaderboard activity for the leaderboard code.
        /// </summary>
        /// <param name="leaderboardCode">Leaderboard code from you applications Google Play Game Services Leaderboards Page</param>
        /// <param name="tcs">The task completion source</param>
        public void ShowLeaderBoardIntentForLeaderboard(string leaderboardCode, TaskCompletionSource<bool> tcs)
        {
            var intent = GamesClass.Leaderboards.GetLeaderboardIntent(client, leaderboardCode);
            this.CurrentGPlayPanelTCS = tcs;

            activity.StartActivityForResult(intent, REQUEST_LEADERBOARD);            
        }

        /// <summary>
        /// Show the built in leaderboard activity for all the leaderboards setup for your application
        /// </summary>
        /// <param name="tcs">The task completion source</param>
        public void ShowAllLeaderBoardsIntent(TaskCompletionSource<bool> tcs)
        {
            var intent = GamesClass.Leaderboards.GetAllLeaderboardsIntent(client);
            this.CurrentGPlayPanelTCS = tcs;

            activity.StartActivityForResult(intent, REQUEST_ALL_LEADERBOARDS);            
        }

        public void LoadAchievements(TaskCompletionSource<List<WaveEngine.Social.Achievement>> tcs)
        {
            var customAchievementsCallback = new CustomAchievementsCallback(tcs);

            var pendingResult = GamesClass.Achievements.Load(client, false);
            pendingResult.SetResultCallback(customAchievementsCallback);
        }

        public void LoadTopScores(string leaderboardCode, TaskCompletionSource<Dictionary<string, List<WaveEngine.Social.LeaderboardScore>>> tcs)
        {
            var customLeaderboardsCallback = new CustomLeaderBoardsCallback(tcs);

            var pendingResult = GamesClass.Leaderboards.LoadTopScores(client, leaderboardCode, 2, 0, 25);
            pendingResult.SetResultCallback(customLeaderboardsCallback);
        }

        #region IGoogleApiClientConnectionCallbacks implementation

        public void OnConnected(Bundle connectionHint)
        {
            resolving = false;
            SignedOut = false;
            signingin = false;

            using (var settings = this.activity.GetSharedPreferences("googleplayservicessettings", FileCreationMode.Private))
            {
                using (var e = settings.Edit())
                {
                    e.PutString("playerid", GamesClass.GetCurrentAccountName(client));
                    e.Commit();
                }
            }

            if (OnSignedIn != null)
            {
                OnSignedIn(this, EventArgs.Empty);
            }
        }

        public void OnConnectionSuspended(int resultCode)
        {
            resolving = false;
            SignedOut = false;
            signingin = false;
            client.Disconnect();
            if (this.OnSignInFailed != null)
            {
                this.OnSignInFailed(this, EventArgs.Empty);
            }
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            if (resolving)
                return;

            if (result.HasResolution)
            {
                resolving = true;
                result.StartResolutionForResult(activity, RC_RESOLVE);
                this.adapter.OnActivityResult += this.OnConnectionFailedActivityResult;
                return;
            }

            resolving = false;
            SignedOut = false;
            signingin = false;
            if (this.OnSignInFailed != null)
                this.OnSignInFailed(this, EventArgs.Empty);
        }

        #endregion

        /// <summary>
        /// Processes the Activity Results from the Signin process. MUST be called from your activity OnActivityResult override.
        /// </summary>
        /// <param name="requestCode">Request code.</param>
        /// <param name="resultCode">Result code.</param>
        /// <param name="data">Data.</param>
        public void OnConnectionFailedActivityResult(int requestCode, Result resultCode, Intent data)
        {
            this.adapter.OnActivityResult -= OnConnectionFailedActivityResult;

            if (requestCode == RC_RESOLVE)
            {
                if (resultCode == Result.Ok)
                {
                    Start();
                }
                else
                {
                    if (this.OnSignInFailed != null)
                        this.OnSignInFailed(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Processes the Activity Results from the GPLay panel process. MUST be called from your activity OnActivityResult override.
        /// </summary>
        /// <param name="requestCode">Request code.</param>
        /// <param name="resultCode">Result code.</param>
        /// <param name="data">Data.</param>
        public void OnGooglePlayPanelActivityResult(int requestCode, Result resultCode, Intent data)
        {
            this.adapter.OnActivityResult -= OnConnectionFailedActivityResult;

            if (requestCode == REQUEST_ALL_LEADERBOARDS
                || requestCode == REQUEST_ACHIEVEMENTS)
            {
                this.currentGPlayPanelTCS.TrySetResult(true);
            }
        }

        internal class AchievementsCallback : Java.Lang.Object, IResultCallback
        {
            GooglePlayGameHelper helper;

            public AchievementsCallback(GooglePlayGameHelper helper)
                : base()
            {
                this.helper = helper;
            }

            #region IResultCallback implementation
            public void OnResult(Java.Lang.Object result)
            {
                var ar = Java.Interop.JavaObjectExtensions.JavaCast<IAchievementsLoadAchievementsResult>(result);
                if (ar != null)
                {
                    helper.achievments.Clear();
                    var count = ar.Achievements.Count;
                    for (int i = 0; i < count; i++)
                    {
                        var item = ar.Achievements.Get(i);
                        var a = Java.Interop.JavaObjectExtensions.JavaCast<IAchievement>(item);
                        helper.achievments.Add(a);
                    }
                }
            }
            #endregion
        }

        internal class LeaderBoardsCallback : Java.Lang.Object, IResultCallback
        {
            GooglePlayGameHelper helper;

            public LeaderBoardsCallback(GooglePlayGameHelper helper)
                : base()
            {
                this.helper = helper;
            }

            #region IResultCallback implementation
            public void OnResult(Java.Lang.Object result)
            {
                var ar = Java.Interop.JavaObjectExtensions.JavaCast<ILeaderboardsLoadScoresResult>(result);
                if (ar != null)
                {
                    var id = ar.Leaderboard.LeaderboardId;
                    if (!helper.scores.ContainsKey(id))
                    {
                        helper.scores.Add(id, new List<ILeaderboardScore>());
                    }
                    helper.scores[id].Clear();
                    var count = ar.Scores.Count;
                    for (int i = 0; i < count; i++)
                    {
                        var item = ar.Scores.Get(i);

                        var score = Java.Interop.JavaObjectExtensions.JavaCast<ILeaderboardScore>(item);
                        helper.scores[id].Add(score);
                    }
                }
            }
            #endregion
        }

        internal class CustomAchievementsCallback : Java.Lang.Object, IResultCallback
        {
            TaskCompletionSource<List<WaveEngine.Social.Achievement>> tcs;

            List<IAchievement> achievements;

            public CustomAchievementsCallback(TaskCompletionSource<List<WaveEngine.Social.Achievement>> tcs)
                : base()
            {
                this.tcs = tcs;
            }

            #region IResultCallback implementation
            public void OnResult(Java.Lang.Object result)
            {
                var ar = Java.Interop.JavaObjectExtensions.JavaCast<IAchievementsLoadAchievementsResult>(result);
                if (ar != null)
                {
                    achievements = new List<IAchievement>();

                    achievements.Clear();
                    var count = ar.Achievements.Count;
                    for (int i = 0; i < count; i++)
                    {
                        var item = ar.Achievements.Get(i);
                        var a = Java.Interop.JavaObjectExtensions.JavaCast<IAchievement>(item);
                        achievements.Add(a);
                    }
                }

                var waveAchievements = Mapper.MapAchievements(achievements);

                tcs.TrySetResult(waveAchievements);
            }

            
            #endregion
        }

        internal class CustomLeaderBoardsCallback : Java.Lang.Object, IResultCallback
        {
            TaskCompletionSource<Dictionary<string, List<WaveEngine.Social.LeaderboardScore>>> tcs;

            Dictionary<string, List<ILeaderboardScore>> scores;

            public CustomLeaderBoardsCallback(TaskCompletionSource<Dictionary<string, List<WaveEngine.Social.LeaderboardScore>>> tcs)
                : base()
            {
                this.tcs = tcs;
            }

            #region IResultCallback implementation
            public void OnResult(Java.Lang.Object result)
            {
                var ar = Java.Interop.JavaObjectExtensions.JavaCast<ILeaderboardsLoadScoresResult>(result);
                if (ar != null)
                {
                    scores = new Dictionary<string, List<ILeaderboardScore>>();

                    var id = ar.Leaderboard.LeaderboardId;
                    if (!scores.ContainsKey(id))
                    {
                        scores.Add(id, new List<ILeaderboardScore>());
                    }
                    scores[id].Clear();
                    var count = ar.Scores.Count;
                    for (int i = 0; i < count; i++)
                    {
                        var item = ar.Scores.Get(i);

                        var score = Java.Interop.JavaObjectExtensions.JavaCast<ILeaderboardScore>(item);
                        scores[id].Add(score);
                    }
                }

                var waveScores = Mapper.MapLeaderBoards(scores);

                tcs.TrySetResult(waveScores);
            }
            #endregion
        }

        internal static class Mapper
        {
            internal static WaveEngine.Social.Player MapPlayer(IPlayer player)
            {
                WaveEngine.Social.Player wavePlayer = null;

                if (player != null)
                {
                    wavePlayer = new WaveEngine.Social.Player()
                    {
                        DisplayName = player.DisplayName,
                        HasHiResImage = player.HasHiResImage,
                        HasIconImage = player.HasIconImage,
                        HiResImageUrl = player.HiResImageUrl,
                        IconImageUrl = player.IconImageUrl,
                        PlayerId = player.PlayerId,
                        RetrievedTimestamp = player.RetrievedTimestamp,
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
                            Player = Mapper.MapPlayer(achievement.Player),
                        };
                        waveAchievements.Add(waveAchievement);
                    }
                }

                return waveAchievements;
            }

            internal static Dictionary<string, List<WaveEngine.Social.LeaderboardScore>> MapLeaderBoards(Dictionary<string, List<ILeaderboardScore>> scores)
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
                                DisplayRank = leaderboardScore.DisplayRank,
                                DisplayScore = leaderboardScore.DisplayScore,
                                Rank = leaderboardScore.Rank,
                                RawScore = leaderboardScore.RawScore,
                                ScoreHolder = Mapper.MapPlayer(leaderboardScore.ScoreHolder),
                                ScoreHolderDisplayName = leaderboardScore.ScoreHolderDisplayName,
                                ScoreHolderHiResImageUrl = leaderboardScore.ScoreHolderHiResImageUrl,
                                ScoreHolderIconImageUrl = leaderboardScore.ScoreHolderIconImageUrl,
                                ScoreTag = leaderboardScore.ScoreTag,
                                TimestampMillis = leaderboardScore.TimestampMillis,
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