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

	public static void SerializeCurrentlyOpenedLevel(string levelName) {

		LevelObject[] levelObjects = Object.FindObjectsOfType<LevelObject>();
		LevelObjectData[] levelObjectDatas = new LevelObjectData[levelObjects.Length];
		for (int i = 0; i < levelObjects.Length; i++) {

			levelObjectDatas[i] = new LevelObjectData();

			levelObjectDatas[i].uniqueID = levelObjects[i].objectID;
			levelObjectDatas[i].posX = Mathf.FloorToInt(levelObjects[i].transform.position.x);
			levelObjectDatas[i].posY = Mathf.FloorToInt(levelObjects[i].transform.position.y);
			levelObjectDatas[i].posZ = Mathf.FloorToInt(levelObjects[i].transform.position.z);

			levelObjectDatas[i].rotX = Mathf.FloorToInt(levelObjects[i].transform.rotation.eulerAngles.x);
			levelObjectDatas[i].rotY = Mathf.FloorToInt(levelObjects[i].transform.rotation.eulerAngles.y);
			levelObjectDatas[i].rotZ = Mathf.FloorToInt(levelObjects[i].transform.rotation.eulerAngles.z);
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
			levelJson[levelObjectsKey][i][level.levelObjects[i].uniqueID][posXKey].AsInt = level.levelObjects[i].posX;
			levelJson[levelObjectsKey][i][level.levelObjects[i].uniqueID][posYKey].AsInt = level.levelObjects[i].posY;
			levelJson[levelObjectsKey][i][level.levelObjects[i].uniqueID][posZKey].AsInt = level.levelObjects[i].posZ;

			levelJson[levelObjectsKey][i][level.levelObjects[i].uniqueID][rotXKey].AsInt = level.levelObjects[i].rotX;
			levelJson[levelObjectsKey][i][level.levelObjects[i].uniqueID][rotYKey].AsInt = level.levelObjects[i].rotY;
			levelJson[levelObjectsKey][i][level.levelObjects[i].uniqueID][rotZKey].AsInt = level.levelObjects[i].rotZ;
		}

		string jsonString = levelJson.ToString();

		string date = System.DateTime.Now.ToString();
		date = date.Replace("/", "-");
		date = date.Replace(":", ".");

		File.WriteAllText("Assets/Levels/" + level.levelName + " (" + date + ")" + ".level", jsonString);
	}

	public static LevelData DeserializeLevel(string levelData) {
		LevelData level = new LevelData();
		return level;
	}
}
