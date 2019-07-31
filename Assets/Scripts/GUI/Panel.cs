using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public abstract class Panel : MonoBehaviour {

	private Button[] buttons;
	private Dictionary<Button, bool> savedButtonInteractibilityStates = new Dictionary<Button, bool>();

	public virtual void Initialize() {
		buttons = GetComponentsInChildren<Button>();
		gameObject.SetActive(false);
	}

	public void ReInitializeButtons() {
		buttons = GetComponentsInChildren<Button>();
	}

	public void DeactivateButtons() {
		savedButtonInteractibilityStates.Clear();
		foreach (var but in buttons) {
			savedButtonInteractibilityStates.Add(but, but.interactable);
			but.interactable = false;
		}
	}

	public void ActivateButtons() {
		bool interactibility = false;
		foreach (var but in buttons) {
			if (savedButtonInteractibilityStates.TryGetValue(but, out interactibility)) {
				but.interactable = interactibility;
				continue;
			} else {
				but.interactable = true;
			}
		}
	}

	public abstract void OpeningAnimation(Action cb);
	public abstract void ClosingAnimation(Action cb);

	public void Open(Action openedCallBack) {
		OnStartedOpening();
		OpeningAnimation(openedCallBack);
	}

	public void Open(Action openedCallBack, Dictionary<object, object> message = null) {
		OnStartedOpening(message);
		OpeningAnimation(openedCallBack);
	}

	public void Close(Action closedCallBack) {
		OnStartedClosing();
		ClosingAnimation(closedCallBack);
	}

	public void CloseThisPopup() {
		PopupManager.Instance.ClosePopup(this);
	}

	public void OpenPanel(int panelIndex) {
		PanelManager.Instance.TryOpenPanel(panelIndex);
	}

	public void OpenPopup(int popupIndex) {
		PopupManager.Instance.TryOpenPopup(popupIndex);
	}

	public virtual void OnStartedOpening() { DeactivateButtons(); }
	public virtual void OnStartedOpening(Dictionary<object, object> message) { DeactivateButtons(); }
	public virtual void OnOpened() { ActivateButtons(); }
	public virtual void OnStartedClosing() { DeactivateButtons(); }
	public virtual void OnClosed() { ActivateButtons(); }
}
