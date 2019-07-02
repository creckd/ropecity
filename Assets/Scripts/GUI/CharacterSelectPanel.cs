using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPanel : AnimatorPanel {
	public CharacterRotator rotator;
	public CharacterDragArea dragArea;

	public override void Initialize() {
		base.Initialize();
		rotator.Initialize();
		dragArea.OnDragging += OnDragging;
		dragArea.DragFinished += DraggingFinished;
	}

	private void OnDragging(Vector2 xDelta) {
		rotator.beingDragged = true;
		rotator.SetVelocity(-xDelta.x);
	}

	private void DraggingFinished(Vector2 xDelta) {
		rotator.SetVelocity(-xDelta.x);
		rotator.beingDragged = false;
	}
}
