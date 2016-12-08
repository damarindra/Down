using UnityEngine;
using System.Collections;

public class Configuration : MonoBehaviour {

    public static Configuration instance = null;

    public string admobBannerKey;
    public string admobInterstitialKey;
    public int interstitialShowPerGame;

    public string leaderboardID;
    public string score100Achievement;
    public string score300Achievement;
    public string score500Achievement;
    public string firstPlayAchievement;
    public string play25TimesAchievement;
    public string play50TimesAchievement;
    public string play100TimesAchievement;
    public string play250TimesAchievement;
    public string firstBuyAchievement;
    public string diamond50Achievement;
    public string diamond100Achievement;
    public string diamond250Achievement;

    // Use this for initialization
    void Awake () {
        if (instance == null)
            instance = this;

        DontDestroyOnLoad(gameObject);
	}
	
}
