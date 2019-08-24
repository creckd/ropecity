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

	private const string IronSourceIOSAppKey = "9e5981f5";
	private const string IronSourceAndroidAppKey = "9e7645f5";

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
	}

	private void InterstitialClosed() {
		interstitialClosed();
		interstitialClosed = delegate { };
		IronSource.Agent.loadInterstitial();
	}

	public void ShowInterstitial(Action callBack) {
		IronSource.Agent.showInterstitial();
		interstitialClosed += callBack;
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
