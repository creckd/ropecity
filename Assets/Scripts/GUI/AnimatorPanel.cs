using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimatorPanel : Panel {

	private Animator anim;
	private const string openingAnimationStateName = "Open";
	private const string closingAnimationStateName = "Close";

	protected bool panelInTransition = false;

	public override void Initialize() {
		anim = GetComponent<Animator>();
		base.Initialize();
	}

	IEnumerator OpeningAnimationRoutine(Action openedCallBack) {
		anim.Play(openingAnimationStateName, 0);
		yield return null;
		yield return new WaitForSecondsRealtime(anim.GetCurrentAnimatorStateInfo(0).length);
		OnOpened();
		if (openedCallBack != null)
			openedCallBack();
		panelInTransition = false;
	}

	IEnumerator ClosingAnimationRoutine(Action closedCallBack) {
		anim.Play(closingAnimationStateName, 0);
		yield return null;
		yield return new WaitForSecondsRealtime(anim.GetCurrentAnimatorStateInfo(0).length);
		OnClosed();
		if (closedCallBack != null)
			closedCallBack();
		panelInTransition = false;
	}

	public override void OpeningAnimation(Action cb) {
		StartCoroutine(OpeningAnimationRoutine(cb));
	}

	public override void ClosingAnimation(Action cb) {
		StartCoroutine(ClosingAnimationRoutine(cb));
	}

	public override void OnStartedClosing() {
		base.OnStartedClosing();
		panelInTransition = true;
	}

	public override void OnStartedOpening() {
		base.OnStartedOpening();
		panelInTransition = true;
	}
}
