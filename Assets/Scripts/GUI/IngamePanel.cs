using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngamePanel : Panel {

	public Image ropeCrossHair;
	public LineRenderer aiderLine;

	private Vector3 crossHairTargetWorldPosition = Vector3.zero;

	private float lastTimePositionWasChanged;
	private bool crossHairShouldBeShown = false;

	public override void OnStartedOpening() {
		base.OnStartedOpening();
		GameController.Instance.FoundPotentionalHitPoint += SetCrossHairPosition;
		GameController.Instance.ShowUIHookAid += () => { crossHairShouldBeShown = true; };
		GameController.Instance.HideUIHookAid += () => { crossHairShouldBeShown = false; };
	}

	public void BackToMainMenu() {
		UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
	}

	public void SetCrossHairPosition(Vector3 worldPosition) {
		crossHairTargetWorldPosition = worldPosition;
	}

	private void Update() {
		Vector3 screenPosition = Camera.main.WorldToScreenPoint(crossHairTargetWorldPosition);
		Vector3 screenPositionNormalized = new Vector3(screenPosition.x / Camera.main.pixelWidth, screenPosition.y / Camera.main.pixelHeight, 0f);
		RectTransform mainCanvasRect = PanelManager.Instance.mainCanvasRect;
		Vector3 guiPosition = new Vector3(screenPositionNormalized.x * mainCanvasRect.rect.width, screenPositionNormalized.y * mainCanvasRect.rect.height, 0f);
		ropeCrossHair.rectTransform.anchoredPosition = guiPosition;
		ropeCrossHair.enabled = crossHairShouldBeShown;
		aiderLine.enabled = crossHairShouldBeShown;
	}

	private void LateUpdate() {
		Vector3[] points = new Vector3[2];
		points[0] = GameController.Instance.currentWorm.gunPositionObject.transform.position;
		points[1] = crossHairTargetWorldPosition;
		aiderLine.positionCount = 2;
		aiderLine.SetPositions(points);
	}
}
