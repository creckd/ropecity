using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Fader : MonoBehaviour {

	public int order;
	public float fadeTime = 1f;
	public float delay = 0f;

	public abstract void FadeIn();
	public abstract void FadeOut();
}
