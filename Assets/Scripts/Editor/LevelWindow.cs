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
		GUI.backgroundColor = Color.green;
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Save Level")) {
			LevelSerializer.SerializeCurrentlyOpenedLevel(levelName);
		}

	}
}
