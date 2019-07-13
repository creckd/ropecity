using System.Collections;
using UnityEngine;

public class Wobble : MonoBehaviour
{
	private Animation anim;
	private float originalScaleX = 0.0f;
	private float originalScaleY = 0.0f;
	private float varyingValueX = 0.0f;
	private float varyingValueY = 0.0f;
	private float endTime = 0.0f;
	private bool larger = true;
	private bool playing = false;
	private bool shouldplay = true;
	private bool shouldLoop = false;
	public bool useSameCurveForBothAxis = false;

	[Range(0.1f, 3.0f)]
	public float minTime = 0.4f;

	[Range(0.1f, 3.0f)]
	public float maxTime = 0.5f;

	[Range(0.01f, 2.0f)]
	public float min = 0.01f;

	[Range(0.01f, 2.0f)]
	public float max = 0.1f;

	private void Start()
	{
		if (gameObject.GetComponent<Animation>() == null)
			gameObject.AddComponent<Animation>();
		anim = GetComponent<Animation>();
		varyingValueX = originalScaleX = transform.localScale.x;
		varyingValueY = originalScaleY = transform.localScale.y;
		//StartCoroutine(Pulse());
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
	public void Setter(bool play = true, float minTime = 0.4f, float maxTime = 0.5f, float min = 0.01f, float max = 0.1f, bool loop = false)
	{
		shouldplay = play;
		this.minTime = minTime;
		this.maxTime = maxTime;
		this.min = min;
		this.max = max;
		shouldLoop = loop;
	}

	public void Setter(bool play)
	{
		shouldplay = play;
	}

	private void Update()
	//IEnumerator Pulse()
	{
		//while (true)
		if (shouldplay) {
			if (!playing) {
				playing = true;

				endTime = Random.Range(minTime, maxTime);
				AnimationCurve curveX = AnimationCurve.EaseInOut(0.0f, varyingValueX, 1f, varyingValueX = originalScaleX * (larger ? Random.Range(1.0f + min, 1.0f + max) : Random.Range(1.0f - max, 1.0f - min)));
				AnimationCurve curveY = AnimationCurve.EaseInOut(0.0f, varyingValueY, 1f, varyingValueY = originalScaleY * (larger ? Random.Range(1.0f - max, 1.0f - min) : Random.Range(1.0f + min, 1.0f + max)));
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
}