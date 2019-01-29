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

	private int lastUsedFingerID = -1;
	private float targetTimeScale = 1f;

	private void Awake() {
		InputController.Instance.TapHappened += TapHappened;
		InputController.Instance.ReleaseHappened += ReleaseHappened;
		currentGameState = GameState.Initialized;
	}

	private void ReleaseHappened(int id) {
		if (currentGameState == GameState.GameFinished)
			return;
		if (id == lastUsedFingerID) {
			targetTimeScale = ConfigDatabase.Instance.slowMotionSpeed;
			lastUsedFingerID = -1;
		}
	}

	private void TapHappened(int id) {
		if (currentGameState == GameState.GameFinished)
			return;
		if (lastUsedFingerID == -1) {
			lastUsedFingerID = id;
			targetTimeScale = ConfigDatabase.Instance.normalSpeed;
		}
	}

	public void SlowTime() {
		targetTimeScale = ConfigDatabase.Instance.slowMotionSpeed;
	}

	private void Start() {
		StartTheGame();
	}

	private void Update() {
		Time.timeScale = Mathf.Lerp(Time.timeScale, targetTimeScale, Time.unscaledDeltaTime * 10f);
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
			targetTimeScale = ConfigDatabase.Instance.normalSpeed;
			StartCoroutine(ReInitiliazeGameAfter());
		}
	}

	IEnumerator ReInitiliazeGameAfter() {
		yield return new WaitForSeconds(1f);
		if (currentWorm != null)
			currentWorm.Die();
		ReInitGame();
	}

	private void ReInitGame() {
		StartCoroutine(ReinitializingGame());
	}

	IEnumerator ReinitializingGame() {
		targetTimeScale = ConfigDatabase.Instance.normalSpeed;
		lastUsedFingerID = -1;
		ReinitalizeGame();
		yield return new WaitForSeconds(ConfigDatabase.Instance.reinitalizingDuration);
		StartTheGame();
	}
}

