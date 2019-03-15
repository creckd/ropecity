﻿using UnityEngine;
using OdinSerializer;

public class Tube : LevelObject {

	[System.Serializable]
	public class TubeData {
		public Vector4 flowDirection;
	}

	public TubeData data;

	public override void DeserializeObjectData(string objectData) {
		if(objectData != null)
		data = SerializationUtility.DeserializeValue<TubeData>(System.Text.Encoding.ASCII.GetBytes(objectData), DataFormat.Binary);
	}

	public override string SerializeObjectData() {
		return System.Text.Encoding.ASCII.GetString(SerializationUtility.SerializeValue(data, DataFormat.Binary));
	}
}
