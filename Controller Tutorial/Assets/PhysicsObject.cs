using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour {

	public float minGroundNormalY = .65f;

	// So we can half gravity, etc.
	public float gravityModifier = 1f;

	protected Vector2 targetVelocity;

	public bool grounded;
	protected Vector2 groundNormal;
	// Add the rigidbody of the object;
	protected Rigidbody2D rb2d;
	protected Vector2 velocity;
	protected ContactFilter2D contactFilter;
	protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
	protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);

	protected const float minMoveDistance = 0.001f;
	// Tiny bit of padding to ensure one collider doesn't get stuck in another.
	protected const float shellRadius = 0.01f;

	public bool climbingLadder;

	// Called every time the object is enabled. (After Awake, before Start?)
	void OnEnable(){
		rb2d = GetComponent < Rigidbody2D> ();
	}

	// Use this for initialization
	void Start () {
		// Don't check collisions for triggers.
		contactFilter.useTriggers = false;
		// Use the project settings for collision mask. This has a matrix so you can set what layers collide with what layers.
		contactFilter.SetLayerMask (Physics2D.GetLayerCollisionMask (gameObject.layer));
		contactFilter.useLayerMask = true;
	}
	
	// Update is called once per frame
	void Update () {
		targetVelocity = Vector2.zero;
		ComputeVelocity();
	}

	protected virtual void ComputeVelocity(){
		
	}

	void FixedUpdate(){

		// Use engine gravity.
		velocity = (climbingLadder)? targetVelocity : velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;

		velocity.x = targetVelocity.x;

		grounded = false;

		// The change in position per the last frame.
		Vector2 deltaPosition = velocity * Time.deltaTime;

		// The ground normal is perpendicular to the y. Using groundNormal.y for the x and -groundNormal.x for the y
		// means that we essentially get the slope of the ground as a vector.
		Vector2 moveAlongGround = new Vector2 (groundNormal.y, -groundNormal.x);

		Vector2 move = moveAlongGround * deltaPosition.x;

		Movement (move, false);

		// Only dealing with vertical movement.
		move = Vector2.up * deltaPosition.y;

		// Move the object (vertically).
		Movement (move, true);

	}

	void Movement(Vector2 move, bool yMovement){

		float distance = move.magnitude;

		// Basically, to check if up against a collision object, so we don't constantly check for collisions.
		if (distance > minMoveDistance) {

			// Cast the rb2d forward to check if it's going to collide in the move direction. 
			// Count is the number of collisions.
			int count = rb2d.Cast (move, contactFilter, hitBuffer, distance + shellRadius);
			// Don't use old data
			hitBufferList.Clear ();

			// We make a list of objects which will overlap our physics collider.
			for (int i = 0; i < count; i++) {
				hitBufferList.Add (hitBuffer [i]);
			}

			for (int i = 0; i < hitBufferList.Count; i++) {
				// Check normal of every raycast2d in list.
				Vector2 currentNormal = hitBufferList [i].normal;

				// If it's greater than the min, we want to make sure the player does not register as grounded.
				// Like if we hit a wall.
				if (currentNormal.y > minGroundNormalY) {
					grounded = true;
					if (yMovement) {
						groundNormal = currentNormal;
						currentNormal.x = 0;
					}
				} else if (currentNormal.y > 0) { 
					//sliding
					float slopeAngle = Vector2.Angle (currentNormal, Vector2.up);
					grounded = true;

					groundNormal = currentNormal;
					currentNormal.x = 0;

					move.x = (Mathf.Abs (move.y) - hitBufferList[i].distance) / Mathf.Tan (slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(currentNormal.x);
				}

				// Get the difference between velocity and current normal.
				// Determine whether we need to subtract from velocity to prevent player from entering
				// another collider.
				// eg. If we hit a roof, we don't want to stop and fall; we want to continue moving
				// horizontally but not vertically.
				float projection = Vector2.Dot (velocity, currentNormal);
				if (projection < 0) {
					// Cancel out the part of the velocity stopped by the current collision.
					velocity = velocity - projection * currentNormal;
				}

				float modifiedDistance = hitBufferList [i].distance - shellRadius;
				distance = modifiedDistance < distance ? modifiedDistance : distance;
			}

		}

		rb2d.position = rb2d.position + move.normalized * distance;

	}
}
