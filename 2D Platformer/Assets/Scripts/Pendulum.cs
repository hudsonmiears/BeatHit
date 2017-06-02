using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Pendulum : RaycastController {

	public GameObject hinge;
	public Rigidbody2D rb2d;
	Transform center;
	public float speed = 1f;

	bool left = false;
	float currentRotationAngle = 0f;
	private Vector3 angleAxis;
	public float boundaryOffset = 0f;

	List<PassengerMovement> passengerMovement;
	Dictionary<Transform,Controller2D> passengerDictionary = new Dictionary<Transform, Controller2D>();
	public LayerMask passengerMask;

	float deltaAngle = 0;

	Vector3 prevPos;
	float radius;
//	Vector3 velocity;

	void Start(){
		base.Start ();

		center = hinge.transform;
		rb2d = this.GetComponent<Rigidbody2D> ();
		angleAxis = new Vector3 (0, 0, 1);
		radius = transform.position.y - center.position.y;

		distanceBetweenRays *= 2;

//		StartCoroutine (CalcVelocity ());

	}

	void Update(){
		UpdateRaycastOrigins (true);
		Vector3 velocity = CalculatePlatformMovement ();
		CalculatePassengerMovement (velocity - transform.position);

//		Debug.Log ("The velocity of the pendulum is: ");
//		Debug.Log (velocity);

		MovePassengers (true);
		transform.position = velocity;
		MovePassengers (false);



		Debug.DrawLine (transform.position, center.position, Color.magenta);
//		Debug.Log (velocity);
	}

	Vector2 CalculatePlatformMovement(){

		deltaAngle += speed * Time.deltaTime;
		float newX = Mathf.Cos(deltaAngle) * radius + center.position.x;
		float newY = Mathf.Sin (deltaAngle) * radius + center.position.y;

		if (newY >= center.position.y + boundaryOffset) {
			speed = -speed;
			StartCoroutine (IncreaseSpeed ());

			deltaAngle += speed * Time.deltaTime;
			newX = Mathf.Cos (deltaAngle) * radius + center.position.x;
			newY = Mathf.Sin (deltaAngle) * radius + center.position.y;
		}

		transform.rotation = Quaternion.LookRotation (Vector3.forward, center.position - transform.position);

		Vector3 newPos = new Vector2 (newX,newY);


		Vector3 returnVect = Vector3.Lerp (transform.position, newPos, 0.5f);


		return returnVect;

	}

	void MovePassengers(bool beforeMovePlatform){
		if (passengerMovement != null) {
			foreach (PassengerMovement passenger in passengerMovement) {
				if (!passengerDictionary.ContainsKey (passenger.transform)) {
					passengerDictionary.Add (passenger.transform, passenger.transform.GetComponent<Controller2D> ());
				}
				if (passenger.moveBeforePlatform == beforeMovePlatform) {
					passengerDictionary [passenger.transform].Move (passenger.velocity, passenger.standingOnPlatform, true, passenger.pendulumRotation);
				}
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
				rayOrigin += (Vector2)transform.right * (verticalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast (rayOrigin, transform.up * directionY, rayLength, passengerMask);

				if (hit) {
					Debug.Log ("hitvert");
				}

				if (hit && hit.distance != 0) {
					Debug.Log ("Hit1");
					if (!movedPassengers.Contains (hit.transform)) {
						movedPassengers.Add (hit.transform);
						float pushX = (directionY == 1) ? velocity.x : 0;
						float pushY = velocity.y - (hit.distance - skinWidth) * directionY;

						passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX,pushY), directionY == 1, true,transform.rotation));
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
				rayOrigin += (Vector2)transform.up * (horizontalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast (rayOrigin, transform.right * directionX, rayLength, passengerMask);

				if (hit) {
					Debug.Log ("hithoriz");
				}

				if (hit && hit.distance != 0) {
					Debug.Log ("Hit2");
					if (!movedPassengers.Contains (hit.transform)) {
						movedPassengers.Add (hit.transform);
						float pushX = velocity.x - (hit.distance - skinWidth) * directionX;
						float pushY = -skinWidth;

						passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX,pushY), true, true,transform.rotation));
					}
				}
			}
		}
//
		// Passenger on top of horizontally or downward moving platform
		if (directionY == -1 || velocity.y == 0 && velocity.x != 0){
			float rayLength = skinWidth * 2;

			for (int i = 0; i < verticalRayCount; i++) {
				Vector2 rayOrigin = raycastOrigins.topLeft + (Vector2)transform.right * (verticalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast (rayOrigin, transform.up, rayLength, passengerMask);

				if (hit) {
					Debug.Log ("hithorizordown");
				}

				if (hit && hit.distance != 0) {
					Debug.Log ("Hit3");
					if (!movedPassengers.Contains (hit.transform)) {
						movedPassengers.Add (hit.transform);
						float pushX = velocity.x;
						float pushY = velocity.y;

						passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX,pushY), true, false,transform.rotation));
					}
				}
			}
		}
	



