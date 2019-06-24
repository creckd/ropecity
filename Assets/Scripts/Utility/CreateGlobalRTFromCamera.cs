using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateGlobalRTFromCamera : MonoBehaviour {

	public string rtName = "_Texture";
	public int downScale = 0;

	private Camera cam;
	private RenderTexture rt;

	public void Awake() {
		cam = GetComponent<Camera>();
		int width, height;
		width = Screen.width >> downScale;
		height = Screen.height >> downScale;
		rt = new RenderTexture(width, height, 16,RenderTextureFormat.ARGB32);
		cam.targetTexture = rt;
		Shader.SetGlobalTexture(rtName, rt);
	}
}
