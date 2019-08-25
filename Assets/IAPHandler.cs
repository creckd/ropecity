using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPHandler : MonoBehaviour {

	public const string premium_edition_characterselect_product_id = "premium_edition";

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
		Blocker.Instane.Block();
		lastCallBack += purchaseFinished;
		iapManager.controller.InitiatePurchase(premium_edition_characterselect_product_id);
	}

	public void PurchaseFailed(Product p, PurchaseFailureReason failureReason) {
		Blocker.Instane.UnBlock();
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
		}
		Blocker.Instane.UnBlock();
		lastCallBack(true);
		lastCallBack = delegate { };
	}
}