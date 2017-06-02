using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {

	Rigidbody2D rb2d;

	public float maxJumpHeight = 4;
	public float minJumpHeight = 1;
	public float timeToJumpApex = .4f;

	float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .1f;
	public float moveSpeed = 6;

	public Vector2 wallJumpClimb;
	public Vector2 wallJumpOff;
	public Vector2 wallLeap;
	public float wallSlideSpeedMax = 3;
	public float wallStickTime = .25f;
	float timeToWallUnstick;

	bool wallSliding;
	int wallDirX;

	bool climbingLadder;
	public float ladderWidth = 0.3f;
	float ladderLeft;

	public float climbSpeed = 1.5f;
	float velocityYSmoothing;

	float gravity;
	float maxJumpVelocity;
	float minJumpVelocity;
	Vector3 velocity;
	float velocityXSmoothing;

	Controller2D controller;

	Vector2 directionalInput;

	Animator anim;
	SpriteRenderer sprite;

	void Start(){
		controller = GetComponent<Controller2D> ();
		anim = GetComponent<Animator>();
		sprite = GetComponent<SpriteRenderer> ();
		rb2d = GetComponent<Rigidbody2D> ();

		gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (gravity) * minJumpHeight);
		print ("Gravity: " + gravity + "Jump Velocity: " + maxJumpVelocity); 

	}

	void Update(){

		CalculateVelocity ();
		HandleWallSliding ();
		CheckLadders ();


		//animation
		anim.SetFloat("speed", Mathf.Abs(Input.GetAxis("Horizontal") * moveSpeed));

		Flip ();



		if (climbingLadder == true) {
			velocity = DoLadders (ladderLeft);
		}
		controller.Move (velocity * Time.deltaTime, directionalInput);


		if (controller.collisions.above || controller.collisions.below) {
			if (controller.collisions.slidingDownMaxSlope) {
				velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
			} else if (controller.collisions.below) {
				anim.SetBool ("jump", false);
				velocity.y = 0;
			}
			else {
				velocity.y = 0;
			}
		}
//		Debug.Log (controller.collisions.climbingLadder);
	}

	public void SetDirectionalInput(Vector2 input){
		directionalInput = input;
	}

	public void OnJumpInput(){
		anim.SetBool ("jump", true);
		if (wallSliding){
			if (wallDirX == directionalInput.x) {
				velocity.x = -wallDirX * wallJumpClimb.x;
				velocity.y = wallJumpClimb.y;
			} else if (directionalInput.x == 0) {
				velocity.x = -wallDirX * wallJumpOff.x;
				velocity.y = wallJumpOff.y;
			} else {
				velocity.x = -wallDirX * wallLeap.x;
				velocity.y = wallLeap.y;
			}
		}
		if (controller.collisions.below){
			if (controller.collisions.slidingDownMaxSlope) {
				if (directionalInput.x != -Mathf.Sign (controller.collisions.slopeNormal.x)) { // not jumping against maxslope
					velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
					velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
				}
			} else {
				velocity.y = maxJumpVelocity;
			}
		}

	}

	public void OnJumpInputUp(){
	if (velocity.y > minJumpVelocity) {
		velocity.y = minJumpVelocity;
	}
	}

	void HandleWallSliding(){
		wallDirX = (controller.collisions.left) ? -1 : 1;
		wallSliding = false;
		if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0) {
			wallSliding = true;

			if (velocity.y < -wallSlideSpeedMax) {
				velocity.y = -wallSlideSpeedMax;
			}

			if (timeToWallUnstick > 0) {

				velocityXSmoothing = 0;
				velocity.x = 0;

				if (directionalInput.x != wallDirX && directionalInput.x != 0) {
					timeToWallUnstick -= Time.deltaTime;
				} else {
					timeToWallUnstick = wallStickTime;
				}
			} else {
				timeToWallUnstick = wallStickTime;
			}
		}
	}

	void CheckLadders(){
		ladderLeft = controller.collisions.ladderX - ladderWidth;
		float ladderRight = controller.collisions.ladderX + ladderWidth;

//		Debug.Log(controller.collisions.ladderX);
		if (transform.position.x > ladderLeft && transform.position.x < ladderRight) {
			if (directionalInput.y != 0) {
				if (controller.collisions.climbingLadder) {
					climbingLadder = true;
				}
			}
		} else {
			climbingLadder = false;
		}
	}

	Vector2 DoLadders(float ladderLeft){
		if (climbingLadder) {
			anim.SetBool ("babyClimb", true);
			velocity.y = (directionalInput.y != 0) ? directionalInput.y * climbSpeed : 0;
			if (!controller.collisions.below) {
				velocity.x = 0;
			}

			if (controller.collisions.below) {
				if (directionalInput.y < 0) {
					velocity.y = 0;
				}
			}

			if (Input.GetButtonDown("Jump")){
				anim.SetBool ("jump", true);
				climbingLadder = false;
				if (directionalInput.x == 0) {
					if (directionalInput.y < 0) {
					} else {
						climbingLadder = true;
						velocity.y += 80f;
					}

				} else {
					velocity.x = (directionalInput.x * wallLeap.x) + (Mathf.Abs(ladderLeft - controller.collisions.ladderX) * Mathf.Sign(directionalInput.x));
					velocity.y = wallLeap.y;
				}
			}
		}
		return velocity;
	}

	void CalculateVelocity(){
		float targetVelocityX = directionalInput.x * moveSpeed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);

			velocity.y += gravity * Time.deltaTime;
	}

	public void SetClimbingLadder(bool climbing){
		controller.collisions.climbingLadder = climbing;
	}

	void Flip(){
		sprite.flipX = (Mathf.Sign (velocity.x) * 1 == 1) ? false : true;
	}
}
