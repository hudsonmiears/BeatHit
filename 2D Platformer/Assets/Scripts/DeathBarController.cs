using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBarController : RaycastController {

	public LayerMask passengerMask;

	Vector3 velocity;
	Vector3 prevPos;
	public GameObject player;
	bool activeDeath = true;

	void Start(){
		base.Start ();
		StartCoroutine (CalcVelocity());
	}

	void Update(){
		UpdateRaycastOrigins ();
	}


	IEnumerator CalcVelocity()
	{
		while( Application.isPlaying )
		{
			// Position at frame start
			prevPos = transform.position;
			// Wait till it the end of the frame
			yield return new WaitForEndOfFrame();
			// Calculate velocity: Velocity = DeltaPosition / DeltaTime
			velocity = (prevPos - transform.position) / Time.deltaTime;
//			Debug.Log( currVel );
		}
	}
}
