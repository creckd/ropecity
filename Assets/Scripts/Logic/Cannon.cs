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
		float windUpTimer = 0f;
		Quaternion defRootRotation = rootObject.transform.rotation;
		Quaternion tarRootRotation = rootObject.transform.rotation * Quaternion.Euler(new Vector3(-aimAngle, 0f, 0f));
		float windUpTime = 2f;
		while (windUpTimer <= windUpTime) {
			windUpTimer += Time.unscaledDeltaTime;
			rootObject.transform.rotation = Quaternion.LerpUnclamped(defRootRotation, tarRootRotation, aimingCurve.Evaluate(windUpTimer / windUpTime));
			yield return null;
		}
		rootObject.transform.rotation = tarRootRotation;
		anim.Play(cannonShootAnimationName, 0, 0f);
		yield return null;
		yield return new WaitForSecondsRealtime(anim.GetCurrentAnimatorStateInfo(0).length - 2.5f);

		//CameraShake.Instance.Shake(0.1f, 0.5f, 4f);
		SoundManager.Instance.CreateOneShot(AudioConfigDatabase.Instance.cannonShoot);
		PlayShootParticles();
		instantiatedWorm.transform.position = cannonMouthPositionObject.transform.position;
		instantiatedWorm.gameObject.SetActive(true);
		instantiatedWorm.AddForce((cannonMouthPositionObject.transform.position - rootObject.transform.position).normalized * ConfigDatabase.Instance.cannonShootForceMultiplier);
		GameController.Instance.StartSlowingTime();
	}

	private void ReinitalizeCannon() {
		anim.Play("Default", 0, 0f);
		rootObject.transform.rotation = defaultRootRotation;
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
