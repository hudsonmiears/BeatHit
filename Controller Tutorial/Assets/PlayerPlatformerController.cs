using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformerController : PhysicsObject {

	public float jumpTakeOffSpeed = 7;

	public float maxSpeed = 7;

	private SpriteRenderer spriteRenderer;
	private Animator animator;

	// Use this for initialization
	void Awake () {
		spriteRenderer = GetComponent<SpriteRenderer> ();
		animator = GetComponent<Animator> ();
	}

	void LateUpdate(){
		Debug.Log (climbingLadder);
	}

	protected override void ComputeVelocity(){
		animator.SetBool ("isOnLadder", false);

		Vector2 move = Vector2.zero;

		move.x = Input.GetAxis ("Horizontal");

		HandleLadders (move);

		if (Input.GetButtonDown ("Jump") && grounded) {
			if (climbingLadder) {
				climbingLadder = false;
				move.x = Input.GetAxis ("Horizontal");
				animator.SetBool ("isOnLadder", false);
			}
			velocity.y = jumpTakeOffSpeed;
		} else if (Input.GetButtonUp ("Jump")) {
			if (velocity.y > 0) {
				// Reduce y velocity by half if player lets go of jump.
				velocity.y = velocity.y * .5f;
			}
		}
			
		bool flipSprite = (spriteRenderer.flipX ? (move.x > 0.01f) : (move.x < 0.01f));
		if (flipSprite){
			spriteRenderer.flipX = !spriteRenderer.flipX;
		}

		animator.SetBool ("grounded", grounded);
		animator.SetFloat("velocityX",Mathf.Abs(velocity.x) / maxSpeed);

		targetVelocity = move * maxSpeed;
	}

	public void HandleLadders(Vector2 move){
		if (climbingLadder) {
			move.y = Input.GetAxis ("Vertical");
			animator.SetBool ("isOnLadder", true);
			if (!grounded) {
				move.x = 0;
			}
		}
	}

	public void ClimbingLadder(bool climbing){
		bool climb;
		if (climbing) {
			if (Input.GetButtonDown ("Vertical")) {
				climb = true;
				if (Input.GetButtonUp ("Vertical") && !grounded) {
					climb = true;
				}
			} else {
				climb = false;
			}
		} else {
			climb = false;
		}
		climbingLadder = climb;
	}
		
}
