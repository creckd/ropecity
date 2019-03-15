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

	private void Awake() {
		DontDestroyOnLoad(this.gameObject);
	}

	[Header("Game")]
	public float reinitalizingDuration = 1f;
	public float normalSpeed = 1f;
	public float slowMotionSpeed = 0.5f;
	public float allTimeMininmumWorldY = 0f;

	[Header("Finish")]
	public float finishingSlowMotionTime = 0.5f;
	public float finishingBlurTime = 0.5f;
	public AnimationCurve finishSlowMotionCurve;

	[Header("Pause")]
	public float pauseBlurTime = 0.5f;

	[Header("Worm")]
	public float gravityScale = 0.2f;
	public float wormMass = 100;
	public float swingForceMultiplier = 10f;
	public float rotationSpeed = 1f;
	public float ropeShootSpeed = 1f;
	public float maxRopeDistance = 10f;
	public float pullForceMultiplier = 1f;
	public float maximumRewardedAngleForPullForce = 30f;
	public float remainingVelocityPercentAfterBounce = 0.75f;
	public float minimumVelocityMagnitudeAfterBounce = 1f;

	[Header("Traps")]
	public float mineLethalRange = 5f;

	[Header("Prefabs")]
	public Worm wormPrefab;

	[Header("Cannon")]
	public Vector2 cannonShootDirection = new Vector2(1f, 1f);
	public float cannonShootForceMultiplier = 1f;

	[Header("LaunchPad")]
	public float launchPadForce = 1f;
}
