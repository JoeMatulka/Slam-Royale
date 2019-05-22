using GoogleMobileAds.Api;
using System.Collections;
using UnityEngine;

public class AdManager : Singleton<AdManager>
{
    private const string ANDROID_APP_ID = "your-app-id";

    private const string INTERSTITIAL_AD_ID = "your-ad-id";

    private const string TEST_INTERSTITIAL_AD_ID = "ca-app-pub-3940256099942544/1033173712";

    public InterstitialAd interstitial;

    public float LastAdShownTime;

    public bool errorLoadingAd;

    protected AdManager() { }

    public void InitializeAds()
    {
        MobileAds.Initialize(ANDROID_APP_ID);
        LastAdShownTime = Time.fixedTime;
    }

    public void RequestInterstitialAd() {
        // Initialize an InterstitialAd.
        interstitial = new InterstitialAd(INTERSTITIAL_AD_ID);
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder()
            .Build();
        // Load the interstitial with the request.
        interstitial.LoadAd(request);
    }

    public IEnumerator ShowInterstitialAd()
    {
        yield return new WaitUntil(() => interstitial.IsLoaded());
        LastAdShownTime = Time.fixedTime;
        interstitial.Show();
    }

    public bool CanShowAd() {
        return !errorLoadingAd && Time.fixedTime - LastAdShownTime > 200;
    }
}
