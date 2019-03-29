public class LevelSaveDatabase {

	public class LevelSaveData {
		public int levelIndex = 0;
		public bool levelCompleted = false;
		public bool isUnlocked = false;
		public int numberOfTries = 0;
	}

	public LevelSaveData[] levelSaveDatas;

	public void CreateEmpty() {
		int numberOfLevels = LevelResourceDatabase.Instance.levelResourceNames.Length;
		levelSaveDatas = new LevelSaveData[numberOfLevels];
		for (int i = 0; i < numberOfLevels; i++) {
			levelSaveDatas[i] = new LevelSaveData();
			levelSaveDatas[i].levelIndex = i;
			levelSaveDatas[i].levelCompleted = false;
			levelSaveDatas[i].isUnlocked = false;
			levelSaveDatas[i].numberOfTries = 0;
		}
		levelSaveDatas[0].isUnlocked = true; //First level open
	}


}
