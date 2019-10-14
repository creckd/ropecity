using System;
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

	public const string defaultOpenedPanelChangedTag = "DefaultOpenedPanelShouldBeThisPanel";
	public int defaultPanelID = 0;

	public List<Panel> panels = new List<Panel>();

	[HideInInspector]
	public Canvas mainCanvas;
	[HideInInspector]
	public RectTransform mainCanvasRect;

	[HideInInspector]
	public Panel currentlyOpenedPanel = null;
	private bool panelTransitionInProgress = false;

	public void InitializeGUI() {
		mainCanvas = GetComponentInParent<Canvas>();
		mainCanvasRect = mainCanvas.GetComponent<RectTransform>();
		foreach (var panel in panels) {
			panel.Initialize();
		}
		object data;
		if (Messenger.Instance.GetMessage(defaultOpenedPanelChangedTag, out data))
			defaultPanelID = (int)data;
		TryOpenPanel(panels[defaultPanelID]);
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

	public void TryOpenPanel(Panel p, Dictionary<object,object> message) {
		if (panelTransitionInProgress)
			return;

		if (currentlyOpenedPanel != null)
			CloseCurrentlyOpenPanelThenOpen(p,message);
		else {
			OpenPanel(p,message);
		}
	}

	public void TryOpenPanel(int panelIndex) {
		TryOpenPanel(panels[panelIndex]);
	}

	public void TryOpenPanel(int panelIndex, Dictionary<object,object> message) {
		TryOpenPanel(panels[panelIndex],message);
	}

	private void OpenPanel(Panel p) {
		panelTransitionInProgress = true;
		p.gameObject.SetActive(true);
		p.Open(() => { currentlyOpenedPanel = p; panelTransitionInProgress = false; });
	}

	private void OpenPanel(Panel p, Dictionary<object,object> message) {
		panelTransitionInProgress = true;
		p.gameObject.SetActive(true);
		p.Open(() => { currentlyOpenedPanel = p; panelTransitionInProgress = false; }, message);
	}

	private void CloseCurrentlyOpenPanelThenOpen(Panel panelToOpen) {
		ClosePanel(currentlyOpenedPanel,delegate { OpenPanel(panelToOpen); });
	}

	private void CloseCurrentlyOpenPanelThenOpen(Panel panelToOpen, Dictionary<object,object> message) {
		ClosePanel(currentlyOpenedPanel, delegate { OpenPanel(panelToOpen,message); });
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
