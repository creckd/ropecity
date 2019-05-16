using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicGlassUI : MonoBehaviour {

	public string globalTextureName = "_GlassUI";
	public RenderTexture rt;

	public void Awake() {
		Shader.SetGlobalTexture(globalTextureName, rt);
	}
}
