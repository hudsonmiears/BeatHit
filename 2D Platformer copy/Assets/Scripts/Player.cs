using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {

	public float jumpHeight = 4;
	public float timeToJumpApex = .4f;

	float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .1f;
	float moveSpeed = 6;

	float gravity;
	float jumpVelocity;
	Vector3 velocity;
	float velocityXSmoothing;

	Controller2D controller;

	Animator anim;
	SpriteRenderer sprite;

	void Start(){
		controller = GetComponent<Controller2D> ();
		anim = GetComponent<Animator>();
		sprite = GetComponent<SpriteRenderer> ();

		gravity = -(2 * jumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		print ("Gravity: " + gravity + "Jump Velocity: " + jumpVelocity); 

	}

	void Update(){
		//animation
		anim.SetFloat("speed", Mathf.Abs(Input.GetAxis("Horizontal") * moveSpeed));
		anim.SetFloat ("jump", 0);
		Flip ();

		if (controller.collisions.above || controller.collisions.below) {
			velocity.y = 0;
		}

		Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));

		if (Input.GetKey (KeyCode.Space) && controller.collisions.below) {
			anim.SetBool ("jump", true);
			velocity.y = jumpVelocity;
		} else if (controller.collisions.below) {
			anim.SetBool ("jump", false);
		}

		// X is the input times the speed. Gravity is added to Y over time. 
		// Then Move is called using the velocity over time.
		float targetVelocityX = input.x * moveSpeed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);
		velocity.y += gravity * Time.deltaTime;
		controller.Move (velocity * Time.deltaTime);
	}

	void Flip(){
		sprite.flipX = (Mathf.Sign (velocity.x) * 1 == 1) ? false : true;
	}
}
