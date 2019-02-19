using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryCameraPosition : MonoBehaviour {

	private static VictoryCameraPosition instance = null;
	public static VictoryCameraPosition Instance {
		get {
			if (instance == null) {
				instance = FindObjectOfType<VictoryCameraPosition>();
			}
			return instance;
		}
	}
}
