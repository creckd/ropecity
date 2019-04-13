using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotate : MonoBehaviour {

	public bool randomizedStartRotation = false;
	public Vector3 axis;
	public float speed;

	private void Start() {
		if (randomizedStartRotation)
			transform.Rotate(new Vector3(axis.x * Random.Range(0f, 360f), axis.y * Random.Range(0f, 360f), axis.z * Random.Range(0f, 360f)));
	}

	public void Update() {
		transform.Rotate(axis * speed * Time.unscaledDeltaTime);
	}
}
