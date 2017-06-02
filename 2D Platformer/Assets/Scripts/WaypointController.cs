using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointController : MonoBehaviour {

	NewBuilder newbuild;

	public void Awake(){
		newbuild = this.GetComponent<NewBuilder> ();
	}

	public Vector3[] CreateWaypoints(){
		List<Vector3> realPoints = new List<Vector3>();
		Vector2[] usedList = newbuild.ReturnVects();
		string[] pathList = newbuild.ReturnStrings ();

		int i = 0;
		foreach (Vector2 vec in usedList) {
			if (pathList [System.Array.IndexOf (usedList, vec)][0] == '0') {
				Debug.Log ("Going Up");// GOING UP
				if (pathList [System.Array.IndexOf (usedList, vec)] [2] != pathList [System.Array.IndexOf (usedList, vec)] [0]) {
					if (pathList [System.Array.IndexOf (usedList, vec)] [2] == '1') { // up, right _|
						Debug.Log("Was going right, now going up");
						Vector3 centerVec = new Vector3 (vec.x * newbuild.distSize, (vec.y * newbuild.distSize));
						Vector3 leftVec = new Vector3 ((vec.x * newbuild.distSize) - 10, vec.y * newbuild.distSize);
						Vector3 topVec = new Vector3 (vec.x * newbuild.distSize, vec.y * newbuild.distSize+10);

						realPoints.Add(leftVec);
						realPoints.Add(centerVec);
						realPoints.Add(topVec);
						i += 3;
						continue;
					}
					else { // up, left |_
						Debug.Log("Was going left, now going up");
						Vector3 centerVec = new Vector3 (vec.x * newbuild.distSize, (vec.y * newbuild.distSize));
						Vector3 rightVec = new Vector3 (vec.x * newbuild.distSize + 10, (vec.y * newbuild.distSize));
						Vector3 topVec = new Vector3 (vec.x * newbuild.distSize, (vec.y * newbuild.distSize)+10);

						realPoints.Add(rightVec);
						realPoints.Add(centerVec);
						realPoints.Add(topVec);
						i += 3;
						continue;
					}
				} else { //up,up
					Debug.Log("Was going up, now going up");
					Vector3 topVec = new Vector3 (vec.x * newbuild.distSize, vec.y * newbuild.distSize+10);

					Vector3 bottomVec = new Vector3 (vec.x * newbuild.distSize, (vec.y * newbuild.distSize)-10);

					realPoints.Add(bottomVec);
					realPoints.Add(topVec);

					i += 2;
					continue;
				}
			}

			else if (pathList [System.Array.IndexOf (usedList, vec)][0] == '1') { // GOING RIGHT
				Debug.Log("Going right!");
				if (pathList [System.Array.IndexOf (usedList, vec)] [2] != pathList [System.Array.IndexOf (usedList, vec)] [0]) {
					if (pathList [System.Array.IndexOf (usedList, vec)] [2] == '0') { // right, up |``
						Debug.Log("Was going up, now going right");
						Vector3 centerVec = new Vector3 (vec.x * newbuild.distSize, (vec.y * newbuild.distSize));
						Vector3 rightVec = new Vector3 ((vec.x * newbuild.distSize) + 10, (vec.y * newbuild.distSize));
						Vector3 bottomVec = new Vector3 (vec.x * newbuild.distSize, (vec.y * newbuild.distSize)-10);

						realPoints.Add(bottomVec);
						realPoints.Add(centerVec);
						realPoints.Add(rightVec);
						i += 3;
						continue;
					}
					else { // right, down |_
						Debug.Log("Was going down, now going right");
						Vector3 centerVec = new Vector3 (vec.x * newbuild.distSize, (vec.y * newbuild.distSize));
						Vector3 rightVec = new Vector3 ((vec.x * newbuild.distSize) + 10, (vec.y * newbuild.distSize));
						Vector3 topVec = new Vector3 (vec.x * newbuild.distSize, vec.y * newbuild.distSize+10);

						realPoints.Add(topVec);
						realPoints.Add(centerVec);
						realPoints.Add(rightVec);
						i += 3;
						continue;
					}
				} else { //right right
					Debug.Log("Was going right, now going right");
					Vector3 leftVec = new Vector3 ((vec.x * newbuild.distSize)-10, (vec.y * newbuild.distSize));
					Vector3 rightVec = new Vector3 ((vec.x * newbuild.distSize)+10, (vec.y * newbuild.distSize));

					realPoints.Add(leftVec);
					realPoints.Add(rightVec);

					i += 2;
					continue;
				}
			}

			else if (pathList [System.Array.IndexOf (usedList, vec)][0] == '2') { // GOING DOWN
				Debug.Log("Going down!");
				if (pathList [System.Array.IndexOf (usedList, vec)] [2] != pathList [System.Array.IndexOf (usedList, vec)] [0]) {
					if (pathList [System.Array.IndexOf (usedList, vec)] [2] == '1') { // down,right ``|
						Debug.Log("Was going right, now going down");
						Vector3 centerVec = new Vector3 (vec.x * newbuild.distSize, (vec.y * newbuild.distSize));
						Vector3 leftVec = new Vector3 ((vec.x * newbuild.distSize) - 10, (vec.y * newbuild.distSize));
						Vector3 bottomVec = new Vector3 (vec.x * newbuild.distSize, (vec.y * newbuild.distSize)-10);

						realPoints.Add(leftVec);
						realPoints.Add(centerVec);
						realPoints.Add(bottomVec);
						i += 3;
						continue;
					}
					else { // down, left |``
						Debug.Log("Was going left, now going down");
						Vector3 centerVec = new Vector3 (vec.x * newbuild.distSize, (vec.y * newbuild.distSize));
						Vector3 rightVec = new Vector3 ((vec.x * newbuild.distSize) + 10, (vec.y * newbuild.distSize));
						Vector3 bottomVec = new Vector3 (vec.x * newbuild.distSize, (vec.y * newbuild.distSize)-10);

						realPoints.Add(rightVec);
						realPoints.Add(centerVec);
						realPoints.Add(bottomVec);
						i += 3;
						continue;
					}
				} else { //down, down
					Debug.Log("Was going down, now going down");
					Vector3 topVec = new Vector3 (vec.x * newbuild.distSize, (vec.y * newbuild.distSize)+10);
					Vector3 bottomVec = new Vector3 (vec.x * newbuild.distSize, (vec.y * newbuild.distSize)-10);

					realPoints.Add(topVec);
					realPoints.Add(bottomVec);

					i += 2;
					continue;
				}
			}
			else{ // GOING LEFT
				Debug.Log("Going left!");
				if (pathList [System.Array.IndexOf (usedList, vec)] [2] != pathList [System.Array.IndexOf (usedList, vec)] [0]) {
					if (pathList [System.Array.IndexOf (usedList, vec)] [2] == '0') { // up, left ``|
						Debug.Log("Was going up, now going left");
						Vector3 centerVec = new Vector3 (vec.x * newbuild.distSize, (vec.y * newbuild.distSize));
						Vector3 leftVec = new Vector3 ((vec.x * newbuild.distSize) - 10, (vec.y * newbuild.distSize));
						Vector3 bottomVec = new Vector3 (vec.x * newbuild.distSize, (vec.y * newbuild.distSize)-10);

						realPoints.Add(bottomVec);
						realPoints.Add (centerVec);
						realPoints.Add(leftVec);
						i += 3;
						continue;
					}
					else { // down, left _|
						Debug.Log("Was going down, now going left");
						Vector3 centerVec = new Vector3 (vec.x * newbuild.distSize, (vec.y * newbuild.distSize));
						Vector3 leftVec = new Vector3 ((vec.x * newbuild.distSize) - 10, (vec.y * newbuild.distSize));
						Vector3 topVec = new Vector3 (vec.x * newbuild.distSize, (vec.y * newbuild.distSize)+10);

						realPoints.Add(topVec);
						realPoints.Add(centerVec);
						realPoints.Add(leftVec);
						i += 3;
						continue;
					}
				} else { //left, left
					Debug.Log("Was going left, now going left");
					Vector3 leftVec = new Vector3 ((vec.x * newbuild.distSize)-10, (vec.y * newbuild.distSize));
					Vector3 rightVec = new Vector3 ((vec.x * newbuild.distSize)+10, (vec.y * newbuild.distSize));

					realPoints.Add(rightVec);
					realPoints.Add(leftVec);

					i += 2;
					continue;
				}
			}

		}
		List<Vector3> realPoints2= new List<Vector3>();
		foreach (Vector3 vec in realPoints) {
			if (!realPoints2.Contains(vec)) {
				realPoints2.Add(vec);
			}
		}
		realPoints2.RemoveAt (0);
		return realPoints2.ToArray();
	}
}
