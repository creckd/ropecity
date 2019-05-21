using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentObjectHandler : MonoBehaviour {

	private static bool initialized = false;

	public GameObject[] persistentObjects;
	private static GameObject[] instantiatedPersistentObjects;

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
		instantiatedPersistentObjects = new GameObject[persistentObjects.Length];
		for (int i = 0; i < persistentObjects.Length; i++) {
			instantiatedPersistentObjects[i] = Instantiate(persistentObjects[i].gameObject, Vector3.zero, Quaternion.identity) as GameObject;
		}
		SavedDataManager.Instance.Load();
	}

	public static void DeletePersistentObjects() {
		for (int i = 0; i < instantiatedPersistentObjects.Length; i++) {
			Destroy(instantiatedPersistentObjects[i].gameObject);
		}
		initialized = false;
	}
}
