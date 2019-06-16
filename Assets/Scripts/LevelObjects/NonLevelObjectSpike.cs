using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NonLevelObjectSpike : MonoBehaviour {

	private bool isRetractableSpike = true;

	public float activationDistance = 10f;
	public float retractionSpeed = 10f;

	public SkinnedMeshRenderer spikeSkinned;
	public ParticleSystem shineParticle;

	private float targetRetractionValue = 0f;
	private float currentRetractionValue = 0f;
	private int frameCheckFrequency = 30;

	private void Awake() {
		currentRetractionValue = 100f;
	}

	private void Update() {
		if (isRetractableSpike) {
			if (Time.frameCount % frameCheckFrequency == 0 && GameController.Instance.currentWorm != null) {
				float distance = Vector3.Distance(GameController.Instance.currentWorm.transform.position, transform.position);
				if (distance < activationDistance)
					Activate();
				else Defuse();
			}
			currentRetractionValue = Mathf.Lerp(currentRetractionValue, targetRetractionValue, Time.deltaTime * retractionSpeed);
			spikeSkinned.SetBlendShapeWeight(0, currentRetractionValue);
		}
	}

	public void Activate() {
		if (targetRetractionValue != 0f)
			SoundManager.Instance.CreateOneShot(AudioConfigDatabase.Instance.spikeDraw);
		targetRetractionValue = 0f;
		shineParticle.gameObject.SetActive(true);
	}

	public void Defuse() {
		targetRetractionValue = 100f;
		shineParticle.gameObject.SetActive(false);
	}

	private void OnTriggerEnter2D(Collider2D other) {
		if (GameController.Instance.currentGameState == GameState.GameStarted && other.gameObject == GameController.Instance.currentWorm.gameObject) {
			GameController.Instance.currentWorm.Die();
		}
	}
}
