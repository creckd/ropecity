using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class Fader : MonoBehaviour {

	public int order;
	public float fadeTime = 1f;

	private CanvasGroup cG;

	private void Awake() {
		cG = GetComponent<CanvasGroup>();
		cG.alpha = 0;
	}

	public void FadeIn() {
		StartCoroutine(Fade(0f, 1f));
	}

	public void FadeOut() {
		StartCoroutine(Fade(1f, 0f));
	}

	IEnumerator Fade(float fromValue, float tarValue) {
		float timer = 0f;
		while (timer <= fadeTime) {
			timer += Time.unscaledDeltaTime;
			cG.alpha = Mathf.Lerp(fromValue, tarValue, timer/fadeTime);
			yield return null;
		}
		cG.alpha = tarValue;
	}
}
