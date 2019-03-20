using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OdinSerializer;

public class Spike : LevelObject {

	[System.Serializable]
	public class SpikeData {
		public bool isRetractableSpike = true;
	}

	public SpikeData data = new SpikeData();

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
		if (data.isRetractableSpike) {
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

	public override void DeserializeObjectData(string objectData) {
		if (objectData != null)
			data = SerializationUtility.DeserializeValue<SpikeData>(System.Text.Encoding.ASCII.GetBytes(objectData), DataFormat.Binary);
	}

	public override string SerializeObjectData() {
		return System.Text.Encoding.ASCII.GetString(SerializationUtility.SerializeValue(data, DataFormat.Binary));
	}
}
