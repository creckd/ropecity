using System;
using System.Collections;
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
		DontDestroyOnLoad(this.gameObject);
	}

	public Section[] sections;

	public string GetResourceWithLevelIndex(int levelIndex) {
		int s = 0;
		for (int i = 0; i < sections.Length; i++) {
			s += sections[i].levelResourceNames.Length;
			if (s > levelIndex) {
				s -= sections[i].levelResourceNames.Length;
				return sections[i].levelResourceNames[levelIndex-s];
			}
		}
		throw new Exception("Túl magas levelindex, nincs ilyen felvéve a Level Resource Database-ben");
		return "";
	}
}
