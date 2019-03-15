using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : LevelObject {

	public GameObject cannonMouthPositionObject;

	private const string cannonShootAnimationName = "Armature|Shoot";
	private Animator anim;

	private void Start() {
		if (GameController.Instance.currentGameState == GameState.Initialized) {
			anim = GetComponent<Animator>();

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
		anim.Play(cannonShootAnimationName, 0, 0f);
		yield return null;
		yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length - 0.7f);

		//CameraShake.Instance.Shake(0.1f, 0.5f, 4f);
		instantiatedWorm.gameObject.SetActive(true);
		instantiatedWorm.AddForce(ConfigDatabase.Instance.cannonShootDirection * ConfigDatabase.Instance.cannonShootForceMultiplier);
		GameController.Instance.StartSlowingTime();
	}

	private void ReinitalizeCannon() {
		anim.Play("Default", 0, 0f);
	}
}
