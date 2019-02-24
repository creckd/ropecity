using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileRopeAiderLine : MonoBehaviour {

	public float speed = 1f;

	private Material mat;
	private float value = 0f;

	private void Awake() {
		mat = GetComponent<LineRenderer>().material;
	}

	private void Update() {
		value = Mathf.Repeat(value + (Time.unscaledDeltaTime * speed), 1f);
		mat.SetTextureOffset("_MainTex", new Vector2(value, 0f));
	}
}
