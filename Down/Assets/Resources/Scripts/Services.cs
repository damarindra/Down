using UnityEngine;
using System.Collections.Generic;
using GooglePlayGames.BasicApi;
using GooglePlayGames;
using GoogleMobileAds.Api;
using System.IO;

public class Services {

    public static Services sInstance = new Services();

    private Dictionary<string, bool> mUnlockedAchievements = new Dictionary<string, bool>();
    private Dictionary<string, int> mPendingIncrements = new Dictionary<string, int>();

    private string linkStore = "https://play.google.com/store/apps/details?id=com.cgranule.down";

    public static Services Instance
    {
        get { return sInstance; }
    }

    public void GPGSSetup()
    {
        // enables saving game progress.
        // registers a callback to handle game invitations received while the game is not running.
        // registers a callback for turn based match notifications received while the
        // game is not running.
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();

        PlayGamesPlatform.InitializeInstance(config);
        // recommended for debugging:
        PlayGamesPlatform.DebugLogEnabled = true;
        // Activate the Google Play Games platform
        PlayGamesPlatform.Activate();

        LoginGooglePlay();
    }

    public void LoginGooglePlay()
    {
        //google play login
        Social.localUser.Authenticate((bool success) => {
        });
    }

    public void LeaderboardShow()
    {
        if (Authenticated)
            Social.ShowLeaderboardUI();
        else
            LoginGooglePlay();
    }

    public void ShowAchievementsUI()
    {
        if (Authenticated)
            Social.ShowAchievementsUI();
        else
            LoginGooglePlay();
    }

    public void UnlockAchievement(string achId)
    {
        if (Authenticated && !mUnlockedAchievements.ContainsKey(achId))
        {
            Social.ReportProgress(achId, 100.0f, (bool success) => { });
            mUnlockedAchievements[achId] = true;
        }
    }

    public void IncrementAchievement(string achId, int steps)
    {
        if (mPendingIncrements.ContainsKey(achId))
        {
            steps += mPendingIncrements[achId];
        }
        mPendingIncrements[achId] = steps;
    }

    public void FlushAchievements()
    {
        if (Authenticated)
        {
            foreach (string ach in mPendingIncrements.Keys)
            {
                // incrementing achievements by a delta is a feature
                // that's specific to the Play Games API and not part of the
                // ISocialPlatform spec, so we have to break the abstraction and
                // use the PlayGamesPlatform rather than ISocialPlatform
                PlayGamesPlatform p = (PlayGamesPlatform)Social.Active;
                p.IncrementAchievement(ach, mPendingIncrements[ach], (bool success) => { });
            }
            mPendingIncrements.Clear();
        }
    }

    public void ReportScore()
    {
        if (GameManager.instance.score > SaveLoad.GetScore()) {
            Social.ReportScore(GameManager.instance.score, Configuration.instance.leaderboardID, (bool success) => {
                // handle success or failure
            });
        }
    }

    public bool Authenticated
    {
        get
        {
            return Social.Active.localUser.authenticated;
        }
    }


    ///SHARE
    public void Share(int score) {
#if UNITY_ANDROID
        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
        intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
        intentObject.Call<AndroidJavaObject>("setType", "text/plain");
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), "SUBJECT");
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), "Hi I got Score " + score.ToString() + "!! Can you beat it? \n\n" + linkStore);
        AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
        currentActivity.Call("startActivity", intentObject);
#endif
    }

    ///ADS
    private BannerView bannerView;
    private InterstitialAd interstitial;

    //Ads
    public void BannerSetup()
    {
        if (Configuration.instance.admobBannerKey.Trim().Length != 0)
        {
            if (bannerView == null)
            {
                bannerView = new BannerView(Configuration.instance.admobBannerKey, AdSize.Banner, AdPosition.Top);
                AdRequest adRequest = new AdRequest.Builder().Build();
                bannerView.LoadAd(adRequest);
            }
            bannerView.Hide();
        }
    }

    public void BannerShow()
    {
        if (bannerView != null)
            bannerView.Show();
    }

    public void BannerHide()
    {
        bannerView.Hide();
    }

    public void InterstitialSetup()
    {
        if (Configuration.instance.admobInterstitialKey.Trim().Length != 0)
        {
            // Initialize an InterstitialAd.
            interstitial = new InterstitialAd(Configuration.instance.admobInterstitialKey);
            // Create an empty ad request.
            AdRequest request = new AdRequest.Builder().Build();
            // Load the interstitial with the request.
            interstitial.LoadAd(request);
        }
    }

    public void InterstitialShow()
    {
        if (interstitial.IsLoaded())
            interstitial.Show();
    }
}
