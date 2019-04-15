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
	[HideInInspector]
	public int currentLevelIndex = -1;

	public LevelSettings settings;

	[System.Serializable]
	public class LevelSettings {
		public bool isVerticalCameraMovementEnabled = false;
		public bool isHorizontalCameraMovementEnabled = true;

		public string GetSerialized() {
			this.isVerticalCameraMovementEnabled = !CameraController.Instance.verticalMovementLocked;
			this.isHorizontalCameraMovementEnabled = !CameraController.Instance.horizontalMovementLocked;
			return StringSerializationAPI.Serialize(typeof(LevelSettings), this);
		}
	}

	public List<LevelObject> initializedLevelObjects = new List<LevelObject>();

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
		DeserializeAndSetLevelSettings(level.settings);

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
			levelObject.DeserializeObjectData(data.componentData);
			levelObject.name = prefab.name + " - " + i.ToString();
			initializedLevelObjects.Add(levelObject);
		}

		Debug.Log("Level initialization successfully completed!");
	}

	public void DeserializeAndSetLevelSettings(string settingsData) {
		if (settingsData != null)
			settings = StringSerializationAPI.Deserialize(typeof(LevelSettings), settingsData) as LevelSettings;
		else settings = new LevelSettings();
	}
}
