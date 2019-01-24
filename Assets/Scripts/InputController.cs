using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {

	private static InputController instance = null;
	public static InputController Instance {
		get {
			if (instance == null)
				instance = FindObjectOfType<InputController>();
			return instance;
		}
	}

	public Action<int> TapHappened = delegate { };
	public Action<int> ReleaseHappened = delegate { };

	private void Update() {
		// PC
		if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) {
			TapHappened(0);
		}
		if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space)) {
			ReleaseHappened(0);
		}
	}
}
