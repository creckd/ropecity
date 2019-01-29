using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceDatabase : MonoBehaviour {

	private static ResourceDatabase instance = null;
	public static ResourceDatabase Instance {
		get {
			if (instance == null)
				instance = FindObjectOfType<ResourceDatabase>();
			return instance;
		}
	}

	[System.Serializable]
	public class Resource {
		public string uniqueID;
		public LevelObject prefab;
	}

	public Resource[] resources;

	public LevelObject GetPrefabWithUniqueID(string uniqueID) {
		foreach (var r in resources) {
			if (r.uniqueID == uniqueID)
				return r.prefab;
		}
		throw new Exception("ResourceDatabase does not contain the following resource : " + uniqueID);
	}
}
