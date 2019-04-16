using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReboundIndicator : MonoBehaviour {

	public Image movingPart;
	public CanvasGroup cg;
	private bool currentlyRebounding = false;
	private float targetAlpha = 0;

	private void Awake() {
		cg.alpha = 0;
	}

	public void StartRebound() {
		currentlyRebounding = true;
		targetAlpha = 1f;
		movingPart.transform.Rotate(new Vector3(0f, 0f, Random.Range(0f, 360f)));
	}

	private void Update() {
		if (currentlyRebounding) {
			movingPart.transform.Rotate(new Vector3(0f, 0f, Time.unscaledDeltaTime * ConfigDatabase.Instance.reboundSpeed * 5f));
		}
		cg.alpha = Mathf.Lerp(cg.alpha, targetAlpha, Time.deltaTime * 10f);
	}

	public void StopRebound() {
		currentlyRebounding = false;
		targetAlpha = 0f;
		float angle = Vector2.Angle(Vector2.up, movingPart.transform.up);
		bool successful = angle <= ConfigDatabase.Instance.reboundSuccessAngleMinimum;
		if (successful)
			GameController.Instance.ReboundJumpSuccessful();
	}
}
