using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngamePanel : FaderPanel {

	public Image ropeCrossHair;
	public LineRenderer aiderLine;
	public float aiderLineInsetAmount = 0.01f;
	public AnimatedLevelText animatedLevelText;
	public CanvasGroup tutorialHoldIndicatorGroup;
	public CanvasGroup tutorialReleaseIndicatorGroup;
	public Animator tutorialHoldIndicatorAnimator;
	public Animator getToTheEndAnimator;
	public Button pauseButton;

	private const string tutorialHoldIndicatorHold = "Hold";
	private const string getToTheEndStateName = "GetToTheEnd";

	private float targetTutorialHoldIndicatorVisibility = 0f;
	private float targetReleaseIndicatorVisibility = 0f;

	private Vector3 crossHairTargetWorldPosition = Vector3.zero;

	private float lastTimePositionWasChanged;
	private bool crossHairShouldBeShown = false;

	public override void OnStartedOpening() {
		base.OnStartedOpening();
		GameController.Instance.GameStarted += ShowLevelInfo;
		GameController.Instance.FoundPotentionalHitPoint += SetCrossHairPosition;
		GameController.Instance.ShowUIHookAid += () => { crossHairShouldBeShown = true; };
		GameController.Instance.HideUIHookAid += () => { crossHairShouldBeShown = false; };
		tutorialHoldIndicatorGroup.alpha = 0f;
		GameController.Instance.ShowHoldIndicator += () => {
			StaticBlurCreator.Instance.CreateStaticBlurImage();
			IngameBlurController.Instance.BlurImage(ConfigDatabase.Instance.tutorialBlurTime, false, true,false);
			tutorialHoldIndicatorAnimator.Play(tutorialHoldIndicatorHold, 0, 0f);
			targetTutorialHoldIndicatorVisibility = 1f;
		};
		GameController.Instance.HideHoldIndicator += () => {
			targetTutorialHoldIndicatorVisibility = 0f;
			if(targetReleaseIndicatorVisibility == 0f)
			IngameBlurController.Instance.UnBlurImage(ConfigDatabase.Instance.tutorialBlurTime,false);
		};
		GameController.Instance.ShowReleaseIndicator += () => {
			StaticBlurCreator.Instance.CreateStaticBlurImage();
			IngameBlurController.Instance.BlurImage(ConfigDatabase.Instance.tutorialBlurTime, false, true, false);
			targetReleaseIndicatorVisibility = 1f;
		};
		GameController.Instance.HideReleaseIndicator += () => {
			targetReleaseIndicatorVisibility = 0f;
			if (targetTutorialHoldIndicatorVisibility == 0f)
				IngameBlurController.Instance.UnBlurImage(ConfigDatabase.Instance.tutorialBlurTime, false);
		};
		GameController.Instance.ShowTutorialLastTask += () => {
			getToTheEndAnimator.gameObject.SetActive(true);
			getToTheEndAnimator.Play(getToTheEndStateName, 0, 0f);
		};

		if (GameController.Instance.shouldStartTutorial) {
			pauseButton.gameObject.SetActive(false);
		}
	}

	public override void OnStartedClosing() {
		base.OnStartedClosing();
		GameController.Instance.FoundPotentionalHitPoint -= SetCrossHairPosition;
		GameController.Instance.GameStarted += ShowLevelInfo;
	}

	private void ShowLevelInfo(bool fastStart) {
		string levelName, triesText;
		if (!GameController.Instance.isDebugTestLevelMode) {
			levelName = string.Format(SmartLocalization.LanguageManager.Instance.GetTextValue("IngamePanel.stageText"), (LevelController.Instance.currentLevelIndex + 1).ToString());
			triesText = string.Format(SmartLocalization.LanguageManager.Instance.GetTextValue("IngamePanel.runText"), SavedDataManager.Instance.GetLevelSaveDataWithLevelIndex(LevelController.Instance.currentLevelIndex).numberOfTries.ToString());
		} else {
			levelName = "Teszt mód";
			triesText = "végtelen";
		}
		animatedLevelText.ShowLevelText(2.5f, levelName, triesText);
	}

	public void BackToMainMenu() {
		Messenger.Instance.SendMessage(PanelManager.defaultOpenedPanelChangedTag, 1);
		LoadingController.LoadScene("MainMenu");
	}

	public void SetCrossHairPosition(Vector3 worldPosition) {
		crossHairTargetWorldPosition = worldPosition;
	}

	private void Update() {
		ropeCrossHair.enabled = crossHairShouldBeShown && GameController.Instance.currentGameState == GameState.GameStarted;
		aiderLine.enabled = crossHairShouldBeShown && GameController.Instance.currentGameState == GameState.GameStarted;
		tutorialHoldIndicatorGroup.alpha = Mathf.Lerp(tutorialHoldIndicatorGroup.alpha, targetTutorialHoldIndicatorVisibility, Time.unscaledDeltaTime * 10f);
		if (tutorialHoldIndicatorGroup.alpha <= 0.05f)
			tutorialHoldIndicatorGroup.alpha = 0f;
		tutorialHoldIndicatorGroup.gameObject.SetActive(tutorialHoldIndicatorGroup.alpha != 0);
		tutorialReleaseIndicatorGroup.alpha = Mathf.Lerp(tutorialReleaseIndicatorGroup.alpha, targetReleaseIndicatorVisibility, Time.unscaledDeltaTime * 10f);
		if (tutorialReleaseIndicatorGroup.alpha <= 0.05f)
			tutorialReleaseIndicatorGroup.alpha = 0f;
		tutorialReleaseIndicatorGroup.gameObject.SetActive(tutorialReleaseIndicatorGroup.alpha != 0);
	}

	private void LateUpdate() {
		if (crossHairShouldBeShown) {
			Vector3[] points = new Vector3[2];
			points[0] = GameController.Instance.currentWorm.gunPositionObject.transform.position;
			points[1] = crossHairTargetWorldPosition;
			Vector3 halfPoint = (points[0] + points[1]) / 2f;
			points[0] = Vector3.Lerp(points[0], halfPoint, aiderLineInsetAmount);
			points[1] = Vector3.Lerp(points[1], halfPoint, aiderLineInsetAmount);
			aiderLine.positionCount = 2;
			aiderLine.SetPositions(points);

			Vector3 screenPosition = Camera.main.WorldToScreenPoint(crossHairTargetWorldPosition);
			Vector3 screenPositionNormalized = new Vector3(screenPosition.x / Camera.main.pixelWidth, screenPosition.y / Camera.main.pixelHeight, 0f);
			RectTransform mainCanvasRect = PanelManager.Instance.mainCanvasRect;
			Vector3 guiPosition = new Vector3(screenPositionNormalized.x * mainCanvasRect.rect.width, screenPositionNormalized.y * mainCanvasRect.rect.height, 0f);
			ropeCrossHair.rectTransform.anchoredPosition = guiPosition;
		}
	}

	public void PauseButton() {
		GameController.Instance.PauseGame();
	}
}
