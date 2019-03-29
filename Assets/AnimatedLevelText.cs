using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedLevelText : MonoBehaviour {

	public Text stageNameText;
	public Text triesText;

	private Animator anim;
	private const string ShowAnimationStateName = "Show";
	private const string HideAnimationStateName = "Hide";

	private float timeActivated;
	private float duration;
	private bool shown = false;

	private void Awake() {
		anim = GetComponent<Animator>();
	}

	public void ShowLevelText(float disappearAfterSeconds, string stage, string tries) {
		anim.Play(ShowAnimationStateName, 0);
		stageNameText.text = stage;
		triesText.text = tries;
		duration = disappearAfterSeconds;
		shown = true;
		timeActivated = Time.realtimeSinceStartup;
	}

	private void Update() {
		if (shown) {
			if (Time.realtimeSinceStartup - timeActivated > duration) {
				anim.Play(HideAnimationStateName,0);
				shown = false;
			}
		}
	}
}
