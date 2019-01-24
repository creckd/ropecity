using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour {

	private static CameraShake instance = null;
	public static CameraShake Instance {
		get {
			if (instance == null)
				instance = FindObjectOfType<CameraShake>();
			return instance;
		}
	}

	public Transform camTransform;

	private float shakeDuration = 0f;
	private float shakeAmount = 0.7f;
	private float decreaseFactor = 1.0f;

	void Awake() {
		if (camTransform == null) {
			camTransform = GetComponent(typeof(Transform)) as Transform;
		}
	}

	public void Shake(float duration,float amount,float decrease) {
		this.shakeDuration = duration;
		this.shakeAmount = amount;
		this.decreaseFactor = decrease;
	}

	void Update() {
		if (shakeDuration > 0) {
			camTransform.localPosition += Random.insideUnitSphere * shakeAmount;

			shakeDuration -= Time.deltaTime * decreaseFactor;
		} else {
			shakeDuration = 0f;
			//camTransform.localPosition = originalPos;
		}
	}
}