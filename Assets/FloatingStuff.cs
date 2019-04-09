using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingStuff : MonoBehaviour {

	public float amplitude = 1f;
	public float speed = 1f;

	Vector3 defaultPosition;


	private void Awake() {
		defaultPosition = transform.position;
	}

	private void Update() {
		transform.position = defaultPosition + Vector3.up * Mathf.Sin(Mathf.Repeat(Time.realtimeSinceStartup * speed, 360f) * Mathf.Deg2Rad) * amplitude;
	}
}
