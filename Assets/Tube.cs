using UnityEngine;
using OdinSerializer;

public class Tube : LevelObject {

	[System.Serializable]
	public class TubeData {
		[OdinSerialize]
		public Vector4 flowDirection;
	}

	public TubeData data;

	private Material tubeMat;

	private void Start() {
		tubeMat = GetComponentInChildren<MeshRenderer>().material;
		tubeMat.SetVector("_TilingDirection", data.flowDirection);
	}

	public override void DeserializeObjectData(string objectData) {
		if(objectData != null)
		data = SerializationUtility.DeserializeValue<TubeData>(System.Text.Encoding.ASCII.GetBytes(objectData), DataFormat.Binary);
	}

	public override string SerializeObjectData() {
		return System.Text.Encoding.ASCII.GetString(SerializationUtility.SerializeValue(data, DataFormat.Binary));
	}
}
