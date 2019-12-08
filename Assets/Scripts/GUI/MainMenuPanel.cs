using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InvictusMoreGames;

public class MainMenuPanel : AnimatorPanel {

	public override void Initialize() {
		base.Initialize();
		StaticBlurCreator.Instance.CreateStaticBlurImage();
	}

	public override void OnStartedOpening() {
		base.OnStartedOpening();
		if (!SavedDataManager.Instance.GetGeneralSaveDatabase().noAdMode) {
			MoreGamesBoxController.Instance.ShowWithAnimation();
			MoreGamesBoxController.Instance.ShowNewGame();
		}

		Time.timeScale = 1f;

		SoundManager.Instance.CreateOneShot(AudioConfigDatabase.Instance.mainMenuOpening);
	}

	public override void OnStartedClosing() {
		base.OnStartedClosing();
		MoreGamesBoxController.Instance.HideWithAnimation();
	}
}
