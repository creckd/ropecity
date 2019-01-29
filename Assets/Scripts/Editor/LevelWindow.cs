using UnityEditor;
using UnityEngine;

public class LevelWindow : EditorWindow {

	[MenuItem("Rope City/Level Saver")]
	public static void ShowWindow() {
		GetWindow<LevelWindow>(false, "Level Window", true);
	}

	string levelName = "";

	void OnGUI() {
		levelName = EditorGUILayout.TextField("Level Name", levelName);
		GUILayout.FlexibleSpace();
		GUI.backgroundColor = Color.yellow;
		if (GUILayout.Button("Open Level")) {
			string levelPath = EditorUtility.OpenFilePanel("Level Opener","Assets" + "\"" + "Level","level");
			if(levelPath != "")
			LevelSerializer.DeserializeLevelFromFile(levelPath);
		}
		GUI.backgroundColor = Color.green;
		if (GUILayout.Button("Save Level")) {
			LevelSerializer.SerializeCurrentlyOpenedLevel(levelName);
		}

	}
}
