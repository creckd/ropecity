public class SavedData {
	public LevelSaveDatabase levelSaveDatabase;
	public GeneralSaveDatabase generalSaveDatabase;

	public void CreateEmpty() {
		levelSaveDatabase = new LevelSaveDatabase();
		generalSaveDatabase = new GeneralSaveDatabase();

		levelSaveDatabase.CreateEmpty();
		generalSaveDatabase.CreateEmpty();
	}
}