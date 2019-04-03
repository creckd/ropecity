using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishPanel : FaderPanel {

	public void BackToMainMenu() {
		DeactivatePanelButtons();
		GameController.Instance.BackToMainMenu();
	}

	public void RestartButton() {
		DeactivatePanelButtons();
		GameController.Instance.RestartButton();
	}

}
