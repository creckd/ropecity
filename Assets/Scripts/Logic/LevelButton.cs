using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour {

	public Text levelNumberText;
	public Button levelButton;
	public Image levelButtonImage;

	[Header("Unlocked")]
	public Sprite unlockedSpriteState_passive;
	public Sprite unlockedSpriteState_active;
	[Header("Completed")]
	public Sprite completedSpriteState_passive;
	public Sprite completedSpriteState_active;

	public GameObject chains;

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
		SpriteState currentState = levelButton.spriteState;
		if (levelCompleted) {
			levelButtonImage.sprite = completedSpriteState_passive;
			currentState.pressedSprite = completedSpriteState_active;
			chains.gameObject.SetActive(false);
		} else if (levelLocked) {
			chains.gameObject.SetActive(true);
			levelNumberText.color = Color.gray;
		} else {
			levelButtonImage.sprite = unlockedSpriteState_passive;
			currentState.pressedSprite = unlockedSpriteState_active;
			chains.gameObject.SetActive(false);
		}
	}

	public void LevelButtonClicked() {
		if(!levelLocked)
		LevelSelectPanel.Instance.PlayLevel(levelIndex);
	}
}
