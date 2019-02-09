using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Panel : MonoBehaviour {

	public List<Fader> faders = new List<Fader>();

	public void Initialize() {

		foreach (var f in GetComponentsInChildren<Fader>()) {
			faders.Add(f);
		}
		gameObject.SetActive(false);
	}

	public void Open(Action openedCallBack) {
		StartCoroutine(OpeningAnimation(openedCallBack));
	}

	public void Close(Action closedCallBack) {
		StartCoroutine(ClosingAnimation(closedCallBack));
	}

	public void OpenPanel(int panelIndex) {
		PanelManager.Instance.TryOpenPanel(panelIndex);
	}

	IEnumerator OpeningAnimation(Action openedCallBack) {
		if (faders.Count > 0) {
			faders = faders.OrderBy(x => x.order).ToList();
			int currentOrder;
			for (int i = 0; i < faders.Count; i++) {
				currentOrder = faders[i].order;
				if (i < faders.Count-1 && faders[i + 1].order == currentOrder) {
					float highestWaitingTime = faders[i].fadeTime;
					while (i < faders.Count - 1 && faders[i + 1].order == currentOrder) {
						faders[i].FadeIn();
						if (faders[i + 1].fadeTime >= highestWaitingTime)
							highestWaitingTime = faders[i].fadeTime;
						i++;
					}
					faders[i].FadeIn();
					yield return new WaitForSeconds(highestWaitingTime);
				} else {
					faders[i].FadeIn();
					yield return new WaitForSeconds(faders[i].fadeTime);
				}
			}
		} else {
			yield return null;
		}
		if (openedCallBack != null)
			openedCallBack();
	}

	IEnumerator ClosingAnimation(Action closedCallBack) {
		faders = faders.OrderBy(x => x.order).ToList();
		if (faders.Count > 0) {
			faders = faders.OrderBy(x => x.order).ToList();
			int currentOrder;
			for (int i = 0; i < faders.Count; i++) {
				currentOrder = faders[i].order;
				if (i < faders.Count-1 && faders[i + 1].order == currentOrder) {
					float highestWaitingTime = faders[i].fadeTime;
					while (i < faders.Count - 1 && faders[i + 1].order == currentOrder) {
						faders[i].FadeOut();
						if (faders[i + 1].fadeTime >= highestWaitingTime)
							highestWaitingTime = faders[i].fadeTime;
						i++;
					}
					faders[i].FadeOut();
					yield return new WaitForSeconds(highestWaitingTime);
				} else {
					faders[i].FadeOut();
					yield return new WaitForSeconds(faders[i].fadeTime);
				}
			}
		} else {
			yield return null;
		}
		if (closedCallBack != null)
			closedCallBack();
	}
}
