using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPad : MonoBehaviour {

	[HideInInspector]
	public CharacterType initializedType;

	public PlatformCharacter[] platformCharacterObjects;
	public PlatformCharacter[] lockedPlatformCharacterObjects;

	public ParticleSystem hologramParticle;

	[HideInInspector]
	public bool snapToForwardRotationConstantly = false;

	public MeshRenderer selectionEffect;
	public Material regularSlectionMat;
	public Material goldenSelectionMat;

	private PlatformCharacter initializedPlatformCharacter = null;

	public void InitalizeCharacter(CharacterType characterType) {
		initializedType = characterType;

		foreach (var platformCharacterObject in platformCharacterObjects) {
			platformCharacterObject.gameObject.SetActive(false);
		}

		initializedPlatformCharacter = GetPlatformCharacterByType(characterType);
		initializedPlatformCharacter.gameObject.SetActive(true);

		RefreshGraphics();
	}

	private void Update() {
		if (snapToForwardRotationConstantly) {
			Quaternion currentRotation = transform.rotation;
			Quaternion targetRotation = Quaternion.identity;

			Vector3 directionToLook = transform.position + (transform.parent.position - transform.position).normalized;

			transform.LookAt(directionToLook, Vector3.up);

			targetRotation = transform.rotation;
			transform.rotation = currentRotation;

			transform.rotation = Quaternion.Lerp(currentRotation, targetRotation, Time.deltaTime * 5f);
		}
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

	private PlatformCharacter GetLockedPlatformCharacterByType(CharacterType type) {
		PlatformCharacter character = null;
		foreach (var platformCharacter in lockedPlatformCharacterObjects) {
			if (platformCharacter.characterType == type) {
				character = platformCharacter;
				break;
			}
		}
		if (character == null)
			throw new System.Exception("Character platform does not have the following character type prepared : " + type.ToString());
		return character;
	}

	public void RotatePlatform(float amount) {
		transform.Rotate(new Vector3(0f, amount * Time.deltaTime, 0f));
	}

	public void Defocused() {
		snapToForwardRotationConstantly = true;
	}

	public void RefreshGraphics() {
		GeneralSaveDatabase.CharacterSaveData sData = SavedDataManager.Instance.GetCharacterSaveDataWithCharacterType(initializedType);
		CharacterData cData = ConfigDatabase.Instance.GetCharacterDataWithType(sData.characterType);
		GetPlatformCharacterByType(initializedType).gameObject.SetActive(sData.owned);
		GetLockedPlatformCharacterByType(initializedType).gameObject.SetActive(!sData.owned);
		hologramParticle.gameObject.SetActive(!sData.owned);
		selectionEffect.material = cData.characterPrice == PriceType.IAP && !sData.owned ? goldenSelectionMat : regularSlectionMat;
		//bool chosenOne = initializedType == SavedDataManager.Instance.GetGeneralSaveDatabase().currentlyEquippedCharacterType;
		//selectionEffect.gameObject.SetActive(chosenOne);
	}
}
