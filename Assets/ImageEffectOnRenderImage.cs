using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageEffectOnRenderImage : MonoBehaviour {

	public Material mat;
	public Material mat2;
	public int downScale = 0;
	public int iterations = 1;

	private void OnRenderImage(RenderTexture source, RenderTexture destination) {
		int width = source.width >> downScale;
		int height = source.height >> downScale;

		RenderTexture rt = RenderTexture.GetTemporary(width, height);
		Graphics.Blit(source,rt);

		for (int i = 0; i < iterations; i++) {
			RenderTexture rt2 = RenderTexture.GetTemporary(width, height);
			Graphics.Blit(rt, rt2, mat);
			RenderTexture.ReleaseTemporary(rt);
			rt = rt2;

			rt2 = RenderTexture.GetTemporary(width, height);
			Graphics.Blit(rt, rt2, mat2);
			RenderTexture.ReleaseTemporary(rt);
			rt = rt2;
		}

		Graphics.Blit(rt, destination);
		RenderTexture.ReleaseTemporary(rt);
	}
}
