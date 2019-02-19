﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelResourceDatabase : MonoBehaviour {

	private static LevelResourceDatabase instance = null;
	public static LevelResourceDatabase Instance {
		get {
			if (instance == null)
				instance = FindObjectOfType<LevelResourceDatabase>();
			return instance;
		}
	}

	private void Awake() {
		if (Instance != this) {
			Destroy(this.gameObject);
			return;
		}
		DontDestroyOnLoad(this.gameObject);
	}

	public string[] levelResourceNames;
}
