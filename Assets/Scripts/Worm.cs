﻿using System;
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

	public void Initialize() {
		InputController.Instance.TapHappened += Tap;
		InputController.Instance.ReleaseHappened += Release;
		CameraController.Instance.target = this.transform;

		gravity = ConfigDatabase.Instance.gravityScale;
		ropeEnd = Instantiate(ropeEndPrefab, transform.position, transform.rotation) as GameObject;
		ropeEnd.gameObject.SetActive(false);
	}

	public void AddForce(Vector2 force) {
		velocity -= force;
	}

	private void Release(int inputIndex) {
		if (currentHoldID == inputIndex) {
			currentHoldID = -1;
			rotationEnabled = true;

			hookPositions.Clear();
			landedHook = false;
			ropeRenderer.positionCount = 0;
			ropeEnd.gameObject.SetActive(false);
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
		if (!landedHook && FindHookPoint(out hitPosition, currentHookSearchDistance)) {
			rotationEnabled = false;
			landedHook = true;
			hookPositions.Add(hitPosition);
			lastTimeAPointWasAdded = Time.realtimeSinceStartup;
			distanceToKeep = Vector3.Distance(hitPosition, transform.position);
		}
	}

	private void RefreshRopeRenderer() {
		if (landedHook || currentHoldID != -1) {
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
			RefreshRopeRenderer();
			RefreshWormRotation();
			CheckILastHitPointIsNotNeccessaryAnymore();
			LookForHookCollision();
		} else if (currentHoldID != -1) { //currently hooking
			SearchForHitPoint();
			RefreshRopeRenderer();
			currentHookSearchDistance += Time.deltaTime * ConfigDatabase.Instance.ropeShootSpeed;
			if (currentHookSearchDistance >= ConfigDatabase.Instance.maxRopeDistance && !landedHook)
				Release(currentHoldID);
		}
		//CheckIfGrounded();
		SimulatePhysics();
	}

	private void CheckIfGrounded() {
		Ray ray = new Ray(transform.position, Vector3.down);
		RaycastHit hit;
		Physics.Raycast(ray, out hit, feetLength);
		isGrounded = hit.collider != null;
	}

	private void SimulatePhysics() {
		velocity -= Vector2.down * gravity * Time.deltaTime;
		//if (isGrounded)
		//	velocity = Vector2.zero;

		Vector3 targetPosition = new Vector3(transform.position.x - velocity.x, transform.position.y - velocity.y, transform.position.z);
		transform.position = targetPosition;

		if (landedHook) {
			float currentDistance = Vector3.Distance(transform.position, hookPositions[hookPositions.Count - 1]);
			float difference = currentDistance - distanceToKeep;
			velocity -= ((Vector2)(hookPositions[hookPositions.Count - 1] - transform.position).normalized * difference);

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
		if (Time.realtimeSinceStartup - lastTimeAPointWasAdded < 0.25f)
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
}
