using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngamePanel : FaderPanel {

	public Image ropeCrossHair;
	public LineRenderer aiderLine;
	public AnimatedLevelText animatedLevelText;
	public Animator tutorialHoldIndicatorAnimator;

	private const string tutorialHoldIndicatorDefault = "Default";
	private const string tutorialHoldIndicatorStartHold = "StartHold";
	private const string tutorialHoldIndicatorHold = "Hold";

	private Vector3 crossHairTargetWorldPosition = Vector3.zero;

	private float lastTimePositionWasChanged;
	private bool crossHairShouldBeShown = false;

	public override void OnStartedOpening() {
		base.OnStartedOpening();
		GameController.Instance.GameStarted += ShowLevelInfo;
		GameController.Instance.FoundPotentionalHitPoint += SetCrossHairPosition;
		GameController.Instance.ShowUIHookAid += () => { crossHairShouldBeShown = true; };
		GameController.Instance.HideUIHookAid += () => { crossHairShouldBeShown = false; };
		GameController.Instance.ShowHoldIndicator += () => { tutorialHoldIndicatorAnimator.Play(tutorialHoldIndicatorStartHold, 1, 0f); tutorialHoldIndicatorAnimator.Play(tutorialHoldIndicatorHold, 0, 0f); };
		GameController.Instance.HideHoldIndicator += () => { tutorialHoldIndicatorAnimator.Play(tutorialHoldIndicatorDefault, 1, 0f); };
	}

	public override void OnStartedClosing() {
		base.OnStartedClosing();
		GameController.Instance.FoundPotentionalHitPoint -= SetCrossHairPosition;
		GameController.Instance.GameStarted += ShowLevelInfo;
	}

	private void ShowLevelInfo() {
		string levelName, triesText;
		if (!GameController.Instance.isDebugTestLevelMode) {
			levelName = "Stage " + (LevelController.Instance.currentLevelIndex+1).ToString();
			triesText = SavedDataManager.Instance.GetLevelSaveDataWithLevelIndex(LevelController.Instance.currentLevelIndex).numberOfTries.ToString();
		} else {
			levelName = "Teszt mód";
			triesText = "végtelen";
		}
		animatedLevelText.ShowLevelText(2.5f, levelName, "Run " + triesText);
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
	}

	private void LateUpdate() {
		if (crossHairShouldBeShown) {
			Vector3[] points = new Vector3[2];
			points[0] = GameController.Instance.currentWorm.gunPositionObject.transform.position;
			points[1] = crossHairTargetWorldPosition;
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
