using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchPad : MonoBehaviour {

	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Player")) {
			GameController.Instance.currentWorm.AddForce(transform.up * ConfigDatabase.Instance.launchPadForce);
		}
	}
}
