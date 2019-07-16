using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPad : MonoBehaviour {

	[HideInInspector]
	public CharacterType initializedType;
	public PlatformCharacter[] platformCharacterObjects;

	private PlatformCharacter initializedPlatformCharacter = null;

	public void InitalizeCharacter(CharacterType characterType) {
		initializedType = characterType;

		foreach (var platformCharacterObject in platformCharacterObjects) {
			platformCharacterObject.gameObject.SetActive(false);
		}

		initializedPlatformCharacter = GetPlatformCharacterByType(characterType);
		initializedPlatformCharacter.gameObject.SetActive(true);
	}

	private PlatformCharacter GetPlatformCharacterByType(CharacterType type) {
		PlatformCharacter character = null;
		foreach (var platformCharacter in platformCharacterObjects) {
			if (platformCharacter.characterType == type) {
				character = platformCharacter;
				break;
			}
		}
		if (character == null)
			throw new System.Exception("Character platform does not have the following character type prepared : " + type.ToString());
		return character;
	}
}
