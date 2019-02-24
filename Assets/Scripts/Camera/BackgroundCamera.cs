using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundCamera : MonoBehaviour {

	private static BackgroundCamera instance = null;
	public static BackgroundCamera Instance {
		get {
			if (instance == null) {
				instance = FindObjectOfType<BackgroundCamera>();
			}
			return instance;
		}
	}

	public GameObject backGround;

	public Camera thisCamera;

	private void Awake() {
		thisCamera = GetComponent<Camera>();
		float worldHeight = thisCamera.orthographicSize * 2f;
		float worldWidth = worldHeight * thisCamera.aspect;
		backGround.transform.localScale = new Vector3(worldWidth, worldHeight, 1f);
	}
}
