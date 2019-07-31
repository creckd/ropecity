using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishPanel : FaderPanel {

	public void BackToMainMenu() {
		DeactivateButtons();
		GameController.Instance.BackToMainMenu();
	}

	public void RestartButton() {
		DeactivateButtons();
		GameController.Instance.RestartButton();
	}

}
