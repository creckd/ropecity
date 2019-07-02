using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterRotator : MonoBehaviour {

    public GameObject sampleObject;
    public float radius = 10f;
	public float sensitivity = 1f;
	public float snapSpeed = 1f;
    public List<GameObject> createdObjects = new List<GameObject>();
    private float XVelocity = 0f;

    public bool beingDragged = false;
    public int currentlySelectedCharIndex = 0;
    GameObject closestObj;

    public void Initialize() {
        float degreePerObject = 360f / ConfigDatabase.Instance.characters.Length;
        for (int i = 0; i < ConfigDatabase.Instance.characters.Length; i++) {
            GameObject characterObject = Instantiate(sampleObject, Vector3.zero, Quaternion.identity,transform) as GameObject;
            characterObject.name = "Character Object Number " + i.ToString();
            characterObject.transform.position = transform.position + (-transform.forward * radius);
            characterObject.transform.RotateAround(transform.position, Vector3.up, degreePerObject * i);
            characterObject.transform.rotation = Quaternion.identity;
            //ModellCharacterSprite modellCharacterSprite = spriteObj.GetComponent<ModellCharacterSprite>();
            //modellCharacterSprite.Initialize(CharacterConfigDatabase.Instance.characters[i].color, SaveManager.Instance.GetCharacterGraphicsData(i, CharacterCustomizationPanelController.CategoryType.headwear), SaveManager.Instance.GetCharacterGraphicsData(i, CharacterCustomizationPanelController.CategoryType.accessory), SaveManager.Instance.GetCharacterGraphicsData(i, CharacterCustomizationPanelController.CategoryType.glass));
            createdObjects.Add(characterObject);
        }

		foreach (var spObj in createdObjects) {
			spObj.transform.LookAt(spObj.transform.position - (spObj.transform.position - transform.position).normalized, Vector3.up);
		}

		closestObj = createdObjects[0].gameObject;
		sampleObject.gameObject.SetActive(false);
    }

    public GameObject GetCharacterByIndex(int index) {
        return createdObjects[index];
    }

    public GameObject GetCurrentlySelectedModell() {
        return createdObjects[currentlySelectedCharIndex];
    }

    public void SetVelocity(float xDelta) {
		transform.Rotate(new Vector3(0f, xDelta * sensitivity, 0f));
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
			// NEW CHAR SELECTED
        }

        if (!beingDragged)
            XVelocity = (closestObj.transform.position.x - transform.position.x) * snapSpeed;
        transform.Rotate(new Vector3(0f, XVelocity, 0f));
    }
}