//		for (int i = 0; i < verticalRayCount; i++) {
//
//			float rayLength = skinWidth * 2;
//			Vector2 rayOrigin = raycastOrigins.topLeft;
//			// Have to add velocity.x because we're moving left/right and need to update rayorigin to new position.
//			rayOrigin += (Vector2)transform.right * (verticalRaySpacing * i);
//			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, transform.up, rayLength, passengerMask);
//
////			Debug.DrawRay (rayOrigin, transform.up);
//
//			if (hit) {
//				Debug.Log ("hit");
//				Debug.DrawRay (rayOrigin, transform.up);
//			}
//
//			if (hit && hit.distance != 0) {
//				Debug.Log ("Hit1");
//				if (!movedPassengers.Contains (hit.transform)) {
//					movedPassengers.Add (hit.transform);
//					float pushX = velocity.x;
//					float pushY = velocity.y;
//
//					passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX,pushY), directionY == 1, true,rb2d.rotation));
//				}
//			}
//		}
	
//
//		//Vertically moving platform;
//		if (velocity.y != 0){
//			float rayLength = Mathf.Abs (velocity.y) + skinWidth;
//
//			for (int i = 0; i < verticalRayCount; i++) {
//				Vector2 rayOrigin = raycastOrigins.topLeft;
//				// Have to add velocity.x because we're moving left/right and need to update rayorigin to new position.
//				rayOrigin += Vector2.right * (verticalRaySpacing * i);
//				RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.up, rayLength, passengerMask);
//
//				if (hit && hit.distance != 0) {
//					if (!movedPassengers.Contains (hit.transform)) {
//						movedPassengers.Add (hit.transform);
//						float pushX = velocity.x - hit.transform.position.x;
//						float pushY = velocity.y - (hit.distance - skinWidth) - hit.transform.position.y;
//
//						passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX,pushY), directionY == 1, true, rb2d.rotation));
//					}
//				}
//			}
//		}
//		// Horizontally Moving Platform
//		if (velocity.x != 0) {
//			// Add skinWidth because we need it to include what's inside the player.
//			float rayLength = Mathf.Abs (velocity.x) + skinWidth;
//
//			// Create the rays along the side of the box.
//			for (int i = 0; i < horizontalRayCount; i++) {
//				Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight; // Rayorigin depending on move direction.
//				// Go up or down by the rayspacing to create rays.
//				rayOrigin += Vector2.up * (horizontalRaySpacing * i);
//				RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right * directionX, rayLength, passengerMask);
//
//				if (hit && hit.distance != 0) {
//					if (!movedPassengers.Contains (hit.transform)) {
//						movedPassengers.Add (hit.transform);
//						float pushX = velocity.x - (hit.distance - skinWidth) * directionX - hit.transform.position.x;
//						float pushY = -skinWidth - hit.transform.position.y;
//
//						passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX,pushY), false, true, rb2d.rotation));
//					}
//				}
//			}
//		}

		// Passenger on top of horizontally or downward moving platform
