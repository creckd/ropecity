using UnityEngine;

public abstract class LevelObject : MonoBehaviour {

	public string objectID = "UNIQUE-ID";
	[HideInInspector]
	public string objectData = "";

	public virtual void DeserializeObjectData(string data) { }
	public virtual string SerializeObjectData() { return "none"; }
	public virtual void HookLandedOnThisObject() { }
	public virtual void HookReleasedOnThisObject() { }
}
