using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngamePanel : Panel {

	public void BackToMainMenu() {
		UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
	}

}
