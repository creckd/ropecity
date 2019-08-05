using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CharacterRotator : MonoBehaviour {

	public Action<CharacterPad> OnNewCharacterPadSelected = delegate { };

    public CharacterPad samplePad;
    public float radius = 10f;
	public float sensitivity = 1f;
	public float snapSpeed = 1f;
	public float snapMargin = 1f;
	public float dragAmount = 1f;
    public List<CharacterPad> createdObjects = new List<CharacterPad>();
	public bool beingDragged = false;
	public int currentlySelectedCharIndex = 0;

	private float XVelocity = 0f;
    private GameObject closestObj;

    public void Initialize() {
        float degreePerObject = 360f / ConfigDatabase.Instance.characters.Length;
        for (int i = 0; i < ConfigDatabase.Instance.characters.Length; i++) {
			CharacterData cData = ConfigDatabase.Instance.characters[i];
			CharacterPad characterPad = (Instantiate(samplePad.gameObject, Vector3.zero, Quaternion.identity, transform) as GameObject).GetComponent<CharacterPad>(); ;
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
		transform.Rotate(new Vector3(0f, xDelta * sensitivity, 0f));
	}

	public void SetVelocity(float magnitude) {
		XVelocity = magnitude;
	}

	public void RefreshAllPlatformGraphics() {
		foreach (var cO in createdObjects) {
			cO.RefreshGraphics();
		}
	}

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
			if (XVelocity <= snapMargin) {
				XVelocity = (closestObj.transform.position.x - transform.position.x) * snapSpeed;
			} else {
				XVelocity = Mathf.Clamp(XVelocity - Time.deltaTime * dragAmount, 0f, Mathf.Infinity);
			}
		}
        transform.Rotate(new Vector3(0f, XVelocity, 0f));
    }
}
