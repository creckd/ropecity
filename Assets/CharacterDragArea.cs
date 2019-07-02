using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class CharacterDragArea : MonoBehaviour, IEndDragHandler, IDragHandler {

	public Action<Vector2> OnDragging = delegate { };
	public Action<Vector2> DragFinished = delegate { };

	public void OnDrag(PointerEventData eventData) {
		OnDragging(eventData.delta / Screen.width);
	}

	public void OnEndDrag(PointerEventData eventData) {
		DragFinished(eventData.delta / Screen.width);
	}
}
