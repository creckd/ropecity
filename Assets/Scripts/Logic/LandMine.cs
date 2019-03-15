using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandMine : MonoBehaviour {

	private const string explodeAnimationStateName = "Explode";
	private const string idleAnimationName = "Idle";

	private Animator anim;
	private SphereCollider trigger;

	public ParticleSystem explodeParticle;

	private void Start() {
		if (GameController.Instance.currentGameState == GameState.Initialized) {
			anim = GetComponent<Animator>();
			trigger = GetComponent<SphereCollider>();
			anim.Play(idleAnimationName, 0, UnityEngine.Random.Range(0f, 1f));

			GameController.Instance.ReinitalizeGame += ReinitalizeMine;
		}
	}

	private void ReinitalizeMine() {
		anim.Play(idleAnimationName, 0, UnityEngine.Random.Range(0f, 1f));
		trigger.enabled = true;
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
		yield return new WaitForSeconds(0.185f);
		if (GameController.Instance.currentGameState == GameState.GameStarted && Vector3.Distance(transform.position, GameController.Instance.currentWorm.transform.position) < ConfigDatabase.Instance.mineLethalRange)
			GameController.Instance.currentWorm.Die();
	}
}
