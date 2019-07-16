using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum CharacterType {
	Worm,
	Toilee,
	Blob
}

[System.Serializable]
public enum RarityType {
	Common,
	Uncommon,
	Rare,
	Epic,
	Legendary
}

[System.Serializable]
public enum PriceType {
	VideoAD,
	IAP
}

[System.Serializable]
public class CharacterData {
	public string characterName;
	public CharacterType characterType;
	public RarityType characterRarity;
	public PriceType characterPrice;
	public int videoAdCost = 0;
}

public class ConfigDatabase : MonoBehaviour {

	private static ConfigDatabase instance = null;
	public static ConfigDatabase Instance {
		get {
			if (instance == null)
				instance = FindObjectOfType<ConfigDatabase>();
			return instance;
		}
	}

	private void Awake() {
		DontDestroyOnLoad(this.gameObject);
	}

	[Header("Game")]
	public float reinitalizingDuration = 1f;
	public float normalSpeed = 1f;
	public float slowMotionSpeed = 0.5f;
	public float unhookedTimeDampeningSpeed = 7.5f;
	public float hookedTimeDampeningSpeed = 3f;
	public float allTimeMininmumWorldY = 0f;

	[Header("Finish")]
	public float finishingSlowMotionTime = 0.5f;
	public float finishingBlurTime = 0.5f;
	public AnimationCurve finishSlowMotionCurve;

	[Header("Pause")]
	public float pauseBlurTime = 0.5f;

	[Header("Tutorial")]
	public float tutorialBlurTime = 1f;
	public float tutorialRequiredWatchTime = 5f;

	[Header("Worm")]
	public float gravityScale = 0.2f;
	public float wormMass = 100;
	public float swingForceMultiplier = 10f;
	public float rotationSpeed = 1f;
	public float ropeShootSpeed = 1f;
	public float minRopeDistance = 1f;
	public float maxRopeDistance = 10f;
	public float pullForceMultiplier = 1f;
	public float maximumRewardedAngleForPullForce = 30f;
	public float remainingVelocityPercentAfterBounce = 0.75f;
	public float minYVelocityAfterGroundCollision = 0.25f;
	public float minimumVelocityMagnitudeAfterBounce = 1f;
	public float slidingAngleThreshHold = 85f;

	[Header("Traps")]
	public float mineLethalRange = 5f;

	[Header("Prefabs")]
	public Worm wormPrefab;

	[Header("Cannon")]
	public Vector2 cannonShootDirection = new Vector2(1f, 1f);
	public float cannonShootForceMultiplier = 1f;

	[Header("LaunchPad")]
	public float launchPadForce = 1f;

	[Header("Transition")]
	public AnimationCurve transitionInCurve;
	public AnimationCurve transitionOutCurve;
	public float transitionTime = 1f;

	[Header("Characters")]
	public CharacterData[] characters;
	public Color commonRarityColor;
	public Color unCommonRarityColor;
	public Color rareRarityColor;
	public Color epicRarityColor;
	public Color legendaryRarityColor;

	public CharacterData GetCharacterDataWithType(CharacterType type) {
		CharacterData data = null;
		for (int i = 0; i < characters.Length; i++) {
			if (characters[i].characterType == type)
				data = characters[i];
		}
		return data;
	}

	public Color GetRarityColor(RarityType rarity) {
		switch (rarity) {
			case RarityType.Common:
				return commonRarityColor;
			case RarityType.Uncommon:
				return unCommonRarityColor;
			case RarityType.Rare:
				return rareRarityColor;
			case RarityType.Epic:
				return epicRarityColor;
			case RarityType.Legendary:
				return legendaryRarityColor;
		}
		return new Color(1f, 0.07f, 0.576f);
	}
}
