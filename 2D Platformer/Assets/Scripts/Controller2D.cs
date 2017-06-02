using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller2D : RaycastController {

	public float maxSlopeAngle = 60;
	Rigidbody2D rb2d;

	bool grounded;

	public CollisionInfo collisions;
	Vector2 playerInput;

	public override void Start(){
		rb2d = GetComponent<Rigidbody2D> ();
		base.Start ();
		collisions.faceDir = 1;
	}

	public void Move(Vector2 moveAmount, bool standingOnPlatform, bool standingOnPendulum = false, Quaternion pendulumRotation = default(Quaternion)){
		Move (moveAmount, Vector2.zero, standingOnPlatform, standingOnPendulum, default(Quaternion));
	}

	public void Move(Vector2 moveAmount, Vector2 input, bool standingOnPlatform = false, bool standingOnPendulum = false, Quaternion pendulumRotation = default(Quaternion)){
		// Because every time we move the raycasts need to move too.
		UpdateRaycastOrigins ();
		collisions.Reset();
		collisions.moveAmountOld = moveAmount;
		playerInput = input;

		if (standingOnPendulum) {
			
		}

		// If moving down (but not fallling?)
		if (moveAmount.y < 0) {
			DescendSlope (ref moveAmount);
		}

		// get the correct x direction.
		if (moveAmount.x != 0) {
			collisions.faceDir = (int)Mathf.Sign (moveAmount.x);
		}
			
		HorizontalCollisions (ref moveAmount);

		// Moving vertically (falling or jumping)
		if (moveAmount.y != 0) {
			VerticalCollisions (ref moveAmount);
		}

		InteractiveCollisions (ref moveAmount);

		// Move by moveAmount.
		transform.Translate (moveAmount);
//		rb2d.position = rb2d.position + moveAmount;
//		if (collisions.climbingLadder && !collisions.below) {
//			collisions.below = true;
//			rb2d.position = new Vector2 (collisions.ladderX, rb2d.position.y);
//		}

		if (standingOnPlatform) {
			collisions.below = true;
		}

		if (standingOnPendulum) {
			collisions.below = true;
			Debug.Log (pendulumRotation);
			transform.rotation = pendulumRotation;
		}

		Debug.Log ("Collisions.below:");
		Debug.Log (collisions.below);
	}

//	public void MoveLadder(Vector2 moveAmount, Vector2 input, float climbSpeed, float gravity, float maxJumpHeight){
//		UpdateRaycastOrigins ();
//		collisions.Reset();
//		collisions.moveAmountOld = moveAmount;
//		playerInput = input;
//
//		moveAmount.y = (playerInput.y != 0) ? playerInput.y * climbSpeed : 0;
//		moveAmount.y -= gravity * Time.deltaTime;
//		moveAmount.x = 0;
//
//		if (collisions.below) {
//			if (moveAmount.y < 0) {
//				moveAmount.y = 0;
//			}
//		}
//
//		if (Input.GetButtonDown("Jump")){
//			if (playerInput.x == 0) {
//				moveAmount.y += maxJumpHeight;
//			} else {
////				climbingLadder = false;
//				moveAmount.x = (playerInput.x * wallJumpOff.x) + (Mathf.Abs(ladderLeft - controller.collisions.ladderX) * Mathf.Sign(directionalInput.x));
//				velocity.y = wallJumpOff.y;
//			}
//		}
//
//	}

	void InteractiveCollisions(ref Vector2 moveAmount){
		float directionX = collisions.faceDir;

		float rayLength = Mathf.Abs (moveAmount.x) + skinWidth;

		if (Mathf.Abs (moveAmount.x) < skinWidth) {
			rayLength = 2 * skinWidth;
		}

		for (int i = 0; i < horizontalRayCount; i++) {
			Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;

			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right * directionX, rayLength, interactiveMask);

			if (hit) {
//				Debug.Log ("hit");
				if (hit.collider.gameObject.tag == "ladder") {
					if (!collisions.groundedLadder) {
						Debug.Log ("ladder");
						collisions.climbingLadder = true;
						collisions.ladderX = hit.transform.position.x;
					}
				}
			}
		}
	}

	void HorizontalCollisions(ref Vector2 moveAmount){
		float directionX = collisions.faceDir;

		// Add skinWidth because we need it to include what's inside the player.
		float rayLength = Mathf.Abs (moveAmount.x) + skinWidth;

		if (Mathf.Abs(moveAmount.x) < skinWidth) {
			rayLength = 2 * skinWidth;
		}

		// Create the rays along the side of the box.
		for (int i = 0; i < horizontalRayCount; i++) {
			Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight; // Rayorigin depending on move direction.
			// Go up or down by the rayspacing to create rays.
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
//			RaycastHit2D hit2 = Physics2D.Raycast (rayOrigin, Vector2.right * directionX, rayLength, interactiveMask);

			Debug.DrawRay (rayOrigin, Vector2.right * directionX, Color.red);


			if (hit) {



				if (hit.distance == 0) {
					continue;
				}

				float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);

				if (i == 0 && slopeAngle <= maxSlopeAngle) {
					if (collisions.descendingSlope) {
						collisions.descendingSlope = false;
						moveAmount = collisions.moveAmountOld;
					}
					float distanceToSlopeStart = 0;
					if (slopeAngle != collisions.slopeAngleOld) {
						distanceToSlopeStart = hit.distance - skinWidth;
						moveAmount.x -= distanceToSlopeStart * directionX;
					}
					ClimbSlope (ref moveAmount, slopeAngle, hit.normal);
					moveAmount.x += distanceToSlopeStart * directionX;
				}

				if (!collisions.climbingSlope || slopeAngle > maxSlopeAngle) {
					moveAmount.x = (hit.distance - skinWidth) * directionX;
					rayLength = hit.distance;

					if (collisions.climbingSlope) {
						moveAmount.y = Mathf.Tan (collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs (moveAmount.x);
					}

					collisions.left = directionX == -1;
					collisions.right = directionX == 1;
				}


				if (hit.collider.tag == "deathBar") {
					gameObject.SetActive (false);

				}

				if (hit.collider.tag == "pendulum") {
					collisions.below = true;
				}
			}
		}
	}

	void VerticalCollisions(ref Vector2 moveAmount){
		float directionY = Mathf.Sign (moveAmount.y);
		float directionX = Mathf.Sign (moveAmount.x);
		float rayLength = Mathf.Abs (moveAmount.y) + skinWidth;
	
		for (int i = 0; i < verticalRayCount; i++) {
			Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			// Have to add moveAmount.x because we're moving left/right and need to update rayorigin to new position.
			rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
			Debug.DrawRay (rayOrigin, Vector2.up * directionY, Color.red);

			if (hit) {

				if (hit.collider.tag == "throughPlatform") {
					if (directionY == 1 || hit.distance == 0) {
						continue;
					}
					if (collisions.fallingThroughPlatform){
						continue;
					}
					if (playerInput.y == -1) {
						collisions.fallingThroughPlatform = true;
						Invoke ("ResetFallingThroughPlatform", .5f);
						continue;
					}
				}



				moveAmount.y = (hit.distance - skinWidth) * directionY;
				rayLength = hit.distance;

				if (collisions.climbingSlope) {
					moveAmount.x = moveAmount.y / Mathf.Tan (collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign (moveAmount.x);
				}

				if (hit.collider.tag == "deathBar") {
					gameObject.SetActive (false);
				}




				collisions.below = directionY == -1;
				collisions.above = directionY == 1;
			}
		}
	}


	void ClimbSlope(ref Vector2 moveAmount, float slopeAngle, Vector2 slopeNormal){
		float moveDistance = Mathf.Abs (moveAmount.x);
		float climbmoveAmountY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
		if (moveAmount.y <= climbmoveAmountY) {
			moveAmount.y = climbmoveAmountY;
			moveAmount.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
			collisions.below = true;
			collisions.climbingSlope = true;
			collisions.slopeAngle = slopeAngle;
			collisions.slopeNormal = slopeNormal;
		}

	}

	void DescendSlope(ref Vector2 moveAmount){

		RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast (raycastOrigins.bottomLeft, Vector2.down, Mathf.Abs (moveAmount.y) + skinWidth, collisionMask);
		RaycastHit2D maxSlopeHitRight = Physics2D.Raycast (raycastOrigins.bottomRight, Vector2.down, Mathf.Abs (moveAmount.y) + skinWidth, collisionMask);

		if (maxSlopeHitLeft ^ maxSlopeHitRight) {
			SlideDownMaxSlope (maxSlopeHitLeft, ref moveAmount);
			SlideDownMaxSlope (maxSlopeHitRight, ref moveAmount);
		}

		if (!collisions.slidingDownMaxSlope) {
			float directionX = Mathf.Sign (moveAmount.x);
			Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

			if (hit) {
				float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);
				if (slopeAngle != 0 && slopeAngle <= maxSlopeAngle) {
					if (Mathf.Sign (hit.normal.x) == directionX) {
						if (hit.distance - skinWidth <= Mathf.Tan (slopeAngle * Mathf.Deg2Rad) * Mathf.Abs (moveAmount.x)) {
							float moveDistance = Mathf.Abs (moveAmount.x);
							float descendmoveAmountY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
							moveAmount.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (moveAmount.x);
							moveAmount.y -= descendmoveAmountY;

							collisions.slopeAngle = slopeAngle;
							collisions.descendingSlope = true;
							collisions.below = true;
							collisions.slopeNormal = hit.normal;
						}
					}
				}
			}
		}
	}

	void SlideDownMaxSlope (RaycastHit2D hit, ref Vector2 moveAmount){
	
		if (hit) {
			float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);
			if (slopeAngle > maxSlopeAngle) {
				moveAmount.x = (Mathf.Abs (moveAmount.y) - hit.distance) / Mathf.Tan (slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(hit.normal.x);

				collisions.slopeAngle = slopeAngle;
				collisions.slidingDownMaxSlope = true;
				collisions.slopeNormal = hit.normal;

			}
		}
	}


	public void ResetFallingThroughPlatform(){
		collisions.fallingThroughPlatform = false;
	}


	public struct CollisionInfo {
		public bool above, below;
		public bool left, right;

		public bool climbingSlope;
		public bool descendingSlope;
		public bool slidingDownMaxSlope;

		public bool climbingLadder;
		public bool groundedLadder;
		public float ladderX;

		public float slopeAngle, slopeAngleOld;
		public Vector2 slopeNormal;
		public Vector2 moveAmountOld;
		public int faceDir;
		public bool fallingThroughPlatform;

		public void Reset() {
			above = below = false;
			left = right = false;
			climbingSlope = false;
			descendingSlope = false;
			slidingDownMaxSlope = false;
			climbingLadder = false;
			groundedLadder = false;
			ladderX = 0;
			slopeNormal = Vector2.zero;

			slopeAngleOld = slopeAngle;
			slopeAngle = 0;
		}
	}

}
