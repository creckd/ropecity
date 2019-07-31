using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PopupManager : MonoBehaviour {
	private static PopupManager instance = null;
	public static PopupManager Instance {
		get {
			if (instance == null) {
				instance = FindObjectOfType<PopupManager>();
			}
			return instance;
		}
	}

	public List<Panel> popups = new List<Panel>();

	[HideInInspector]
	public Canvas mainCanvas;
	[HideInInspector]
	public RectTransform mainCanvasRect;

	private Panel currentlyOpenedPopup = null;
	private bool panelTransitionInProgress = false;

	public void InitializeGUI() {
		mainCanvas = GetComponentInParent<Canvas>();
		mainCanvasRect = mainCanvas.GetComponent<RectTransform>();
		foreach (var panel in popups) {
			panel.Initialize();
			panel.gameObject.SetActive(false);
		}
	}

	public void TryOpenPopup(Panel p) {
		if (panelTransitionInProgress)
			return;

		OpenPopup(p);
	}

	public void TryOpenPopup(Panel p, Dictionary<object,object> message) {
		if (panelTransitionInProgress)
			return;

		OpenPopup(p, message);
	}

	public void TryOpenPopup(int panelIndex) {
		TryOpenPopup(popups[panelIndex]);
	}

	public void TryOpenPopup(int panelIndex, Dictionary<object,object> message) {
		TryOpenPopup(popups[panelIndex],message);
	}

	private void OpenPopup(Panel p) {
		panelTransitionInProgress = true;
		p.gameObject.SetActive(true);
		p.Open(() => { currentlyOpenedPopup = p; panelTransitionInProgress = false; });
	}

	private void OpenPopup(Panel p, Dictionary<object,object> message) {
		panelTransitionInProgress = true;
		p.gameObject.SetActive(true);
		p.Open(() => { currentlyOpenedPopup = p; panelTransitionInProgress = false; }, message);
	}

	private void CloseCurrentlyOpenPanelThenOpen(Panel panelToOpen) {
		ClosePopup(currentlyOpenedPopup,delegate { OpenPopup(panelToOpen); });
	}

	public void ClosePopup(Panel p, Action panelWasClosed) {
		if (panelTransitionInProgress)
			return;
		panelTransitionInProgress = true;
		p.Close(() => { currentlyOpenedPopup = null; panelTransitionInProgress = false; p.gameObject.SetActive(false); panelWasClosed(); });
	}

	public void ClosePopup(Panel p) {
		ClosePopup(p, delegate { });
	}
}
