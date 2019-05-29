using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour {

	private static TutorialController instance = null;
	public static TutorialController Instance {
		get {
			if (instance == null)
				instance = FindObjectOfType<TutorialController>();
			return instance;
		}
	}

	public void InitializeTutorial() {

	}

	private void Update() {
		
	}
}
