﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialTiler : MonoBehaviour {

	private Material mat;

	public float speed = 1f;

	private float timer;
	private LineRenderer lineRenderer;

	private void Awake() {
		lineRenderer = GetComponent<LineRenderer>();
		mat = lineRenderer.material;
	}

	private void Update() {
		timer = Mathf.Repeat(timer + Time.unscaledDeltaTime * speed, 1f);
		mat.SetTextureOffset("_MainTex", new Vector2(timer, 0f));
	}
}