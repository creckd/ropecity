using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour {

	private static LevelController instance = null;
	public static LevelController Instance {
		get {
			if (instance == null)
				instance = FindObjectOfType<LevelController>();
			return instance;
		}
	}

	public LevelData level;

	private List<LevelObject> initializedLevelObjects = new List<LevelObject>();

	public void InitializeLevel(LevelData level) {

		if (this.level == level) {
			bool thereIsAnObjectMissing = false;
			if (initializedLevelObjects.Count == 0)
				thereIsAnObjectMissing = true;
			foreach (var obj in initializedLevelObjects) {
				if (obj == null)
					thereIsAnObjectMissing = true;
			}
			if (!thereIsAnObjectMissing) {
				Debug.Log("Level already initialized!");
				return;
			}
		}

		this.level = level;

		foreach (var remaining in FindObjectsOfType<LevelObject>()) {
			Destroy(remaining.gameObject);
		}
		initializedLevelObjects.Clear();

		for (int i = 0; i < level.levelObjects.Length; i++) {
			LevelObjectData data = level.levelObjects[i];
			LevelObject prefab = ResourceDatabase.Instance.GetPrefabWithUniqueID(data.uniqueID);
			Vector3 position = new Vector3(data.posX, data.posY, data.posZ);
			Vector3 rotation = new Vector3(data.rotX, data.rotY, data.rotZ);
			LevelObject levelObject = Instantiate(prefab, position, Quaternion.Euler(rotation), transform) as LevelObject;
			levelObject.name = prefab.name + " - " + i.ToString();
			initializedLevelObjects.Add(levelObject);
		}

		Debug.Log("Level initialization successfully completed!");
	}

}
