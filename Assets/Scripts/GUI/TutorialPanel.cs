using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPanel : FaderPanel {

	private float timeOpened = 0f;
	public Animator tutorialPhoneAnimator;

	public override void OnStartedOpening() {
		base.OnStartedOpening();
		tutorialPhoneAnimator.enabled = true;
		IngameBlurController.Instance.BlurImage(ConfigDatabase.Instance.tutorialBlurTime, true);
	}

	public override void OnOpened() {
		base.OnOpened();
		DeactivateButtons();
		timeOpened = Time.realtimeSinceStartup;
	}

	private void Update() {
		if (timeOpened != 0f) {
			if (Time.realtimeSinceStartup - timeOpened >= ConfigDatabase.Instance.tutorialRequiredWatchTime) {
				ActivateButtons();
				timeOpened = 0f;
			}
		}
	}

	public override void OnStartedClosing() {
		tutorialPhoneAnimator.enabled = false;
		base.OnStartedClosing();
		IngameBlurController.Instance.UnBlurImage(ConfigDatabase.Instance.tutorialBlurTime);
	}

	public override void OnClosed() {
		base.OnClosed();
		GameController.Instance.FinishTutorial();
	}

	public void CloseTutorial() {
		PanelManager.Instance.TryOpenPanel(0);
	}
}
