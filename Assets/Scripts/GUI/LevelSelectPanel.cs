﻿using System;
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

	private int currentlyOpenedSection = -1;
	private float sectionAnimationTime = 1f;

	public override void Initialize() {
		InitializeSections();
		ReInitializeButtons();
		base.Initialize();
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
		sampleLevelButton.gameObject.SetActive(false);
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

		if (currentlyOpenedSection != -1) {
			CloseCurrentlyOpenedSection();
			StartCoroutine(WaitAndCallBack(sectionAnimationTime, () => { OpenSection(sectionNumber); }));
			return;
		}

		for (int i = 0; i < instantiatedSectionButtons.Count; i++) {
			bool isThisTheSectionToBeOpened = i == sectionNumber;
			foreach (var b in instantiatedSectionButtons[i].instantiatedLevelButtons) {
				b.gameObject.SetActive(isThisTheSectionToBeOpened);
			}
		}

		SectionButtons sButtons = GetSectionButtonsForSection(sectionNumber);
		foreach (var b in sButtons.instantiatedLevelButtons) {
			b.PlayAppearAnimation();
		}
		currentlyOpenedSection = sectionNumber;
	}

	private void CloseCurrentlyOpenedSection() {

		SectionButtons sButtons = GetSectionButtonsForSection(currentlyOpenedSection);
		foreach (var b in sButtons.instantiatedLevelButtons) {
			b.PlayDisappearAnimation();
		}

		currentlyOpenedSection = -1;
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

	public void PlayLevel(int levelIndex) {
		DeactivatePanelButtons();
		ImageTransitionHandler.Instance.TransitionIn(() => { StartLevel(levelIndex); });
	}

	private void StartLevel(int levelIndex) {
		Messenger.Instance.SendMessage(LevelIndexKey, levelIndex);
		LoadingController.LoadScene("Game");
	}
}
