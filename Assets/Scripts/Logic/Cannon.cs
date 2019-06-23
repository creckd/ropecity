using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : LevelObject {

	public AnimationCurve aimingCurve;
	private float aimAngle = 30f;

	public GameObject cannonMouthPositionObject;
	public GameObject rootObject;
	public ParticleSystem explosionParticle;
	public Animator wormInTheCannon;

	private const string wormInTheCannonAnimationName = "GettingReady";
	private const string cannonShootAnimationName = "Armature|CannonShoot";
	private Animator anim;
	private Quaternion defaultRootRotation;

	[System.Serializable]
	public class CannonData {
		public float aimAngle = 30f;
	}

	public CannonData data = null;

	private void Start() {
		if (GameController.Instance.currentGameState == GameState.Initialized) {
			anim = GetComponentInChildren<Animator>();

			defaultRootRotation = rootObject.transform.rotation;
			GameController.Instance.GameStarted += StartGame;
			GameController.Instance.ReinitalizeGame += ReinitalizeCannon;

			if (data != null) {
				aimAngle = data.aimAngle;
			} else aimAngle = 30f;
		}
	}

	private void StartGame(bool fastStart) {
		ShootWorm(ConfigDatabase.Instance.wormPrefab, fastStart);
	}

	private void ShootWorm(Worm worm, bool fastStart) {
		StartCoroutine(Shoot(worm, fastStart));
	}

	IEnumerator Shoot(Worm worm, bool fastStart) {
		if(!fastStart)
		wormInTheCannon.Play(wormInTheCannonAnimationName, 0, 0f);
		Worm instantiatedWorm = Instantiate(worm, cannonMouthPositionObject.transform.position, worm.transform.rotation) as Worm;
		instantiatedWorm.Initialize();
		GameController.Instance.currentWorm = instantiatedWorm;

		instantiatedWorm.gameObject.SetActive(false);
		Quaternion defRootRotation = rootObject.transform.rotation;
		Quaternion tarRootRotation = rootObject.transform.rotation * Quaternion.Euler(new Vector3(-aimAngle, 0f, 0f));
		if (!fastStart) {
			float windUpTimer = 0f;
			float windUpTime = 2f;
			while (windUpTimer <= windUpTime) {
				windUpTimer += Time.deltaTime;
				rootObject.transform.rotation = Quaternion.LerpUnclamped(defRootRotation, tarRootRotation, aimingCurve.Evaluate(windUpTimer / windUpTime));
				yield return null;
			}
		} else {
			rootObject.transform.rotation = tarRootRotation;
		}
		rootObject.transform.rotation = tarRootRotation;
		anim.Play(cannonShootAnimationName, 0, 0f);
		yield return null;
		yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length - 2.5f);

		SoundManager.Instance.CreateOneShot(AudioConfigDatabase.Instance.cannonShoot);
		PlayShootParticles();
		instantiatedWorm.transform.position = cannonMouthPositionObject.transform.position;
		instantiatedWorm.gameObject.SetActive(true);
		wormInTheCannon.gameObject.SetActive(false);
		instantiatedWorm.AddForce((cannonMouthPositionObject.transform.position - rootObject.transform.position).normalized * ConfigDatabase.Instance.cannonShootForceMultiplier);
		GameController.Instance.StartSlowingTime();
	}

	private void ReinitalizeCannon() {
		anim.Play("Default", 0, 0f);
		rootObject.transform.rotation = defaultRootRotation;
		wormInTheCannon.gameObject.SetActive(true);
	}

	private void PlayShootParticles() {
		if(explosionParticle != null)
		explosionParticle.Play();
	}

	public override void DeserializeObjectData(string objectData) {
		if (objectData != null && objectData != "none")
			data = StringSerializationAPI.Deserialize(typeof(CannonData), objectData) as CannonData;
	}

	public override string SerializeObjectData() {
		return StringSerializationAPI.Serialize(typeof(CannonData), data);
	}
}
