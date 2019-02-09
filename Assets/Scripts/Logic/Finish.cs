using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Finish : MonoBehaviour {

	private const string finishAnimationStateName = "Finished";
	private const string lampColorPropertyName = "_MainColor";

	private Animator anim;
	private Material mat;

	public MeshRenderer bulbMeshRenderer;
	public float lampInterpolationSpeed = 1f;
	public Color lampLightColor;
	public Text speedText;

	private void Start() {
		if (GameController.Instance.currentGameState == GameState.Initialized) {
			anim = GetComponent<Animator>();
			bulbMeshRenderer.material = mat = Instantiate(bulbMeshRenderer.material);
			GameController.Instance.ReinitalizeGame += ReInitFinish;
		}
	}

	private void ReInitFinish() {
		anim.Play("Default", 0, 0f);
		speedText.text = "";
	}

	private void OnTriggerEnter(Collider other) {
		bool isWorm = other.CompareTag("Player");

		if (isWorm && GameController.Instance.currentGameState == GameState.GameStarted) {
			Worm worm = other.GetComponent<Worm>();
			anim.Play(finishAnimationStateName, 0, 0f);
			StartCoroutine(TurnOnLight());
			speedText.text = ConvertXVelocityToKMH(worm.Velocity.x).ToString();
			GameController.Instance.FinishGame(true);
		}
	}

	IEnumerator TurnOnLight() {
		float timer = 0f;
		Color startColor = mat.GetColor(lampColorPropertyName);
		while (timer <= lampInterpolationSpeed) {
			timer += Time.deltaTime;
			mat.SetColor(lampColorPropertyName, Color.Lerp(startColor, lampLightColor, timer / lampInterpolationSpeed));
			yield return null;
		}
	}

	private int ConvertXVelocityToKMH(float xVelocity) {
		return Mathf.CeilToInt(Mathf.Abs(xVelocity * 50f));
	}
}
