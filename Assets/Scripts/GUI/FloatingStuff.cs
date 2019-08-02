using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingStuff : MonoBehaviour {

	public float amplitude = 1f;
	public float speed = 1f;

	Vector3 defaultPosition;
	private float randomSeed = 0f;

	private void Awake() {
		defaultPosition = transform.position;
		randomSeed = Random.Range(0f, 360f);
	}

	private void Update() {
		Vector3 newPosition = transform.position;
		newPosition.y = defaultPosition.y + Mathf.Sin(Mathf.Repeat(Time.realtimeSinceStartup * speed + randomSeed, 360f) * Mathf.Deg2Rad) * amplitude;
		transform.position = newPosition;
	}
}
