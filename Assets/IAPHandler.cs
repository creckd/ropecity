﻿using System;
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
		iapManager.controller.InitiatePurchase(premium_edition_characterselect_product_id);
		lastCallBack += purchaseFinished;
	}

	public void PurchaseFailed(Product p, PurchaseFailureReason failureReason) {
		lastCallBack(false);
		lastCallBack = delegate { };
	}

	public void GrantPurchase(Product p) {
		switch (p.ToString()) {
			case premium_edition_characterselect_product_id: {
				SavedDataManager.Instance.GetGeneralSaveDatabase().noAdMode = true;
				SavedDataManager.Instance.GetCharacterSaveDataWithCharacterType(CharacterType.Froggy).owned = true;
				SavedDataManager.Instance.Save();
			}
			break;
		}
		lastCallBack(true);
		lastCallBack = delegate { };
	}

}