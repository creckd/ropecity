using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Panel : MonoBehaviour {

	public virtual void Initialize() {}

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

	public virtual void OnStartedOpening() { } 
	public virtual void OnOpened() { }
	public virtual void OnStartedClosing() { }
	public virtual void OnClosed() { }
}
