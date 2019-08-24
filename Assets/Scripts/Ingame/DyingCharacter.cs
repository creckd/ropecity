using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DyingCharacter : MonoBehaviour {

	public Renderer[] renderers;
	public ParticleSystem thanosParticle;

	private Animator anim;
	private const string dieAnimName = "Die";

	public float thanosParticleSpeedMultiplier = 1f;

	[Range(0f,1f)]
	public float holoFadeAmount = 0f;

	[Range(0f, 1f)]
	public float disappearAmount = 0f;

	public Color tintColor;
	private Vector2 velocity = Vector2.zero;

	ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime;

	private void Awake() {
		anim = GetComponent<Animator>();
		//renderer.sharedMaterial = Instantiate(renderer.material);
		velocityOverLifetime = thanosParticle.velocityOverLifetime;
	}

	[ExecuteInEditMode]
	private void Update() {
		for(int i = 0;i < renderers.Length;i++) {
			for (int j = 0; j < renderers[i].materials.Length; j++) {
				renderers[i].materials[j].SetFloat("_AnimT", holoFadeAmount);
				renderers[i].materials[j].SetFloat("_SecondAnimT", disappearAmount);
				renderers[i].materials[j].SetColor("_TintColor", tintColor);
			}
		}

		Vector3 targetPosition = new Vector3(transform.position.x + (velocity.x * Time.deltaTime * 20 * 100f / ConfigDatabase.Instance.wormMass), transform.position.y + (velocity.y * Time.deltaTime * 20 * 100f / ConfigDatabase.Instance.wormMass), transform.position.z);
		transform.position = targetPosition;
	}

	public void PlayDeath(Vector2 dyingVelocity, Action callBack) {
		velocity = dyingVelocity;
		anim.Play(dieAnimName);
		StartCoroutine(WaitAndCallBack(callBack));
		SetThanosParticleVelocity(-dyingVelocity);
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
