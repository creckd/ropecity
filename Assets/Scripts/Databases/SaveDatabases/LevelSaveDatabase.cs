public class LevelSaveDatabase {

	public class LevelSaveData {
		public int levelIndex = -1;
		public bool levelCompleted = false;
	}

	public LevelSaveData[] levelSaveDatas;

	public void CreateEmpty() {
		int numberOfLevels = LevelResourceDatabase.Instance.levelResourceNames.Length;
		levelSaveDatas = new LevelSaveData[numberOfLevels];
		for (int i = 0; i < numberOfLevels; i++) {
			levelSaveDatas[i] = new LevelSaveData();
			levelSaveDatas[i].levelIndex = i;
			levelSaveDatas[i].levelCompleted = false;
		}
	}


}
