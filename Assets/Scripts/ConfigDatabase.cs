using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigDatabase : MonoBehaviour {

	private static ConfigDatabase instance = null;
	public static ConfigDatabase Instance {
		get {
			if (instance == null)
				instance = FindObjectOfType<ConfigDatabase>();
			return instance;
		}
	}
	[Header("Worm")]
	public float gravityScale = 0.2f;
	public float rotationSpeed = 1f;
	public float ropeShootSpeed = 1f;
	public float maxRopeDistance = 10f;

	[Header("Prefabs")]
	public Worm wormPrefab;

	[Header("Cannon")]
	public Vector2 cannonShootDirection = new Vector2(1f, 1f);
	public float cannonShootForceMultiplier = 1f;

}
