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
	public Image sectionMask;
	public float sectionAnimationTime = 0.6f;

	private int currentlyOpenedSection = -1;
	private int animatingDirection = 1;

	public override void Initialize() {
		base.Initialize();
		InitializeSections();
		ReInitializeButtons();
		RefreshAllSectionButtonActiveness();
		sampleLevelButton.gameObject.SetActive(false);
		sectionMask.material = Instantiate(sectionMask.material);
	}

	public override void OnStartedOpening() {
		base.OnStartedOpening();
		OpenSection(0);
		foreach (var b in GetSectionButtonsForSection(currentlyOpenedSection).instantiatedLevelButtons) {
			b.PlayAppearAnimation();
		}
	}

	public override void OnStartedClosing() {
		base.OnStartedClosing();
		foreach (var b in GetSectionButtonsForSection(currentlyOpenedSection).instantiatedLevelButtons) {
			b.PlayDisappearAnimation();
		}
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

		if (currentlyOpenedSection != -1) {
			animatingDirection = sectionNumber < currentlyOpenedSection ? 0 : 1;
			CloseCurrentlyOpenedSection();
			StartCoroutine(WaitAndCallBack(sectionAnimationTime, () => { OpenSection(sectionNumber); }));
			return;
		}

		Debug.Log(animatingDirection);


		if (!panelInTransition)
			DeactivatePanelButtons();

		currentlyOpenedSection = sectionNumber;
		RefreshAllSectionButtonActiveness();
		sectionText.text = (currentlyOpenedSection + 1).ToString();

		ShowSection();

		if(!panelInTransition)
			StartCoroutine(WaitAndCallBack(sectionAnimationTime, ActivatePanelButtons));
	}

	private void CloseCurrentlyOpenedSection() {

		if (!panelInTransition)
			DeactivatePanelButtons();

		HideSection();

		currentlyOpenedSection = -1;

		if (!panelInTransition)
			StartCoroutine(WaitAndCallBack(sectionAnimationTime, ActivatePanelButtons));
	}

	public void HideSection() {
		StartCoroutine(MaskSection(0f));
	}

	public void ShowSection() {
		StartCoroutine(MaskSection(1f));
	}

	IEnumerator MaskSection(float targetValue = 0f) {
		float uvFlipped = System.Convert.ToBoolean(animatingDirection) ?  1f - targetValue : targetValue;
		sectionMask.material.SetInt("_UVFlipped", (int)uvFlipped);
		//sectionMask.material.SetInt("_Seed", UnityEngine.Random.Range(0, 40));
		float timer = 0f;
		while (timer <= sectionAnimationTime) {
			timer += Time.unscaledDeltaTime;
			sectionMask.material.SetFloat("_T", Mathf.Lerp(1 - targetValue, targetValue, timer / sectionAnimationTime));
			yield return null;
		}
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
