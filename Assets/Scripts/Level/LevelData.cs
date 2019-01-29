public class LevelData {

	public string levelName;
	public LevelObjectData[] levelObjects;

	public static bool operator ==(LevelData a, LevelData b) {
		bool theyAreEqual = true;
		if (a.levelName != b.levelName)
			theyAreEqual = false;
		if (a.levelObjects.Length != b.levelObjects.Length)
			theyAreEqual = false;

		return theyAreEqual;
	}

	public static bool operator !=(LevelData a, LevelData b) {
		bool theyAreEqual = true;
		if (a.levelName != b.levelName)
			theyAreEqual = false;
		if (a.levelObjects.Length != b.levelObjects.Length)
			theyAreEqual = false;

		return !theyAreEqual;
	}
}
