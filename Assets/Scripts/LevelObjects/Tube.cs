using UnityEngine;

public class Tube : LevelObject {

	[System.Serializable]
	public class TubeData {
		public Vector4 flowDirection = new Vector4(1f,0f,0f,0f);
	}

	public TubeData data;

	private Material tubeMat;

	private void Start() {
		tubeMat = GetComponentInChildren<MeshRenderer>().material;
		tubeMat.SetVector("_TilingDirection", data.flowDirection);
	}

	private void Update() {
		tubeMat.SetFloat("_WorldLiquidHeight", transform.position.y);
	}

	public override void DeserializeObjectData(string objectData) {
		if (objectData != null)
			data = StringSerializationAPI.Deserialize(typeof(TubeData), objectData) as TubeData;
	}

	public override string SerializeObjectData() {
		return StringSerializationAPI.Serialize(typeof(TubeData), data);
	}
}
