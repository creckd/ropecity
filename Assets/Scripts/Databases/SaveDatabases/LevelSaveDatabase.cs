﻿public class LevelSaveDatabase {

	public class LevelSaveData {
		public string levelID = "invalid";
		public bool levelCompleted = false;
		public bool isUnlocked = false;
	}

	public LevelSaveData[] levelSaveDatas;

	public void CreateEmpty() {
		int numberOfLevels = LevelResourceDatabase.Instance.levelResourceNames.Length;
		levelSaveDatas = new LevelSaveData[numberOfLevels];
		for (int i = 0; i < numberOfLevels; i++) {
			levelSaveDatas[i] = new LevelSaveData();
			levelSaveDatas[i].levelID = LevelResourceDatabase.Instance.levelResourceNames[i];
			levelSaveDatas[i].levelCompleted = false;
			levelSaveDatas[i].isUnlocked = false;
		}
		levelSaveDatas[0].isUnlocked = true; //First level open
	}


}