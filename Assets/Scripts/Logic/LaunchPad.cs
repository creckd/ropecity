using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchPad : LevelObject {

	private void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag("Player")) {
			GameController.Instance.currentWorm.SetVelocity(transform.up * ConfigDatabase.Instance.launchPadForce);
		}
	}
}
