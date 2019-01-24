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

	private void Initialize() {
		offset = target.position - transform.position;
	}

	void Update () {
		if (target != null) {

			Vector3 tarPosition = Vector3.Lerp(transform.position, target.position - offset, Time.deltaTime * 10f);
			transform.position = new Vector3(tarPosition.x,transform.position.y,transform.position.z);
		}
	}
}
