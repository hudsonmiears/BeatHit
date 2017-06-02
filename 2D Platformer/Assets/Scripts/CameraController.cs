using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	Vector3 targetPos;
	public GameObject levelController;
	WaypointController waypointController;

	float speed;

	Vector3 velocity;

	Vector3[] localWaypoints;
	public Vector3[] globalWaypoints;
	int fromWaypointIndex;
	float percentBetweenWaypoints;
	public GameObject topBar;

	int progress;

	void Start () {
		waypointController = levelController.GetComponent<WaypointController> ();
		targetPos = transform.position;
		speed = topBar.GetComponent<TopBarController> ().speed;
		globalWaypoints = waypointController.CreateWaypoints ();
		velocity = (progress == 100) ? Vector3.zero : CalculateMovement ();
	}

	void LateUpdate(){
		velocity = (progress == 100) ? Vector3.zero : CalculateMovement ();
//		if (velocity.x != 0) {
//			if (velocity.x > 0) {
//				velocity.y -= 10;
//			}else if (velocity.x < 0) {
//				velocity.y += 10;
//			}
//		}else if (velocity.y != 0) {
//			if (velocity.y > 0) {
//				velocity.x -= 10;
//			}else if (velocity.y < 0) {
//				velocity.x += 10;
//			}
//		}
//		velocity.z -= 10;
//		Debug.Log (velocity);
//		transform.Translate (velocity,Space.World);
//		transform.Translate (new Vector3 (-10, 0, 0), Space.Self);
		transform.rotation = Quaternion.identity;
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
}
