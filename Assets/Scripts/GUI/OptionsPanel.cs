﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsPanel : AnimatorPanel {

	public LanguageSelector languageSelector;

	public override void Initialize() {
		base.Initialize();
		languageSelector.InitializeLanguageSelector();
	}

	public override void OnStartedOpening() {
		base.OnStartedOpening();
		languageSelector.ResetLanguageSelector();
	}

	public void DebugDeleteSave() {
		SavedDataManager.DeleteLocalSave();
	}

	public void WooshSound() {
		SoundManager.Instance.CreateOneShot(AudioConfigDatabase.Instance.optionsWoosh);
	}

	public override void OnClosed() {
		base.OnClosed();
		if (SavedDataManager.Instance.GetGeneralSaveDatabase().currentlySelectedLanguageCode != languageSelector.currentlySelectedLanguage.languageCode) {
			SavedDataManager.Instance.GetGeneralSaveDatabase().currentlySelectedLanguageCode = languageSelector.currentlySelectedLanguage.languageCode;
			SavedDataManager.Instance.Save();
		}
	}
}
