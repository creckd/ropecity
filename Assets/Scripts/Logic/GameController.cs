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

	public Action<Vector3> FoundPotentionalHitPoint = delegate { };

	public Action LandedHook = delegate { };
	public Action ReleasedHook = delegate { };

	[HideInInspector]
	public Worm currentWorm = null;

	private float targetTimeScale = 1f;

	private bool canStartSlowingTime = false;

	private void Awake() {
		InitializeGame();
	}

	public void InitializeGame() {
		LandedHook += UnSlowTime;
		ReleasedHook += SlowTime;

		int levelIndex = 0;
		string levelPath = "dani_002";
		object levelMessage;
		if (Messenger.Instance != null && Messenger.Instance.GetMessage(LevelSelectPanel.LevelIndexKey, out levelMessage)) {
			levelIndex = (int)levelMessage;
			levelPath = LevelResourceDatabase.Instance.levelResourceNames[levelIndex];
		}
		TextAsset levelAsset = (TextAsset)Resources.Load(levelPath, typeof(TextAsset));
		LevelData data = LevelSerializer.DeserializeLevel(levelAsset.text);
		LevelController.Instance.InitializeLevel(data);
		currentGameState = GameState.Initialized;
	}

	private void SlowTime() {
		if (currentGameState != GameState.GameStarted || !canStartSlowingTime)
			return;
		targetTimeScale = ConfigDatabase.Instance.slowMotionSpeed;
	}

	private void UnSlowTime() {
		if (currentGameState != GameState.GameStarted || !canStartSlowingTime)
			return;
		targetTimeScale = ConfigDatabase.Instance.normalSpeed;
	}

	public void StartSlowingTime() {
		targetTimeScale = ConfigDatabase.Instance.slowMotionSpeed;
		canStartSlowingTime = true;
	}

	private void Start() {
		StartCoroutine(StartTheGameAfterAFewFrames()); // for subscriptions
	}

	IEnumerator StartTheGameAfterAFewFrames() {
		yield return null;
		yield return null;
		yield return null;
		StartTheGame();
	}

	private void Update() {
		Time.timeScale = Mathf.Lerp(Time.timeScale, targetTimeScale, Time.unscaledDeltaTime * 7.5f);
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
			targetTimeScale = ConfigDatabase.Instance.normalSpeed;
			StartCoroutine(ReInitiliazeGameAfter());
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
		canStartSlowingTime = false;
		ReinitalizeGame();
		yield return new WaitForSeconds(ConfigDatabase.Instance.reinitalizingDuration);
		StartTheGame();
	}
}

