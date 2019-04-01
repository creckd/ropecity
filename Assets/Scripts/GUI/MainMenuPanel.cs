using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuPanel : FaderPanel {

	public override void OnStartedOpening() {
		base.OnStartedOpening();

		Time.timeScale = 1f;
	}
}
