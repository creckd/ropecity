using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LineRenderer))]
public class LineRendererFader : Fader {

	private LineRenderer lineRenderer;
	private Material materialToFade;

	public string colorPropertyName = "_TintColor";

	private Color defaultColor;

	private void Awake() {
		lineRenderer = GetComponent<LineRenderer>();
		materialToFade = lineRenderer.material;// = Instantiate(lineRenderer.material);

		defaultColor = materialToFade.GetColor(colorPropertyName);
		materialToFade.SetColor(colorPropertyName, new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0f));
	}

	public override void FadeIn() {
		StartCoroutine(Fade(0f, 1f));
	}

	public override void FadeOut() {
		StartCoroutine(Fade(1f, 0f));
	}

	IEnumerator Fade(float fromValue, float tarValue) {
		yield return new WaitForSecondsRealtime(delay); 
		Color fromColor = new Color(defaultColor.r, defaultColor.g, defaultColor.b, fromValue);
		Color tarColor = new Color(defaultColor.r, defaultColor.g, defaultColor.b, tarValue);
		float timer = 0f;
		while (timer <= fadeTime) {
			timer += Time.unscaledDeltaTime;
			materialToFade.SetColor(colorPropertyName, Color.Lerp(fromColor,tarColor,timer/fadeTime));
			yield return null;
		}
		materialToFade.SetColor(colorPropertyName, tarColor);
	}
}
