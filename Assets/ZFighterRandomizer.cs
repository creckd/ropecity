using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZFighterRandomizer : MonoBehaviour {

	public float amount = 0.1f;

	public void Awake() {
		transform.position = new Vector3(transform.position.x,transform.position.y,transform.position.z + Random.Range(-amount, amount));
	}
}
