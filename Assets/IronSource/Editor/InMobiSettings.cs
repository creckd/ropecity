#if UNITY_IPHONE 
using UnityEngine;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using System.IO;

namespace IronSource.Editor
{
	public class InMobiSettings : IAdapterSettings
	{
		public void updateProject (BuildTarget buildTarget, string projectPath)
		{
			Debug.Log ("IronSource - Update project for InMobi");

			PBXProject project = new PBXProject ();
			project.ReadFromString (File.ReadAllText (projectPath));

			string targetId = project.TargetGuidByName (PBXProject.GetUnityTargetName ());
						
			project.AddFileToBuild (targetId, project.AddFile ("usr/lib/libsqlite3.0.tbd", "Frameworks/libsqlite3.0.tbd", PBXSourceTree.Sdk));
			project.AddFileToBuild (targetId, project.AddFile ("usr/lib/libz.tbd", "Frameworks/libz.tbd", PBXSourceTree.Sdk));

			// Custom Link Flag
			project.AddBuildProperty (targetId, "OTHER_LDFLAGS", "-ObjC");

			File.WriteAllText (projectPath, project.WriteToString ());
		}

		public void updateProjectPlist (BuildTarget buildTarget, string plistPath)
		{
			Debug.Log ("IronSource - Update plist for InMobi");
		}
	}
}
#endif