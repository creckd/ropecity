using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashTransition : MonoBehaviour {

	private static FlashTransition instance = null;
	public static FlashTransition Instance {
		get {
			if (instance == null)
				instance = FindObjectOfType<FlashTransition>();
			return instance;
		}
	}

	public Image flashTransitionImage;
	public float flashTransitionTime = 0.5f;

	public void TransitionIn(Action callBack) {
		StartCoroutine(Transition(0f, 1f, callBack));
	}

	public void TransitionIn() {
		TransitionIn(delegate { });
	}

	public void TransitionOut(Action callBack) {
		StartCoroutine(Transition(1f, 0f, callBack));
	}

	public void TransitionOut() {
		TransitionOut(delegate { });
	}

	IEnumerator Transition(float from, float tar, Action callBack) {
		flashTransitionImage.gameObject.SetActive(true);
		Color fromColor = new Color(flashTransitionImage.color.r, flashTransitionImage.color.g, flashTransitionImage.color.b, from);
		Color tarColor = new Color(flashTransitionImage.color.r, flashTransitionImage.color.g, flashTransitionImage.color.b, tar);

		float t = 0f;
		while (t <= flashTransitionTime) {
			t += Time.unscaledDeltaTime;
			flashTransitionImage.color = Color.Lerp(fromColor, tarColor, t/flashTransitionTime);
			yield return null;
		}
		flashTransitionImage.color = tarColor;
		flashTransitionImage.gameObject.SetActive(tar != 0f);
		if (callBack != null)
			callBack();
	}
}
