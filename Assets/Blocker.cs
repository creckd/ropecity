using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blocker : MonoBehaviour {

	private static Blocker instance = null;
	public static Blocker Instane {
		get {
			if (instance == null)
				instance = FindObjectOfType<Blocker>();
			return instance;
		}
	}

	public GameObject root;

	public void Block() {
		root.gameObject.SetActive(true);
	}

	public void UnBlock() {
		root.gameObject.SetActive(false);
	}
}
