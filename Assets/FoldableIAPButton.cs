using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoldableIAPButton : MonoBehaviour {

	private static FoldableIAPButton instance = null;
	public static FoldableIAPButton Instance {
		get {
			if (instance == null)
				instance = FindObjectOfType<FoldableIAPButton>();
			return instance;
		}
	}

	public Animator anim;
	public Text priceText;
	private bool currentlyFolded = true;

	public GameObject iapButtonObject;
	public Image purchasedStateObject;
	public Image bandImage;
	public Button buyButton;
	public Material greyScaleMaterial;

	private void Start() {
		//bool purchased = SavedDataManager.Instance.GetGeneralSaveDatabase().noAdMode;
		//if (!purchased) {
		//	buyButton.interactable = false;
		//	if (!IAPHandler.Instance.iapManager.initialized)
		//		IAPHandler.Instance.iapManager.IAPInitialized += FetchPrice;
		//	else FetchPrice();
		//}
		//RefreshGraphics();
	}

	private void FetchPrice() {
		//UnityEngine.Purchasing.Product p = IAPHandler.Instance.iapManager.controller.products.WithStoreSpecificID(IAPHandler.premium_edition_general_product_id);
		//priceText.text = p.metadata.localizedPriceString;
		//buyButton.interactable = true;
		//RefreshGraphics();
	}

	private void OnEnable() {
		currentlyFolded = true;
	}

	public void RefreshGraphics() {
		bool isPurchased = SavedDataManager.Instance.GetGeneralSaveDatabase().noAdMode;
		iapButtonObject.gameObject.SetActive(!isPurchased);
		purchasedStateObject.gameObject.SetActive(isPurchased);
		bandImage.material = buyButton.interactable ? null : greyScaleMaterial;
	}

	public void ChangeState() {
		currentlyFolded = !currentlyFolded;
		anim.SetBool("Folded", currentlyFolded);

		AnalyticsManager.LogEvent("TappedIAPUnfold");
	}

	public void BuyNoAds() {
		//IAPHandler.Instance.BuyPremiumEditionFromGeneral((bool s) => {
		//	if (s) {
		//		if (PanelManager.Instance.currentlyOpenedPanel != null) {
		//			CharacterSelectPanel characterSelectPanel = PanelManager.Instance.currentlyOpenedPanel.gameObject.GetComponent<CharacterSelectPanel>();
		//			if (characterSelectPanel != null) {
		//				characterSelectPanel.RefreshGUIAndCharacters();
		//			}
		//		}
		//		RefreshGraphics();
		//		//InvictusMoreGames.MoreGamesBoxController.Instance.Hide();
		//	}
		//});
	}
}
