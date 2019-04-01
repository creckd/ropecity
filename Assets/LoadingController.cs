using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingController : MonoBehaviour {

	public static void LoadScene(string sceneName) {
		Messenger.Instance.SendMessage(LoadingSceneInfoTag, sceneName);
		SceneManager.LoadScene("Loading");
	}

	public const string LoadingSceneInfoTag = "LSIT";

	private void Start() {
		object msg = null;
		if (Messenger.Instance.GetMessage(LoadingSceneInfoTag, out msg)) {
			SceneManager.LoadScene(msg.ToString());
		}
	}
}
