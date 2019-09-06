using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandMine : LevelObject {

	private const string explodeAnimationStateName = "Explode";
	private const string idleAnimationName = "Idle";

	private Animator anim;
	private CircleCollider2D trigger;

	public ParticleSystem explodeParticle;

	private void Start() {
		if (GameController.Instance.currentGameState == GameState.Initialized) {
			anim = GetComponent<Animator>();
			trigger = GetComponent<CircleCollider2D>();
			anim.Play(idleAnimationName, 0, UnityEngine.Random.Range(0f, 1f));

			GameController.Instance.ReinitalizeGame += ReinitalizeMine;
		}
	}

	private void ReinitalizeMine() {
		anim.Play(idleAnimationName, 0, UnityEngine.Random.Range(0f, 1f));
		trigger.enabled = true;
	}

	private void OnTriggerEnter2D(Collider2D other) {
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
		yield return new WaitForSecondsRealtime(0.3f);
		explodeParticle.Play();
		yield return new WaitForSecondsRealtime(0.1f);
		if (GameController.Instance.currentGameState == GameState.GameStarted && Vector3.Distance(transform.position, GameController.Instance.currentWorm.transform.position) < ConfigDatabase.Instance.mineLethalRange)
			GameController.Instance.currentWorm.Die();
	}
}
