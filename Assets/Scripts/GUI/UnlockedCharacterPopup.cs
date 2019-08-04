using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnlockedCharacterPopup : AnimatorPanel {

	public const string CharacterTypeMessageID = "Character_Type";
	public Text messageText;
	public Text characterName;
	public PlatformCharacter[] characterObjects;

	public override void Initialize() {
		base.Initialize();
		for (int i = 0; i < characterObjects.Length; i++) {
			characterObjects[i].gameObject.SetActive(false);
		}
	}

	public override void OnStartedOpening(Dictionary<object, object> message) {
		base.OnStartedOpening(message);
		CharacterType typeToInitialize = (CharacterType)message[CharacterTypeMessageID];
		CharacterData data = ConfigDatabase.Instance.GetCharacterDataWithType(typeToInitialize);

		GetCharacterObject(typeToInitialize).gameObject.SetActive(true);

		Color rarityColor = ConfigDatabase.Instance.GetRarityColor(data.characterRarity);
		characterName.text = data.characterName;
		string rarityHex = ColorUtility.ToHtmlStringRGB(rarityColor);
		messageText.text = "The character <color=#"+rarityHex+">"+ data.characterName +"</color> is now available for you!";
	}

	private PlatformCharacter GetCharacterObject(CharacterType cType) {
		for (int i = 0; i < characterObjects.Length; i++) {
			if (characterObjects[i].characterType == cType)
				return characterObjects[i];
		}
		return null;
	}

	public void EquipButton() {
		PopupManager.Instance.ClosePopup(this,() => {
			PanelManager.Instance.TryOpenPanel(3);
		});
	}
}
