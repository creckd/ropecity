using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using System.Collections;

public class SoundManager : MonoBehaviour 
{
    private static SoundManager instance;
    public static SoundManager Instance {
        get {
            if (instance == null)
                instance = FindObjectOfType<SoundManager>();
                return instance;
        }
    }

    Dictionary<string, GameObject> identifiedObjects = new Dictionary<string, GameObject>();

	public void LoopUntilStopped(AudioClip clip, float volume, float pitch, string id) {
		if (identifiedObjects.ContainsKey(id))
			return;

        GameObject holder = new GameObject();
        holder.name = id;
        AudioSource aSorce = holder.AddComponent<AudioSource>();
        aSorce.volume = volume;
        aSorce.pitch = pitch;
        aSorce.clip = clip;
        aSorce.loop = true;
        if (!aSorce.isPlaying)
            aSorce.Play();
        identifiedObjects.Add(id, holder);
    }

    public void LoopUntilStopped(CustomClip customClip, string id) {
        float volume = customClip.randomizeVolume ? Random.Range(customClip.randomizeVolumeBetween.x, customClip.randomizeVolumeBetween.y) : customClip.volume;
        float pitch = customClip.randomizePitch ? Random.Range(customClip.randomizePitchBetween.x, customClip.randomizePitchBetween.y) : customClip.pitch;

        LoopUntilStopped(customClip.audioClip, volume, pitch, id);
    }

    public void FadeLoopTo(CustomClip fadeToClip, string loopID, float fadeUnderSeconds = 1f) {
        AudioSource currentLoop = null;
        foreach (var dicEntry in identifiedObjects.Keys) {
            if (dicEntry == loopID) {
                currentLoop = identifiedObjects[dicEntry].GetComponent<AudioSource>();
            }
        }
        if (currentLoop == null) {
            Debug.LogException(new System.Exception("Nem található ilyen loop - nem lehetséges az átfadelés"));
            return;
        }
        StartCoroutine(FadeToVolume(currentLoop, 0f,fadeUnderSeconds));
        identifiedObjects.Remove(loopID);
        LoopUntilStopped(fadeToClip, loopID);
        AudioSource fadeASorce = identifiedObjects[loopID].GetComponent<AudioSource>();
        float fadeDefVol = fadeASorce.volume;
        fadeASorce.volume = 0f;
        StartCoroutine(FadeToVolume(fadeASorce, fadeDefVol,fadeUnderSeconds));
    }

    IEnumerator FadeToVolume(AudioSource aSorce,float tarVolume, float fadeUnderSeconds) {
        float timer = 0f;
        float defVolume = aSorce.volume;

        while(timer <= fadeUnderSeconds) {
            timer += Time.deltaTime;
            aSorce.volume = Mathf.Lerp(defVolume, tarVolume, timer / fadeUnderSeconds);
            yield return null;
        }
        aSorce.volume = tarVolume;

        if (tarVolume == 0f)
            Destroy(aSorce.gameObject);
    }

    public void StopLoopedSound(string id) {
        
        if (identifiedObjects.ContainsKey(id))
        {
            GameObject holder = identifiedObjects[id];
            identifiedObjects.Remove(id);
            Destroy(holder);
        }
    }

    public void CreateOneShot(AudioClip clip, float volume, float pitch) {
        GameObject holder = new GameObject();
        holder.name = "One Shot Sound Effect(Created By SoundManager)";
        AudioSource aSorce = holder.AddComponent<AudioSource>();
        aSorce.volume = volume;
        aSorce.pitch = pitch;
        aSorce.clip = clip;
        if (!aSorce.isPlaying)
            aSorce.Play();
        Destroy(holder, clip.length);
    }

    public void CreateOneShot(CustomClip customClip) {
        if (customClip.audioClip == null)
            return;

        float volume = customClip.randomizeVolume ? Random.Range(customClip.randomizeVolumeBetween.x, customClip.randomizeVolumeBetween.y) : customClip.volume;
        float pitch = customClip.randomizePitch ? Random.Range(customClip.randomizePitchBetween.x, customClip.randomizePitchBetween.y) : customClip.pitch;

        CreateOneShot(customClip.audioClip, volume, pitch);

    }

	private void OnApplicationFocus(bool focus) {
		if (focus) {
			if(!AdvertManager.adCurrentlyPlaying)
			AudioListener.volume = 1f;
		} else {
			AudioListener.volume = 0f;
		}
	}

}