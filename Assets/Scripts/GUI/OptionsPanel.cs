using System.Collections;
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

}
