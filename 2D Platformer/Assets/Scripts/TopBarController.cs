using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Linq = System.Linq;

// Made by Hudson Miears, Feb 2017.

public class TopBarController : RaycastController {

	public float speed;
	public int directionX = 0;
	public int directionY = 1;
	public Vector3 deathBarVelocity;
	Vector2 deathBarDirMovement = new Vector2 (0, 1);

	public GameObject deathBar;
	Transform deathBarTransform;
	Vector2 deathBarPos;

	Transform topBarTransform;

	Vector3 velocity;

	Vector3[] localWaypoints;
	public Vector3[] globalWaypoints;
	int fromWaypointIndex;
	float percentBetweenWaypoints;
	int progress;
	bool waitTrue = true;
	float waitTime = 3f;
	float startTime = 3f;

	public GameObject player;

	public GameObject levelController;
	WaypointController waypointController;
//	public Camera mainCam;

	public LayerMask passengerMask;

	List<PassengerMovement> passengerMovement;
	Dictionary<Transform,Controller2D> passengerDictionary = new Dictionary<Transform, Controller2D>();


	public override void Start () {
		waypointController = levelController.GetComponent<WaypointController> ();
		globalWaypoints = waypointController.CreateWaypoints ();
		topBarTransform = transform;
		deathBarTransform = GetComponentInChildren<Transform> ();
		base.Start ();
		player.transform.position = globalWaypoints [0];
	}

	void Update(){
		UpdateRaycastOrigins ();
		velocity = (progress == 100) ? Vector3.zero : CalculateMovement ();
//		CalculateDeathBarVelocity ();
		CalculateTurns ();
		CalculatePassengerMovement (velocity);

		MovePassengers (true);
		transform.Translate (velocity,Space.World);
		MovePassengers (false);
//		mainCam.transform.rotation = Quaternion.identity;
	}
//
//	Vector3 CalculateDeathBarVelocity(){
//		deathBarVelocity.x = deathBarDirMovement.x * speed;
//		deathBarVelocity.y = deathBarDirMovement.y * speed;
//		return deathBarVelocity;
//	}

	Vector2 GetDeathBarPos(){
		deathBarPos = deathBarTransform.position;
		return deathBarPos;
	}

	Vector2 CalculateDirection(){
		GetDeathBarPos ();

		if (transform.position.x - deathBarPos.x != 0) {
			deathBarDirMovement.x = Mathf.Abs (transform.position.x - deathBarPos.x);
		} else {
			deathBarDirMovement.x = 0;
		}
		if (transform.position.y - deathBarPos.y != 0) {
			deathBarDirMovement.y = Mathf.Sign (transform.position.y - deathBarPos.y);
		} else {
			deathBarDirMovement.y = 0;
		}
		return deathBarDirMovement;
	}

	Vector3 CalculateMovement(){

//		if (Time.time < startTime) {
//			return globalWaypoints [0] - transform.position;
//		}

		fromWaypointIndex %= globalWaypoints.Length;
		int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
		float distanceBetweenWaypoints = Vector3.Distance (globalWaypoints [fromWaypointIndex], globalWaypoints [toWaypointIndex]);
		percentBetweenWaypoints += Time.deltaTime * speed/distanceBetweenWaypoints;
		percentBetweenWaypoints = Mathf.Clamp01 (percentBetweenWaypoints);

		Vector3 newPos = Vector3.Lerp (globalWaypoints [fromWaypointIndex], globalWaypoints [toWaypointIndex], percentBetweenWaypoints);

		if (percentBetweenWaypoints >= 1) {
			percentBetweenWaypoints = 0;
			fromWaypointIndex++;
				if (fromWaypointIndex >= globalWaypoints.Length - 1) {
				progress = 100;
				}
		}

		return newPos - transform.position;
	}
		


	void CalculateTurns(){
		if (progress != 100) {
			Vector2 moveDirection = velocity;
			float angle = Mathf.Atan2 (-moveDirection.x, moveDirection.y) * Mathf.Rad2Deg;
			Quaternion newRot = Quaternion.AngleAxis (angle, Vector3.forward);
			transform.rotation = newRot;
//			if (newRot != transform.rotation) {
//				if (velocity.y > 0) {
//					velocity += new Vector3 (0, 10);
//					GameObject cloneBar = (GameObject)Instantiate (gameObject);
//					cloneBar.transform.position = transform.position;
//					velocity -= new Vector3 (0, 10);
//					transform.rotation = newRot;
//				}
//
//			}
		}
	}


