﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectPanel : Panel {

	public const string LevelIndexKey = "LevelIndex";

	private static LevelSelectPanel instance = null;
	public static LevelSelectPanel Instance {
		get {
			if (instance == null) {
				instance = FindObjectOfType<LevelSelectPanel>();
			}
			return instance;
		}
	}

	public GridLayoutGroup gridLayout;
	public LevelButton sampleLevelButton;

	private List<LevelButton> instantiatedLevelButtons = new List<LevelButton>();

	public override void Initialize() {
		InstantiateLevelButtons();
		base.Initialize();
	}

	public void InstantiateLevelButtons() {
		for (int i = 0; i < LevelResourceDatabase.Instance.levelResourceNames.Length; i++) {
			instantiatedLevelButtons.Add(Instantiate(sampleLevelButton, Vector3.zero, Quaternion.identity, gridLayout.transform) as LevelButton);
			instantiatedLevelButtons[i].Initialize(i);
		}
		sampleLevelButton.gameObject.SetActive(false);
	}

	public void PlayLevel(int levelIndex) {
		Messenger.Instance.SendMessage(LevelIndexKey, levelIndex);
		UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
	}
}