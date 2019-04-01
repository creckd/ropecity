using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausePanel : FaderPanel {
	public void MenuButton() {
		GameController.Instance.BackToMainMenu();
	}

	public void ResumeButton() {
		GameController.Instance.ResumeGame();
	}
}
