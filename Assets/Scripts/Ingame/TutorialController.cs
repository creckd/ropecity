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
	private float perfectSwingEndAngle = -25f;

	private Worm currentWorm;
	private bool currentlySwinging = false;
	private bool pausedSwinging = false;
	private int numberOfTimesSwinged = 0;

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
		numberOfTimesSwinged++;
		if (numberOfTimesSwinged < 3) {
			GameController.Instance.wormInputEnabled = false;
		} else {
			FinishTutorial();
		}
	}

	private void FinishTutorial() {
		GameController.Instance.LandedHook -= LandedHook;
		GameController.Instance.ReleasedHook -= ReleasedHook;
		InputController.Instance.TapHappened -= TapHappened;
		InputController.Instance.ReleaseHappened -= ReleaseHappened;
		StartCoroutine(ShowLastTask());
	}

	IEnumerator ShowLastTask() {
		GameController.Instance.wormInputEnabled = false;
		yield return new WaitForSecondsRealtime(0.25f);
		float pausingTime = 0.5f;
		StaticBlurCreator.Instance.CreateStaticBlurImage();
		IngameBlurController.Instance.BlurImage(pausingTime, false, true, true);
		float t = 0f;
		float defaultTimeScale = Time.timeScale;
		GameController.Instance.gameControllerControlsTime = false;

		while (t <= pausingTime) {
			t += Time.unscaledDeltaTime;
			Time.timeScale = Mathf.Lerp(defaultTimeScale, 0f, t / pausingTime);
			yield return null;
		}

		GameController.Instance.ShowTutorialLastTask();

		yield return new WaitForSecondsRealtime(3f);
		GameController.Instance.wormInputEnabled = true;
		IngameBlurController.Instance.UnBlurImage(pausingTime);

		//float resumingTime = 0.5f;
		//t = 0f;
		//defaultTimeScale = Time.timeScale;
		//GameController.Instance.gameControllerControlsTime = false;

		//while (t <= resumingTime) {
		//	t += Time.unscaledDeltaTime;
		//	Time.timeScale = Mathf.Lerp(defaultTimeScale, ConfigDatabase.Instance.slow, t / pausingTime);
		//	yield return null;
		//}

		GameController.Instance.gameControllerControlsTime = true;
	}

	private void LandedHook(Vector3 obj) {
		currentlySwinging = true;
		GameController.Instance.gameControllerControlsTime = true;
		GameController.Instance.wormInputEnabled = false;
	}

	private void Update() {

		Worm currentWorm = GameController.Instance.currentWorm;
		if (numberOfTimesSwinged < 3 && currentWorm != null) {

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
						GameController.Instance.HideHoldIndicator();
						GameController.Instance.wormInputEnabled = true;
					}
				}
			}
		}

	}
}
