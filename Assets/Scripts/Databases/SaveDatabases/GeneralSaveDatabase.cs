using System;

public class GeneralSaveDatabase {

	public class CharacterSaveData {
		public CharacterType characterType;
		public bool owned = false;
	}

	public int lastPlayedLevelIndex = 0;
	public CharacterSaveData[] charactersSaveData;
	public CharacterType currentlyEquippedCharacterType = CharacterType.Worm;

	public void CreateEmpty() {
		lastPlayedLevelIndex = 0;
		charactersSaveData = new CharacterSaveData[ConfigDatabase.Instance.characters.Length];
		for (int i = 0; i < ConfigDatabase.Instance.characters.Length; i++) {
			charactersSaveData[i] = new CharacterSaveData();
			charactersSaveData[i].characterType = ConfigDatabase.Instance.characters[i].characterType;
			charactersSaveData[i].owned = false;
		}
		charactersSaveData[0].owned = true;
	}

}
