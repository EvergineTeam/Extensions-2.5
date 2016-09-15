#region Using Statements

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Games;
using Android.Gms.Games.Achievement;
using Android.OS;
using Android.Views;
using WaveEngine.Social;
using Android.Gms.Games.LeaderBoard;

#endregion

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
        /// Initializes a new instance of the <see cref="GooglePlayGameHelper"/> class.
        /// </summary>
        /// <param name="activity">The activity.</param>
        /// <param name="adapter">The adapter.</param>
        public GooglePlayGameHelper(Activity activity, WaveEngine.Adapter.Adapter adapter)
        {
            this.adapter = adapter;
            this.activity = activity;
            this.GravityForPopups = GravityFlags.Top | GravityFlags.Center;

            this.adapter.OnActivityResult += this.OnGooglePlayPanelActivityResult;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
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
            builder.SetGravityForPopups((int)this.GravityForPopups);

            if (this.ViewForPopups != null)
            {
                builder.SetViewForPopups(this.ViewForPopups);
            }

            if (!string.IsNullOrEmpty(id))
            {
                builder.SetAccountName(id);
            }

            this.client = builder.Build();
        }

        /// <summary>
        /// Start the GooglePlayClient. This should be called from your Activity Start
        /// </summary>
        public void Start()
        {
            if (this.SignedOut && !this.signingin)
            {
                return;
            }

            if (this.client != null && !this.client.IsConnected)
            {
                this.client.Connect();
            }
        }

        /// <summary>
        /// Disconnects from the GooglePlayClient. This should be called from your Activity Stop
        /// </summary>
        public void Stop()
        {
            if (this.client != null && this.client.IsConnected)
            {
                this.client.Disconnect();
            }
        }

        /// <summary>
        /// Reconnect to google play.
        /// </summary>
        public void Reconnect()
        {
            if (this.client != null)
            {
                this.client.Reconnect();
            }
        }

        /// <summary>
        /// Sign out of Google Play and make sure we don't try to auto sign in on the next startup
        /// </summary>
        public void SignOut()
        {
            this.SignedOut = true;

            if (this.client.IsConnected)
            {
                GamesClass.SignOut(this.client);
                this.Stop();

                using (var settings = this.activity.GetSharedPreferences("googleplayservicessettings", FileCreationMode.Private))
                {
                    using (var e = settings.Edit())
                    {
                        e.PutString("playerid", String.Empty);
                        e.Commit();
                    }
                }

                this.client.Dispose();
                this.client = null;

                if (this.OnSignedOut != null)
                {
                    this.OnSignedOut(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Attempt to Sign in to Google Play
        /// </summary>
        public void SignIn()
        {
            this.signingin = true;

            if (this.client == null)
            {
                this.CreateClient();
            }

            if (this.client.IsConnected ||
                this.client.IsConnecting)
            {
                return;
            }

            var result = GooglePlayServicesUtil.IsGooglePlayServicesAvailable(activity);
            if (result != ConnectionResult.Success)
            {
                return;
            }

            this.Start();

        }

        /// <summary>
        /// Gets the local player.
        /// </summary>
        /// <returns>The local player.</returns>
        public Player GetLocalPlayer()
        {
            Player result = null;

            if (this.client != null)
            {
                var localPlayer = GamesClass.Players.GetCurrentPlayer(this.client);
                result = AndroidMapper.MapPlayer(localPlayer);
            }

            return result;
        }

        /// <summary>
        /// Unlocks the achievement.
        /// </summary>
        /// <param name="achievementCode">Achievement code from you applications Google Play Game Services Achievements Page</param>
        /// <returns>
        /// <c>true</c> if the achievement has been unlocked; otherwise, <c>false</c>.
        /// </returns>
        public void UnlockAchievement(string achievementCode)
        {
            GamesClass.Achievements.Unlock(this.client, achievementCode);
        }

        /// <summary>
        /// Increments the achievement progress.
        /// </summary>
        /// <param name="achievementCode">Achievement code from you applications Google Play Game Services Achievements Page</param>
        /// <param name="progress">The progress.</param>
        /// <returns>
        /// <c>true</c> if the achievement has been incremented; otherwise, <c>false</c>.
        /// </returns>
        public void IncrementAchievement(string achievementCode, int progress)
        {
            GamesClass.Achievements.Increment(this.client, achievementCode, progress);
        }

        /// <summary>
        /// Show the built in google Achievements Activity. This will cause your application to go into a Paused State
        /// </summary>
        /// <param name="tcs">The task completion source</param>
        public void ShowAchievements(TaskCompletionSource<bool> tcs)
        {
            var intent = GamesClass.Achievements.GetAchievementsIntent(this.client);
            this.activity.StartActivityForResult(intent, REQUEST_ACHIEVEMENTS);
        }

        /// <summary>
        /// Submit a score to google play. The score will only be updated if it is greater than the existing score. 
        /// This is not immediate but will occur at the next sync of the google play client.
        /// </summary>
        /// <param name="leaderboardCode">Leaderboard code from you applications Google Play Game Services Leaderboards Page</param>
        /// <param name="value">The value of the score</param>
        public void SubmitScore(string leaderboardCode, long value)
        {
            GamesClass.Leaderboards.SubmitScore(this.client, leaderboardCode, value);
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
            GamesClass.Leaderboards.SubmitScore(this.client, leaderboardCode, value, metadata);
        }

        /// <summary>
        /// Show the built in leaderboard activity for the leaderboard code.
        /// </summary>
        /// <param name="leaderboardCode">Leaderboard code from you applications Google Play Game Services Leaderboards Page</param>
        /// <param name="tcs">The task completion source</param>
        public void ShowLeaderBoardIntentForLeaderboard(string leaderboardCode, TaskCompletionSource<bool> tcs)
        {
            var intent = GamesClass.Leaderboards.GetLeaderboardIntent(this.client, leaderboardCode);
            this.CurrentGPlayPanelTCS = tcs;

            this.activity.StartActivityForResult(intent, REQUEST_LEADERBOARD);
        }

        /// <summary>
        /// Show the built in leaderboard activity for all the leaderboards setup for your application
        /// </summary>
        /// <param name="tcs">The task completion source</param>
        public void ShowAllLeaderBoardsIntent(TaskCompletionSource<bool> tcs)
        {
            var intent = GamesClass.Leaderboards.GetAllLeaderboardsIntent(this.client);
            this.CurrentGPlayPanelTCS = tcs;

            this.activity.StartActivityForResult(intent, REQUEST_ALL_LEADERBOARDS);
        }

        /// <summary>
        /// Loads the achievements.
        /// </summary>
        /// <param name="tcs">The TCS.</param>
        public void LoadAchievements(TaskCompletionSource<IEnumerable<WaveEngine.Social.Achievement>> tcs)
        {
            var customAchievementsCallback = new CustomAchievementsCallback(tcs);

            var pendingResult = GamesClass.Achievements.Load(this.client, false);
            pendingResult.SetResultCallback(customAchievementsCallback);
        }

        /// <summary>
        /// Loads the top page of scores for a given leaderboard.
        /// </summary>
        /// <param name="leaderboardCode">The leaderboard code.</param>
        /// <param name="count">The maximum number of scores. Must be between 1 and 25.</param>
        /// <param name="socialOnly">If <c>true</c>, the result will only contain the scores of players in the viewing player's circles.</param>
        /// <param name="forceReload">If <c>true</c>, this call will clear any locally cached data and attempt to fetch the latest data from the server. This would commonly be used for something like a user-initiated refresh. Normally, this should be set to false to gain advantages of data caching.</param>
        /// <param name="tcs">The TCS.</param>
        public void LoadTopScores(string leaderboardCode, int count, bool socialOnly, bool forceReload, TaskCompletionSource<IEnumerable<WaveEngine.Social.LeaderboardScore>> tcs)
        {
            var leaderboardCollection = socialOnly ? LeaderboardVariant.CollectionSocial : LeaderboardVariant.CollectionPublic; 

            var customLeaderboardsCallback = new CustomLeaderBoardsCallback(tcs);
            var pendingResult = GamesClass.Leaderboards.LoadTopScores(this.client, leaderboardCode, LeaderboardVariant.TimeSpanAllTime, leaderboardCollection, count, forceReload);
            pendingResult.SetResultCallback(customLeaderboardsCallback);
        }

        /// <summary>
        /// Loads the player-centered page of scores for a given leaderboard. If the player does not have a score on this leaderboard, this call will return the top page instead.
        /// </summary>
        /// <param name="leaderboardCode">The leaderboard code.</param>
        /// <param name="count">The maximum number of scores. Must be between 1 and 25.</param>
        /// <param name="socialOnly">If <c>true</c>, the result will only contain the scores of players in the viewing player's circles.</param>
        /// <param name="forceReload">If <c>true</c>, this call will clear any locally cached data and attempt to fetch the latest data from the server. This would commonly be used for something like a user-initiated refresh. Normally, this should be set to false to gain advantages of data caching.</param>
        /// <param name="tcs">The TCS.</param>
        public void LoadPlayerCenteredScores(string leaderboardCode, int count, bool socialOnly, bool forceReload, TaskCompletionSource<IEnumerable<WaveEngine.Social.LeaderboardScore>> tcs)
        {
            var leaderboardCollection = socialOnly ? LeaderboardVariant.CollectionSocial : LeaderboardVariant.CollectionPublic;

            var customLeaderboardsCallback = new CustomLeaderBoardsCallback(tcs);
            var pendingResult = GamesClass.Leaderboards.LoadPlayerCenteredScores(this.client, leaderboardCode, LeaderboardVariant.TimeSpanAllTime, leaderboardCollection, count, forceReload);
            pendingResult.SetResultCallback(customLeaderboardsCallback);
        }

        #region IGoogleApiClientConnectionCallbacks implementation

        /// <summary>
        /// Called when [connected].
        /// </summary>
        /// <param name="connectionHint">The connection hint.</param>
        public void OnConnected(Bundle connectionHint)
        {
            this.resolving = false;
            this.SignedOut = false;
            this.signingin = false;

            using (var settings = this.activity.GetSharedPreferences("googleplayservicessettings", FileCreationMode.Private))
            {
                using (var e = settings.Edit())
                {
                    e.PutString("playerid", GamesClass.GetCurrentAccountName(this.client));
                    e.Commit();
                }
            }

            if (this.OnSignedIn != null)
            {
                this.OnSignedIn(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Called when [connection suspended].
        /// </summary>
        /// <param name="resultCode">The result code.</param>
        public void OnConnectionSuspended(int resultCode)
        {
            this.resolving = false;
            this.SignedOut = false;
            this.signingin = false;
            this.client.Disconnect();

            if (this.OnSignInFailed != null)
            {
                this.OnSignInFailed(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Called when [connection failed].
        /// </summary>
        /// <param name="result">The result.</param>
        public void OnConnectionFailed(ConnectionResult result)
        {
            if (this.resolving)
            {
                return;
            }

            if (result.HasResolution)
            {
                this.resolving = true;
                result.StartResolutionForResult(this.activity, RC_RESOLVE);
                this.adapter.OnActivityResult += this.OnConnectionFailedActivityResult;
                return;
            }

            this.resolving = false;
            this.SignedOut = false;
            this.signingin = false;

            if (this.OnSignInFailed != null)
            {
                this.OnSignInFailed(this, EventArgs.Empty);
            }
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
            this.adapter.OnActivityResult -= this.OnConnectionFailedActivityResult;

            if (requestCode == RC_RESOLVE)
            {
                if (resultCode == Result.Ok)
                {
                    this.Start();
                }
                else if (this.OnSignInFailed != null)
                {
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
            this.adapter.OnActivityResult -= this.OnConnectionFailedActivityResult;

            if (requestCode == REQUEST_ALL_LEADERBOARDS ||
                requestCode == REQUEST_ACHIEVEMENTS)
            {
                this.currentGPlayPanelTCS.TrySetResult(true);
            }
        }
    }
}