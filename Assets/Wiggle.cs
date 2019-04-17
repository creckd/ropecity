using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wiggle : MonoBehaviour {

	public float rotationAmount = 5f;
	public float rotationAmountRandomness = 0f;
	public float rotationSpeed = 5f;
	public float rotationSpeedRandomness = 0f;

	private Quaternion fromQuaternion;
	private Quaternion toQuaternion;

	private float randomStart;

	private void Awake() {
		Vector3 euler = transform.rotation.eulerAngles;
		fromQuaternion = Quaternion.Euler(euler.x, euler.y, euler.z - (rotationAmount + Random.Range(-rotationAmountRandomness,rotationAmountRandomness)));
		toQuaternion = Quaternion.Euler(euler.x, euler.y, euler.z + (rotationAmount + Random.Range(-rotationAmountRandomness,rotationAmountRandomness)));
		rotationSpeedRandomness = Random.Range(-rotationSpeedRandomness, rotationSpeedRandomness);
		randomStart = Random.Range(0f, 360f);
	}

	void Update () {
		transform.rotation = Quaternion.Lerp(fromQuaternion, toQuaternion, (Mathf.Sin(Mathf.Repeat(Time.timeSinceLevelLoad * (rotationSpeed + rotationSpeedRandomness) + randomStart, 360f) * Mathf.Deg2Rad) + 1) / 2);
	}
}
