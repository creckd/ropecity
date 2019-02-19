using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundCamera : MonoBehaviour {

	public GameObject backGround;

	private Camera thisCamera;

	private void Awake() {
		thisCamera = GetComponent<Camera>();
		float worldHeight = thisCamera.orthographicSize * 2f;
		float worldWidth = worldHeight * thisCamera.aspect;
		backGround.transform.localScale = new Vector3(worldWidth, worldHeight, 1f);
	}
}
