using System;
using System.Collections;
using UnityEngine;

public enum GameState {
	NotInitialized,
	Initialized,
	GameStarted,
	GameFinished,
	GamePaused
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

	public bool isDebugTestLevelMode = false;

	[HideInInspector]
	public GameState currentGameState = GameState.NotInitialized;

	public Action GameStarted = delegate { };
	public Action GameInitialized = delegate { }; //First initialization
	public Action<bool> GameFinished = delegate { };
	public Action ReinitalizeGame = delegate { };

	public Action<Vector3> FoundPotentionalHitPoint = delegate { };

	public Action<Vector3> LandedHook = delegate { };
	public Action ReleasedHook = delegate { };

	public Action ShowUIHookAid = delegate { };
	public Action HideUIHookAid = delegate { };

	[HideInInspector]
	public Worm currentWorm = null;

	private float targetTimeScale = 1f;

	private bool canStartSlowingTime = false;

	public void InitializeGame() {
		LandedHook += UnSlowTime;
		ReleasedHook += SlowTime;

		if (!isDebugTestLevelMode) {
			int levelIndex = 0;
			string levelPath = "dani_001";
			object levelMessage;
			if (Messenger.Instance != null && Messenger.Instance.GetMessage(LevelSelectPanel.LevelIndexKey, out levelMessage)) {
				levelIndex = (int)levelMessage;
				levelPath = LevelResourceDatabase.Instance.GetResourceWithLevelIndex(levelIndex);
			}
			LevelController.Instance.currentLevelIndex = levelIndex;
			TextAsset levelAsset = (TextAsset)Resources.Load(levelPath, typeof(TextAsset));
			LevelData data = LevelSerializer.DeserializeLevel(levelAsset.text);
			LevelController.Instance.InitializeLevel(data);
		}
		currentGameState = GameState.Initialized;
		GameInitialized();
	}

	private void SlowTime() {
		if (currentGameState != GameState.GameStarted || !canStartSlowingTime)
			return;
		targetTimeScale = ConfigDatabase.Instance.slowMotionSpeed;
	}

	private void UnSlowTime(Vector3 hp) {
		if (currentGameState != GameState.GameStarted || !canStartSlowingTime)
			return;
		targetTimeScale = ConfigDatabase.Instance.normalSpeed;
	}

	public void StartSlowingTime() {
		targetTimeScale = ConfigDatabase.Instance.slowMotionSpeed;
		canStartSlowingTime = true;
	}

	private void Awake() {
		InitializeGame();
		StartCoroutine(StartTheGameAfterAFewFrames()); // for subscriptions
	}

	IEnumerator StartTheGameAfterAFewFrames() {
		yield return null;
		yield return null;
		yield return null;
		StartTheGame();
	}

	private void Update() {
		if(currentGameState != GameState.GameFinished && currentGameState != GameState.GamePaused)
		Time.timeScale = Mathf.Lerp(Time.timeScale, targetTimeScale, Time.unscaledDeltaTime * 7.5f);
	}

	private void StartTheGame() {
		if(!isDebugTestLevelMode)
		SavedDataManager.Instance.GetLevelSaveDataWithLevelIndex(LevelController.Instance.currentLevelIndex).numberOfTries++;
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
			UnlockNextLevel();
			StartCoroutine(FinishingCoroutine());
		}
	}

	private void UnlockNextLevel() {
		SavedDataManager.Instance.GetLevelSaveDataWithLevelIndex(LevelController.Instance.currentLevelIndex).levelCompleted = true;
		SavedDataManager.Instance.GetLevelSaveDataWithLevelIndex(LevelController.Instance.currentLevelIndex + 1).isUnlocked = true;
		SavedDataManager.Instance.Save();
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
		yield return new WaitForSeconds(0f);
		if (currentWorm != null)
			currentWorm.Die();
		ReInitGame();
	}

	private void ReInitGame() {
		StartCoroutine(ReinitializingGame());
	}

	IEnumerator ReinitializingGame() {
		ImageTransitionHandler.Instance.TransitionIn();
		yield return new WaitForSecondsRealtime(ConfigDatabase.Instance.transitionTime);
		targetTimeScale = ConfigDatabase.Instance.normalSpeed;
		canStartSlowingTime = false;
		ReinitalizeGame();
		yield return new WaitForSeconds(ConfigDatabase.Instance.reinitalizingDuration);
		ImageTransitionHandler.Instance.TransitionOut();
		StartTheGame();
	}

	public void PauseGame() {
		if (currentGameState == GameState.GameStarted) {
			currentGameState = GameState.GamePaused;
			Time.timeScale = 0f;
			IngameBlurController.Instance.BlurImage(ConfigDatabase.Instance.pauseBlurTime, true);
			PanelManager.Instance.TryOpenPanel(2);
		}
	}

	public void ResumeGame() {
		if (currentGameState == GameState.GamePaused) {
			PanelManager.Instance.TryOpenPanel(0);
			IngameBlurController.Instance.UnBlurImage(ConfigDatabase.Instance.pauseBlurTime);
			currentGameState = GameState.GameStarted;
			Time.timeScale = targetTimeScale;
		}
	}

	public void BackToMainMenu() {
		StartCoroutine(BackToMainMenuRoutine());
	}

	IEnumerator BackToMainMenuRoutine() {
		ImageTransitionHandler.Instance.TransitionIn();
		yield return new WaitForSecondsRealtime(ConfigDatabase.Instance.transitionTime);
		Messenger.Instance.SendMessage(PanelManager.defaultOpenedPanelChangedTag, 1);
		LoadingController.LoadScene("MainMenu");
	}

	public void RestartButton() {
		StartCoroutine(RestartButtonRoutine());
	}

	IEnumerator RestartButtonRoutine() {
		ImageTransitionHandler.Instance.TransitionIn();
		yield return new WaitForSecondsRealtime(ConfigDatabase.Instance.transitionTime);
		Messenger.Instance.SendMessage(LevelSelectPanel.LevelIndexKey, LevelController.Instance.currentLevelIndex);
		LoadingController.LoadScene("Game");
	}
}

