using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotate : MonoBehaviour {

	public Vector3 axis;
	public float speed;

	public void Update() {
		transform.Rotate(axis * speed * Time.unscaledDeltaTime);
	}
}
