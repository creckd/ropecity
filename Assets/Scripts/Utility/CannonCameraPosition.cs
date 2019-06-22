using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonCameraPosition : MonoBehaviour {

	private static CannonCameraPosition instance = null;
	public static CannonCameraPosition Instance {
		get {
			if (instance == null) {
				instance = FindObjectOfType<CannonCameraPosition>();
			}
			return instance;
		}
	}
}
