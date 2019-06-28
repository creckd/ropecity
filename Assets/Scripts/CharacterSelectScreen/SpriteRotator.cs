using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpriteRotator : MonoBehaviour {

    public GameObject sampleObject;
    public float radius = 10f;
    public float XVelocity = 0f;
    public float VelocityLoseAmount = 0.1f;
    public List<ModellCharacterSprite> createdObjects = new List<ModellCharacterSprite>();

    private static SpriteRotator instance;
    public static SpriteRotator Instance {
        get {
            if (instance == null) {
                instance = FindObjectOfType<SpriteRotator>();
            }
            return instance;
        }
    }

    public bool beingDragged = false;
    public int currentlySelectedCharIndex = 0;
    GameObject closestObj;

    void Start() {
        float degreePerObject = 360f / CharacterConfigDatabase.Instance.characters.Count;
        for (int i = 0; i < CharacterConfigDatabase.Instance.characters.Count; i++) {
            GameObject spriteObj = Instantiate(sampleObject, Vector3.zero, Quaternion.identity) as GameObject;
            spriteObj.name = "Sprite Object Number " + i.ToString();
            spriteObj.transform.position = transform.position + (-transform.forward * radius);
            spriteObj.transform.RotateAround(transform.position, Vector3.up, degreePerObject * i);
            spriteObj.transform.SetParent(transform);
            spriteObj.transform.rotation = Quaternion.identity;
            ModellCharacterSprite modellCharacterSprite = spriteObj.GetComponent<ModellCharacterSprite>();
            modellCharacterSprite.Initialize(CharacterConfigDatabase.Instance.characters[i].color, SaveManager.Instance.GetCharacterGraphicsData(i, CharacterCustomizationPanelController.CategoryType.headwear), SaveManager.Instance.GetCharacterGraphicsData(i, CharacterCustomizationPanelController.CategoryType.accessory), SaveManager.Instance.GetCharacterGraphicsData(i, CharacterCustomizationPanelController.CategoryType.glass));
            createdObjects.Add(modellCharacterSprite);
        }

        closestObj = createdObjects[0].gameObject;
    }

    public ModellCharacterSprite GetModellByIndex(int index) {
        return createdObjects[index];
    }

    public ModellCharacterSprite GetCurrentlySelectedModell() {
        return createdObjects[currentlySelectedCharIndex];
    }

    public void SetVelocity(float velocity) {
        XVelocity = velocity;
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
            CharacterCustomizationPanelController.Instance.ButtonNextCharacter(currentlySelectedCharIndex);
            SoundManager.Instance.CreateOneShot(AudioConfigDatabase.Instance.CCPanelSwapSound);
        }

        if (!beingDragged)
            XVelocity = closestObj.transform.position.x - transform.position.x;
        transform.Rotate(new Vector3(0f, XVelocity, 0f));

        foreach (var spObj in createdObjects) {
            spObj.transform.LookAt(spObj.transform.position + Vector3.forward, Vector3.up);
        }
    }


    public void RefreshCharacterStyles(bool instantChange = false) {
        foreach (var cObj in createdObjects) {
            cObj.RefreshStyle(instantChange);
        }
    }

    public void ReRollCharacterAnimations() {
        foreach (var cObj in createdObjects) {
            cObj.ReRollAnimation();
        }
    }

    public void ReInitializeFromSave() {
        for (int i = 0; i < createdObjects.Count; i++) {
            createdObjects[i].ReInitalizeFromSave(SaveManager.Instance.GetCharacterGraphicsData(i, CharacterCustomizationPanelController.CategoryType.headwear), SaveManager.Instance.GetCharacterGraphicsData(i, CharacterCustomizationPanelController.CategoryType.glass), SaveManager.Instance.GetCharacterGraphicsData(i, CharacterCustomizationPanelController.CategoryType.accessory));
        }
    }
}
