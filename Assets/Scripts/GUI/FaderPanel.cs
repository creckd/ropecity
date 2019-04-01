using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FaderPanel : Panel {

	[HideInInspector]
	public List<Fader> faders = new List<Fader>();

	public override void Initialize() {

		foreach (var f in GetComponentsInChildren<Fader>()) {
			faders.Add(f);
		}
		base.Initialize();
	}

	IEnumerator OpeningAnimationRoutine(Action openedCallBack) {
		if (faders.Count > 0) {
			faders = faders.OrderBy(x => x.order).ToList();
			int currentOrder;
			for (int i = 0; i < faders.Count; i++) {
				currentOrder = faders[i].order;
				if (i < faders.Count-1 && faders[i + 1].order == currentOrder) {
					float highestWaitingTime = faders[i].fadeTime + faders[i].delay;
					while (i < faders.Count - 1 && faders[i + 1].order == currentOrder) {
						faders[i].FadeIn();
						if (faders[i + 1].fadeTime + faders[i + 1].delay >= highestWaitingTime)
							highestWaitingTime = faders[i].fadeTime + faders[i].delay;
						i++;
					}
					if (faders[i].delay + faders[i].fadeTime > highestWaitingTime)
						highestWaitingTime = faders[i].fadeTime + faders[i].delay;
					faders[i].FadeIn();
					yield return new WaitForSecondsRealtime(highestWaitingTime);
				} else {
					faders[i].FadeIn();
					yield return new WaitForSecondsRealtime(faders[i].fadeTime + faders[i].delay);
				}
			}
		} else {
			yield return null;
		}
		OnOpened();
		if (openedCallBack != null)
			openedCallBack();
	}

	IEnumerator ClosingAnimationRoutine(Action closedCallBack) {
		faders = faders.OrderByDescending(x => x.order).ToList();
		if (faders.Count > 0) {
			faders = faders.OrderByDescending(x => x.order).ToList();
			int currentOrder;
			for (int i = 0; i < faders.Count; i++) {
				currentOrder = faders[i].order;
				if (i < faders.Count-1 && faders[i + 1].order == currentOrder) {
					float highestWaitingTime = faders[i].fadeTime + faders[i].delay;
					while (i < faders.Count - 1 && faders[i + 1].order == currentOrder) {
						faders[i].FadeOut();
						if (faders[i + 1].fadeTime + faders[i+1].delay >= highestWaitingTime)
							highestWaitingTime = faders[i].fadeTime + faders[i].delay;
						i++;
					}
					if (faders[i].delay + faders[i].fadeTime > highestWaitingTime)
						highestWaitingTime = faders[i].fadeTime + faders[i].delay;
					faders[i].FadeOut();
					yield return new WaitForSecondsRealtime(highestWaitingTime);
				} else {
					faders[i].FadeOut();
					yield return new WaitForSecondsRealtime(faders[i].fadeTime + faders[i].delay);
				}
			}
		} else {
			yield return null;
		}
		OnClosed();
		if (closedCallBack != null)
			closedCallBack();
	}

	public override void OpeningAnimation(Action cb) {
		StartCoroutine(OpeningAnimationRoutine(cb));
	}

	public override void ClosingAnimation(Action cb) {
		StartCoroutine(ClosingAnimationRoutine(cb));
	}
}
