using System;
using System.Collections;
using System.Collections.Generic;
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

	public bool gameControllerControlsTime = true;
	public bool wormInputEnabled = true;

	public Action<bool> GameStarted = delegate { };
	public Action GameInitialized = delegate { }; //First initialization
	public Action<bool> GameFinished = delegate { };
	public Action ReinitalizeGame = delegate { };

	public Action<Vector3> WormDiedAtPosition = delegate { };

	public Action<Vector3> FoundPotentionalHitPoint = delegate { };

	public Action<Vector3> LandedHook = delegate { };
	public Action ReleasedHook = delegate { };

	public Action ShowUIHookAid = delegate { };
	public Action HideUIHookAid = delegate { };

	public Action ShowHoldIndicator = delegate { };
	public Action HideHoldIndicator = delegate { };

	public Action ShowReleaseIndicator = delegate { };
	public Action HideReleaseIndicator = delegate { };

	public Action ShowTutorialLastTask = delegate { };

	[HideInInspector]
	public Worm currentWorm = null;

	[HideInInspector]
	public bool shouldStartTutorial = false;

	private float targetTimeScale = 1f;
	private float currentDampeningValue = 7.5f;

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
		currentDampeningValue = ConfigDatabase.Instance.unhookedTimeDampeningSpeed;

	}

	private void UnSlowTime(Vector3 hp) {
		if (currentGameState != GameState.GameStarted || !canStartSlowingTime)
			return;
		targetTimeScale = ConfigDatabase.Instance.normalSpeed;
		currentDampeningValue = ConfigDatabase.Instance.hookedTimeDampeningSpeed;
	}

	public void StartSlowingTime() {
		targetTimeScale = ConfigDatabase.Instance.slowMotionSpeed;
		currentDampeningValue = ConfigDatabase.Instance.unhookedTimeDampeningSpeed;
		canStartSlowingTime = true;
	}

	private void Awake() {
		InitializeGame();
		StartCoroutine(StartTheGameAfterAFewFrames()); // for subscriptions
	}

	private void Start() {
		SoundManager.Instance.LoopUntilStopped(AudioConfigDatabase.Instance.ingameMusic.CloneToCustomClip(), "ingameMusic");
		SoundManager.Instance.LoopUntilStopped(AudioConfigDatabase.Instance.ingameAmbient.CloneToCustomClip(), "ingameAmbient");
	}

	IEnumerator StartTheGameAfterAFewFrames() {

		yield return null;

		if (!isDebugTestLevelMode) {
			LevelSaveDatabase.LevelSaveData saveData = SavedDataManager.Instance.GetLevelSaveDataWithLevelIndex(LevelController.Instance.currentLevelIndex);
			shouldStartTutorial = LevelController.Instance.currentLevelIndex == 0 && saveData.levelCompleted == false;
		}

		if (shouldStartTutorial) {
			TutorialController.Instance.StartTutorial();
		} else {
			Destroy(TutorialController.Instance.gameObject);
		}

		PanelManager.Instance.InitializeGUI();

		while (currentGameState != GameState.Initialized)
			yield return null;

		yield return null;

		CameraController.Instance.StartCinematic();
		StartTheGame(false);
	}

	private void Update() {
		if(currentGameState != GameState.GameFinished && currentGameState != GameState.GamePaused && gameControllerControlsTime)
		Time.timeScale = Mathf.Lerp(Time.timeScale, targetTimeScale, Time.unscaledDeltaTime * currentDampeningValue);
	}

	private void StartTheGame(bool fastStart) {
		if (!isDebugTestLevelMode) {
			LevelSaveDatabase.LevelSaveData saveData = SavedDataManager.Instance.GetLevelSaveDataWithLevelIndex(LevelController.Instance.currentLevelIndex);
			saveData.numberOfTries++;

			string levelName = LevelResourceDatabase.Instance.GetResourceWithLevelIndex(LevelController.Instance.currentLevelIndex);
			int tries = SavedDataManager.Instance.GetLevelSaveDataWithLevelIndex(LevelController.Instance.currentLevelIndex).numberOfTries;
			Dictionary<string, object> parameters = new Dictionary<string, object>();
			parameters.Add("name", levelName);
			parameters.Add("try", tries);
			AnalyticsManager.LogEvent("LevelStarted", parameters);
		}
		GameStarted(fastStart);
		currentGameState = GameState.GameStarted;
	}

	public void FinishGame(bool success) {
		if (currentGameState == GameState.GameFinished)
			return;

		currentGameState = GameState.GameFinished;
		GameFinished(success);
		HideUIHookAid();
		if (!success) {
			SoundManager.Instance.CreateOneShot(AudioConfigDatabase.Instance.failure);
			targetTimeScale = 0.1f;
			Time.timeScale = 0.1f;
			//targetTimeScale = ConfigDatabase.Instance.normalSpeed;
			CameraController.Instance.SwitchGreyScale(true);
			StartCoroutine(ReInitiliazeGameAfter());
		} else {
			SoundManager.Instance.CreateOneShot(AudioConfigDatabase.Instance.victory);
			UnlockNextLevel();
			StartCoroutine(FinishingCoroutine());
		}

		if (!isDebugTestLevelMode) {
			string levelName = LevelResourceDatabase.Instance.GetResourceWithLevelIndex(LevelController.Instance.currentLevelIndex);
			int tries = SavedDataManager.Instance.GetLevelSaveDataWithLevelIndex(LevelController.Instance.currentLevelIndex).numberOfTries;
			Dictionary<string, object> parameters = new Dictionary<string, object>();
			parameters.Add("name", levelName);
			parameters.Add("try", tries);
			parameters.Add("success", success);
			AnalyticsManager.LogEvent("LevelFinished", parameters);
		}
	}

	private void UnlockNextLevel() {
		int currLevelIndex = LevelController.Instance.currentLevelIndex;
		SavedDataManager.Instance.GetLevelSaveDataWithLevelIndex(currLevelIndex).levelCompleted = true;
		LevelSaveDatabase.LevelSaveData nextSaveData = SavedDataManager.Instance.GetLevelSaveDataWithLevelIndex(currLevelIndex + 1);
		if(nextSaveData != null)
		nextSaveData.isUnlocked = true;
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
		yield return new WaitForSecondsRealtime(0.75f);
		if (currentWorm != null)
			currentWorm.Die();
		ReInitGame();
	}

	private void ReInitGame() {
		StartCoroutine(ReinitializingGame());
	}

	IEnumerator ReinitializingGame() {
		FlashTransition.Instance.TransitionIn();
		//ImageTransitionHandler.Instance.TransitionIn();
		yield return new WaitForSecondsRealtime(0.5f);
		targetTimeScale = ConfigDatabase.Instance.normalSpeed;
		canStartSlowingTime = false;
		ReinitalizeGame();
		yield return new WaitForSeconds(ConfigDatabase.Instance.reinitalizingDuration);
		CameraController.Instance.SwitchGreyScale(false);
		FlashTransition.Instance.TransitionOut();
		//ImageTransitionHandler.Instance.TransitionOut();
		StartTheGame(true);
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

