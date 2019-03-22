using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZFighterRandomizer : MonoBehaviour {

	public float amount = 0.1f;
	private static int index = 0;

	public void Awake() {
		transform.position = new Vector3(transform.position.x,transform.position.y + amount * index,transform.position.z + amount * index);
		index++;
	}
}
