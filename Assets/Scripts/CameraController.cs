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

	private Transform actualTarget = null;
	public Transform target {
		get {
			return actualTarget;
		}
		set {
			actualTarget = value;
			if (actualTarget != null) {
				Initialize();
			}
		}
	}
	private Vector3 offset;
	private Vector3 cameraStartingPosition;

	public float xDifferenceAllowed = 1f;
	public float yDifferenceAllowed = 1f;
	public float compensationSpeed = 10f;

	private void Initialize() {
		offset = target.position - transform.position;
		cameraStartingPosition = transform.position;

		GameController.Instance.ReinitalizeGame += ReinitalizeCamera;
	}

	void LateUpdate () {
		if (target != null) {
			Vector3 wormCameraPosition = target.position - offset;
			if (Mathf.Abs(wormCameraPosition.x - transform.position.x) > xDifferenceAllowed) {
				float diff = wormCameraPosition.x - transform.position.x;
				transform.position += Vector3.right * Mathf.Sign(diff) * Mathf.Abs(xDifferenceAllowed - Mathf.Abs(diff)) * Time.deltaTime * compensationSpeed;
			}

			if (Mathf.Abs(wormCameraPosition.y - transform.position.y) > yDifferenceAllowed) {
				float diff = wormCameraPosition.y - transform.position.y;
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
	}
}
