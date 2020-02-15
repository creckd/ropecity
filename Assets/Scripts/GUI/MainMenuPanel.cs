using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using InvictusMoreGames;
using System;

public class MainMenuPanel : AnimatorPanel {

	public override void Initialize() {
		base.Initialize();
		StaticBlurCreator.Instance.CreateStaticBlurImage();
	}

	public override void OnStartedOpening() {
		base.OnStartedOpening();
		//if (!SavedDataManager.Instance.GetGeneralSaveDatabase().noAdMode) {
		//	if (MoreGamesBoxController.Instance.JsonReadSuccess) {
		//		MoreGamesBoxController.Instance.ShowWithAnimation();
		//		MoreGamesBoxController.Instance.ShowNewGame();
		//	} else {
		//		MoreGamesBoxController.Instance.onJsonReadSuccess += MoreGamesJSonRead;
		//	}
		//}

		Time.timeScale = 1f;

		SoundManager.Instance.CreateOneShot(AudioConfigDatabase.Instance.mainMenuOpening);
	}

	//private void MoreGamesJSonRead(bool read) {
	//	if (read && !MoreGamesBoxController.Instance.IsActive) {
	//		MoreGamesBoxController.Instance.ShowWithAnimation();
	//		MoreGamesBoxController.Instance.ShowNewGame();
	//	}
	//}

	public override void OnStartedClosing() {
		base.OnStartedClosing();
		//MoreGamesBoxController.Instance.HideWithAnimation();
		//MoreGamesBoxController.Instance.onJsonReadSuccess -= MoreGamesJSonRead;
	}
}
