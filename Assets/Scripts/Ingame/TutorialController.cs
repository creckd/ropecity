using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour {

	private static TutorialController instance = null;
	public static TutorialController Instance {
		get {
			if (instance == null)
				instance = FindObjectOfType<TutorialController>();
			return instance;
		}
	}

	private float perfectSwingStartAngle = 45f;
	private float perfectSwingEndAngle = -30f;

	private Worm currentWorm;
	private bool currentlySwinging = false;
	private bool pausedSwinging = false;

	public void StartTutorial() {
		GameController.Instance.LandedHook += LandedHook;
		GameController.Instance.ReleasedHook += ReleasedHook;
		InputController.Instance.TapHappened += TapHappened;
		InputController.Instance.ReleaseHappened += ReleaseHappened;
		GameController.Instance.wormInputEnabled = false;
	}

	private void ReleaseHappened(int fingerID) {
		if (!GameController.Instance.wormInputEnabled && currentlySwinging && !pausedSwinging) {
			GameController.Instance.ShowHoldIndicator();
			GameController.Instance.gameControllerControlsTime = false;
			pausedSwinging = true;
			Time.timeScale = 0f;
		}
	}

	private void TapHappened(int fingerID) {
		GameController.Instance.HideHoldIndicator();
		GameController.Instance.gameControllerControlsTime = true;
		pausedSwinging = false;
	}

	private void ReleasedHook() {
		currentlySwinging = false;
		GameController.Instance.HideHoldIndicator();
		GameController.Instance.HideReleaseIndicator();
		GameController.Instance.gameControllerControlsTime = true;
		GameController.Instance.wormInputEnabled = false;
	}

	private void LandedHook(Vector3 obj) {
		currentlySwinging = true;
		GameController.Instance.gameControllerControlsTime = true;
		GameController.Instance.wormInputEnabled = false;
	}

	private void Update() {

		Worm currentWorm = GameController.Instance.currentWorm;
		if (currentWorm != null) {

			float angle = Vector3.SignedAngle(currentWorm.wormAimDirection, Vector3.up, Vector3.forward);
			float differenceFromPerfectStartAngle = angle - perfectSwingStartAngle;
			float differenceFromPerfectEndAngle = angle - perfectSwingEndAngle;
			float angleMargin = 25f;
			float releasableMargin = 3f;

			if (!currentlySwinging && differenceFromPerfectStartAngle >= 0 && differenceFromPerfectStartAngle <= angleMargin && !currentWorm.landedHook) {
				GameController.Instance.gameControllerControlsTime = false;
				Time.timeScale = Mathf.Lerp(0f, ConfigDatabase.Instance.slowMotionSpeed, differenceFromPerfectStartAngle / angleMargin);
				if (differenceFromPerfectStartAngle <= releasableMargin) {
					if (!GameController.Instance.wormInputEnabled) {
						GameController.Instance.wormInputEnabled = true;
						GameController.Instance.ShowHoldIndicator();
					}
				}
			} else if (currentlySwinging && differenceFromPerfectEndAngle >= 0 &&  differenceFromPerfectEndAngle <= angleMargin && currentWorm.landedHook) {
				if (!pausedSwinging) {
					GameController.Instance.gameControllerControlsTime = false;
					Time.timeScale = Mathf.Lerp(0f, ConfigDatabase.Instance.slowMotionSpeed, differenceFromPerfectEndAngle / angleMargin);
				}
				if (differenceFromPerfectEndAngle <= releasableMargin) {
					if (!GameController.Instance.wormInputEnabled) {
						GameController.Instance.ShowReleaseIndicator();
						GameController.Instance.wormInputEnabled = true;
					}
				}
			}


		}
	}
}
