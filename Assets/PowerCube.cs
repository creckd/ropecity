using UnityEngine;

public class PowerCube : LevelObject {

	public SimpleRotate rotationScript;

	public override void HookLandedOnThisObject() {
		rotationScript.enabled = false;
	}

	public override void HookReleasedOnThisObject() {
		rotationScript.enabled = true;
	}

}
