using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worm : MonoBehaviour {

	public GameObject ropeEndPrefab;

	public GameObject gunPositionObject;
	public LineRenderer ropeRenderer;

	public Vector2 Velocity {
		get {
			return velocity;
		}
	}

	public GameObject ragdoll;

	private bool rotationEnabled = true;
	private GameObject ropeEnd;

	private bool landedHook = false;
	private List<Vector3> hookPositions = new List<Vector3>();
	private float distanceToKeep = 0f;

	private bool isGrounded = false;
	private Vector2 velocity;
	private float gravity = 0.2f;
	private float feetLength = 2f;
	private float lastTimeAPointWasAdded = 0f;

	private int currentHoldID = -1;
	private float currentHookSearchDistance = 0f;

	Rigidbody rb;
	BoxCollider boxCollider;

	public void Initialize() {
		boxCollider = GetComponent<BoxCollider>();
		rb = GetComponent<Rigidbody>();

		InputController.Instance.TapHappened += Tap;
		InputController.Instance.ReleaseHappened += Release;
		CameraController.Instance.target = this.transform;

		gravity = ConfigDatabase.Instance.gravityScale;
		ropeEnd = Instantiate(ropeEndPrefab, transform.position, transform.rotation) as GameObject;
		ropeEnd.gameObject.SetActive(false);
	}

	public void AddForce(Vector2 force) {
		velocity += force;
	}

	private void Release(int inputIndex) {
		if (currentHoldID == inputIndex) {
			currentHoldID = -1;
			rotationEnabled = true;

			hookPositions.Clear();
			landedHook = false;
			ropeRenderer.positionCount = 0;
			ropeEnd.gameObject.SetActive(false);
			GameController.Instance.ReleasedHook();
		}
	}

	private void Tap(int inputIndex) {

		if (currentHoldID == inputIndex)
			Release(inputIndex);

		currentHoldID = inputIndex;

		currentHookSearchDistance = 0f;

		SearchForHitPoint();
	}

	private void SearchForHitPoint() {
		Vector3 hitPosition = Vector3.zero;
		if (!landedHook && FindHookPoint(out hitPosition, ConfigDatabase.Instance.maxRopeDistance)) {
			rotationEnabled = false;
			landedHook = true;
			hookPositions.Add(hitPosition);
			lastTimeAPointWasAdded = Time.realtimeSinceStartup;
			distanceToKeep = Vector3.Distance(hitPosition, transform.position);
			AddWormPullingForce(hitPosition);
			GameController.Instance.FoundPotentionalHitPoint(hitPosition);
			GameController.Instance.LandedHook();
		}
	}

	private void AddWormPullingForce(Vector3 hitPosition) {
		Vector2 forceDirection = (hitPosition - gunPositionObject.transform.position).normalized;
		float angle = Vector2.Angle(Vector2.up, forceDirection);
		forceDirection = Rotate(forceDirection, -Mathf.Sign(hitPosition.x - gunPositionObject.transform.position.x) * 90f);
		float maximumRewardedAngle = ConfigDatabase.Instance.maximumRewardedAngleForPullForce;
		float effectivenessMultiplier = Mathf.Clamp((180f - Mathf.Abs(angle - 45f)) - (180f - maximumRewardedAngle), 0f, maximumRewardedAngle);
		effectivenessMultiplier = effectivenessMultiplier / maximumRewardedAngle;
		float reverseVelocityEfficient = 2 * (1f - Vector2.Dot(forceDirection.normalized, velocity.normalized));
		AddForce(forceDirection * effectivenessMultiplier * 0.01f * ConfigDatabase.Instance.pullForceMultiplier * ((1f/(0.1f + velocity.magnitude * 0.1f)) + reverseVelocityEfficient));
	}

	private void RefreshRopeRenderer() {
		if (landedHook) {
			List<Vector3> lineRendererPoints = new List<Vector3>();
			lineRendererPoints.Add(gunPositionObject.transform.position);

			Vector3 direction = Vector3.zero;
			if (landedHook) {
				ropeRenderer.positionCount = hookPositions.Count + 1;
				for (int i = hookPositions.Count - 1; i >= 0; i--) {
					lineRendererPoints.Add(hookPositions[i]);
				}
				if (hookPositions.Count >= 2)
					direction = (hookPositions[0] - hookPositions[1]).normalized;
				else
					direction = (hookPositions[0] - gunPositionObject.transform.position).normalized;
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
		Vector3 hookDir = hookPositions[hookPositions.Count - 1] - gunPositionObject.transform.position;
		currentFacingDir = currentFacingDir.normalized;
		hookDir = hookDir.normalized;
		Vector3 cross = Vector3.Cross(currentFacingDir, hookDir);

		transform.Rotate(new Vector3(0f, 0f, cross.z * Vector3.Angle(hookDir, currentFacingDir)));
	}

	private void Update() {
		if (rotationEnabled) {
			transform.Rotate(new Vector3(0f, 0f, Time.deltaTime * ConfigDatabase.Instance.rotationSpeed));
		}
		if (landedHook) {
			RefreshWormRotation();
			CheckILastHitPointIsNotNeccessaryAnymore();
			LookForHookCollision();
		} else {
			Vector3 hitPosition;
			if (FindHookPoint(out hitPosition, ConfigDatabase.Instance.maxRopeDistance)) {
				GameController.Instance.FoundPotentionalHitPoint(hitPosition);
			}
		}
		SimulatePhysics();
		CheckIfOutOfBoundaries();
	}

	private void LateUpdate() {
		if(landedHook)
			RefreshRopeRenderer();
	}

	private void FixedUpdate() {
		HookCorrection();
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
		if (transform.position.y < ConfigDatabase.Instance.allTimeMininmumWorldY)
			Die();
	}

	private void SimulatePhysics() {
		rb.velocity = Vector3.zero;

		if (!isGrounded)
		velocity += Vector2.down * gravity * Time.deltaTime;

		Vector3 targetPosition = new Vector3(transform.position.x + (velocity.x * Time.deltaTime * 20 * 100f / ConfigDatabase.Instance.wormMass), transform.position.y + (velocity.y * Time.deltaTime * 20 * 100f / ConfigDatabase.Instance.wormMass), transform.position.z);
		transform.position = targetPosition;

	}

	private void HookCorrection() {
		if (landedHook) {
			float currentDistance = Vector3.Distance(transform.position, hookPositions[hookPositions.Count - 1]);
			float difference = currentDistance - distanceToKeep;
			Vector2 differenceVelocity = ((Vector2)(hookPositions[hookPositions.Count - 1] - transform.position).normalized * difference);
			velocity += differenceVelocity * Time.deltaTime * 10f * ConfigDatabase.Instance.swingForceMultiplier;

			transform.position += ((hookPositions[hookPositions.Count - 1] - transform.position).normalized * difference);
		}
	}

	public bool FindHookPoint(out Vector3 hookPoint, float distanceToUse) {
		RaycastHit hit;
		hookPoint = Vector3.zero;
		Ray ray = new Ray(gunPositionObject.transform.position, (gunPositionObject.transform.position - transform.position).normalized);
		Physics.Raycast(ray, out hit, distanceToUse);
		if (hit.collider != null) {
			hookPoint = hit.point;
		}
		return hit.collider != null;
	}

	public void LookForHookCollision() {
		if (Time.realtimeSinceStartup - lastTimeAPointWasAdded < 0.025f)
			return;
		Ray ray = new Ray(gunPositionObject.transform.position, (hookPositions[hookPositions.Count - 1] - gunPositionObject.transform.position).normalized);
		RaycastHit hit;
		Physics.Raycast(ray, out hit);
		if (hit.point != hookPositions[hookPositions.Count - 1]) {
			lastTimeAPointWasAdded = Time.realtimeSinceStartup;
			hookPositions.Add(hit.point);
			distanceToKeep = Vector3.Distance(hit.point, transform.position);
		}
	}

	private void CheckILastHitPointIsNotNeccessaryAnymore() {
		if (hookPositions.Count >= 2) {
			float diffDot = Vector3.Dot((hookPositions[hookPositions.Count - 1] - gunPositionObject.transform.position).normalized, (hookPositions[hookPositions.Count - 2] - gunPositionObject.transform.position).normalized);
			if (Mathf.Abs(diffDot - 1f) < 0.01f) {
				RaycastHit hit;
				Ray ray = new Ray(gunPositionObject.transform.position, (hookPositions[hookPositions.Count - 2] - gunPositionObject.transform.position).normalized);
				Physics.Raycast(ray, out hit);
				float distance = Vector3.Distance(hookPositions[hookPositions.Count - 2], hit.point);
				if (distance < 1f)
					DeleteLastHitPoint();
			}
		}
	}

	private void DeleteLastHitPoint() {
		lastTimeAPointWasAdded = Time.realtimeSinceStartup;
		hookPositions.Remove(hookPositions[hookPositions.Count - 1]);
		distanceToKeep = Vector3.Distance(hookPositions[hookPositions.Count - 1], transform.position);
	}

	public void Die() {
		GameController.Instance.FinishGame(false);

		InputController.Instance.TapHappened -= Tap;
		InputController.Instance.ReleaseHappened -= Release;
		Destroy(ropeEnd.gameObject);
		ragdoll.gameObject.SetActive(true);
		ragdoll.transform.SetParent(null);
		foreach (var rb in ragdoll.GetComponentsInChildren<Rigidbody>()) {
			rb.AddForce(velocity * 100f,ForceMode.Impulse);
		}
		Destroy(this.gameObject);
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

	private void OnCollisionEnter(Collision collision) {
		if (landedHook) {
			if (velocity.magnitude > 0.01f) {
				velocity = -velocity * ConfigDatabase.Instance.remainingVelocityPercentAfterBounce;
			}
		} else if (collision.collider.CompareTag("Finish")) {
			if (velocity.magnitude > 0.01f) {
				velocity = Vector2.Reflect(velocity, FindMedian(collision.contacts).medianNormal);
			}
		} else if (!collision.collider.CompareTag("LaunchPad")) {
			Die();
		}
	}

	private void OnCollisionStay(Collision collision) {
		Debug.Log((FindMedian(collision.contacts).medianPoint - transform.position).normalized);
			float dot = Vector3.Dot(velocity.normalized, (FindMedian(collision.contacts).medianPoint - transform.position).normalized);
			dot = (dot + 1) / 2f;
		Debug.Log(dot);
			velocity *= 1 - dot;
	}

	MedianPoint FindMedian(ContactPoint[] contacts) {
		MedianPoint mP = new MedianPoint();
		mP.medianPoint = Vector3.zero;
		mP.medianNormal = Vector3.zero;
		foreach (var cP in contacts) {
			mP.medianPoint.x += cP.point.x;
			mP.medianPoint.y += cP.point.y;
			mP.medianPoint.z += cP.point.z;

			mP.medianNormal.x += cP.normal.x;
			mP.medianNormal.y += cP.normal.y;
			mP.medianNormal.z += cP.normal.z;
		}
		mP.medianPoint /= contacts.Length;
		mP.medianNormal /= contacts.Length;
		return mP;
	}

	public class MedianPoint {
		public Vector3 medianPoint;
		public Vector3 medianNormal;
	}
}
