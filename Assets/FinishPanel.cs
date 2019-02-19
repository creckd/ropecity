using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishPanel : Panel {

	public void BackToMainMenu() {
		GameController.Instance.BackToMainMenu();
	}

	public void RestartButton() {
		GameController.Instance.RestartButton();
	}

}
