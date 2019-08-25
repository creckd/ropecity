using System;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager : IStoreListener {

	public IStoreController controller;
	public IExtensionProvider extensions;

	public Action<Product> GrantPurchase = delegate { };
	public Action<Product,PurchaseFailureReason> PurchaseFailed = delegate { };
	public Action IAPInitialized = delegate { };

	public IAPManager() {
		var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
		builder.AddProduct(IAPHandler.premium_edition_characterselect_product_id, ProductType.NonConsumable);
		builder.AddProduct(IAPHandler.premium_edition_general_product_id, ProductType.NonConsumable);
		//builder.AddProduct("100_gold_coins", ProductType.Consumable, new IDs
		//{
		//	{"100_gold_coins_google", GooglePlay.Name},
		//	{"100_gold_coins_mac", MacAppStore.Name}
		//});

		UnityPurchasing.Initialize(this, builder);
	}

	/// <summary>
	/// Called when Unity IAP is ready to make purchases.
	/// </summary>
	public void OnInitialized(IStoreController controller, IExtensionProvider extensions) {
		this.controller = controller;
		this.extensions = extensions;
		IAPInitialized();
	}

	/// <summary>
	/// Called when Unity IAP encounters an unrecoverable initialization error.
	///
	/// Note that this will not be called if Internet is unavailable; Unity IAP
	/// will attempt initialization until it becomes available.
	/// </summary>
	public void OnInitializeFailed(InitializationFailureReason error) {
	}

	/// <summary>
	/// Called when a purchase completes.
	///
	/// May be called at any time after OnInitialized().
	/// </summary>
	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e) {
		GrantPurchase(e.purchasedProduct);
		return PurchaseProcessingResult.Complete;
	}

	/// <summary>
	/// Called when a purchase fails.
	/// </summary>
	public void OnPurchaseFailed(Product i, PurchaseFailureReason p) {
		PurchaseFailed(i, p);
	}
}