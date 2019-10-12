using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	private Action interstitialClosed = delegate { };

	private bool initialized = false;

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
		IronSource.Agent.init(appKey, IronSourceAdUnits.INTERSTITIAL);
		IronSource.Agent.validateIntegration();
		IronSource.Agent.loadInterstitial();

		IronSourceEvents.onInterstitialAdClosedEvent += InterstitialClosed;
		adCurrentlyPlaying = false;
	}

	private void InterstitialClosed() {
		interstitialClosed();
		interstitialClosed = delegate { };
		IronSource.Agent.loadInterstitial();
		AudioListener.volume = 1f;
		adCurrentlyPlaying = false;
	}

	public void ShowInterstitial(Action callBack) {
		IronSource.Agent.showInterstitial();
		interstitialClosed += callBack;
		AudioListener.volume = 0f;
		adCurrentlyPlaying = true;
	}

	public void ShowInterstitial() {
		ShowInterstitial(delegate { });
	}

	public bool IsInterstitialAvailable() {
		bool isReady = IronSource.Agent.isInterstitialReady();
		if (!isReady)
			IronSource.Agent.loadInterstitial();
		return isReady;
	}
}
