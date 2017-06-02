using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Physics : MonoBehaviour {

	protected Rigidbody2D rb2d;
	protected ContactFilter2D contactFilter;
	protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
	protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);

	protected Vector2 velocity;

	float gravity;
	public float maxJumpHeight = 4;
	public float minJumpHeight = 1;
	public float timeToJumpApex = .4f;
	float maxJumpVelocity;
	float minJumpVelocity;
	float moveSpeed = 6;
	float velocityXSmoothing;
	float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .1f;
	public float minGroundNormalY = .65f;

	protected const float minMoveDistance = 0.001f;
	// Tiny bit of padding to ensure one collider doesn't get stuck in another.
	protected const float shellRadius = 0.01f;

	CollisionInfo collisions;

	Vector2 playerInput;

	void OnEnable(){
		rb2d = GetComponent<Rigidbody2D> ();
	}

	void Start () {
		// Don't check collisions for triggers.
		contactFilter.useTriggers = false;
		// Use the project settings for collision mask. This has a matrix so you can set what layers collide with what layers.
		contactFilter.SetLayerMask (Physics2D.GetLayerCollisionMask (gameObject.layer));
		contactFilter.useLayerMask = true;
	}
	
	// Update is called once per frame
	void Update () {
		GetInput ();
		// The change in position per the last frame.
		Vector2 deltaPosition = velocity * Time.deltaTime;

		CalculateVelocity(deltaPosition);

		MovePlayer (playerInput);
	}

	void GetInput(){
		playerInput.x = Input.GetAxis ("Horizontal");
		playerInput.y = Input.GetAxis ("Vertical");
	}

	Vector2 CalculateVelocity(Vector2 move){
		collisions.Reset ();

		// First, let's calculate the y-velocity. This means calculating gravity.
		// So, first calculate gravity;
		gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		// Then the jump velocities. Look this up again.
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (gravity) * minJumpHeight);

		// Before we do anything, let's make sure we're not already touching something.
		float distance = move.magnitude;
		if (distance > minMoveDistance){

			// Now we'll calculate collisions.
			// Cast the rb2d forward to check if it's going to collide in the move direction. 
			// Count is the number of collisions.
			int count = rb2d.Cast (move, contactFilter, hitBuffer, distance + shellRadius);
			// Don't use old data
			hitBufferList.Clear ();

			for (int i = 0; i < hitBufferList.Count; i++) {
				Debug.Log (hitBufferList [i]);
				Vector2 currentNormal = hitBufferList [i].normal;
				Vector2 hitDistance = hitBufferList [i].distance;
				CalculateCollision (currentNormal,hitDistance);
			}
		}
		if (!collisions.slidingDownMaxSlope) {
			float targetVelocityX = move.x * moveSpeed;
			velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
			velocity.y += gravity * Time.deltaTime;
		} else {
			
		}
		
	}

	void CalculateCollision (Vector2 currentNormal, Vector2 hitDistance){
		if (Mathf.Sign (currentNormal.x) < 0 && hitDistance < shellRadius) {
			collisions.right = true;
		} else if (hitDistance < shellRadius) {
			collisions.left = true;
		}
		if (Mathf.Sign (currentNormal.y) < 0 && hitDistance < shellRadius) {
			collisions.above = true;
		} else if (hitDistance < shellRadius) {
			collisions.below = true;
		}

		if ((collisions.left || collisions.right) && collisions.below) {
			collisions.onSlope = true;
			if (Mathf.Sign (velocity.x) == Mathf.Sign (currentNormal.x)) {
				collisions.descendingSlope = true;
			} else {
				collisions.climbingSlope = true;
			}
			if (currentNormal.y < minGroundNormalY) {
				if (currentNormal.y > 0) {
					collisions.slidingDownMaxSlope = true;
				}
			}
		}

	}

	void HorizontalCollisions(){
		float directionX = Mathf.Sign (velocity.x);
	}

	void MovePlayer(Vector2 velocity){
		rb2d.position = rb2d.position + velocity;
	}

	public struct CollisionInfo{
		public bool above, below;
		public bool left, right;

		public bool onSlope;
		public bool descendingSlope;
		public bool climbingSlope;
		public bool slidingDownMaxSlope;

		public void Reset(){
			above = below = false;
			left = right = false;
			onSlope = false;
			descendingSlope = false;
			climbingSlope = false;
			slidingDownMaxSlope = false;
		}
	}
}
