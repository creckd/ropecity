public class LevelData {

	public string levelName;
	public LevelObjectData[] levelObjects;
	public string settings;

	public static bool operator ==(LevelData a, LevelData b) {
		bool theyAreEqual = true;
		if (ReferenceEquals(a,null) && ReferenceEquals(b,null))
			return true;
		if ((ReferenceEquals(a,null) && !ReferenceEquals(b,null)) || (ReferenceEquals(a,null) && !ReferenceEquals(b,null)))
			return false;
		if (a.levelName != b.levelName)
			theyAreEqual = false;
		if (a.levelObjects.Length != b.levelObjects.Length)
			theyAreEqual = false;

		return theyAreEqual;
	}

	public static bool operator !=(LevelData a, LevelData b) {
		return !(a == b);
	}
}
