using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRotatorGUI : MonoBehaviour {

	public RenderTexture rt;

	private void Start() {
		rt.width = Screen.width;
		rt.height = Screen.height;
	}
}
