using System;
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

	public class SectionButtons {
		public int sectionNumber;
		public List<LevelButton> instantiatedLevelButtons = new List<LevelButton>();
	}
	private List<SectionButtons> instantiatedSectionButtons = new List<SectionButtons>();

	public GridLayoutGroup gridLayout;
	public LevelButton sampleLevelButton;
	public Text sectionText;

	private int currentlyOpenedSection = -1;
	private float sectionAnimationTime = 0.5f;

	public override void Initialize() {
		base.Initialize();
		InitializeSections();
		ReInitializeButtons();
		RefreshAllSectionButtonActiveness();
		sampleLevelButton.gameObject.SetActive(false);
	}

	public override void OnStartedOpening() {
		base.OnStartedOpening();
		OpenSection(0);
	}

	public override void OnStartedClosing() {
		base.OnStartedClosing();
		CloseCurrentlyOpenedSection();
	}

	private void InitializeSections() {
		int globalLevelIndex = 0;

		for (int i = 0; i < LevelResourceDatabase.Instance.sections.Length; i++) {
			SectionButtons sButtons = new SectionButtons();
			sButtons.sectionNumber = i;

			int numberOfLevelsInSection = LevelResourceDatabase.Instance.sections[i].levelResourceNames.Length;

			for (int j = 0; j < numberOfLevelsInSection; j++) {
				int levelIndex = globalLevelIndex + j;
				sButtons.instantiatedLevelButtons.Add(Instantiate(sampleLevelButton, Vector3.zero, Quaternion.identity, gridLayout.transform) as LevelButton);
				sButtons.instantiatedLevelButtons[j].Initialize(levelIndex);
			}
			globalLevelIndex += numberOfLevelsInSection;
			instantiatedSectionButtons.Add(sButtons);
		}
	}

	private void OpenSection(int sectionNumber) {
		if (sectionNumber == currentlyOpenedSection)
			return;

		sectionText.text = (sectionNumber + 1).ToString();

		if (currentlyOpenedSection != -1) {
			CloseCurrentlyOpenedSection();
			StartCoroutine(WaitAndCallBack(sectionAnimationTime, () => { OpenSection(sectionNumber); }));
			return;
		}

		if (!panelInTransition)
			DeactivatePanelButtons();

		currentlyOpenedSection = sectionNumber;
		RefreshAllSectionButtonActiveness();

		SectionButtons sButtons = GetSectionButtonsForSection(sectionNumber);
		foreach (var b in sButtons.instantiatedLevelButtons) {
			b.PlayAppearAnimation();
		}

		if(!panelInTransition)
			StartCoroutine(WaitAndCallBack(sectionAnimationTime, ActivatePanelButtons));
	}

	private void CloseCurrentlyOpenedSection() {

		if (!panelInTransition)
			DeactivatePanelButtons();

		SectionButtons sButtons = GetSectionButtonsForSection(currentlyOpenedSection);
		foreach (var b in sButtons.instantiatedLevelButtons) {
			b.PlayDisappearAnimation();
		}

		currentlyOpenedSection = -1;

		if (!panelInTransition)
			StartCoroutine(WaitAndCallBack(sectionAnimationTime, ActivatePanelButtons));
	}

	private void RefreshAllSectionButtonActiveness() {
		for (int i = 0; i < instantiatedSectionButtons.Count; i++) {
			bool isThisTheSectionToBeOpened = i == currentlyOpenedSection;
			foreach (var b in instantiatedSectionButtons[i].instantiatedLevelButtons) {
				b.gameObject.SetActive(isThisTheSectionToBeOpened);
			}
		}
	}

	IEnumerator WaitAndCallBack(float waitTime, Action cb) {
		yield return new WaitForSeconds(waitTime);
		if (cb != null)
			cb();
	}

	private SectionButtons GetSectionButtonsForSection(int sectionNumber) {
		foreach (var sB in instantiatedSectionButtons) {
			if (sB.sectionNumber == sectionNumber)
				return sB;
		}
		return null;
	}

	public void NextSectionButton() {
		int nextSectionNumber = currentlyOpenedSection + 1;
		if (nextSectionNumber < LevelResourceDatabase.Instance.sections.Length) {
			OpenSection(nextSectionNumber);
		}
	}

	public void PreviousSectionButton() {
		int previousSectionNumber = currentlyOpenedSection - 1;
		if (previousSectionNumber >= 0) {
			OpenSection(previousSectionNumber);
		}
	}

	public void PlayLevel(int levelIndex) {
		DeactivatePanelButtons();
		ImageTransitionHandler.Instance.TransitionIn(() => { StartLevel(levelIndex); });
	}

	private void StartLevel(int levelIndex) {
		Messenger.Instance.SendMessage(LevelIndexKey, levelIndex);
		LoadingController.LoadScene("Game");
	}
}
