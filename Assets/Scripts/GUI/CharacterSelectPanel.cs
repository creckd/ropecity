using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterSelectPanel : AnimatorPanel {

	public float platformRotateSpeed = 1f;

	public CharacterRotator rotator;
	public CharacterDragArea dragArea;

	public Button unlockedByLevelButton;
	public Text stageText;
	public Button buyButton;
	public Button selectButton;
	public Text characterName;
	public Image rarityImage;

	private bool characterRotationInProgress = false;
	private bool rotateForward = false;

	public override void Initialize() {
		base.Initialize();
		rotator.OnNewCharacterPadSelected += NewCharacterPadSelected;
		rotator.Initialize();

		dragArea.OnDragging += OnDragging;
		dragArea.DragFinished += DraggingFinished;
	}

	private void Update() {
		ApplyPlatformRotation();
	}

	private void NewCharacterPadSelected(CharacterPad obj) {
		RefreshCharacterDataGUI();
	}

	private void RefreshCharacterDataGUI() {
		CharacterType selectedCharacterType = rotator.GetCurrentlySelectedPad().initializedType;
		CharacterData selectedCharData = ConfigDatabase.Instance.GetCharacterDataWithType(selectedCharacterType);
		GeneralSaveDatabase.CharacterSaveData selectedCharSaveData = SavedDataManager.Instance.GetCharacterSaveDataWithCharacterType(selectedCharacterType);

		unlockedByLevelButton.gameObject.SetActive(false);
		buyButton.gameObject.SetActive(false);
		selectButton.gameObject.SetActive(false);

		characterName.text = selectedCharData.characterName;
		rarityImage.color = ConfigDatabase.Instance.GetRarityColor(selectedCharData.characterRarity);

		if (selectedCharSaveData.owned)
			selectButton.gameObject.SetActive(true);
		else {
			switch (selectedCharData.characterPrice) {
				case PriceType.UnlockedByLevel:
					unlockedByLevelButton.gameObject.SetActive(true);
					stageText.text = string.Format("Complete Stage {0} to unlock!", selectedCharData.unlockedByLevelIndex + 1);
					break;
				case PriceType.IAP:
					buyButton.gameObject.SetActive(true);
					break;
			}
		}
	}

	private void OnDragging(Vector2 xDelta) {
		rotator.beingDragged = true;
		rotator.SetVelocity(-xDelta.x);
	}

	private void DraggingFinished(Vector2 xDelta) {
		rotator.SetVelocity(-xDelta.x);
		rotator.beingDragged = false;
	}

	private void ApplyPlatformRotation() {
		if (characterRotationInProgress) {
			int sign = System.Convert.ToInt32(rotateForward) * 2 - 1;
			rotator.GetCurrentlySelectedPad().RotatePlatform(platformRotateSpeed * sign);
		}
	}

	public void StartRotateCurrentlySelectedCharacter(bool rotateForward) {
		if (!characterRotationInProgress) {
			rotator.GetCurrentlySelectedPad().snapToForwardRotationConstantly = false;
			characterRotationInProgress = true;
			this.rotateForward = rotateForward;
		}
	}

	public void StopPlatformRotation() {
		characterRotationInProgress = false;
		rotateForward = false;
	}

	public void BuyCurrentlySelectedCharacter() {
		CharacterType selectedCharacterType = rotator.GetCurrentlySelectedPad().initializedType;
		GeneralSaveDatabase.CharacterSaveData selectedCharSaveData = SavedDataManager.Instance.GetCharacterSaveDataWithCharacterType(selectedCharacterType);
		selectedCharSaveData.owned = true;
		RefreshCharacterDataGUI();
		SavedDataManager.Instance.Save();
	}

	public void EquipCharacter() {
		SavedDataManager.Instance.GetGeneralSaveDatabase().currentlyEquippedCharacterType = rotator.GetCurrentlySelectedPad().initializedType;
		rotator.RefreshAllPlatformGraphics();
	}
}
