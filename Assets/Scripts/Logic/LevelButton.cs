﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour {

	public Text levelNumberText;
	public Button levelButton;
	public Image levelButtonImage;

	public GameObject chains;
	public Image paddleLock;
	public Image checkBoxImage;

	private int levelIndex = 0;
	private bool levelCompleted = false;
	private bool levelLocked = false;

	public void Initialize(int levelIndex) {
		this.levelIndex = levelIndex;

		levelCompleted = SavedDataManager.Instance.GetLevelSaveDataWithLevelIndex(levelIndex).levelCompleted;
		levelLocked = !SavedDataManager.Instance.GetLevelSaveDataWithLevelIndex(levelIndex).isUnlocked;

		levelNumberText.text = (levelIndex + 1).ToString();

		RefreshButtonState();
	}

	private void RefreshButtonState() {
		if (levelCompleted) {
			chains.gameObject.SetActive(false);
			paddleLock.gameObject.SetActive(false);
			checkBoxImage.gameObject.SetActive(true);
		} else if (levelLocked) {
			chains.gameObject.SetActive(true);
			paddleLock.gameObject.SetActive(true);
			checkBoxImage.gameObject.SetActive(false);

			levelNumberText.color = Color.gray;
		} else {

			chains.gameObject.SetActive(false);
			paddleLock.gameObject.SetActive(false);
			checkBoxImage.gameObject.SetActive(true);

		}
	}

	public void LevelButtonClicked() {
		if(!levelLocked)
		LevelSelectPanel.Instance.PlayLevel(levelIndex);
	}
}
