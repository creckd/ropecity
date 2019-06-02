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
	public Camera maskCamera;

	public Material vertical;
	public Material horizontal;
	public int downScale = 0;
	public int iterations = 1;

	public bool disableObjectAfterSnapShot = false;
	public SimpleImageEffectApplier imgEffect;

	public void CreateStaticBlurImage() {
		if (imgEffect != null)
			imgEffect.enabled = false;
		attachedCamera = GetComponent<Camera>();
		int width = attachedCamera.pixelWidth >> downScale;
		int height = attachedCamera.pixelHeight >> downScale;
		RenderTexture rt = RenderTexture.GetTemporary(width,height,16,RenderTextureFormat.ARGB32);

		if (BackgroundCamera.Instance != null) {
			BackgroundCamera.Instance.thisCamera.targetTexture = rt;
			BackgroundCamera.Instance.thisCamera.Render();
			BackgroundCamera.Instance.thisCamera.targetTexture = null;
		}

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
		if (disableObjectAfterSnapShot)
			gameObject.SetActive(false);
		if (imgEffect != null)
			imgEffect.enabled = true;
	}

	public void EnableMaskCamera() {
		maskCamera.gameObject.SetActive(true);
	}

	public void DisableMaskCamera() {
		maskCamera.gameObject.SetActive(false);
	}

}
