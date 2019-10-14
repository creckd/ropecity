using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class CharacterRotator : MonoBehaviour {

	public Action<CharacterPad> OnNewCharacterPadSelected = delegate { };

    public CharacterPad samplePad;
    public float radius = 10f;
	public float sensitivity = 1f;
	public float snapSpeed = 1f;
	public float snapMargin = 1f;
	public float dragAmount = 1f;
	public float releaseImpulseMultiplier = 1f;
    public List<CharacterPad> createdObjects = new List<CharacterPad>();
	public bool beingDragged = false;
	public int currentlySelectedCharIndex = 0;

	private float XVelocity = 0f;
    private GameObject closestObj;

    public void Initialize() {
		int numberOfCharacters = ConfigDatabase.Instance.characters.Length;
		float degreePerObject = 360f / numberOfCharacters;

		CharacterData[] charactersInOrder = new CharacterData[numberOfCharacters];
		Array.Copy(ConfigDatabase.Instance.characters, charactersInOrder, numberOfCharacters);
		charactersInOrder = charactersInOrder.OrderBy(character => (int)character.characterPrice).ThenBy(character => character.unlockedByLevelIndex).ToArray();

		for (int i = 0; i < numberOfCharacters; i++) {
			CharacterData cData = charactersInOrder[i];
			CharacterPad characterPad = (Instantiate(samplePad.gameObject, Vector3.zero, Quaternion.identity, transform) as GameObject).GetComponent<CharacterPad>();
            characterPad.name = "Character type: " + cData.characterType.ToString() + " on Character pad: " + i.ToString();
            characterPad.transform.position = transform.position + (-transform.forward * radius);
            characterPad.transform.RotateAround(transform.position, Vector3.up, degreePerObject * i);
            characterPad.transform.rotation = Quaternion.identity;
			characterPad.transform.LookAt(characterPad.transform.position - (characterPad.transform.position - transform.position).normalized, Vector3.up);
			characterPad.InitalizeCharacter(cData.characterType);
			characterPad.RefreshGraphics();

            createdObjects.Add(characterPad);
        }

		closestObj = createdObjects[0].gameObject;
		samplePad.gameObject.SetActive(false);
		OnNewCharacterPadSelected(createdObjects[0]);
    }

    public CharacterPad GetCharacterPadByIndex(int index) {
        return createdObjects[index];
    }

    public CharacterPad GetCurrentlySelectedPad() {
        return createdObjects[currentlySelectedCharIndex];
    }

    public void Rotate(float xDelta) {
		startedSnapping = false;
		transform.Rotate(new Vector3(0f, xDelta * sensitivity, 0f));
	}

	public void SetVelocity(float magnitude) {
		startedSnapping = false;
		XVelocity = magnitude * releaseImpulseMultiplier;
	}

	public void RefreshAllPlatformGraphics() {
		foreach (var cO in createdObjects) {
			cO.RefreshGraphics();
		}
	}

	private bool startedSnapping = false;

    void Update() {
        int prevIndex = currentlySelectedCharIndex;
        for (int i = 0; i < createdObjects.Count; i++) {
            if (Vector3.Distance(createdObjects[i].transform.position, transform.position - Vector3.forward * radius) <= Vector3.Distance(closestObj.transform.position, transform.position - Vector3.forward * radius)) {
                closestObj = createdObjects[i].gameObject;
                currentlySelectedCharIndex = i;
            }
        }
        if (prevIndex != currentlySelectedCharIndex) {
			OnNewCharacterPadSelected(createdObjects[currentlySelectedCharIndex]);
			createdObjects[prevIndex].Defocused();
        }

		if (!beingDragged) {
			if (Mathf.Abs(XVelocity) <= snapMargin || startedSnapping) {
				if (!startedSnapping)
					startedSnapping = true;
				XVelocity = (closestObj.transform.position.x - transform.position.x) * snapSpeed;
			} else if(!startedSnapping) {
				XVelocity = XVelocity - (Time.deltaTime * Mathf.Sign(XVelocity)) * dragAmount;
			}
			transform.Rotate(new Vector3(0f, XVelocity, 0f));
		} else {
			XVelocity = 0f;
		}
    }

	public void SnapToCharacterIndex(int index) {
		float angleToRotate = Vector3.SignedAngle((transform.position - Vector3.forward * radius) - transform.position, createdObjects[index].transform.position - transform.position, Vector3.up);
		transform.Rotate(new Vector3(0f, -angleToRotate, 0f));
	}

	public void SnapToCharacterType(CharacterType type) {
		CharacterPad pad = null;
		int i = 0;
		for (i = 0; i < createdObjects.Count; i++) {
			if (createdObjects[i].initializedType == type) {
				pad = createdObjects[i];
				break;
			}
		}
		if (pad != null) {
			SnapToCharacterIndex(i);
		}
	}
}
