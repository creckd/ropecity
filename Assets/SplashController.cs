using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashController : MonoBehaviour {

	public float splashDurationInSeconds = 3f;

	private void Start() {
		StartCoroutine(WaitAndLoadMainMenu(splashDurationInSeconds));
	}

	IEnumerator WaitAndLoadMainMenu(float secondsToWait) {
		yield return new WaitForSeconds(secondsToWait);
		SceneManager.LoadScene("MainMenu");
	}
}
