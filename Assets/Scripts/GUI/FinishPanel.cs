using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishPanel : FaderPanel {

	public override void OnOpened() {
#if !UNITY_EDITOR
		Blocker.Instance.Block();
		AdvertManager.Instance.ShowInterstitial(() => { Blocker.Instance.UnBlock(); });
#endif
		base.OnOpened();
	}

	public void BackToMainMenu() {
		DeactivateButtons();
		GameController.Instance.BackToMainMenu();
	}

	public void RestartButton() {
		DeactivateButtons();
		GameController.Instance.RestartButton();
	}

}