	void MovePassengers(bool beforeMovePlatform){
		foreach (PassengerMovement passenger in passengerMovement) {
			if (!passengerDictionary.ContainsKey (passenger.transform)) {
				passengerDictionary.Add (passenger.transform, passenger.transform.GetComponent<Controller2D> ());
			}
			if (passenger.moveBeforePlatform == beforeMovePlatform) {
				passengerDictionary[passenger.transform].Move (passenger.velocity, passenger.standingOnPlatform);
			}
		}
	}

	void CalculatePassengerMovement(Vector3 velocity){
		HashSet<Transform> movedPassengers = new HashSet<Transform> ();
		passengerMovement = new List<PassengerMovement>();

		float directionX = Mathf.Sign (velocity.x);
		float directionY = Mathf.Sign (velocity.y);

		//Vertically moving platform;
		if (velocity.y != 0){
			float rayLength = Mathf.Abs (velocity.y) + skinWidth;

			for (int i = 0; i < verticalRayCount; i++) {
				Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
				// Have to add velocity.x because we're moving left/right and need to update rayorigin to new position.
				rayOrigin += Vector2.right * (verticalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.up * directionY, rayLength, passengerMask);

				if (hit && hit.distance != 0) {
					if (!movedPassengers.Contains (hit.transform)) {
						movedPassengers.Add (hit.transform);
						float pushX = (directionY == 1) ? velocity.x : 0;
						float pushY = velocity.y - (hit.distance - skinWidth) * directionY;

						passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX,pushY), directionY == 1, true));
					}
				}
			}
		}
		// Horizontally Moving Platform
		if (velocity.x != 0) {
			// Add skinWidth because we need it to include what's inside the player.
			float rayLength = Mathf.Abs (velocity.x) + skinWidth;

			// Create the rays along the side of the box.
			for (int i = 0; i < horizontalRayCount; i++) {
				Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight; // Rayorigin depending on move direction.
				// Go up or down by the rayspacing to create rays.
				rayOrigin += Vector2.up * (horizontalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right * directionX, rayLength, passengerMask);

				if (hit && hit.distance != 0) {
					if (!movedPassengers.Contains (hit.transform)) {
						movedPassengers.Add (hit.transform);
						float pushX = velocity.x - (hit.distance - skinWidth) * directionX;
						float pushY = -skinWidth;

						passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX,pushY), false, true));
					}
				}
			}
		}

		// Passenger on top of horizontally or downward moving platform
		if (directionY == -1 || velocity.y == 0 && velocity.x != 0){
			float rayLength = skinWidth * 2;

			for (int i = 0; i < verticalRayCount; i++) {
				Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.up, rayLength, passengerMask);

				if (hit && hit.distance != 0) {
					if (!movedPassengers.Contains (hit.transform)) {
						movedPassengers.Add (hit.transform);
						float pushX = velocity.x;
						float pushY = velocity.y;

						passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX,pushY), true, false));
					}
				}
			}
		}
	}

	struct PassengerMovement{
		public Transform transform;
		public Vector3 velocity;
		public bool standingOnPlatform;
		public bool moveBeforePlatform;

		public PassengerMovement(Transform _transform, Vector3 _velocity, bool _standingOnPlatform, bool _moveBeforePlatform){
			transform = _transform;
			velocity = _velocity;
			standingOnPlatform = _standingOnPlatform;
			moveBeforePlatform = _moveBeforePlatform;
		}
	}

	void OnDrawGizmos(){
		if (globalWaypoints != null) {
			Gizmos.color = Color.green;
			float size = .7f;

			for (int i = 0; i < globalWaypoints.Length; i++) {
				Vector3 globalWaypointPos = (Application.isPlaying)?globalWaypoints[i] : localWaypoints [i] + transform.position;
				Gizmos.DrawLine (globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
				Gizmos.DrawLine (globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
			}
		}
	}

}
