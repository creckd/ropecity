using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : LevelObject {

	public GameObject cannonMouthPositionObject;
	public ParticleSystem explosionParticle;

	private const string cannonShootAnimationName = "Armature|CannonShoot";
	private Animator anim;


	private void Start() {
		if (GameController.Instance.currentGameState == GameState.Initialized) {
			anim = GetComponentInChildren<Animator>();

			GameController.Instance.GameStarted += StartGame;
			GameController.Instance.ReinitalizeGame += ReinitalizeCannon;
		}
	}

	private void StartGame() {
		ShootWorm(ConfigDatabase.Instance.wormPrefab);
	}

	private void ShootWorm(Worm worm) {
		StartCoroutine(Shoot(worm));
	}

	IEnumerator Shoot(Worm worm) {
		Worm instantiatedWorm = Instantiate(worm, cannonMouthPositionObject.transform.position, worm.transform.rotation) as Worm;
		instantiatedWorm.Initialize();
		GameController.Instance.currentWorm = instantiatedWorm;

		instantiatedWorm.gameObject.SetActive(false);
		yield return new WaitForSecondsRealtime(0.8f);
		anim.Play(cannonShootAnimationName, 0, 0f);
		yield return null;
		yield return new WaitForSecondsRealtime(anim.GetCurrentAnimatorStateInfo(0).length - 5.5f);

		//CameraShake.Instance.Shake(0.1f, 0.5f, 4f);
		PlayShootParticles();
		instantiatedWorm.gameObject.SetActive(true);
		instantiatedWorm.AddForce(ConfigDatabase.Instance.cannonShootDirection * ConfigDatabase.Instance.cannonShootForceMultiplier);
		GameController.Instance.StartSlowingTime();
	}

	private void ReinitalizeCannon() {
		anim.Play("Default", 0, 0f);
	}

	private void PlayShootParticles() {
		if(explosionParticle != null)
		explosionParticle.Play();
	}
}
