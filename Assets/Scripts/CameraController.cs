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

	private void Initialize() {
		offset = target.position - transform.position;
		cameraStartingPosition = transform.position;

		GameController.Instance.ReinitalizeGame += ReinitalizeCamera;
	}

	void Update () {
		if (target != null) {

			Vector3 tarPosition = Vector3.Lerp(transform.position, target.position - offset, Time.deltaTime * 10f);
			transform.position = new Vector3(tarPosition.x,transform.position.y,transform.position.z);
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
