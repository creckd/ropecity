using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPanel : AnimatorPanel {
	public CharacterRotator rotator;
	public CharacterDragArea dragArea;

	public Button videoAdButton;
	public Button buyButton;
	public Button selectButton;
	public Text characterName;
	public Image rarityImage;

	public override void Initialize() {
		base.Initialize();
		rotator.OnNewCharacterPadSelected += NewCharacterPadSelected;
		rotator.Initialize();

		dragArea.OnDragging += OnDragging;
		dragArea.DragFinished += DraggingFinished;
	}

	private void NewCharacterPadSelected(CharacterPad obj) {
		RefreshCharacterDataGUI();
	}

	private void RefreshCharacterDataGUI() {
		CharacterType selectedCharacterType = rotator.GetCurrentlySelectedPad().initializedType;
		CharacterData selectedCharData = ConfigDatabase.Instance.GetCharacterDataWithType(selectedCharacterType);
		GeneralSaveDatabase.CharacterSaveData selectedCharSaveData = SavedDataManager.Instance.GetCharacterSaveDataWithCharacterType(selectedCharacterType);

		videoAdButton.gameObject.SetActive(false);
		buyButton.gameObject.SetActive(false);
		selectButton.gameObject.SetActive(false);

		characterName.text = selectedCharData.characterName;
		rarityImage.color = ConfigDatabase.Instance.GetRarityColor(selectedCharData.characterRarity);

		if (selectedCharSaveData.owned)
			selectButton.gameObject.SetActive(true);
		else {
			switch (selectedCharData.characterPrice) {
				case PriceType.VideoAD:
					videoAdButton.gameObject.SetActive(true);
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
}
