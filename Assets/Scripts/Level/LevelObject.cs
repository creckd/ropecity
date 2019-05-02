using System.Collections.Generic;
using UnityEngine;

public abstract class LevelObject : MonoBehaviour {

	public string objectID = "UNIQUE-ID";
	[HideInInspector]
	public string objectData = "";

	public virtual void DeserializeObjectData(string data) { }
	public virtual string SerializeObjectData() { return "none"; }
	public virtual void HookLandedOnThisObject() { }
	public virtual void HookReleasedOnThisObject() { }

	public Vector2 velocity {
		get {
			Vector2 sumVelocity = Vector2.zero;
			for (int i = 0; i < samples.Count; i++) {
				sumVelocity.x += samples[i].x;
				sumVelocity.y += samples[i].y;
			}
			float ratio = 0.36012f;
			return (sumVelocity / samples.Count) * ratio;
		}
	}

	private Vector2 currentAverageVelocity;

	private float sampleTime = 0.15f;
	private float timesBetweenSamples = 0.05f;
	private float lastTimeSampled = 0f;
	private List<Vector2> samples = new List<Vector2>();

	private Vector3 lastPosition = Vector3.zero;

	protected virtual void Update() {
		if(lastPosition == Vector3.zero)
		lastPosition = transform.position;
		if (Time.realtimeSinceStartup - lastTimeSampled > timesBetweenSamples) {
			lastTimeSampled = Time.realtimeSinceStartup;
			samples.Add(transform.position - lastPosition);
			lastPosition = transform.position;
			if (samples.Count > Mathf.FloorToInt(sampleTime / timesBetweenSamples))
				samples.Remove(samples[0]);
		}

		currentAverageVelocity = velocity;
	}
}
