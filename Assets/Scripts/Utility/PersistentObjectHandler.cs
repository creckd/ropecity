using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentObjectHandler : MonoBehaviour {

	private static bool initialized = false;

	public GameObject[] persistentObjects;

	private void Awake() {
		if (!initialized) {
			initialized = true;
			InitializePersistentObjects();
			Destroy(this.gameObject);
		} else {
			Destroy(this.gameObject);
		}
	}

	private void InitializePersistentObjects() {
		initialized = true;
		for (int i = 0; i < persistentObjects.Length; i++) {
			Instantiate(persistentObjects[i].gameObject, Vector3.zero, Quaternion.identity);
		}
		SavedDataManager.Instance.Load();
	}
}
