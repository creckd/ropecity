using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumper : LevelObject {
	public AnimationCurve jumpCurve;
	public float amountToJump = 5f;
	public float periodWaitTime = 1f;
	public float jumpTime = 0.1f;
	public bool startingDirectionIsRight = true;

	private float lastTimeJumped = 0f;
	private int currSign = 1;

	private void Awake() {
		currSign = startingDirectionIsRight ? 1 : -1;
	}

	override protected void Update() {
		base.Update();
		if (Time.realtimeSinceStartup - lastTimeJumped >= periodWaitTime) {
			lastTimeJumped = Time.realtimeSinceStartup;
			Jump();
		}
	}

	private void Jump() {
		StartCoroutine(JumpRoutine());
	}

	IEnumerator JumpRoutine() {
		float timer = 0f;
		Vector3 currPos = transform.position;
		Vector3 destPos = transform.position + transform.right * amountToJump * currSign;
		while (timer <= jumpTime) {
			timer += Time.unscaledDeltaTime;
			transform.position = Vector3.Lerp(currPos, destPos, jumpCurve.Evaluate(timer / jumpTime));
			yield return null;
		}
		transform.position = destPos;
		currSign *= -1;
	}
}
