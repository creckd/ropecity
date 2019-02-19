using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour {

	public Text levelNumberText;

	private int levelIndex = 0;

	public void Initialize(int levelIndex) {
		this.levelIndex = levelIndex;

		levelNumberText.text = (levelIndex + 1).ToString();
	}

	public void LevelButtonClicked() {
		LevelSelectPanel.Instance.PlayLevel(levelIndex);
	}
}
