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
	public Text iapPriceText;
	public Button selectButton;
	public Button rotateForwardButton;
	public Button rotateBackwardButton;
	public Text characterName;
	public Image rarityImage;

	public Image selectButtonTargetGraphicImage;
	public Sprite defaultSelectButtonSprite;
	public Sprite equippedSelectButtonSprite;

	public Material greyScaleMaterial;

	private bool characterRotationInProgress = false;
	private bool rotateForward = false;

	public override void Initialize() {
		base.Initialize();
		rotator.OnNewCharacterPadSelected += NewCharacterPadSelected;
		rotator.Initialize();

		dragArea.OnDragging += OnDragging;
		dragArea.DragFinished += DraggingFinished;
	}

	public void RefreshGUIAndCharacters() {
		rotator.RefreshAllPlatformGraphics();
		RefreshCharacterDataGUI();
	}

	public override void OnStartedOpening() {
		base.OnStartedOpening();
		rotator.SnapToCharacterType(SavedDataManager.Instance.GetGeneralSaveDatabase().currentlyEquippedCharacterType);
		RefreshGUIAndCharacters();
		SoundManager.Instance.CreateOneShot(AudioConfigDatabase.Instance.characterSelectPanelOpenSoundEffect);

		AnalyticsManager.LogEvent("OpenedCharacterSelectPanel");
	}

	public override void OnStartedOpening(Dictionary<object, object> message) {
		base.OnStartedOpening(message);
		CharacterType characterTypeToOpen = (CharacterType)message[UnlockedCharacterPopup.CharacterTypeMessageID];
		rotator.SnapToCharacterType(characterTypeToOpen);
		RefreshGUIAndCharacters();
		SoundManager.Instance.CreateOneShot(AudioConfigDatabase.Instance.characterSelectPanelOpenSoundEffect);


		AnalyticsManager.LogEvent("OpenedCharacterSelectPanel");
	}

	public override void OnStartedClosing() {
		base.OnStartedClosing();
		SoundManager.Instance.CreateOneShot(AudioConfigDatabase.Instance.characterSelectPanelOpenSoundEffect);
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

		rotateForwardButton.gameObject.SetActive(true);
		rotateBackwardButton.gameObject.SetActive(true);

		characterName.text = selectedCharData.characterName;
		rarityImage.color = ConfigDatabase.Instance.GetRarityColor(selectedCharData.characterRarity);

		bool chosenOne = rotator.GetCurrentlySelectedPad().initializedType == SavedDataManager.Instance.GetGeneralSaveDatabase().currentlyEquippedCharacterType;
		selectButtonTargetGraphicImage.sprite = chosenOne ? equippedSelectButtonSprite : defaultSelectButtonSprite;
		//selectButton.interactable = !chosenOne;

		if (selectedCharSaveData.owned) {
			selectButton.gameObject.SetActive(true);
		} else {
			switch (selectedCharData.characterPrice) {
				case PriceType.UnlockedByLevel:
					unlockedByLevelButton.gameObject.SetActive(true);
					stageText.text = string.Format(SmartLocalization.LanguageManager.Instance.GetTextValue("CharacterSleect.StageUnlock"), selectedCharData.unlockedByLevelIndex + 1);
					break;
				case PriceType.IAP:
					buyButton.gameObject.SetActive(true);
					if (IAPHandler.Instance.iapManager.initialized) {
						UnityEngine.Purchasing.Product p = IAPHandler.Instance.iapManager.controller.products.WithStoreSpecificID(IAPHandler.premium_edition_characterselect_product_id);
						iapPriceText.text = p.metadata.localizedPriceString;
						buyButton.image.material = null;
						buyButton.interactable = true;
					} else {
						buyButton.image.material = greyScaleMaterial;
						buyButton.interactable = false;
					}
					rotateForwardButton.gameObject.SetActive(false);
					rotateBackwardButton.gameObject.SetActive(false);
					break;
			}
		}
	}

	private float timeStartedDragging;
	private Vector2 posAtStart;

	private void OnDragging(Vector2 xDelta) {
		if (!rotator.beingDragged) {
			rotator.beingDragged = true;

			timeStartedDragging = Time.realtimeSinceStartup;
			posAtStart = Input.mousePosition;
		}
		rotator.Rotate(-xDelta.x);
	}

	private void DraggingFinished(Vector2 xDelta) {
		rotator.beingDragged = false;
		float dragDuration = Time.realtimeSinceStartup - timeStartedDragging;
		float dragLength = Vector2.Distance(Input.mousePosition, posAtStart);
		float sign = Mathf.Sign(Input.mousePosition.x - posAtStart.x);
		float referenceWidth = 2628f;
		float screenWidth = Screen.width;
		rotator.SetVelocity((dragLength * 0.1f) * (1f/dragDuration) * -sign * (referenceWidth / screenWidth));
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
		//AdvertManager.Instance.ShowInterstitial();
		IAPHandler.Instance.BuyPremiumEditionFromCharacterScreen((bool s) => {
		RefreshGUIAndCharacters();
		FoldableIAPButton.Instance.RefreshGraphics();
		});
	}

	public void EquipCharacter() {
		SavedDataManager.Instance.GetGeneralSaveDatabase().currentlyEquippedCharacterType = rotator.GetCurrentlySelectedPad().initializedType;
		RefreshGUIAndCharacters();
		OpenPanel(0);
	}
}
