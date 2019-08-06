using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DyingCharacter : MonoBehaviour {

	public SkinnedMeshRenderer renderer;
	public ParticleSystem thanosParticle;

	private Animator anim;
	private const string dieAnimName = "Die";

	public float thanosParticleSpeedMultiplier = 1f;

	[Range(0f,1f)]
	public float holoFadeAmount = 0f;

	[Range(0f, 1f)]
	public float disappearAmount = 0f;

	public Color tintColor;

	ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime;

	private void Awake() {
		anim = GetComponent<Animator>();
		renderer.sharedMaterial = Instantiate(renderer.material);
		velocityOverLifetime = thanosParticle.velocityOverLifetime;
	}

	[ExecuteInEditMode]
	private void Update() {
		renderer.sharedMaterial.SetFloat("_AnimT", holoFadeAmount);
		renderer.sharedMaterial.SetFloat("_SecondAnimT", disappearAmount);
		renderer.sharedMaterial.SetColor("_TintColor", tintColor);
	}

	public void PlayDeath(Vector2 dyingVelocity, Action callBack) {
		anim.Play(dieAnimName);
		StartCoroutine(WaitAndCallBack(callBack));
		SetThanosParticleVelocity(dyingVelocity);
	}

	IEnumerator WaitAndCallBack(Action callBack) {
		yield return null;
		yield return new WaitForSecondsRealtime(anim.GetCurrentAnimatorStateInfo(0).length);
		callBack();
	}

	public void SetThanosParticleVelocity(Vector2 velocity) {
		velocityOverLifetime.x = velocity.x * thanosParticleSpeedMultiplier;
		velocityOverLifetime.y = velocity.y * thanosParticleSpeedMultiplier;
	}
}
