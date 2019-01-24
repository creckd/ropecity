using System;
using System.Collections;
using System.Collections.Generic;
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

	public GameState currentGameState = GameState.NotInitialized;

	public Action StartTheGame = delegate { };

	public Worm currentWorm = null;

	private void Awake() {
		currentGameState = GameState.Initialized;
	}

	private void Start() {
		StartTheGame();
		currentGameState = GameState.GameStarted;
	}
}

