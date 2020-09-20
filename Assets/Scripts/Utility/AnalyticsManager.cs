using System.Collections.Generic;
using UnityEngine;
using Prime31;

// NO ANALYTICS ON THEBALANCE BRANCH

public class AnalyticsManager : MonoBehaviour {

	static string flurryApiKeyAndroid = "GGJWWYXJQ5Q3MVZJXV8K";
	static string flurryApiKeyIOS = "T4WRTSKP36NNMDZ5N3CG";

	static bool initialized = false;

	public static void Initialize() {
		if (initialized)
			return;

//	#if UNITY_ANDROID
//		FlurryAnalytics.startSession(flurryApiKeyAndroid,true);
//#elif UNITY_IOS
//		FlurryAnalytics.startSession(flurryApiKeyIOS, true);
//#endif

		initialized = true;
	}

	public static void LogEvent(string name, Dictionary<string, object> dictionary = null) {
		if (!initialized)
			return;

//#if UNITY_ANDROID
//		if (dictionary == null) {
//			FlurryAnalytics.logEvent(name, false);
//		} else {
//			FlurryAnalytics.logEvent(name, ConvertObjectToStringDic(dictionary),false);
//		}
//#elif UNITY_IOS
//		if (dictionary == null) {
//			FlurryAnalytics.logEvent(name,false);
//		} else {
//			FlurryAnalytics.logEventWithParameters(name, ConvertObjectToStringDic(dictionary),false);
//		}
//#endif
	}

	public static void LogEvent(string name, string parameterName, object parameterValue) {
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add(parameterName, parameterValue);

		LogEvent(name, dictionary);
	}

	public static void LogEventOnce(string name, Dictionary<string, object> dictionary = null) {
		bool alreadyLoggedOnce = DidILogThisEvent(name);

		if (!alreadyLoggedOnce) {
			LogEvent(name, dictionary);
		}
	}

	public static void LogEventOnce(string name, string parameterName, object parameterValue) {
		bool alreadyLoggedOnce = DidILogThisEvent(name);

		if (!alreadyLoggedOnce) {
			LogEvent(name, parameterName,parameterValue);
		}
	}

	private static bool DidILogThisEvent(string eventName) {
		bool hasKey = PlayerPrefs.HasKey(eventName);
		if (hasKey)
			return true;
		else {
			PlayerPrefs.SetInt(eventName, 1);
			return false;
		}
	}

	private static Dictionary<string, string> ConvertObjectToStringDic(Dictionary<string, object> objDic) {
		Dictionary<string, string> newDic = new Dictionary<string, string>();
		foreach (var kvp in objDic) {
			newDic.Add(kvp.Key, kvp.Value.ToString());
		}
		return newDic;
	}
}