using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectPanel : AnimatorPanel {

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
		ReInitializeButtons();
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
		DeactivatePanelButtons();
		ImageTransitionHandler.Instance.TransitionIn(() => { StartLevel(levelIndex); },true);
	}

	private void StartLevel(int levelIndex) {
		Messenger.Instance.SendMessage(LevelIndexKey, levelIndex);
		LoadingController.LoadScene("Game");
	}
}
