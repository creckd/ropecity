using UnityEditor;
using UnityEngine;

public class LevelWindow : EditorWindow {

	[MenuItem("Rope City/Level Saver")]
	public static void ShowWindow() {
		GetWindow<LevelWindow>(false, "Level Window", true);
	}

	string levelName = "";
	string levelPath = "";

	void OnGUI() {
		levelName = EditorGUILayout.TextField("Level Name", levelName);
		EditorGUILayout.LabelField("Selected level: " + levelPath);
		GUILayout.FlexibleSpace();
		GUI.backgroundColor = Color.yellow;
		if (GUILayout.Button("Open Level")) {
			levelPath = EditorUtility.OpenFilePanel("Level Opener","Assets" + "\"" + "Level","txt");
		}
		GUI.backgroundColor = Color.cyan;
		if (GUILayout.Button("Load Level")) {
			if (levelPath != "") {
				foreach (var remaining in FindObjectsOfType<LevelObject>()) {
					DestroyImmediate(remaining.gameObject);
				}
				LevelData levelData = LevelSerializer.DeserializeLevelFromFile(levelPath);
				LevelController.Instance.InitializeLevel(levelData);
				EditorApplication.MarkSceneDirty();
			} else {
				EditorUtility.DisplayDialog("No level!", "No level was selected, nothing to load.", "Okay");
			}
		}
		GUI.backgroundColor = Color.red;
		if (GUILayout.Button("Delete Level")) {
			int i = 0;
			foreach (var remaining in FindObjectsOfType<LevelObject>()) {
				DestroyImmediate(remaining.gameObject);
				i++;
			}
			EditorApplication.MarkSceneDirty();
			Debug.Log("Deleted " + i.ToString() + " objects.");
		}
		GUI.backgroundColor = Color.green;
		if (GUILayout.Button("Save Level")) {
			LevelSerializer.SerializeCurrentlyOpenedLevel(levelName);
			Debug.Log("Save Successful!");
		}

	}
}
