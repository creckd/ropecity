using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.IO;

public static class LevelSerializer {

	private const string levelNameKey = "lvlName";
	private const string levelObjectsKey = "lvlObjects";

	private const string posXKey = "X";
	private const string posYKey = "Y";
	private const string posZKey = "Z";
	private const string rotXKey = "RX";
	private const string rotYKey = "RY";
	private const string rotZKey = "RZ";


	public static void SerializeLevel(LevelData level) {
		JSONObject levelJson = new JSONObject();
		levelJson[levelNameKey] = level.levelName;

		for (int i = 0; i < level.levelObjects.Length; i++) {
			levelJson[levelObjectsKey][level.levelObjects[i].uniqueID][posXKey].AsInt = level.levelObjects[i].posX;
			levelJson[levelObjectsKey][level.levelObjects[i].uniqueID][posYKey].AsInt = level.levelObjects[i].posY;
			levelJson[levelObjectsKey][level.levelObjects[i].uniqueID][posZKey].AsInt = level.levelObjects[i].posZ;

			levelJson[levelObjectsKey][level.levelObjects[i].uniqueID][rotXKey].AsInt = level.levelObjects[i].rotX;
			levelJson[levelObjectsKey][level.levelObjects[i].uniqueID][rotYKey].AsInt = level.levelObjects[i].rotY;
			levelJson[levelObjectsKey][level.levelObjects[i].uniqueID][rotZKey].AsInt = level.levelObjects[i].rotZ;
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
