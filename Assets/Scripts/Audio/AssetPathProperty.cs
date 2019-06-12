using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;

[CustomPropertyDrawer(typeof(AssetPathAttribute))]
public class ResourcePathPropertyDrawer : PropertyDrawer
{
	[System.Serializable]
	class SearchData
	{
		public bool enable = false;
		public string filter = "";
		public int oldIndex = 0;
	}

	bool initialized = false;
	string[] paths = null;
	string searchInFolder = "";
	string filter = "";
	string extersion = "";
	Dictionary<Rect, SearchData> searchDataDict = new Dictionary<Rect, SearchData>();

	void InitPaths()
	{
		AssetPathAttribute ap = attribute as AssetPathAttribute;
		filter = ap.filter;
		string[] guids = AssetDatabase.FindAssets(ap.filter, ap.searchInFolders);
		paths = new string[guids.Length + 1];
		paths[0] = "Empty";
		for (int i = 0; i < guids.Length; i++)
		{
			string path = AssetDatabase.GUIDToAssetPath(guids[i]);
			if (ap.searchInFolders[0] != null)
				path = path.Substring(ap.searchInFolders[0].Length + 1);
			if (ap.cutoffExtension)
			{
				int index = path.LastIndexOf('.');
				path = path.Substring(0, index);
			}
			paths[i + 1] = path;
		}

		searchInFolder = ap.searchInFolders[0];
		//filter = ap.filter;

		foreach (var fileName in paths)
		{
			foreach (var fileInfo in new DirectoryInfo(searchInFolder).GetFiles("*.*", SearchOption.AllDirectories))
			{
				if (fileInfo.Name.Contains(fileName))
				{
					extersion = fileInfo.Extension;
					break;
				}
			}

			if (extersion != "")
				break;
		}
	}

	int GetIndexWithValue(string[] array, string value)
	{
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == value)
			{
				return i;
			}
		}
		return -1;
	}

	string GetStringValueWihtIndex(string[] array, int index)
	{
		return (index > 0 && index < array.Length) ? array[index] : "";
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		int index = 0;
		float selectButtonWidth = 60f;
		float searchButtonWidth = 20f;
		Rect rect = new Rect(position.x, position.y, position.width - selectButtonWidth - searchButtonWidth, position.height);
		Rect selectRect = new Rect(position.x + position.width - selectButtonWidth - searchButtonWidth, position.y, selectButtonWidth, position.height);
		Rect sreachButtonRect = new Rect(position.x + position.width - searchButtonWidth, position.y, searchButtonWidth, position.height);

		if (!searchDataDict.ContainsKey(position))
			searchDataDict.Add(position, new SearchData());

		EditorGUI.BeginProperty(position, label, property);
		if (property.type == "string")
		{
			if (!initialized)
			{
				initialized = true;
				InitPaths();
			}

			EditorGUI.BeginChangeCheck();
			if (searchDataDict[position].enable)
			{
				string[] filteredPathsTemp = paths.Where(p => p.IndexOf(searchDataDict[position].filter, System.StringComparison.CurrentCultureIgnoreCase) >= 0).ToArray();
				string[] filteredPaths = new string[filteredPathsTemp.Length + 1];
				filteredPaths[0] = "Empty";
				for (int i = 0; i < filteredPathsTemp.Length; i++)
				{
					filteredPaths[i + 1] = filteredPathsTemp[i];
				}

				index = GetIndexWithValue(filteredPaths, property.stringValue);
				index = EditorGUI.Popup(rect, label.text, index, filteredPaths);
				property.stringValue = GetStringValueWihtIndex(filteredPaths, index);
			}
			else
			{
				index = GetIndexWithValue(paths, property.stringValue);
				index = EditorGUI.Popup(rect, label.text, index, paths);
				property.stringValue = GetStringValueWihtIndex(paths, index);
			}

			if (EditorGUI.EndChangeCheck())
			{
				searchDataDict[position].oldIndex = GetIndexWithValue(paths, property.stringValue);
			}

			//if (!filter.Contains("texture"))
			//{
			GUIContent content = new GUIContent();
			content.text = "Select";
			content.tooltip = searchInFolder;
			if (!searchDataDict[position].enable && GUI.Button(selectRect, content))
			{
				Object found_asset = AssetDatabase.LoadAssetAtPath(searchInFolder + "/" + property.stringValue + extersion, typeof(Object));
				if (found_asset != null)
				{
					Selection.activeObject = found_asset;
				}
			}
			//}
			/*else
			{
				Texture2D myTexture = Resources.Load(property.stringValue) as Texture2D;
				GUIContent content = new GUIContent();
				content.image = myTexture;
				EditorGUI.LabelField(new Rect(selectRect.position - new Vector2(30, 5), new Vector2(100,100)), content);
			}*/

			if (searchDataDict[position].enable)
				searchDataDict[position].filter = GUI.TextField(selectRect, searchDataDict[position].filter);

			if (GUI.Button(sreachButtonRect, "o"))
			{
				searchDataDict[position].enable = !searchDataDict[position].enable;
				searchDataDict[position].filter = "";

				if (searchDataDict[position].enable)
					searchDataDict[position].oldIndex = index;
				else
				{
					index = searchDataDict[position].oldIndex;
					property.stringValue = GetStringValueWihtIndex(paths, index);
				}
			}
		}
		else
		{
			EditorGUI.LabelField(position, label.text, "Use AssetPath with string.");
		}

		EditorGUI.EndProperty();
	}
}
#endif

public sealed class AssetPathAttribute : PropertyAttribute
{
	internal string filter;
	internal string[] searchInFolders = new string[1];
	internal bool cutoffExtension;

	public AssetPathAttribute(string filter, string searchInFolder, bool cutoffExtension = true)
	{
		this.filter = filter;
		this.searchInFolders[0] = searchInFolder;
		this.cutoffExtension = cutoffExtension;
	}
}