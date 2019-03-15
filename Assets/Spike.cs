using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour {

	public bool isRetractableSpike = true;
	public float activationDistance = 10f;
	public float retractionSpeed = 10f;

	public SkinnedMeshRenderer spikeSkinned;

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
		targetRetractionValue = 0f;
	}

	public void Defuse() {
		targetRetractionValue = 100f;
	}

	private void OnTriggerEnter(Collider other) {
		if (GameController.Instance.currentGameState == GameState.GameStarted && other.gameObject == GameController.Instance.currentWorm.gameObject) {
			GameController.Instance.currentWorm.Die();
		}
	}
}
