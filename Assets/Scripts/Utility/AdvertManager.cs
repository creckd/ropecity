using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdvertManager : MonoBehaviour {

	private static AdvertManager instance = null;
	public static AdvertManager Instance {
		get {
			if (instance == null)
				instance = FindObjectOfType<AdvertManager>();
			return instance;
		}
	}

	public static bool adCurrentlyPlaying = false;

	private const string IronSourceIOSAppKey = "a27e03ed";
	private const string IronSourceAndroidAppKey = "a27f5d1d";

    private const string adUnitId = "ca-app-pub-4466868006731309/2417505856";


    private Action interstitialClosed = delegate { };

	private bool initialized = false;

#if UNITY_ANDROID
    InterstitialAd currentInterstitial = null;
#endif

    private void OnApplicationPause(bool isPaused) {
		IronSource.Agent.onApplicationPause(isPaused);
	}

	private void Awake() {
		DontDestroyOnLoad(this.gameObject);
		Initialize();
	}

	public void Initialize() {
		if (initialized)
			return;

		initialized = true;
		string appKey = "";
#if UNITY_IOS
		appKey = IronSourceIOSAppKey;
#elif UNITY_ANDROID
		appKey = IronSourceAndroidAppKey;
#endif

#if UNITY_IOS
        IronSource.Agent.init(appKey, IronSourceAdUnits.INTERSTITIAL);
		IronSource.Agent.validateIntegration();
		IronSource.Agent.loadInterstitial();

		IronSourceEvents.onInterstitialAdClosedEvent += InterstitialClosed;
		IronSourceEvents.onInterstitialAdShowFailedEvent += InterstitialAdShowFailed;
#endif
#if UNITY_ANDROID
        MobileAds.Initialize(initStatus => {
            LoadAdmobInterstitial();
        });
#endif
        adCurrentlyPlaying = false;
	}

	private void InterstitialAdShowFailed(IronSourceError err) {
		InterstitialClosed();
	}

	private void InterstitialClosed() {
		interstitialClosed();
		interstitialClosed = delegate { };
#if UNITY_IOS
        IronSource.Agent.loadInterstitial();
#endif
#if UNITY_ANDROID
        LoadAdmobInterstitial();
#endif
        AudioListener.volume = 1f;
		adCurrentlyPlaying = false;
	}

    private void InterstitialClosed(object sender, EventArgs e)
    {
        InterstitialClosed();
    }

    private void AdmobFailedToLoadAd(object sender, AdFailedToLoadEventArgs e)
    {
    }

    public void ShowInterstitial(Action callBack) {
#if UNTIY_IOS
        IronSource.Agent.showInterstitial();
#endif
#if UNITY_ANDROID
        if (currentInterstitial.IsLoaded())
        {
            currentInterstitial.OnAdClosed += InterstitialClosed;
            currentInterstitial.Show();
        }
#endif
        interstitialClosed += callBack;
		AudioListener.volume = 0f;
		adCurrentlyPlaying = true;
	}

	public void ShowInterstitial() {
		ShowInterstitial(delegate { });
	}

	public bool IsInterstitialAvailable() {
        bool isReady = false;
#if UNITY_IOS
        isReady = IronSource.Agent.isInterstitialReady();
#endif
#if UNITY_ANDROID
        isReady = currentInterstitial != null && currentInterstitial.IsLoaded();
#endif
        if (!isReady)
        {
#if UNITY_ANDROID
            LoadAdmobInterstitial();
#endif
#if UNITY_IOS
            IronSource.Agent.loadInterstitial();
#endif
        }
		return isReady;
	}

    private void LoadAdmobInterstitial()
    {
        if (currentInterstitial != null)
            currentInterstitial.Destroy();

        currentInterstitial = new InterstitialAd(adUnitId);
        AdRequest request = new AdRequest.Builder().Build();
        currentInterstitial.LoadAd(request);
        currentInterstitial.OnAdFailedToLoad += AdmobFailedToLoadAd;
    }
}
