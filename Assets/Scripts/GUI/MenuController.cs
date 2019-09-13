using SmartLocalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour {

	private void Start() {

		Time.timeScale = 1;
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 300;
		CheckAndSetLanguage();
		PanelManager.Instance.InitializeGUI();
		PopupManager.Instance.InitializeGUI();

		SoundManager.Instance.LoopUntilStopped(AudioConfigDatabase.Instance.mainMenuMusic.CloneToCustomClip(), "MainMenuMusic");
		AnalyticsManager.Initialize();
	}

	private void CheckAndSetLanguage() {
		GeneralSaveDatabase generalSaveDatabase = SavedDataManager.Instance.GetGeneralSaveDatabase();
		if (generalSaveDatabase.currentlySelectedLanguageCode == "") {
			string systemLangCode = LanguageManager.Instance.GetSupportedSystemLanguageCode();
			generalSaveDatabase.currentlySelectedLanguageCode = systemLangCode == string.Empty ? "en" : systemLangCode;
		}

		SmartCultureInfo savedLanguage = null;
		List<SmartCultureInfo> supportedLanguages = new List<SmartCultureInfo>();
		supportedLanguages = LanguageManager.Instance.GetSupportedLanguages();
		for (int i = 0; i < supportedLanguages.Count; i++) {
			if (supportedLanguages[i].languageCode == generalSaveDatabase.currentlySelectedLanguageCode) {
				savedLanguage = supportedLanguages[i];
				break;
			}
		}
		LanguageManager.Instance.ChangeLanguage(savedLanguage);
		LanguageManager.SetDontDestroyOnLoad();
	}
}
