using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using UnityEditor.iOS.Xcode;
using System.IO;
using Prime31.Xcode;

public class RopeCityPlistEditor {

	[PostProcessBuild]
	public static void ChangeXcodePlist(BuildTarget buildTarget, string pathToBuiltProject) {

		if (buildTarget == BuildTarget.iOS) {

			// Get plist
			string plistPath = pathToBuiltProject + "/Info.plist";
			PlistDocument plist = new PlistDocument();
			plist.ReadFromString(File.ReadAllText(plistPath));

			// Get root
			PlistElementDict rootDict = plist.root;

			// ADMOB
			var admobKey = "GADApplicationIdentifier";
			rootDict.SetString(admobKey, "ca-app-pub-9539815930599175~9209076344");
			PlistElementDict transportDict = rootDict.CreateDict("NSAppTransportSecurity");
			transportDict.SetBoolean("NSAllowsArbitraryLoads", true);

			// Write to file
			File.WriteAllText(plistPath, plist.WriteToString());
		}
	}
}
