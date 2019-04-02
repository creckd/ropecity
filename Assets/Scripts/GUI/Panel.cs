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

	public void DeactivatePanelButtons() {
		savedButtonInteractibilityStates.Clear();
		foreach (var but in buttons) {
			savedButtonInteractibilityStates.Add(but, but.interactable);
			but.interactable = false;
		}
	}

	public void ActivatePanelButtons() {
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

	public void Close(Action closedCallBack) {
		OnStartedClosing();
		ClosingAnimation(closedCallBack);
	}

	public void OpenPanel(int panelIndex) {
		PanelManager.Instance.TryOpenPanel(panelIndex);
	}

	public virtual void OnStartedOpening() { DeactivatePanelButtons(); } 
	public virtual void OnOpened() { ActivatePanelButtons(); }
	public virtual void OnStartedClosing() { DeactivatePanelButtons(); }
	public virtual void OnClosed() { ActivatePanelButtons(); }
}
