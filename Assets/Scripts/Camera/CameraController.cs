using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	private static CameraController instance = null;
	public static CameraController Instance {
		get {
			if (instance == null)
				instance = FindObjectOfType<CameraController>();
			return instance;
		}
	}

	private Transform target = null;
	private Vector3 offset;
	private Vector3 cameraStartingPosition;
	private Quaternion cameraStartingRotation;
	private Vector3 cannonCameraStartPosition;
	private bool initializedOffsets = false;

	public float xDifferenceAllowed = 1f;
	public float yDifferenceAllowed = 1f;
	public bool horizontalMovementLocked = false;
	public bool verticalMovementLocked = false;
	public float compensationSpeed = 10f;

	public AnimationCurve sweepCurve;
	public AnimationCurve startSweepCurve;
	public AnimationCurve deathSweepCurve;
	public float sweepingTime = 1f;
	public float startingSweepTime = 2f;
	public float deathSweepTime = 0.5f;

	public Vector3 deathCameraOffset;
	public Vector3 deathCameraEulerRot;

	private float lastHookedPositionY = 0f;
	private float lastHookedTime = 0f;

	public SimpleImageEffectApplier greyScaleImgEffect;
	public float greyScaleChangeTime = 1f;

	private Vector2 referenceResolution = new Vector2(2436, 1125);
	public float cameraResolutionScaleAmount = 1f;

	private void Awake() {
		GameController.Instance.GameFinished += GameFinished;
		GameController.Instance.ReinitalizeGame += ReinitalizeCamera;
		GameController.Instance.LandedHook += LandedHook;
		GameController.Instance.WormDiedAtPosition += WormDied;
		greyScaleImgEffect.enabled = false;

		Vector2 currentResolution = new Vector2(Screen.width, Screen.height);
		float referenceAspect = referenceResolution.x / referenceResolution.y;
		float currentAspect = currentResolution.x / currentResolution.y;

		transform.position -= new Vector3(0f, 0f, cameraResolutionScaleAmount * (1 - Mathf.Clamp01(currentAspect / referenceAspect)));
	}

	public void SwitchGreyScale(bool state) {
		greyScaleImgEffect.enabled = state;
		if (state)
			StartCoroutine(TurnOnGreyScale());
	}

	IEnumerator TurnOnGreyScale() {
		float timer = 0f;
		while (timer <= greyScaleChangeTime) {
			timer += Time.unscaledDeltaTime;
			greyScaleImgEffect.mat.SetFloat("_GlitchStrength", timer / greyScaleChangeTime);
			yield return null;
		}
	}

	private void LandedHook(Vector3 hp) {
		lastHookedPositionY = hp.y;
		lastHookedTime = Time.realtimeSinceStartup;
	}

	private void Start() {
		cameraStartingPosition = transform.position;
		cameraStartingRotation = transform.rotation;
		transform.position = CannonCameraPosition.Instance.transform.position;
		transform.rotation = CannonCameraPosition.Instance.transform.rotation;
		if (!GameController.Instance.isDebugTestLevelMode) {
			horizontalMovementLocked = !LevelController.Instance.settings.isHorizontalCameraMovementEnabled;
			verticalMovementLocked = !LevelController.Instance.settings.isVerticalCameraMovementEnabled;
		}
	}

	public void StartCinematic() {
		StartCoroutine(SweepToStartingPosition());
	}

	public void StartTracking(Transform target) {
		this.target = target;

		if (!initializedOffsets) {
			initializedOffsets = true;
			offset = target.position - cameraStartingPosition;
			//cameraStartingPosition = transform.position;
		}

	}

	private void StopTracking() {
		this.target = null;
	}

	private void GameFinished(bool win) {
		if (win) {
			StopTracking();
			StartCoroutine(SweepToVictoryPosition(GetVictoryCameraTransfom()));
		}
	}

	private void WormDied(Vector3 deathPos) {
		StopTracking();
		//IngameBlurController.Instance.BlurImage(0.25f, true, true, true);
		StartCoroutine(SweepToDeathPosition(deathPos));
	}

	IEnumerator SweepToVictoryPosition(Transform victoryTransform) {
		float timer = 0f;
		Vector3 defPosition = transform.position;
		Quaternion defRotation = transform.rotation;
		while (timer < sweepingTime) {
			timer += Time.unscaledDeltaTime;
			transform.position = Vector3.Lerp(defPosition, victoryTransform.position, sweepCurve.Evaluate(timer / sweepingTime));
			transform.rotation = Quaternion.Lerp(defRotation, victoryTransform.rotation, sweepCurve.Evaluate(timer / sweepingTime));
			yield return null;
		}
	}

	IEnumerator SweepToStartingPosition() {
		Vector3 currPos = transform.position;
		float timer = 0f;
		while (timer <= startingSweepTime) {
			timer += Time.unscaledDeltaTime;
			transform.position = Vector3.Lerp(currPos, cameraStartingPosition, startSweepCurve.Evaluate(timer / startingSweepTime));
			transform.rotation = Quaternion.Lerp(CannonCameraPosition.Instance.transform.rotation, cameraStartingRotation, startSweepCurve.Evaluate(timer / startingSweepTime));
			yield return null;
		}
	}

	IEnumerator SweepToDeathPosition(Vector3 deathPosition) {
		Vector3 currPosition = transform.position;
		float timer = 0f;
		while (timer <= deathSweepTime) {
			timer += Time.unscaledDeltaTime;
			transform.position = Vector3.Lerp(currPosition, deathPosition + deathCameraOffset, deathSweepCurve.Evaluate(timer / deathSweepTime));
			transform.rotation = Quaternion.Lerp(cameraStartingRotation, Quaternion.Euler(deathCameraEulerRot.x,deathCameraEulerRot.y,deathCameraEulerRot.z), deathSweepCurve.Evaluate(timer / deathSweepTime));
			yield return null;
		}
	}

	private Transform GetVictoryCameraTransfom() {
		return VictoryCameraPosition.Instance.transform;
	}

	void LateUpdate () {
		if (target != null && target.gameObject.activeSelf) {
			Vector3 wormCameraPosition = target.position - offset;
			if(!horizontalMovementLocked)
			if (Mathf.Abs(wormCameraPosition.x - transform.position.x) > xDifferenceAllowed) {
				float diff = wormCameraPosition.x - transform.position.x;
				transform.position += Vector3.right * Mathf.Sign(diff) * Mathf.Abs(xDifferenceAllowed - Mathf.Abs(diff)) * Time.deltaTime * compensationSpeed;
			}

			float wormY = GameController.Instance.currentWorm.transform.position.y;
			if (lastHookedPositionY != 0f && Time.realtimeSinceStartup - lastHookedTime <= 2f) {
				float t = GameController.Instance.currentWorm.landedHook ? 0.5f : 0.25f;
				wormY = Mathf.Lerp(wormY, lastHookedPositionY, t);
			}
			if(!verticalMovementLocked)
			if (GameController.Instance.currentWorm.gameObject.activeSelf && Mathf.Abs(wormY - transform.position.y) > yDifferenceAllowed) {
				float diff = wormY - transform.position.y;
				transform.position += Vector3.up * Mathf.Sign(diff) * Mathf.Abs(yDifferenceAllowed - Mathf.Abs(diff)) * Time.deltaTime * compensationSpeed;
			}
			if(GameController.Instance.currentGameState == GameState.GameStarted)
			CheckIfWormInsideCamera();
		}
	}

	void CheckIfWormInsideCamera() {
		Camera cam = GetComponent<Camera>();
		Worm worm = GameController.Instance.currentWorm;
		float distance = (worm.transform.position - transform.position).magnitude;
		float frustumHeight = 2.0f * distance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
		float frustumWidth = frustumHeight * cam.aspect;

		bool wormOutOfView = false;
		if (Mathf.Abs(worm.transform.position.x - transform.position.x) > frustumWidth / 2f)
			wormOutOfView = true;
		if (Mathf.Abs(worm.transform.position.y - transform.position.y) > frustumHeight / 2f)
			wormOutOfView = true;

		if (wormOutOfView)
			worm.Die();
	}


	private void ReinitalizeCamera() {
		StartCoroutine(CameraReinitalizing());
	}

	IEnumerator CameraReinitalizing() {
		float timer = 0f;
		Vector3 currentCameraPosition = transform.position;
		while (timer <= ConfigDatabase.Instance.reinitalizingDuration) {
			transform.position = Vector3.Lerp(currentCameraPosition, cameraStartingPosition, timer / ConfigDatabase.Instance.reinitalizingDuration);
			transform.rotation = cameraStartingRotation;
			timer += Time.deltaTime;
			yield return null;
		}
		transform.position = cameraStartingPosition;
	}
}
