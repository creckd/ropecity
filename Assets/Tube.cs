using UnityEngine;

public class Tube : LevelObject {

	[System.Serializable]
	public class TubeData {
		public Vector4 flowDirection;
	}

	public TubeData data;

	private Material tubeMat;

	private void Start() {
		tubeMat = GetComponentInChildren<MeshRenderer>().material;
		tubeMat.SetVector("_TilingDirection", data.flowDirection);
	}

	public override void DeserializeObjectData(string objectData) {
		if (objectData != null)
			data = StringSerializationAPI.Deserialize(typeof(TubeData), objectData) as TubeData;
	}

	public override string SerializeObjectData() {
		return StringSerializationAPI.Serialize(typeof(TubeData), data);
	}
}
