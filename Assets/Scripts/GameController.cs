using System;
using System.Collections;
using UnityEngine;

public enum GameState {
	NotInitialized,
	Initialized,
	GameStarted,
	GameFinished
}

public class GameController : MonoBehaviour {

	private static GameController instance = null;
	public static GameController Instance {
		get {
			if (instance == null)
				instance = FindObjectOfType<GameController>();
			return instance;
		}
	}

	[HideInInspector]
	public GameState currentGameState = GameState.NotInitialized;

	public Action GameStarted = delegate { };
	public Action<bool> GameFinished = delegate { };
	public Action ReinitalizeGame = delegate { };

	[HideInInspector]
	public Worm currentWorm = null;

	private void Awake() {
		currentGameState = GameState.Initialized;
	}

	private void Start() {
		StartTheGame();
	}

	private void StartTheGame() {
		GameStarted();
		currentGameState = GameState.GameStarted;
	}

	public void FinishGame(bool success) {
		if (currentGameState == GameState.GameFinished)
			return;

		currentGameState = GameState.GameFinished;
		GameFinished(success);
		if (!success) {
			ReInitGame();
		} else {
			Time.timeScale = 0.1f;
		}
	}

	private void ReInitGame() {
		StartCoroutine(ReinitializingGame());
	}

	IEnumerator ReinitializingGame() {
		ReinitalizeGame();
		yield return new WaitForSeconds(ConfigDatabase.Instance.reinitalizingDuration);
		StartTheGame();
	}
}

