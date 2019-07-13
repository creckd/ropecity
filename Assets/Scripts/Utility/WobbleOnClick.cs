using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class WobbleOnClick : MonoBehaviour {
	[System.Serializable]
	public enum StartingWobbleBehaviour {
		Shrink,
		Grow,
		Random
	}

	private float originalScaleX = 0.0f;
	private float originalScaleY = 0.0f;
	private float varyingValueX = 0.0f;
	private float varyingValueY = 0.0f;
	private float endTime = 0.0f;
	private bool larger = true;
	private bool playing = false;
	private bool shouldplay = true;
	public bool useSameCurveForBothAxis = false;
	public StartingWobbleBehaviour startingWobbleBehaviour = StartingWobbleBehaviour.Grow;

	[Range(0.1f, 3.0f)]
	public float minTime = 0.4f;

	[Range(0.1f, 3.0f)]
	public float maxTime = 0.5f;

	[Range(0.01f, 2.0f)]
	public float min = 0.01f;

	[Range(0.01f, 2.0f)]
	public float max = 0.1f;

	private void Start() {
		varyingValueX = originalScaleX = transform.localScale.x;
		varyingValueY = originalScaleY = transform.localScale.y;
		shouldplay = false;
		currentNumberOfTimesToPlayWobble = numberOfTimesToPlayWobble;
		Button parentButton = GetComponentInParent<Button>();
		if (parentButton) {
			parentButton.onClick.AddListener(() => { Click(); });
		}
	}

	/// <summary>
	/// Setter for the PulsingAnimation variables. Every playback end the animation is re-randoming variables.
	/// </summary>
	/// <param name="play">Should the animation play.</param>
	/// <param name="minTime">Minimum in the random range of playback time.</param>
	/// <param name="maxTime">Maximum in the random range of playback time.</param>
	/// <param name="min">Minimum value which will be added or substracted from 1 for scaling.</param>
	/// <param name="max">Maximum value which will be added or substracted from 1 for scaling.</param>
	/// <param name="loop">If loop is <code>true</code>, the animation won't be randomed every playback end.</param>
	public void Setter(bool play = true, float minTime = 0.4f, float maxTime = 0.5f, float min = 0.01f, float max = 0.1f, bool loop = false) {
		shouldplay = play;
		this.minTime = minTime;
		this.maxTime = maxTime;
		this.min = min;
		this.max = max;
	}

	private void Update() {
		if (shouldplay) {
			if (!playing && currentNumberOfTimesToPlayWobble > 0) {
				currentNumberOfTimesToPlayWobble--;
				playing = !playing;

				endTime = Random.Range(minTime, maxTime);
				AnimationCurve curveX;
				AnimationCurve curveY;
				if (currentNumberOfTimesToPlayWobble != 0) {
					curveX = AnimationCurve.EaseInOut(0.0f, varyingValueX, 1f, varyingValueX = originalScaleX * (larger ? Random.Range(1.0f + min, 1.0f + max) : Random.Range(1.0f - max, 1.0f - min)));
					curveY = AnimationCurve.EaseInOut(0.0f, varyingValueY, 1f, varyingValueY = originalScaleY * (larger ? Random.Range(1.0f - max, 1.0f - min) : Random.Range(1.0f + min, 1.0f + max)));
				} else {
					curveX = AnimationCurve.EaseInOut(0.0f, varyingValueX, 1f, varyingValueX = 1f);
					curveY = AnimationCurve.EaseInOut(0.0f, varyingValueY, 1f, varyingValueY = 1f);
				}
				if (useSameCurveForBothAxis)
					curveY = curveX;
				larger = !larger;

				StartCoroutine(Pulse(curveX, curveY, endTime));
			}
		} else if (playing) {
			playing = false;
			StopAllCoroutines();
			transform.localScale = new Vector3(originalScaleX, originalScaleY, transform.localScale.z);
		}
	}
	private void OnDisable() {
		if (playing) {
			playing = false;
			StopAllCoroutines();
			transform.localScale = new Vector3(originalScaleX, originalScaleY, transform.localScale.z);
		}
	}

	IEnumerator Pulse(AnimationCurve curveX, AnimationCurve curveY, float lenght) {
		float timer = 0f;
		while (timer <= lenght) {
			timer += Time.deltaTime;
			transform.localScale = new Vector3(curveX.Evaluate(timer / lenght), curveY.Evaluate(timer / lenght), transform.localScale.z);
			yield return null;
		}
		playing = false;
	}

	public int numberOfTimesToPlayWobble = 2;
	private int currentNumberOfTimesToPlayWobble = 2;

	public void Click() {
		if (playing)
			return;
		shouldplay = true;
		switch (startingWobbleBehaviour) {
			case StartingWobbleBehaviour.Shrink:
				larger = false;
				break;
			case StartingWobbleBehaviour.Grow:
				larger = true;
				break;
			case StartingWobbleBehaviour.Random:
				larger = System.Convert.ToBoolean(Random.Range(0, 2));
				break;
		}
		currentNumberOfTimesToPlayWobble = numberOfTimesToPlayWobble;
	}
}