using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiCamera : MonoBehaviour {

	private static GuiCamera instance = null;
	public static GuiCamera Instance {
		get {
			if (instance == null) {
				instance = FindObjectOfType<GuiCamera>();
			}
			return instance;
		}
	}

	public Camera thisCamera;

	private void Awake() {
		thisCamera = GetComponent<Camera>();
	}
}
