using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformCharacter : MonoBehaviour {

	public CharacterType characterType;
	private Animator anim;

	private void Awake() {
		anim = GetComponent<Animator>();
	}
}
