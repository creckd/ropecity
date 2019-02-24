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

	public Action ShowUIHookAid = delegate { };
	public Action HideUIHookAid = delegate { };

	[HideInInspector]
	public Worm currentWorm = null;

	private float targetTimeScale = 1f;

	private bool canStartSlowingTime = false;

	private void Awake() {
		InitializeGame();
	}

	public void InitializeGame() {
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 300;

		LandedHook += UnSlowTime;
		ReleasedHook += SlowTime;

		int levelIndex = 0;
		string levelPath = "dani_001";
		object levelMessage;
		if (Messenger.Instance != null && Messenger.Instance.GetMessage(LevelSelectPanel.LevelIndexKey, out levelMessage)) {
			levelIndex = (int)levelMessage;
			levelPath = LevelResourceDatabase.Instance.levelResourceNames[levelIndex];
		}
		LevelController.Instance.currentLevelIndex = levelIndex;
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
		if(currentGameState != GameState.GameFinished)
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
		HideUIHookAid();
		if (!success) {
			targetTimeScale = ConfigDatabase.Instance.normalSpeed;
			StartCoroutine(ReInitiliazeGameAfter());
		} else {
			StartCoroutine(FinishingCoroutine());
		}
	}

	IEnumerator FinishingCoroutine() {
		float timer = 0f;
		float deftimeScale = Time.timeScale;
		while (timer <= ConfigDatabase.Instance.finishingSlowMotionTime) {
			timer += Time.unscaledDeltaTime;
			Time.timeScale = Mathf.Lerp(deftimeScale, 0f, ConfigDatabase.Instance.finishSlowMotionCurve.Evaluate(timer / ConfigDatabase.Instance.finishingSlowMotionTime));
			yield return null;
		}
		Time.timeScale = 0f;
		PanelManager.Instance.TryOpenPanel(1);
		IngameBlurController.Instance.BlurImage(ConfigDatabase.Instance.finishingBlurTime);
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

	public void BackToMainMenu() {
		UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
	}

	public void RestartButton() {
		Messenger.Instance.SendMessage(LevelSelectPanel.LevelIndexKey, LevelController.Instance.currentLevelIndex);
		UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
	}
}

