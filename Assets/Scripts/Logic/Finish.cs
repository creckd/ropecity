using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Finish : LevelObject {

	private const string finishAnimationStateName = "Finished";

	public GameObject originalGlassObject;
	public GameObject brokenGlassHolderObject;
	private Rigidbody[] shards;

	private Animator anim;
	private Material mat;

	//public Text speedText;

	private void Start() {
		if (GameController.Instance.currentGameState == GameState.Initialized) {
			anim = GetComponent<Animator>();
			shards = brokenGlassHolderObject.GetComponentsInChildren<Rigidbody>();
			brokenGlassHolderObject.gameObject.SetActive(false);
			GameController.Instance.ReinitalizeGame += ReInitFinish;
		}
	}

	private void ReInitFinish() {
		anim.Play("Default", 0, 0f);
		//speedText.text = "";
	}

	private void OnTriggerEnter2D(Collider2D other) {
		bool isWorm = other.CompareTag("Player");

		if (isWorm && GameController.Instance.currentGameState == GameState.GameStarted) {
			Worm worm = other.GetComponent<Worm>();
			BreakGlass(worm);
			anim.Play(finishAnimationStateName, 0, 0f);
			//speedText.text = ConvertXVelocityToKMH(worm.Velocity.x).ToString();
			Vector2 direction = ((transform.position + (transform.forward * 5f)) - worm.transform.position).normalized;
			worm.AddForce(direction * 0.5f);
			GameController.Instance.FinishGame(true);
		}
	}

	private void BreakGlass(Worm worm) {
		originalGlassObject.gameObject.SetActive(false);
		brokenGlassHolderObject.gameObject.SetActive(true);
		for (int i = 0; i < shards.Length; i++) {
			float distanceFromworm = Vector3.Distance(worm.transform.position, shards[i].transform.position);
			float forceMultiplier = 50f;
			float finalForce = (worm.Velocity.magnitude / (Mathf.Clamp(distanceFromworm, 1f, Mathf.Infinity) * 0.1f)) * forceMultiplier;
			//Debug.Log(finalForce);
			float minimumForceToMove = 50f;
			if (finalForce < minimumForceToMove)
				shards[i].isKinematic = true;
			else {
				shards[i].AddForce((Vector3.right + (RandomVector3()).normalized * 0.1f) * finalForce, ForceMode.Impulse);
				shards[i].AddTorque(RandomVector3().normalized * forceMultiplier * 0.1f,ForceMode.Impulse);
			}
		}
	}

	private Vector3 RandomVector3() {
		return new Vector3(r01(), r01(), r01());
	}

	private float r01() {
		return Random.Range(0f, 1f);
	}

	//private int ConvertXVelocityToKMH(float xVelocity) {
	//	return Mathf.CeilToInt(Mathf.Abs(xVelocity * 50f));
	//}
}
