using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	private static CameraController instance = null;
	public static CameraController Instance {
		get {
			if (instance == null)
				instance = FindObjectOfType<CameraController>();
			return instance;
		}
	}

	private Transform target = null;
	private Vector3 offset;
	private Vector3 cameraStartingPosition;
	private bool initializedOffsets = false;

	public float xDifferenceAllowed = 1f;
	public float yDifferenceAllowed = 1f;
	public bool horizontalMovementLocked = false;
	public bool verticalMovementLocked = false;
	public float compensationSpeed = 10f;

	public AnimationCurve sweepCurve;
	public float sweepingTime = 1f;

	private void Awake() {
		GameController.Instance.GameFinished += GameFinished;
		GameController.Instance.ReinitalizeGame += ReinitalizeCamera;
	}

	private void Start() {
		if (!GameController.Instance.isDebugTestLevelMode) {
			horizontalMovementLocked = !LevelController.Instance.settings.isHorizontalCameraMovementEnabled;
			verticalMovementLocked = !LevelController.Instance.settings.isVerticalCameraMovementEnabled;
		}
	}

	public void StartTracking(Transform target) {
		this.target = target;

		if (!initializedOffsets) {
			initializedOffsets = true;
			offset = target.position - transform.position;
			cameraStartingPosition = transform.position;
		}

	}

	private void StopTracking() {
		this.target = null;
	}

	private void GameFinished(bool win) {
		if (win) {
			StopTracking();
			StartCoroutine(SweepToVictoryPosition(GetVictoryCameraTransfom()));
		}
	}

	IEnumerator SweepToVictoryPosition(Transform victoryTransform) {
		float timer = 0f;
		Vector3 defPosition = transform.position;
		Quaternion defRotation = transform.rotation;
		while (timer < sweepingTime) {
			timer += Time.unscaledDeltaTime;
			transform.position = Vector3.Lerp(defPosition, victoryTransform.position, sweepCurve.Evaluate(timer / sweepingTime));
			transform.rotation = Quaternion.Lerp(defRotation, victoryTransform.rotation, sweepCurve.Evaluate(timer / sweepingTime));
			yield return null;
		}
	}

	private Transform GetVictoryCameraTransfom() {
		return VictoryCameraPosition.Instance.transform;
	}

	void LateUpdate () {
		if (target != null) {
			Vector3 wormCameraPosition = target.position - offset;
			if(!horizontalMovementLocked)
			if (Mathf.Abs(wormCameraPosition.x - transform.position.x) > xDifferenceAllowed) {
				float diff = wormCameraPosition.x - transform.position.x;
				transform.position += Vector3.right * Mathf.Sign(diff) * Mathf.Abs(xDifferenceAllowed - Mathf.Abs(diff)) * Time.deltaTime * compensationSpeed;
			}

			if(!verticalMovementLocked)
			if (GameController.Instance.currentWorm.gameObject.activeSelf && Mathf.Abs(GameController.Instance.currentWorm.transform.position.y - transform.position.y) > yDifferenceAllowed) {
				float diff = GameController.Instance.currentWorm.transform.position.y - transform.position.y;
				transform.position += Vector3.up * Mathf.Sign(diff) * Mathf.Abs(yDifferenceAllowed - Mathf.Abs(diff)) * Time.deltaTime * compensationSpeed;
			}
		}
	}


	private void ReinitalizeCamera() {
		StartCoroutine(CameraReinitalizing());
	}

	IEnumerator CameraReinitalizing() {
		float timer = 0f;
		Vector3 currentCameraPosition = transform.position;
		while (timer <= ConfigDatabase.Instance.reinitalizingDuration) {
			transform.position = Vector3.Lerp(currentCameraPosition, cameraStartingPosition, timer / ConfigDatabase.Instance.reinitalizingDuration);
			timer += Time.deltaTime;
			yield return null;
		}
		transform.position = cameraStartingPosition;
	}
}
