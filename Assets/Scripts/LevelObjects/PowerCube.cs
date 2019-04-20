using UnityEngine;

public class PowerCube : LevelObject {

	public SimpleRotate rotationScript;
	public float amountToMoveWhenHit = 2f;
	public float movementSpeed = 1f;

	private Vector3 defaultPosition;
	private bool currentlyWormHangingOnMe = false;
	private float t;

	private void Start() {
		defaultPosition = transform.position;
	}

	private void Update() {

		if (currentlyWormHangingOnMe)
			t = Mathf.Clamp(t + Time.deltaTime * movementSpeed, 0f, 1f);
		else
			t = Mathf.Clamp(t - Time.deltaTime * movementSpeed, 0f, 1f);

		transform.position = Vector3.Lerp(defaultPosition, defaultPosition - Vector3.up * amountToMoveWhenHit, t);
	}

	public override void HookLandedOnThisObject() {
		rotationScript.enabled = false;
		currentlyWormHangingOnMe = true;
	}

	public override void HookReleasedOnThisObject() {
		rotationScript.enabled = true;
		currentlyWormHangingOnMe = false;
	}

}
