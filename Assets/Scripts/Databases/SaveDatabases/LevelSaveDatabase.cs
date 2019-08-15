public class LevelSaveDatabase {

	public class LevelSaveData {
		public int levelIndex = 0;
		public bool levelCompleted = false;
		public bool isUnlocked = false;
		public int numberOfTries = 0;
	}

	public LevelSaveData[] levelSaveDatas;

	public void CreateEmpty() {
		int numberOfAllLevels = 0;
		foreach (var s in LevelResourceDatabase.Instance.sections) {
			numberOfAllLevels += s.levelResourceNames.Length;
		}
		levelSaveDatas = new LevelSaveData[numberOfAllLevels];
		int globalIndex = 0;
		for (int i = 0; i < LevelResourceDatabase.Instance.sections.Length; i++) {
			int numberOfLevelsInThisSection = LevelResourceDatabase.Instance.sections[i].levelResourceNames.Length;
			for (int j = 0; j < numberOfLevelsInThisSection; j++) {
				levelSaveDatas[globalIndex + j] = new LevelSaveData();
				levelSaveDatas[globalIndex + j].levelIndex = globalIndex + j;
				levelSaveDatas[globalIndex + j].levelCompleted = false;
				levelSaveDatas[globalIndex + j].isUnlocked = false;
				levelSaveDatas[globalIndex + j].numberOfTries = 0;
			}
			globalIndex += numberOfLevelsInThisSection;
		}
		levelSaveDatas[0].isUnlocked = true; //First level open
	}

	public int GetLastCompletedLevelIndex() {
		int lastCompletedLevel = -1;
		for (int i = 0; i < levelSaveDatas.Length; i++) {
			if (!levelSaveDatas[i].levelCompleted) {
				lastCompletedLevel = i - 1;
				break;
			}
		}
		return lastCompletedLevel;
	}

}
