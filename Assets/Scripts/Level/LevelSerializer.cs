using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.IO;

public class LevelSerializer {

	private const string levelNameKey = "lvlName";
	private const string levelObjectsKey = "lvlObjects";

	private const string posXKey = "X";
	private const string posYKey = "Y";
	private const string posZKey = "Z";
	private const string rotXKey = "RX";
	private const string rotYKey = "RY";
	private const string rotZKey = "RZ";
	private const string componentDataKey = "data";

	public static void SerializeCurrentlyOpenedLevel(string levelName) {

		LevelObject[] levelObjects = Object.FindObjectsOfType<LevelObject>();
		LevelObjectData[] levelObjectDatas = new LevelObjectData[levelObjects.Length];
		for (int i = 0; i < levelObjects.Length; i++) {

			levelObjectDatas[i] = new LevelObjectData();

			levelObjectDatas[i].uniqueID = levelObjects[i].objectID;
			levelObjectDatas[i].posX = (levelObjects[i].transform.position.x);
			levelObjectDatas[i].posY = (levelObjects[i].transform.position.y);
			levelObjectDatas[i].posZ = (levelObjects[i].transform.position.z);

			levelObjectDatas[i].rotX = (levelObjects[i].transform.rotation.eulerAngles.x);
			levelObjectDatas[i].rotY = (levelObjects[i].transform.rotation.eulerAngles.y);
			levelObjectDatas[i].rotZ = (levelObjects[i].transform.rotation.eulerAngles.z);

			levelObjectDatas[i].componentData = levelObjects[i].SerializeObjectData();
		}

		LevelData data = new LevelData();
		data.levelName = levelName;
		data.levelObjects = levelObjectDatas;

		SerializeLevel(data);

	}

	public static void SerializeLevel(LevelData level) {
		JSONObject levelJson = new JSONObject();
		levelJson[levelNameKey] = level.levelName;

		for (int i = 0; i < level.levelObjects.Length; i++) {
			levelJson[levelObjectsKey][i][level.levelObjects[i].uniqueID][posXKey].AsFloat = level.levelObjects[i].posX;
			levelJson[levelObjectsKey][i][level.levelObjects[i].uniqueID][posYKey].AsFloat = level.levelObjects[i].posY;
			levelJson[levelObjectsKey][i][level.levelObjects[i].uniqueID][posZKey].AsFloat = level.levelObjects[i].posZ;

			levelJson[levelObjectsKey][i][level.levelObjects[i].uniqueID][rotXKey].AsFloat = level.levelObjects[i].rotX;
			levelJson[levelObjectsKey][i][level.levelObjects[i].uniqueID][rotYKey].AsFloat = level.levelObjects[i].rotY;
			levelJson[levelObjectsKey][i][level.levelObjects[i].uniqueID][rotZKey].AsFloat = level.levelObjects[i].rotZ;

			levelJson[levelObjectsKey][i][level.levelObjects[i].uniqueID][componentDataKey] = level.levelObjects[i].componentData;
		}

		string jsonString = levelJson.ToString();

		string date = System.DateTime.Now.ToString();
		date = date.Replace("/", "-");
		date = date.Replace(":", ".");

		File.WriteAllText("Assets/Levels/Resources/" + level.levelName + " (" + date + ")" + ".txt", jsonString);
	}

	public static LevelData DeserializeLevel(string levelData) {
		JSONNode jsonLevel = JSON.Parse(levelData);

		LevelData level = new LevelData();

		JSONArray levelObjects = jsonLevel[levelObjectsKey].AsArray;
		LevelObjectData[] objDatas = new LevelObjectData[levelObjects.Count];

		for (int i = 0; i < levelObjects.Count; i++) {
			string uniqueID = "";
			foreach (var k in jsonLevel[levelObjectsKey][i].Keys) {
				uniqueID = k.Value;
			}
			float posX = jsonLevel[levelObjectsKey][i][uniqueID][posXKey].AsFloat;
			float posY = jsonLevel[levelObjectsKey][i][uniqueID][posYKey].AsFloat;
			float posZ = jsonLevel[levelObjectsKey][i][uniqueID][posZKey].AsFloat;
			float rotX = jsonLevel[levelObjectsKey][i][uniqueID][rotXKey].AsFloat;
			float rotY = jsonLevel[levelObjectsKey][i][uniqueID][rotYKey].AsFloat;
			float rotZ = jsonLevel[levelObjectsKey][i][uniqueID][rotZKey].AsFloat;
			string componentData = jsonLevel[levelObjectsKey][i][uniqueID][componentDataKey];

			LevelObjectData objData = new LevelObjectData();
			objData.uniqueID = uniqueID;
			objData.posX = posX;
			objData.posY = posY;
			objData.posZ = posZ;

			objData.rotX = rotX;
			objData.rotY = rotY;
			objData.rotZ = rotZ;

			objData.componentData = componentData;

			objDatas[i] = objData;

		}

		level.levelName = jsonLevel[levelNameKey];
		level.levelObjects = objDatas;

		return level;
	}

	public static LevelData DeserializeLevelFromFile(string filePath) {
		return DeserializeLevel(File.ReadAllText(filePath));
	}
}
