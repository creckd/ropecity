using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandMine : MonoBehaviour {

	private const string explodeAnimationStateName = "Explode";
	private const string idleAnimationName = "Idle";

	private Animator anim;
	private SphereCollider trigger;

	public ParticleSystem explodeParticle;

	private void Awake() {
		anim = GetComponent<Animator>();
		trigger = GetComponent<SphereCollider>();
		anim.Play(idleAnimationName, 0, Random.Range(0f, 1f));
	}

	private void OnTriggerEnter(Collider other) {
		bool isWorm = other.CompareTag("Player");

		if (isWorm && GameController.Instance.currentGameState == GameState.GameStarted) {
			Detonate();
		}
	}

	private void Detonate() {
		StartCoroutine(ExplodeRoutine());
	}

	IEnumerator ExplodeRoutine() {
		anim.CrossFade(explodeAnimationStateName, 0.1f);
		trigger.enabled = false;
		yield return null;
		//yield return new WaitForSeconds(0.25f);
		explodeParticle.Play();
	}
}
