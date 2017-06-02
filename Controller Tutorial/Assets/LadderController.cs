using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderController : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "Player") {
			if (other.bounds.max.y < this.GetComponent<BoxCollider2D> ().bounds.max.y) {
				other.gameObject.SendMessage ("ClimbingLadder", true);
			}
		}
	}

	void OnTriggerStay2D(Collider2D other){
		if (other.tag == "Player") {
			if (other.bounds.max.y < this.GetComponent<BoxCollider2D> ().bounds.max.y) {
				other.gameObject.SendMessage ("ClimbingLadder", true);
			}
		}
	}

	void OnTriggerExit2D(Collider2D other){
		if (other.tag == "Player") {
			other.gameObject.SendMessage ("ClimbingLadder", false);
		}
	}
}
