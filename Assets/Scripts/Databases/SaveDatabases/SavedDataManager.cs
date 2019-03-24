using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SavedDataManager : MonoBehaviour {

	private const string fileName = "/_save";

	private static SavedDataManager instance = null;
	public static SavedDataManager Instance {
		get {
			if (instance == null)
				instance = FindObjectOfType<SavedDataManager>();
			return instance;
		}
	}

	private SavedData savedData;

	private void Awake() {
		DontDestroyOnLoad(this.gameObject);
	}

	private void Save(SavedData data, string filePath) {
		string saveString = StringSerializationAPI.Serialize(typeof(SavedData),data);
		File.WriteAllText(Application.persistentDataPath + fileName, saveString);
	}

	private SavedData Load(string filePath) {
		string saveString = File.ReadAllText(filePath);
		return StringSerializationAPI.Deserialize(typeof(SavedData),saveString) as SavedData;
	}

	private void CreateEmptySave() {
		savedData = new SavedData();
		savedData.CreateEmpty();
		Save();
	}

	public GeneralSaveDatabase GetGeneralSaveDatabase() {
		return savedData.generalSaveDatabase;
	}

	public LevelSaveDatabase GetLevelSaveDatabase() {
		return savedData.levelSaveDatabase;
	}

	public string GetLevelIDWithLevelIndex(int levelIndex) {
		return LevelResourceDatabase.Instance.levelResourceNames[levelIndex];
	}

	public LevelSaveDatabase.LevelSaveData GetLevelSaveDataWithID(string levelID) {
		for (int i = 0; i < LevelResourceDatabase.Instance.levelResourceNames.Length; i++) {
			if (LevelResourceDatabase.Instance.levelResourceNames[i] == levelID)
				return savedData.levelSaveDatabase.levelSaveDatas[i];
		}
		return null;
		throw new System.Exception("nincs mentés erről a levelID-ról");
	}

	public LevelSaveDatabase.LevelSaveData GetLevelSaveDataWithLevelIndex(int levelIndex) {
		return savedData.levelSaveDatabase.levelSaveDatas[levelIndex];
	}

	public void Load() {
		bool saveExists = File.Exists(Application.persistentDataPath + fileName);

		if (!saveExists) {
			CreateEmptySave();
			return;
		}

		savedData = Load(Application.persistentDataPath + fileName);
	}

	public void Save() {
		Save(savedData, Application.persistentDataPath + fileName);
	}

	public static void DeleteLocalSave() {
		if (File.Exists(Application.persistentDataPath + fileName))
			File.Delete(Application.persistentDataPath + fileName);
	}
}
