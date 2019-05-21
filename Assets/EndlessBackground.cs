using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessBackground : MonoBehaviour {

	[System.Serializable]
	public class BackgroundPart {
		public GameObject backgroundObject;
		public float width;
	}

	public List<BackgroundPart> parts = new List<BackgroundPart>();
	public float referenceX = 0f;
	public float cameraWorldSpaceVisibleXLength = 50f;

	private BackgroundPart MiddlePart {
		get {
			return parts[1];
		}
	}

	private void Update() {
		referenceX = Camera.main.transform.position.x;
		float diff = referenceX - MiddlePart.backgroundObject.transform.position.x;
		if (Mathf.Abs(diff) >= cameraWorldSpaceVisibleXLength / 2f) {
			if (Mathf.Sign(diff) == 1) {
				SnapForward();
			} else {
				SnapBackward();
			}
		}
	}

	private void SnapForward() {
		BackgroundPart firstPart = parts[0];
		BackgroundPart lastPart = parts[2];
		firstPart.backgroundObject.transform.position = lastPart.backgroundObject.transform.position + Vector3.right * firstPart.width;
		parts.Remove(firstPart);
		parts.Add(firstPart);
	}

	private void SnapBackward() {
		BackgroundPart firstPart = parts[0];
		BackgroundPart lastPart = parts[2];
		lastPart.backgroundObject.transform.position = firstPart.backgroundObject.transform.position - Vector3.right * lastPart.width;
		parts.Remove(lastPart);
		parts.Insert(0, lastPart);
	}
}
