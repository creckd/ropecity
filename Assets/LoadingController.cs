using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingController : MonoBehaviour {

	public float minimumLoadingTime = 1f;

	public static void LoadScene(string sceneName) {
		Messenger.Instance.SendMessage(LoadingSceneInfoTag, sceneName);
		SceneManager.LoadScene("Loading");
	}

	public const string LoadingSceneInfoTag = "LSIT";

	private void Start() {
		Time.timeScale = 1f;
		object msg = null;
		if (Messenger.Instance.GetMessage(LoadingSceneInfoTag, out msg)) {
			StartCoroutine(LoadSceneAsync(msg.ToString()));
		}
	}

	IEnumerator LoadSceneAsync(string sceneToLoad) {
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);
		asyncLoad.allowSceneActivation = false;
		float timeStartedLoading = Time.realtimeSinceStartup;

		while (asyncLoad.progress < 0.9f || Time.realtimeSinceStartup - timeStartedLoading < minimumLoadingTime)
			yield return null;

		asyncLoad.allowSceneActivation = true;
	}
}
