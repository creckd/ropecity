using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageTransitionHandler : MonoBehaviour {

	private static ImageTransitionHandler instance = null;
	public static ImageTransitionHandler Instance {
		get {
			if (instance = null)
				instance = FindObjectOfType<ImageTransitionHandler>();
			return instance;
		}
	}

	public SimpleImageEffectApplier imageEffectApplier;
	public float transitionTime = 1f;

	private const string _TProperty = "_T";
	private static float t = 0f;

	private bool currentlyTransitioning = false;
	private Coroutine currentTransitionerCoroutine = null;
	private Material material;

	private void Awake() {
		Initialize();
		TransitionIn();
	}

	private void Initialize() {
		material = imageEffectApplier.mat;
		material.SetFloat(_TProperty, t);
		if (t == 0f) {
			imageEffectApplier.enabled = false;
		}
	}

	public void TransitionIn() {
		StopCurrentTransitionIfExists();
		currentTransitionerCoroutine = StartCoroutine(TransitionRoutine(0f, 1f));
	}

	public void TransitionOut() {
		StopCurrentTransitionIfExists();
		currentTransitionerCoroutine = StartCoroutine(TransitionRoutine(1f, 0f));
	}

	private void StopCurrentTransitionIfExists() {
		if (currentlyTransitioning) {
			StopCoroutine(currentTransitionerCoroutine);
			currentTransitionerCoroutine = null;
		}
	}

	IEnumerator TransitionRoutine(float fromValue, float tarValue) {
		yield return new WaitForSeconds(1f);
		currentlyTransitioning = true;
		if (tarValue == 1f)
			imageEffectApplier.enabled = true;
		float timer = 0f;
		while (timer <= transitionTime) {
			timer += Time.unscaledDeltaTime;
			t = Mathf.Lerp(fromValue, tarValue, timer / transitionTime);
			material.SetFloat(_TProperty, t);
			yield return null;
		}
		t = tarValue;
		material.SetFloat(_TProperty, t);
		if (tarValue == 0f)
			imageEffectApplier.enabled = false;
		currentlyTransitioning = false;
	}

}
