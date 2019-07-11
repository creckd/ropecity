using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPad : MonoBehaviour {

	public CharacterType initializedType;

	public void InitalizeCharacter(CharacterType characterType) {
		initializedType = characterType;
	}
}
