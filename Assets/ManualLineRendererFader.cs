using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualLineRendererFader : MonoBehaviour {

	private LineRenderer lineRenderer;
	private Material materialToFade;

	public string colorPropertyName = "_TintColor";

	private Color defaultColor;

	public float alphaValue = 1f;

	private void Awake() {
		lineRenderer = GetComponent<LineRenderer>();
		materialToFade = lineRenderer.material;// = Instantiate(lineRenderer.material);

		defaultColor = materialToFade.GetColor(colorPropertyName);
	}

	private void Update() {
		materialToFade.SetColor(colorPropertyName, new Color(defaultColor.r,defaultColor.g,defaultColor.b, alphaValue));
	}
}
