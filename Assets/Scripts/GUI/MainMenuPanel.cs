﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuPanel : AnimatorPanel {

	public override void OnStartedOpening() {
		base.OnStartedOpening();

		Time.timeScale = 1f;
	}
}
