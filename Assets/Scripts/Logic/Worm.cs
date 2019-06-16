using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Worm : MonoBehaviour {

	private const string trailMaterialTransparencyName = "_Transparency";

	public LayerMask ropeLayerMask;
	public LayerMask collisionLayerMask;
	public GameObject ropeEndPrefab;

	public GameObject gunPositionObject;
	public LineRenderer ropeRenderer;
	public Text accuracyText;

	public Vector2 Velocity {
		get {
			return velocity;
		}
	}

	public GameObject ragdoll;
	public TrailRenderer trail;
	public TrailRenderer subTrail1;
	public TrailRenderer subTrail2;

	public ParticleSystem bounceParticle;
	public ParticleSystem hookHitParticle;
	public ParticleSystem slideParticle;

	public bool landedHook = false;
	public float totalRotationSinceStart = 0f;

	public Vector3 wormAimDirection {
		get {
			return gunPositionObject.transform.position - transform.position;
		}
	}

	private bool rotationEnabled = true;
	private GameObject ropeEnd;

	private List<HitPoint> hitPoints = new List<HitPoint>();
	private float distanceToKeep = 0f;
	private bool perfectHitHappened = false;

	private bool isGrounded = false;
	private Vector2 velocity;
	private float gravity = 0.2f;
	private float feetLength = 2f;

	private int currentHoldID = -1;
	private float currentHookSearchDistance = 0f;

	Rigidbody2D rb;
	CircleCollider2D circleCollider;

	private bool playedSwingSoundOnThisHook = false;

	class HitPoint {
		public Vector3 hookPosition {
			get {
				if (connectedLevelObject == null)
					return actualHookPosition;
				else return hookHolderPositionHolder.transform.position;
			}
		}
		private Vector3 actualHookPosition;
		public Vector3 attachedCross;

		public LevelObject connectedLevelObject = null;
		public GameObject hookHolderPositionHolder = null;
		public bool isDynamicHitPoint {
			get {
				return connectedLevelObject != null;
			}
		}

		public HitPoint(Vector3 hookPosition, Vector3 attachedVelocityCrossVector) {
			this.actualHookPosition = hookPosition;
			this.attachedCross = attachedVelocityCrossVector;
		}

		public HitPoint(Vector3 hookPosition) {
			this.actualHookPosition = hookPosition;
			this.attachedCross = Vector3.zero;
		}

		public void SetConnectedLevelObject(LevelObject connected) {
			connectedLevelObject = connected;
			GameObject hookHolder = new GameObject();
			hookHolder.name = "Dynamic Hook Position";
			hookHolder.transform.parent = connected.transform;
			hookHolder.transform.position = actualHookPosition;
			hookHolderPositionHolder = hookHolder;
		}
	}

	public void Initialize() {
		circleCollider = GetComponent<CircleCollider2D>();
		rb = GetComponent<Rigidbody2D>();

		InputController.Instance.TapHappened += Tap;
		InputController.Instance.ReleaseHappened += Release;
		GameController.Instance.GameFinished += GameFinished;
		CameraController.Instance.StartTracking(this.transform);

		gravity = ConfigDatabase.Instance.gravityScale;
		ropeEnd = Instantiate(ropeEndPrefab, transform.position, transform.rotation) as GameObject;
		ropeEnd.gameObject.SetActive(false);
	}

	public void DestroyWorm() {
		InputController.Instance.TapHappened -= Tap;
		InputController.Instance.ReleaseHappened -= Release;
		GameController.Instance.GameFinished -= GameFinished;
		Destroy(this.gameObject);
	}

	public void AddForce(Vector2 force) {
		velocity += force;
	}

	public void SetVelocity(Vector2 velocity) {
		this.velocity = velocity;
	}

	private void Release(int inputIndex) {
		if (!GameController.Instance.wormInputEnabled || !gameObject.activeSelf || GameController.Instance.currentGameState == GameState.GameFinished)
			return;
		if (currentHoldID == inputIndex) {
			currentHoldID = -1;
			rotationEnabled = true;

			CalculateReleaseJump();

			foreach (var hp in hitPoints) {
				if(hp.connectedLevelObject != null)
				hp.connectedLevelObject.HookReleasedOnThisObject();
				if (hp.hookHolderPositionHolder != null)
					Destroy(hp.hookHolderPositionHolder);
			}

			hitPoints.Clear();
			landedHook = false;
			perfectHitHappened = false;
			ropeRenderer.positionCount = 0;
			ropeEnd.gameObject.SetActive(false);
			GameController.Instance.ReleasedHook();
		}
	}

	private void CalculateReleaseJump() {

		if (distanceToKeep >= ConfigDatabase.Instance.maxRopeDistance * 0.35f) {
			Ray ray = new Ray(transform.position, Vector3.up);
			RaycastHit2D hit;
			hit = Physics2D.Raycast(ray.origin, ray.direction, 5f, collisionLayerMask);
			bool upwardsIsClear = hit.collider == null;
			if (upwardsIsClear) {
				float coefficent = Mathf.Clamp01(0.05f / Mathf.Clamp(velocity.magnitude, 0.1f, Mathf.Infinity));
				AddForce(coefficent * Vector2.up * 0.5f);
			}
		}
	}

	private void Tap(int inputIndex) {
		if (landedHook || !GameController.Instance.wormInputEnabled || !gameObject.activeSelf || GameController.Instance.currentGameState == GameState.GameFinished)
			return;
		if (currentHoldID == inputIndex)
			Release(inputIndex);

		currentHoldID = inputIndex;

		currentHookSearchDistance = 0f;

		SearchForHitPoint();
	}

	private void SearchForHitPoint(bool useInrementalDistance = false) {
		RaycastHit2D hit;
		float tarDistance = useInrementalDistance ? currentHookSearchDistance : ConfigDatabase.Instance.maxRopeDistance;

		if (!landedHook && FindHookPoint(out hit, tarDistance) && !WormOverlappingPhysicalCollider()) {

			SoundManager.Instance.CreateOneShot(AudioConfigDatabase.Instance.wormShoot);

			rotationEnabled = false;
			landedHook = true;
			HitPoint newHookPosition = new HitPoint(hit.point);
			LevelObject dynamicLevelObjectHit = hit.collider.GetComponent<LevelObject>();
			if (dynamicLevelObjectHit != null) {
				dynamicLevelObjectHit.HookLandedOnThisObject();
				newHookPosition.SetConnectedLevelObject(dynamicLevelObjectHit);
			}
			hitPoints.Add(newHookPosition);
			distanceToKeep = Mathf.Clamp(Vector3.Distance(hit.point, transform.position),ConfigDatabase.Instance.minRopeDistance,ConfigDatabase.Instance.maxRopeDistance);
			AddWormPullingForce(hit.point);
			GameController.Instance.FoundPotentionalHitPoint(hit.point);
			GameController.Instance.LandedHook(hit.point);
			GameController.Instance.HideUIHookAid();
			hookHitParticle.transform.position = hit.point;
			hookHitParticle.Play();
		}
	}

	private void AddWormPullingForce(Vector3 hitPosition) {
		Vector2 forceDirection = (hitPosition - gunPositionObject.transform.position).normalized;
		float angle = Vector2.Angle(Vector2.up, forceDirection);
		forceDirection = Rotate(forceDirection, -Mathf.Sign(hitPosition.x - gunPositionObject.transform.position.x) * 90f);
		float maximumRewardedAngle = ConfigDatabase.Instance.maximumRewardedAngleForPullForce;
		float effectivenessMultiplier = Mathf.Clamp((180f - Mathf.Abs(angle - 45f)) - (180f - maximumRewardedAngle), 0f, maximumRewardedAngle);
		effectivenessMultiplier = effectivenessMultiplier / maximumRewardedAngle;
		float reverseOrNot = Mathf.Sign(Vector2.Dot(forceDirection.normalized, velocity.normalized));
		float slowVelocityCoefficent = (1f - 1f * (Mathf.Clamp(reverseOrNot * Mathf.Abs(velocity.x), 0f, 0.25f) / 0.25f));
		slowVelocityCoefficent = Mathf.Clamp(slowVelocityCoefficent, 0f, 0.5f);
		float totalMultiplier = effectivenessMultiplier + slowVelocityCoefficent;
		totalMultiplier *= ConfigDatabase.Instance.pullForceMultiplier;
		AddForce(forceDirection * totalMultiplier);

		if (effectivenessMultiplier > 0.75f)
			PerfectHit();
	}

	private void RefreshRopeRenderer() {
		if (landedHook || currentHoldID != -1) {
			List<Vector3> lineRendererPoints = new List<Vector3>();
			lineRendererPoints.Add(gunPositionObject.transform.position);

			Vector3 direction = Vector3.zero;
			if (landedHook) {
				ropeRenderer.positionCount = hitPoints.Count + 1;
				for (int i = hitPoints.Count - 1; i >= 0; i--) {
					lineRendererPoints.Add(hitPoints[i].hookPosition);
				}
				if (hitPoints.Count >= 2)
					direction = (hitPoints[0].hookPosition - hitPoints[1].hookPosition).normalized;
				else
					direction = (hitPoints[0].hookPosition - gunPositionObject.transform.position).normalized;
			} else {
				ropeRenderer.positionCount = 2;
				lineRendererPoints.Add(gunPositionObject.transform.position + (gunPositionObject.transform.position - transform.position).normalized * currentHookSearchDistance);
				direction = (gunPositionObject.transform.position - transform.position).normalized;
			}
			Vector3[] points = lineRendererPoints.ToArray();
			ropeEnd.transform.position = points[points.Length - 1] - direction * 0.5f;
			ropeEnd.transform.LookAt(points[points.Length - 1] + direction);
			ropeEnd.gameObject.SetActive(true);
			ropeRenderer.SetPositions(points);
		}
	}

	private void RefreshWormRotation() {
		Vector3 currentFacingDir = gunPositionObject.transform.position - transform.position;
		Vector3 hookDir = hitPoints[hitPoints.Count - 1].hookPosition - gunPositionObject.transform.position;
		currentFacingDir = currentFacingDir.normalized;
		hookDir = hookDir.normalized;
		Vector3 cross = Vector3.Cross(currentFacingDir, hookDir);

		transform.Rotate(new Vector3(0f, 0f, cross.z * Vector3.Angle(hookDir, currentFacingDir)));
	}

	private void Update() {
		if (rotationEnabled) {
			float ZRotationThisFrame = Time.deltaTime * ConfigDatabase.Instance.rotationSpeed;
			transform.Rotate(new Vector3(0f, 0f, ZRotationThisFrame));
			totalRotationSinceStart += ZRotationThisFrame;
		}
		if (GameController.Instance.currentGameState != GameState.GameFinished) {
			if (landedHook) {
				RefreshWormRotation();
				CheckILastHitPointIsNotNeccessaryAnymore();
				//LookForDynamicHookCollisions();
				LookForHookCollision();
			} else {
				RaycastHit2D hit;
				if (FindHookPoint(out hit, ConfigDatabase.Instance.maxRopeDistance)) {
					GameController.Instance.FoundPotentionalHitPoint(hit.point);
					GameController.Instance.ShowUIHookAid();
				} else {
					GameController.Instance.HideUIHookAid();
				}
			}
			if (!landedHook && currentHoldID != -1) {
				currentHookSearchDistance += Time.deltaTime * ConfigDatabase.Instance.ropeShootSpeed;
				SearchForHitPoint(true);
				if (currentHookSearchDistance >= ConfigDatabase.Instance.maxRopeDistance && !landedHook)
					Release(currentHoldID);
			}
		}
		SimulatePhysics();
		CheckIfOutOfBoundaries();
		HandleSlideParticle();
		//HandleAccuracyText();
		HandleSwingSoundEffect();
	}

	private void LateUpdate() {
		if(landedHook || currentHoldID != -1)
			RefreshRopeRenderer();
		HandleSpeedTrails();
	}

	private void FixedUpdate() {
		HookCorrection();
		CheckForPhysicsCollision();
		SlidingMechanic();
	}

	private void CheckForPhysicsCollision() {
		if(!sliding)
		Destuck();

		RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, circleCollider.radius + 0.1f, velocity, 0.4f,collisionLayerMask);
		bool physicalCollisionHappened = false;
		foreach (var hit in hits) {
			if (!hit.collider.CompareTag("Player") && !hit.collider.isTrigger) {
				if (physicalCollisionHappened && sliding) {
					float wouldReflectAngle = Vector3.Angle(velocity, Vector3.Reflect(velocity, hit.normal));
					if (wouldReflectAngle > ConfigDatabase.Instance.slidingAngleThreshHold) {
						Collided(hit);
					}
				}
				if (physicalCollisionHappened)
					continue;
				Collided(hit);
				physicalCollisionHappened = true;
			} else if (hit.collider.isTrigger)
				hit.collider.SendMessage("OnTriggerEnter2D", circleCollider as Collider2D);
		}
	}

	private void Destuck() {
		RaycastHit2D[] initialHits = Physics2D.CircleCastAll(transform.position, circleCollider.radius, Vector2.zero, 0f, collisionLayerMask);

		foreach (var initHit in initialHits) {
			if (!initHit.collider.isTrigger) {
				Vector3 firstDestuckPoint = initHit.point + ((Vector2)transform.position - initHit.point).normalized * (circleCollider.radius + 0.1f);
				bool firstDestuckPointFailed = false;
				Collider2D[] colls = Physics2D.OverlapCircleAll(firstDestuckPoint, circleCollider.radius, ~LayerMask.GetMask("Worm"));
				foreach (var col in colls) {
					if (!col.isTrigger) {
						firstDestuckPointFailed = true;
						break;
					}
				}
				if (firstDestuckPointFailed) {
					Vector3 startingDirection = ((Vector2)transform.position - initHit.point).normalized;
					float increments = 90f;
					int currDistanceMultiplier = 1;
					float currAngle = 0f;
					bool foundFreePoint = false;
					while (!foundFreePoint) {
						bool destuckPointFree = true;

						float currDistance = circleCollider.radius * 2f * currDistanceMultiplier;
						Vector3 dirWithLength = Quaternion.Euler(0f, 0f, currAngle) * startingDirection;
						dirWithLength *= currDistance;
						Vector3 pointToTest = transform.position + dirWithLength;
						Collider2D[] colliders = Physics2D.OverlapCircleAll(pointToTest, circleCollider.radius, ~LayerMask.GetMask("Worm"));
						foreach (var col in colliders) {
							if (!col.isTrigger) {
								destuckPointFree = false;
								break;
							}
						}
						if (destuckPointFree) {
							foundFreePoint = true;
							transform.position = pointToTest;
						} else {
							currAngle += increments;
							if (currAngle == 360f) {
								currAngle = 0f;
								currDistanceMultiplier++;
							}
						}
					}
				} else {
					transform.position = firstDestuckPoint;
				}
			}
		}
	}

	private void CheckIfGrounded() {
		Ray ray = new Ray(transform.position, Vector3.down);
		RaycastHit hit;
		Physics.Raycast(ray, out hit, feetLength);
		isGrounded = hit.collider != null;
	}

	private void CheckIfOutOfBoundaries() {
		if (GameController.Instance.currentGameState != GameState.GameStarted)
			return;
		if (transform.position.y < ConfigDatabase.Instance.allTimeMininmumWorldY) {
				Die();
		}
	}

	bool sliding = false;
	Vector2 direction = Vector2.zero;

	private void SimulatePhysics() {
		//rb.velocity = Vector3.zero;

		if (!isGrounded)
		velocity += Vector2.down * gravity * Time.deltaTime;

		Vector3 targetPosition = new Vector3(transform.position.x + (velocity.x * Time.deltaTime * 20 * 100f / ConfigDatabase.Instance.wormMass), transform.position.y + (velocity.y * Time.deltaTime * 20 * 100f / ConfigDatabase.Instance.wormMass), transform.position.z);
		transform.position = targetPosition;
	}

	private void HookCorrection(bool gainVelocity = true) {
		if (landedHook) {
			float currentDistance = Vector3.Distance(transform.position, hitPoints[hitPoints.Count - 1].hookPosition);
			float difference = currentDistance - distanceToKeep;
			Vector2 differenceVelocity = ((Vector2)(hitPoints[hitPoints.Count - 1].hookPosition - transform.position).normalized * difference);
			if(gainVelocity)
			velocity += differenceVelocity * Time.deltaTime * 10f * ConfigDatabase.Instance.swingForceMultiplier;

			transform.position += ((hitPoints[hitPoints.Count - 1].hookPosition - transform.position).normalized * difference);
		}
	}

	private void SlidingMechanic() {
		if (landedHook) {
			Vector2 dir;
			if (sliding)
				dir = direction;
			else dir = velocity;

			RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, circleCollider.radius, dir, 1f);

			bool hitSolid = false;
			RaycastHit2D solidHit = new RaycastHit2D();

			foreach (var hit in hits) {
				if (!hit.collider.CompareTag("Player") && !hit.collider.isTrigger) {
					solidHit = hit;
					hitSolid = true;
					break;
				}
			}

			if (hitSolid) {
				float wouldReflectAngle = Vector3.Angle(velocity, Vector3.Reflect(dir, solidHit.normal));
				if (sliding) {
					bool overlapping = true;
					while (overlapping && distanceToKeep > 2f) {
						distanceToKeep = Mathf.Clamp(distanceToKeep - 0.01f, 2f, ConfigDatabase.Instance.maxRopeDistance);
						HookCorrection(false);
						overlapping = WormOverlappingPhysicalCollider();
					}
				}
				if (wouldReflectAngle < ConfigDatabase.Instance.slidingAngleThreshHold) {
					if (!sliding) {
						SoundManager.Instance.CreateOneShot(AudioConfigDatabase.Instance.wormSlideLoop);

						sliding = true;
						direction = solidHit.point - (Vector2)transform.position;
						direction = direction.normalized;
					}
				}
			} else {
				if (sliding) {
					sliding = false;
					SoundManager.Instance.StopLoopedSound("slide");
				}
			}
		} else {
			if (sliding) {
				sliding = false;
				SoundManager.Instance.StopLoopedSound("slide");
			}
		}
	}

	private bool WormOverlappingPhysicalCollider() {
		bool isOverlapping = false;
		Collider2D[] overlappedColls = Physics2D.OverlapCircleAll(transform.position, circleCollider.radius);
		foreach (var coll in overlappedColls) {
			if (!coll.CompareTag("Player") && !coll.isTrigger) {
				isOverlapping = true;
				break;
			}
		}
		return isOverlapping;
	}

	public bool FindHookPoint(out RaycastHit2D raycastHit, float distanceToUse) {
		RaycastHit2D hit;

		Ray ray = new Ray(transform.position, (gunPositionObject.transform.position - transform.position).normalized);
		hit = Physics2D.Raycast(ray.origin,ray.direction,distanceToUse, ropeLayerMask);
		raycastHit = hit;
		return hit.collider != null;
	}

	public void LookForHookCollision() {
		Vector3 from = transform.position;
		Vector3 to = (hitPoints[hitPoints.Count - 1].hookPosition - transform.position).normalized;
		Ray ray = new Ray(from, to);
		RaycastHit2D hit;
		hit = Physics2D.Raycast(ray.origin,ray.direction,Vector3.Distance(transform.position,hitPoints[hitPoints.Count-1].hookPosition) - 1f, ropeLayerMask);
		Vector3 hitPosition = new Vector3(hit.point.x, hit.point.y, 0f);
		if (hit.collider != null && Vector3.Distance(hitPosition,hitPoints[hitPoints.Count-1].hookPosition) >= 0.1f) {

			LevelObject dynamicLevelObjectHit = hit.collider.GetComponent<LevelObject>();

			RaycastHit2D secondHit;
			Vector3 fromPosition = from + (Vector3)velocity.normalized * 0.5f;
			Vector3 sumVelocity = velocity;
			//if (dynamicLevelObjectHit != null)
			//	sumVelocity -= (Vector3)dynamicLevelObjectHit.velocity;
			Vector3 toPosition = hitPoints[hitPoints.Count - 1].hookPosition + sumVelocity.normalized * 0.5f;
			Ray secondRay = new Ray(fromPosition, toPosition - fromPosition);
			secondHit = Physics2D.Raycast(secondRay.origin, secondRay.direction, Vector3.Distance(transform.position, hitPoints[hitPoints.Count - 1].hookPosition) - 1f, ropeLayerMask);

			float firstHitDistance = Vector3.Distance(hitPosition, transform.position);
			float secondHitDistance = Vector3.Distance(secondHit.point, transform.position);

			if (secondHitDistance > firstHitDistance)
				return;

			Vector3 normalizedVelocity = velocity;
			//if(dynamicLevelObjectHit != null)
			//	normalizedVelocity -= (Vector3)dynamicLevelObjectHit.velocity;
			Vector3 fst = (transform.position + normalizedVelocity) - (Vector3)hit.point;
			Vector3 snd = (Vector3)hit.point- hitPoints[hitPoints.Count - 1].hookPosition;
			Vector3 cross = Vector3.Cross(fst,snd);
			HitPoint newHookPosition = new HitPoint(hit.point, cross);

			if (dynamicLevelObjectHit != null) {
				dynamicLevelObjectHit.HookLandedOnThisObject();
				newHookPosition.SetConnectedLevelObject(dynamicLevelObjectHit);
			}
			hitPoints.Add(newHookPosition);
			//distanceToKeep = Vector3.Distance(hit.point, transform.position);
			distanceToKeep = Mathf.Clamp(Vector3.Distance(hit.point, transform.position),ConfigDatabase.Instance.minRopeDistance,ConfigDatabase.Instance.maxRopeDistance);
		}
	}

	//private void LookForDynamicHookCollisions() {
	//	if (hitPoints.Count >= 2) {
	//		for (int i = hitPoints.Count - 1; i > 0; i--) {
	//			if (!hitPoints[i].isDynamicHitPoint)
	//				continue;
	//			Vector3 from = hitPoints[i-1].hookPosition;
	//			Vector3 to = (hitPoints[i].hookPosition - from).normalized;
	//			Ray ray = new Ray(from, to);
	//			Debug.DrawRay(ray.origin + ray.direction * 0.1f, ray.direction, Color.red);
	//			RaycastHit2D hit = new RaycastHit2D();
	//			bool pointClear = Physics2D.OverlapPoint(ray.origin + ray.direction * 0.1f, ropeLayerMask) == null;
	//			if (pointClear) {
	//				hit = Physics2D.Raycast(ray.origin + ray.direction * 0.1f, ray.direction, Vector3.Distance(from, hitPoints[i].hookPosition), ropeLayerMask);
	//			} else {
	//				RaycastHit2D[] allHits = Physics2D.RaycastAll(ray.origin + ray.direction * 0.1f, ray.direction, Vector3.Distance(from, hitPoints[i].hookPosition), ropeLayerMask);
	//				if (allHits.Length > 1 && !pointClear)
	//					hit = allHits[1];
	//			}
	//			Vector3 hitPosition = new Vector3(hit.point.x, hit.point.y, 0f);
	//			LevelObject dynamicLevelObjectHit = null;
	//			if(hit.collider != null)
	//			dynamicLevelObjectHit = hit.collider.GetComponent<LevelObject>();

	//			if (hit.collider != null && dynamicLevelObjectHit != null && dynamicLevelObjectHit.velocity.magnitude >= 0.01f && Vector3.Distance(hitPosition, (Vector2)hitPoints[i].hookPosition) >= 0.1f && Vector3.Distance(hitPosition,(Vector2)hitPoints[i-1].hookPosition) >= 0.1f) {
	//				Debug.Log("New Hit Point " + i.ToString() + " " + hitPosition);
	//				Vector3 normalizedVelocity = dynamicLevelObjectHit.velocity;
	//				Vector3 fst = hitPoints[i-1].hookPosition - (Vector3)hit.point;
	//				Vector3 snd = (hitPoints[i].hookPosition - (Vector3)dynamicLevelObjectHit.velocity) - (Vector3)hit.point;
	//				Vector3 cross = Vector3.Cross(fst, snd);
	//				HitPoint newHookPosition = new HitPoint(hit.point,cross);
	//				if (dynamicLevelObjectHit != null) {
	//					newHookPosition.SetConnectedLevelObject(dynamicLevelObjectHit);
	//				}
	//				hitPoints.Insert(i,newHookPosition);
	//			}

	//		}
	//	}
	//}

	private bool lastPointBeingDeleted = false;
	private Vector3 lastPositionWhenStartedDeleting = Vector3.zero;
	private Vector3 lastDynamicPositionWhenStartedDeleting = Vector3.zero;
	private void CheckILastHitPointIsNotNeccessaryAnymore() {
		if (hitPoints.Count >= 2) {
			HitPoint lastHitPoint = hitPoints[hitPoints.Count - 1];
			//if (lastHitPoint.connectedLevelObject == null)
			//	return;
			Vector3 fst = transform.position - hitPoints[hitPoints.Count - 1].hookPosition;
			Vector3 snd = hitPoints[hitPoints.Count - 2].hookPosition - hitPoints[hitPoints.Count - 1].hookPosition;
			Vector3 currentCross = Vector3.Cross(fst,snd);
			float dot = Vector3.Dot(currentCross.normalized, lastHitPoint.attachedCross.normalized);
			if (dot > 0f) {
				DeleteLastHitPoint();
			}
		}
	}

	private void DeleteLastHitPoint() {
		Debug.Log("Delete Last Hitpoint");
		if(hitPoints[hitPoints.Count-1].connectedLevelObject != null)
		hitPoints[hitPoints.Count - 1].connectedLevelObject.HookReleasedOnThisObject();
		if (hitPoints[hitPoints.Count - 1].hookHolderPositionHolder != null)
			Destroy(hitPoints[hitPoints.Count - 1].hookHolderPositionHolder);
		hitPoints.Remove(hitPoints[hitPoints.Count - 1]);
		distanceToKeep = Vector3.Distance(hitPoints[hitPoints.Count - 1].hookPosition, transform.position);
	}

	public void Die() {
		if (GameController.Instance.currentGameState == GameState.GameFinished)
			return;

		SoundManager.Instance.StopLoopedSound("slide");

		GameController.Instance.FinishGame(false);

		Destroy(ropeEnd.gameObject);
		ragdoll.gameObject.SetActive(true);
		ragdoll.transform.SetParent(null);
		foreach (var rb in ragdoll.GetComponentsInChildren<Rigidbody2D>()) {
			rb.AddForce(velocity * 50f,ForceMode2D.Impulse);
			rb.AddForce(rb.transform.forward * 20f * Vector3.Distance(rb.transform.position,transform.position), ForceMode2D.Impulse);
		}
		DestroyWorm();
	}

	private Vector2 Rotate(Vector2 v, float degrees) {
		float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
		float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

		float tx = v.x;
		float ty = v.y;
		v.x = (cos * tx) - (sin * ty);
		v.y = (sin * tx) + (cos * ty);
		return v;
	}

	private void Collided(RaycastHit2D hitInfo) {
		if(!sliding)
		SoundManager.Instance.CreateOneShot(AudioConfigDatabase.Instance.wormBounce);
		float wouldReflectAngle = Vector3.Angle(velocity, Vector3.Reflect(velocity, hitInfo.normal));
		if (landedHook && wouldReflectAngle < ConfigDatabase.Instance.slidingAngleThreshHold)
			return;
		else if (sliding && wouldReflectAngle > ConfigDatabase.Instance.slidingAngleThreshHold) {
			SoundManager.Instance.StopLoopedSound("slide");
			sliding = false;
		}
		if (!hitInfo.collider.CompareTag("LaunchPad")) {
			Vector2 reflected;
			if (landedHook) {
				reflected = -velocity * ConfigDatabase.Instance.remainingVelocityPercentAfterBounce;
			} else
				reflected = Vector2.Reflect(velocity, hitInfo.normal);
			if (reflected.magnitude < ConfigDatabase.Instance.minimumVelocityMagnitudeAfterBounce) {
				reflected *= (1f / (reflected.magnitude / ConfigDatabase.Instance.minimumVelocityMagnitudeAfterBounce));
			}
			Vector3 collisionDirection = ((Vector2)transform.position - hitInfo.point).normalized;
			float verticalDot = Vector3.Dot(Vector3.up, collisionDirection);

			bool isGroundCollision = verticalDot > 0.5f;
			if (isGroundCollision)
				reflected.y = Mathf.Clamp(reflected.y, ConfigDatabase.Instance.minYVelocityAfterGroundCollision, Mathf.Infinity);

			float dot = Vector2.Dot(velocity.normalized, reflected.normalized);

			if (landedHook && dot > 0)
				return;

			bounceParticle.transform.position = hitInfo.point;
			bounceParticle.Play();
			velocity = reflected;
		}
	}

	//private void OnCollisionStay2D(Collision2D collision) {
	//		bool isTouchingSomethingSolid = false;
	//		float ropeDecreaseValue = 0.1f;
	//		if (distanceToKeep > ropeDecreaseValue * 5f)
	//		do {
	//			isTouchingSomethingSolid = false;
	//			distanceToKeep -= ropeDecreaseValue;
	//			transform.position += ((hookPositions[hookPositions.Count - 1] - transform.position).normalized * ropeDecreaseValue);
	//			Collider2D[] results = Physics2D.OverlapCircleAll(transform.position, circleCollider.radius + 0.1f);
	//			foreach (var r in results) {
	//				if (r != null) {
	//					if (!r.CompareTag("Player") && !r.isTrigger) {
	//						isTouchingSomethingSolid = true;
	//					}
	//				}
	//			}
	//		if (distanceToKeep < ropeDecreaseValue * 5f)
	//			break;
	//		} while (isTouchingSomethingSolid);
	//}


	MedianPoint FindMedian(ContactPoint2D[] contacts) {
		MedianPoint mP = new MedianPoint();
		mP.medianPoint = Vector3.zero;
		mP.medianNormal = Vector3.zero;
		foreach (var cP in contacts) {
			mP.medianPoint.x += cP.point.x;
			mP.medianPoint.y += cP.point.y;

			mP.medianNormal.x += cP.normal.x;
			mP.medianNormal.y += cP.normal.y;
		}
		mP.medianPoint /= contacts.Length;
		mP.medianNormal /= contacts.Length;
		return mP;
	}

	public class MedianPoint {
		public Vector3 medianPoint;
		public Vector3 medianNormal;
	}

	private void GameFinished(bool win) {
		if (win)
			ClearWormHook();
	}

	private void ClearWormHook() {
		currentHoldID = -1;
		rotationEnabled = true;

		foreach (var hp in hitPoints) {
			if (hp.connectedLevelObject != null)
				hp.connectedLevelObject.HookReleasedOnThisObject();
			if (hp.hookHolderPositionHolder != null)
				Destroy(hp.hookHolderPositionHolder);
		}

		hitPoints.Clear();
		landedHook = false;
		ropeRenderer.positionCount = 0;
		ropeEnd.gameObject.SetActive(false);
		GameController.Instance.ReleasedHook();
	}

	private void HandleSlideParticle() {
		if (sliding) {
			if (!slideParticle.isPlaying)
				slideParticle.Play();
		} else {
			if (slideParticle.isPlaying)
				slideParticle.Stop();
		}
	}

	private float currentTrailTransparency = 0f;

	private void HandleSpeedTrails() {
		trail.transform.position = transform.position - new Vector3(velocity.normalized.x, velocity.normalized.y, 0f) * 3f;
		subTrail1.transform.position = trail.transform.position + Vector3.Cross(Vector3.forward, transform.position - trail.transform.position).normalized * 0.5f;
		subTrail1.transform.position += (transform.position - trail.transform.position).normalized * 0.5f;
		subTrail2.transform.position = trail.transform.position + Vector3.Cross(-Vector3.forward, transform.position - trail.transform.position).normalized * 0.5f;
		subTrail2.transform.position -= (transform.position - trail.transform.position).normalized * 0.5f;

		if (landedHook) {
			currentTrailTransparency = Mathf.InverseLerp(0.1f, 0.2f, velocity.magnitude);
		} else {
			currentTrailTransparency = Mathf.Clamp(currentTrailTransparency - Time.deltaTime * 8f, 0f, Mathf.Infinity);
		}
		trail.sharedMaterial.SetFloat(trailMaterialTransparencyName, currentTrailTransparency);
		bool visible = currentTrailTransparency != 0f;
		trail.gameObject.SetActive(visible);
	}

	private void PerfectHit() {
		perfectHitHappened = true;
	}

	private void HandleAccuracyText() {
		if (landedHook && perfectHitHappened) {
			if (!accuracyText.transform.parent.gameObject.activeSelf)
				accuracyText.transform.parent.gameObject.SetActive(true);
			Vector3 cross, cross1, cross2;
			Vector3 hitPosition = hitPoints[hitPoints.Count - 1].hookPosition;
			cross1 = Vector3.Cross((hitPosition - transform.position).normalized, Vector3.forward);
			cross2 = Vector3.Cross(Vector3.forward, (hitPosition - transform.position).normalized);
			cross = transform.position.y < hitPosition.y ? cross1.normalized : cross2.normalized;
			float textDistance = 2f;
			accuracyText.transform.parent.position = ((hitPoints[hitPoints.Count - 1].hookPosition + transform.position) / 2f) - cross * textDistance;
			accuracyText.transform.parent.transform.LookAt(accuracyText.transform.parent.position - cross, Vector3.up);
		} else {
			if (accuracyText.transform.parent.gameObject.activeSelf)
				accuracyText.transform.parent.gameObject.SetActive(false);
		}
	}

	private void HandleSwingSoundEffect() {
		float angle = Vector3.Angle(Vector3.up, gunPositionObject.transform.position - transform.position);
		if (!playedSwingSoundOnThisHook && angle <= 30f && landedHook) {
			playedSwingSoundOnThisHook = true;
			float maxVelLength = 0.9f;
			float vol = 0.25f * (velocity.magnitude/maxVelLength);
			float pitch = 0.25f + ((velocity.magnitude / maxVelLength) * 0.75f);
			SoundManager.Instance.CreateOneShot(AudioConfigDatabase.Instance.swing.audioClip,vol,pitch);
		} else if (angle > 30f) {
			playedSwingSoundOnThisHook = false;
		}
	}

	void OnDrawGizmos() {
		foreach (var hp in hitPoints) {
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(hp.hookPosition, 0.25f);
		}
	}
}
