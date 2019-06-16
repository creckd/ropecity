using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class CustomClip {
    [HideInInspector]
    public string name = "Zene";
    public AudioClip audioClip;
    [Range(0f, 1f)]
    public float volume = 0.5f;
    public bool randomizeVolume = false;
    public Vector2 randomizeVolumeBetween;
    [Range(-3f, 3f)]
    public float pitch = 1f;
    public bool randomizePitch = false;
    public Vector2 randomizePitchBetween;
}

[System.Serializable]
public class CustomClipMemoryFriendly {
    [HideInInspector]
    public string name = "Zene";
    [AssetPath("","Assets/Music/Resources")]
    public string audioClipPath;
    public AudioClip audioClip = null;
    [Range(0f, 1f)]
    public float volume = 0.5f;
    public bool randomizeVolume = false;
    public Vector2 randomizeVolumeBetween;
    [Range(-3f, 3f)]
    public float pitch = 1f;
    public bool randomizePitch = false;
    public Vector2 randomizePitchBetween;

    public CustomClip CloneToCustomClip() {
        CustomClip customClip = new CustomClip();
        if (audioClip == null) {
            audioClip = Resources.Load<AudioClip>(audioClipPath);
        }
        customClip.audioClip = audioClip;
        customClip.name = name;
        customClip.pitch = pitch;
        customClip.randomizePitch = randomizePitch;
        customClip.randomizePitchBetween = randomizePitchBetween;
        customClip.randomizeVolume = randomizeVolume;
        customClip.randomizeVolumeBetween = randomizeVolumeBetween;

        return customClip;
    }
}

public class AudioConfigDatabase : MonoBehaviour 
{
    private static AudioConfigDatabase instance;
    public static AudioConfigDatabase Instance {
        get {
            if (instance == null)
                instance = FindObjectOfType<AudioConfigDatabase>();
            return instance;
        }
    }

	public CustomClipMemoryFriendly mainMenuMusic;
	public CustomClipMemoryFriendly ingameMusic;

	public CustomClip optionsLanguageSelectorTick;
	public CustomClip optionsWoosh;
	public CustomClip levelSelectOpening;
	public CustomClip levelSelectChanging;
	public CustomClip mainMenuOpening;

	public CustomClip cannonShoot;
	public CustomClip wormShoot;
	public CustomClip wormSlideLoop;
	public CustomClip finishGlassBreak;

	private void Awake() {
		DontDestroyOnLoad(this.gameObject);
	}

}