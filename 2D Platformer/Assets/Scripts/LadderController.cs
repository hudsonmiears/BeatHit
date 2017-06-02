using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderController : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "Player") {
			other.gameObject.GetComponent<Player>().SendMessage ("SetClimbingLadder", true);
			Debug.Log ("enter");
		}
	}

	void OnTriggerStay2D(Collider2D other){
		if (other.tag == "Player") {
			other.gameObject.GetComponent<Player>().SendMessage ("SetClimbingLadder", true);
			Debug.Log ("stay");
		}
	}

	void OnTriggerExit2D(Collider2D other){
		if (other.tag == "Player") {
			other.gameObject.GetComponent<Player>().SendMessage ("SetClimbingLadder", false);
			Debug.Log ("exit");
		}
	}
}
