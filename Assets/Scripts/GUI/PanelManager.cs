﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PanelManager : MonoBehaviour {
	private static PanelManager instance = null;
	public static PanelManager Instance {
		get {
			if (instance == null) {
				instance = FindObjectOfType<PanelManager>();
			}
			return instance;
		}
	}

	public List<Panel> panels = new List<Panel>();
	public Canvas mainCanvas;
	public RectTransform mainCanvasRect;

	private Panel currentlyOpenedPanel = null;
	private bool panelTransitionInProgress = false;

	private void Awake() {
		mainCanvas = GetComponentInParent<Canvas>();
		mainCanvasRect = mainCanvas.GetComponent<RectTransform>();
		foreach (var panel in panels) {
			panel.Initialize();
		}
		TryOpenPanel(panels[0]);
	}

	public void TryOpenPanel(Panel p) {
		if (panelTransitionInProgress)
			return;

		if (currentlyOpenedPanel != null)
			CloseCurrentlyOpenPanelThenOpen(p);
		else {
			OpenPanel(p);
		}
	}

	public void TryOpenPanel(int panelIndex) {
		TryOpenPanel(panels[panelIndex]);
	}

	private void OpenPanel(Panel p) {
		panelTransitionInProgress = true;
		p.gameObject.SetActive(true);
		p.Open(() => { currentlyOpenedPanel = p; panelTransitionInProgress = false; });
	}

	private void CloseCurrentlyOpenPanelThenOpen(Panel panelToOpen) {
		ClosePanel(currentlyOpenedPanel,delegate { OpenPanel(panelToOpen); });
	}

	public void ClosePanel(Panel p, Action panelWasClosed) {
		if (panelTransitionInProgress)
			return;
		panelTransitionInProgress = true;
		p.Close(() => { currentlyOpenedPanel = null; panelTransitionInProgress = false; p.gameObject.SetActive(false); panelWasClosed(); });
	}

	public void ClosePanel(Panel p) {
		ClosePanel(p, delegate { });
	}
}