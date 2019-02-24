using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameBlurController : MonoBehaviour {

	private static IngameBlurController instance = null;
	public static IngameBlurController Instance {
		get {
			if (instance == null)
				instance = FindObjectOfType<IngameBlurController>();
			return instance;
		}
	}

	public SimpleImageEffectApplier blurApplier;
	private bool currentlyBlured = false;
	private Coroutine currentRunningBlurRoutine = null;

	private void Awake() {
		blurApplier.enabled = false;
	}

	public void BlurImage(float blurTime = 1f) {
		StaticBlurCreator.Instance.CreateStaticBlurImage();
		blurApplier.mat.SetTexture("_SecondTex", Shader.GetGlobalTexture(StaticBlurCreator.renderTextureGlobalPropName));
		blurApplier.mat.SetFloat("_T", 0f);
		blurApplier.enabled = true;
		StartCoroutine(Blur(0f, 1f, blurTime));
	}

	public void UnBlurImage(float blurTime = 1f) {
	}

	IEnumerator Blur(float from, float target, float time) {
		float timer = 0f;
		while (timer < time) {
			timer += Time.unscaledDeltaTime;
			blurApplier.mat.SetFloat("_T", timer / time);
			yield return null;
		}
		blurApplier.mat.SetFloat("_T", target);
	}
}
