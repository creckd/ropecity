using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour {

	private void Start() {

		Time.timeScale = 1;
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 300;
		PanelManager.Instance.InitializeGUI();
		PopupManager.Instance.InitializeGUI();

		SoundManager.Instance.LoopUntilStopped(AudioConfigDatabase.Instance.mainMenuMusic.CloneToCustomClip(), "MainMenuMusic");

	}
}
