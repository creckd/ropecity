using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spike : LevelObject {

	private bool isRetractableSpike = true;

	public float activationDistance = 10f;
	public float retractionSpeed = 10f;

	public SkinnedMeshRenderer spikeSkinned;
	public CanvasGroup warningCanvasGroup;
	public ParticleSystem shineParticle;

	private float targetRetractionValue = 0f;
	private float currentRetractionValue = 0f;
	private int frameCheckFrequency = 30;

	private void Awake() {
		currentRetractionValue = 100f;
		warningCanvasGroup.transform.up = Vector3.up;
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
			warningCanvasGroup.alpha = Mathf.Lerp(warningCanvasGroup.alpha,(targetRetractionValue / 100f),Time.deltaTime * 12f);
			spikeSkinned.SetBlendShapeWeight(0, currentRetractionValue);
		}
	}

	public void Activate() {
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