////		if (directionY == -1 || velocity.y == 0 && velocity.x != 0){
//		float rayLength = skinWidth * 2;
//
//		List<RaycastHit2D> hitList = new List<RaycastHit2D>();
//
//		for (int i = 0; i < verticalRayCount; i++) {
//			Vector2 rayOrigin = raycastOrigins.topLeft + (Vector2) transform.TransformDirection(Vector3.right) * (verticalRaySpacing * i);
//			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, (Vector2)transform.TransformDirection(Vector3.up), rayLength, passengerMask);
//			Debug.DrawRay (rayOrigin, (Vector2)transform.TransformDirection(Vector3.up), Color.red);
//
//			if (hit && hit.distance != 0) {
//
//				//				Debug.Break ();
//				Debug.Log ("Hit transform:");
//				Debug.Log (hit.transform.position);
//				Debug.Log ("Current Platform Velocity:");
//				Debug.Log (velocity);
//				if (!movedPassengers.Contains (hit.transform)) {
//					movedPassengers.Add (hit.transform);
//
//					float pushX = velocity.x;
//					float pushY = velocity.y;
////					float pushX = Mathf.Cos (deltaAngle) * radius + center.position.x - hit.transform.position.x;
////					float pushY = Mathf.Sin (deltaAngle) * radius + center.position.y - hit.transform.position.y;
//
//					Vector3 pushVector = new Vector3 (pushX, pushY);
//
//					passengerMovement.Add (new PassengerMovement (hit.transform, pushVector, true, false, rb2d.rotation));
//				}	
//				}
//			}
//		for (int i = 0; i < verticalRayCount; i++) {
//			Vector2 rayOrigin = raycastOrigins.bottomLeft + (Vector2)transform.TransformDirection(Vector3.right) * (verticalRaySpacing * i);
//			RaycastHit2D hitDown = Physics2D.Raycast (rayOrigin, (Vector2)transform.TransformDirection(Vector3.down), rayLength, passengerMask);
//			hitList.Add(hitDown);
//			Debug.DrawRay (rayOrigin, (Vector2)transform.TransformDirection(Vector3.down), Color.red);
//		}
//		for (int i = 0; i < horizontalRayCount; i++) {
//			Vector2 rayOrigin = raycastOrigins.bottomLeft + (Vector2)transform.TransformDirection(Vector3.up) * (horizontalRaySpacing * i);
//			RaycastHit2D hitLeft = Physics2D.Raycast (rayOrigin, (Vector2)transform.TransformDirection(Vector3.left), rayLength, passengerMask);
//			hitList.Add(hitLeft);
//			Debug.DrawRay (rayOrigin, (Vector2)transform.TransformDirection(Vector3.left), Color.red);
//		}
//		for (int i = 0; i < horizontalRayCount; i++) {
//			Vector2 rayOrigin = raycastOrigins.bottomRight + (Vector2)transform.TransformDirection(Vector3.up) * (horizontalRaySpacing * i);
//			RaycastHit2D hitRight = Physics2D.Raycast (rayOrigin, (Vector2)transform.TransformDirection(Vector3.right), rayLength, passengerMask);
//			hitList.Add(hitRight);
//			Debug.DrawRay (rayOrigin, (Vector2)transform.TransformDirection(Vector3.right), Color.red);
//		}

//		foreach (RaycastHit2D hit in hitList) {
//			if (hit && hit.distance != 0) {
//
////				Debug.Break ();
//				Debug.Log ("Hit transform:");
//				Debug.Log (hit.transform.position);
//				Debug.Log ("Current Platform Velocity:");
//				Debug.Log (velocity);
//				if (!movedPassengers.Contains (hit.transform)) {
//					movedPassengers.Add (hit.transform);
//
//					float pushX = velocity.x - (hit.distance - skinWidth) * directionX - hit.transform.position.x;
//					float pushY = velocity.y - (hit.distance - skinWidth) * directionY - hit.transform.position.y;
////					float pushX = Mathf.Cos (deltaAngle) * radius + center.position.x;
////					float pushY = Mathf.Sin (deltaAngle) * radius + center.position.y;
//
//					Vector3 pushVector = new Vector3 (pushX, pushY);
//
//					passengerMovement.Add(new PassengerMovement(hit.transform, pushVector, true, false, rb2d.rotation));
//				}
//			}
		}

//		Debug.DrawLine (raycastOrigins.bottomLeft, raycastOrigins.bottomRight, Color.magenta);
//		Debug.DrawLine (raycastOrigins.topLeft, raycastOrigins.topRight, Color.magenta);
//	}


//			if (hit && hit.distance != 0) {
//				if (!movedPassengers.Contains (hit.transform)) {
//					movedPassengers.Add (hit.transform);
//					float pushX = velocity.x;
//					float pushY = velocity.y;
//
//					passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX,pushY), true, false, rb2d.rotation));
//					}
//				}
//			}
//		}
//	}
//				}}}}

//	IEnumerator CalcVelocity()
//	{
//		while( Application.isPlaying )
//		{
//			// Position at frame start
//			prevPos = transform.position;
//			// Wait till it the end of the frame
//			yield return new WaitForEndOfFrame();
//			// Calculate velocity: Velocity = DeltaPosition / DeltaTime
//			velocity = (prevPos - transform.position) / Time.deltaTime;
////						Debug.Log( velocity );
//		}
//	}

	IEnumerator IncreaseSpeed(){
		speed = speed * 2;
		yield return new WaitForEndOfFrame ();
		speed = speed / 2;
	}

	struct PassengerMovement{
		public Transform transform;
		public Vector3 velocity;
		public bool standingOnPlatform;
		public bool moveBeforePlatform;
		public Quaternion pendulumRotation;

		public PassengerMovement(Transform _transform, Vector3 _velocity, bool _standingOnPlatform, bool _moveBeforePlatform, Quaternion _pendulumRotation){
			transform = _transform;
			velocity = _velocity;
			standingOnPlatform = _standingOnPlatform;
			moveBeforePlatform = _moveBeforePlatform;
			pendulumRotation = _pendulumRotation;
		}
	}

}
