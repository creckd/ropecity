using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPHandler : MonoBehaviour {

	public const string premium_edition_characterselect_product_id = "premium_edition";
	public const string premium_edition_general_product_id = "premium_edition_general";

	private static IAPHandler instance = null;
	public static IAPHandler Instance {
		get {
			if (instance == null)
				instance = FindObjectOfType<IAPHandler>();
			return instance;
		}
	}

	public IAPManager iapManager;

	private void Awake() {
		DontDestroyOnLoad(this.gameObject);
		iapManager = new IAPManager();
		iapManager.GrantPurchase += GrantPurchase;
		iapManager.PurchaseFailed += PurchaseFailed;
	}

	private Action<bool> lastCallBack = delegate { };

	public void BuyPremiumEditionFromCharacterScreen(Action<bool> purchaseFinished) {
		Blocker.Instance.Block();
		lastCallBack += purchaseFinished;
		iapManager.controller.InitiatePurchase(premium_edition_characterselect_product_id);
	}

	public void BuyPremiumEditionFromGeneral(Action<bool> purchaseFinished) {
		Blocker.Instance.Block();
		lastCallBack += purchaseFinished;
		iapManager.controller.InitiatePurchase(premium_edition_general_product_id);
	}

	public void PurchaseFailed(Product p, PurchaseFailureReason failureReason) {
		Blocker.Instance.UnBlock();
		lastCallBack(false);
		lastCallBack = delegate { };
	}

	public void GrantPurchase(Product p) {
		switch (p.definition.id) {
			case premium_edition_characterselect_product_id: {
				SavedDataManager.Instance.GetGeneralSaveDatabase().noAdMode = true;
				SavedDataManager.Instance.GetCharacterSaveDataWithCharacterType(CharacterType.Froggy).owned = true;
				SavedDataManager.Instance.Save();
			}
			break;
			case premium_edition_general_product_id: {
					SavedDataManager.Instance.GetGeneralSaveDatabase().noAdMode = true;
					SavedDataManager.Instance.GetCharacterSaveDataWithCharacterType(CharacterType.Froggy).owned = true;
					SavedDataManager.Instance.Save();
			}
			break;
		}
		Blocker.Instance.UnBlock();
		lastCallBack(true);
		lastCallBack = delegate { };
	}
}