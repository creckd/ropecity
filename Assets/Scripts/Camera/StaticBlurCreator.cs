using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticBlurCreator : MonoBehaviour {

	private static StaticBlurCreator instance = null;
	public static StaticBlurCreator Instance {
		get {
			if (instance == null)
				instance = FindObjectOfType<StaticBlurCreator>();
			return instance;
		}
	}

	public static string renderTextureGlobalPropName = "_StaticBlurImage";

	private Camera attachedCamera;

	public Material vertical;
	public Material horizontal;
	public int downScale = 0;
	public int iterations = 1;


	public void CreateStaticBlurImage() {
		attachedCamera = GetComponent<Camera>();
		int width = attachedCamera.pixelWidth >> downScale;
		int height = attachedCamera.pixelHeight >> downScale;
		RenderTexture rt = RenderTexture.GetTemporary(width, height);

		BackgroundCamera.Instance.thisCamera.targetTexture = rt;
		BackgroundCamera.Instance.thisCamera.Render();
		BackgroundCamera.Instance.thisCamera.targetTexture = null;

		attachedCamera.targetTexture = rt;
		attachedCamera.Render();
		attachedCamera.targetTexture = null;

		for (int i = 0; i < iterations; i++) {
			RenderTexture rt2 = RenderTexture.GetTemporary(width, height);
			Graphics.Blit(rt, rt2, vertical);
			RenderTexture.ReleaseTemporary(rt);
			rt = rt2;

			rt2 = RenderTexture.GetTemporary(width, height);
			Graphics.Blit(rt, rt2, horizontal);
			RenderTexture.ReleaseTemporary(rt);
			rt = rt2;
		}

		Shader.SetGlobalTexture(renderTextureGlobalPropName, rt);
		//RenderTexture.ReleaseTemporary(rt);
	}
}
