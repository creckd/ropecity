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

	public void BlurImage(float blurTime = 1f, bool greyScale = false, bool useMask = false, bool forceStartingValue = true) {
		StopAllCoroutines();
		currentlyBlured = true;
		StaticBlurCreator.Instance.CreateStaticBlurImage();
		blurApplier.mat.SetTexture("_SecondTex", Shader.GetGlobalTexture(StaticBlurCreator.renderTextureGlobalPropName));
		blurApplier.mat.SetInt("_UseMask", useMask ? 1 : 0);
		blurApplier.mat.SetInt("_GreyScale", greyScale ? 1 : 0);
		blurApplier.mat.SetFloat("_T", 0f);
		blurApplier.enabled = true;
		if(forceStartingValue)
		blurApplier.mat.SetFloat("_T", 0f);
		StartCoroutine(Blur(blurApplier.mat.GetFloat("_T"), 1f, blurTime));
	}

	public void UnBlurImage(float blurTime = 1f, bool forceStartingValue = true) {
		StopAllCoroutines();
		currentlyBlured = false;
		if (forceStartingValue)
		blurApplier.mat.SetFloat("_T", 1f);
		StartCoroutine(Blur(blurApplier.mat.GetFloat("_T"), 0f, blurTime));
	}

	IEnumerator Blur(float from, float target, float time) {
		float timer = 0f;
		while (timer < time) {
			timer += Time.unscaledDeltaTime;
			blurApplier.mat.SetFloat("_T", Mathf.Lerp(from,target,timer / time));
			yield return null;
		}
		blurApplier.mat.SetFloat("_T", target);
	}
}
