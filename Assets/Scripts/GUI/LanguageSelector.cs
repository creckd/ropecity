using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class LanguageSelector : MonoBehaviour, IEndDragHandler, IBeginDragHandler {

	public enum SupportedLanguages {
		English,
		French,
		Russian,
		Portuguese,
		Hungarian,
		ChineseS,
		ChineseT,
	}

	public float snappingSpeed = 5f;

	public ScrollRect languageScrollRect;
	public Text sampleText;
	public Image marker;

	private Dictionary<SupportedLanguages, Text> mappedLanguagesTexts = new Dictionary<SupportedLanguages, Text>();
	private List<Text> instantiatedTexts = new List<Text>();
	private float freezedNormalizedPosition = 0f;
	private bool shouldRestrict = false;
	private float centerY;

	private bool snapping = false;
	private Vector3 snapTargetWorldPosition;
	private Text snapTargetText = null;

	private SupportedLanguages currentlySelectedLanguage = SupportedLanguages.English;

	public void InitializeLanguageSelector() {
		int numberOfSupportedLanguages = Enum.GetNames(typeof(SupportedLanguages)).Length;

		for (int i = 0; i < numberOfSupportedLanguages; i++) {
			Text languageText = (Instantiate(sampleText.gameObject, languageScrollRect.content.transform) as GameObject).GetComponent<Text>();
			SupportedLanguages l = (SupportedLanguages)i;
			languageText.text = l.ToString();
			mappedLanguagesTexts.Add(l, languageText);
			instantiatedTexts.Add(languageText);
		}

		centerY = marker.transform.position.y;
		sampleText.gameObject.SetActive(false);
	}

	public void ResetLanguageSelector() {
		StartCoroutine(ResetRoutine());
	}

	IEnumerator ResetRoutine() {
		yield return null; //Wait for 2 frames because UI positions are not updated immeadiatly
		yield return null;
		Text first = instantiatedTexts[0];
		languageScrollRect.content.transform.position = languageScrollRect.content.transform.position + Vector3.up * (centerY - first.transform.position.y);
		SupportedLanguages selected = mappedLanguagesTexts.FirstOrDefault(x => x.Value == first).Key;
		SelectLanguage(selected);
	}

	private void Update() {
		CheckIfShouldRestrict();
		if (shouldRestrict)
			Restrict();

		if (snapping) {
			languageScrollRect.content.transform.position = Vector3.Lerp(languageScrollRect.content.transform.position, snapTargetWorldPosition, Time.deltaTime * snappingSpeed);
			float dst = Vector3.Distance(languageScrollRect.content.transform.position, snapTargetWorldPosition);
			if (dst < 0.01f) {
				languageScrollRect.content.transform.position = snapTargetWorldPosition;
				SupportedLanguages selected = mappedLanguagesTexts.FirstOrDefault(x => x.Value == snapTargetText).Key;
				SelectLanguage(selected);
				snapping = false;
			}
		}
	}

	private void CheckIfShouldRestrict() {

		bool outOfBounds = instantiatedTexts[0].transform.position.y <= centerY || instantiatedTexts[instantiatedTexts.Count - 1].transform.position.y >= centerY;

		if (!shouldRestrict && outOfBounds) {
			freezedNormalizedPosition = languageScrollRect.verticalNormalizedPosition;
			shouldRestrict = true;
		} else if (shouldRestrict && !outOfBounds) {
			shouldRestrict = false;
		}
	}

	private void Restrict() {
		if(Mathf.Sign(freezedNormalizedPosition) == -1)
		languageScrollRect.verticalNormalizedPosition = Mathf.Clamp(languageScrollRect.verticalNormalizedPosition, freezedNormalizedPosition, 0f);
		else
			languageScrollRect.verticalNormalizedPosition = Mathf.Clamp(languageScrollRect.verticalNormalizedPosition, 0f, freezedNormalizedPosition);
	}

	private Text GetClosestTextToCenter() {
		Text closestText = instantiatedTexts[0];
		float closestDst = Mathf.Abs(centerY - instantiatedTexts[0].transform.position.y);
		for (int i = 1; i < instantiatedTexts.Count; i++) {
			float currDst = Mathf.Abs(centerY - instantiatedTexts[i].transform.position.y);
			if (currDst < closestDst) {
				closestDst = currDst;
				closestText = instantiatedTexts[i];
			}
		}
		return closestText;
	}

	public void OnEndDrag(PointerEventData eventData) {
		SnapToClosestText();
	}

	public void OnBeginDrag(PointerEventData eventData) {
		snapping = false;
	}

	private void SnapToClosestText() {
		Text closest = GetClosestTextToCenter();
		languageScrollRect.velocity = Vector2.zero;
		snapTargetWorldPosition = languageScrollRect.content.transform.position + Vector3.up * (centerY - closest.transform.position.y);
		snapTargetText = closest;
		snapping = true;
	}

	private void SelectLanguage(SupportedLanguages language) {
		if (currentlySelectedLanguage != language) {
			currentlySelectedLanguage = language;
			Debug.Log("Changed language to: " + language.ToString());
			//TODO : IMPLEMENT LANGUAGE CHANGE HERE
		}
		RefreshTextStyles();
	}

	private void RefreshTextStyles() {
		foreach (var t in instantiatedTexts) {
			bool isSelected = mappedLanguagesTexts[currentlySelectedLanguage] == t;
			if (isSelected)
				t.fontStyle = FontStyle.Bold;
			else t.fontStyle = FontStyle.Normal;
		}
	}
}
