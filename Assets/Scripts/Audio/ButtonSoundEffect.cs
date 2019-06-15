using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSoundEffect : MonoBehaviour {

	private Button button;

	public CustomClipMemoryFriendly clipToUse;

	private void Awake() {
		button = GetComponent<Button>();
		button.onClick.AddListener( () => {
			SoundManager.Instance.CreateOneShot(clipToUse.CloneToCustomClip());
		});
	}
}
