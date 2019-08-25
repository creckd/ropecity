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

	private void Start() {
		RefreshState();
		IAPHandler.Instance.iapManager.IAPInitialized += RefreshPrice;
	}

	private void RefreshPrice() {
		UnityEngine.Purchasing.Product p = IAPHandler.Instance.iapManager.controller.products.WithStoreSpecificID(IAPHandler.premium_edition_general_product_id);
		priceText.text = p.metadata.localizedPriceString;
	}

	private void OnEnable() {
		currentlyFolded = true;
	}

	public void RefreshState() {
		if (SavedDataManager.Instance.GetGeneralSaveDatabase().noAdMode)
			gameObject.SetActive(false);
	}

	public void ChangeState() {
		currentlyFolded = !currentlyFolded;
		anim.SetBool("Folded", currentlyFolded);
	}

	public void BuyNoAds() {
		IAPHandler.Instance.BuyPremiumEditionFromGeneral((bool s) => {
			if (s) {
				gameObject.SetActive(false);
				if (PanelManager.Instance.currentlyOpenedPanel != null) {
					CharacterSelectPanel characterSelectPanel = PanelManager.Instance.currentlyOpenedPanel.gameObject.GetComponent<CharacterSelectPanel>();
					if (characterSelectPanel != null) {
						characterSelectPanel.RefreshGUIAndCharacters();
					}
				}
			}
		});
	}
}
