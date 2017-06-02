using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NewBuilder : MonoBehaviour {

	int[,] grid;
	public int rows = 10;
	public int columns = 10;

	public int pathSize = 10;

	public int distSize = 20;

	public List<Vector2> usedList = new List<Vector2>();

	public GameObject topBar;

	public List<string> pathList = new List<string>();

	private void Initialize(){
		
		for (var x = 0; x < rows; x++) {
			for (var y = 0; y < columns; y++) {
				grid [x, y] = 0;
			}
		}
	}
		

	private void RandomSet(){
		grid = new int[rows, columns];
		Vector2 startingPos = new Vector2 (rows/2, columns/2);
		Vector2 newPos;
		Vector2 newNewPos;

		newPos = RandomWalk (startingPos);
		usedList.Add (startingPos);
		usedList.Add (newPos);

		Debug.DrawLine (new Vector3 (startingPos.x * distSize, startingPos.y * distSize), new Vector3 (newPos.x * distSize, newPos.y * distSize), Color.red, 20, true);

		for (int i = 0; i < pathSize - 2; i++) {
			newNewPos = RandomWalk (newPos);
			if (!usedList.Contains (newNewPos) && !(newNewPos.x < 0) && !(newNewPos.x > columns) && !(newNewPos.y < 0) && !(newNewPos.y > rows)) {
				usedList.Add (newNewPos);
				Debug.DrawLine (new Vector3 (newPos.x * distSize, newPos.y * distSize), new Vector3 (newNewPos.x * distSize, newNewPos.y * distSize), Color.red, 20, true);
				newPos = newNewPos;
			} else {
				i -= 1;
				continue;
			}
		}

		for (var x = 0; x < rows; x++) {
			for (var y = 0; y < columns; y++) {
				if (usedList.Contains (new Vector2 (x, y))) {
					grid [x, y] = 1;
				} else {
					grid [x, y] = 0;
				}
			}
		}
	}

	private void Visualize(){
		for (var x = 0; x < rows; x++) {
			for (var y = 0; y < columns; y++){
				GameObject vis = GameObject.CreatePrimitive (PrimitiveType.Quad);
				vis.transform.position = new Vector2 (x * distSize, y * distSize);
			}
		}
	}

	private void InstantiateParts(){
		foreach (Vector2 targetVect in usedList) {
			int indexnow = usedList.IndexOf (targetVect);
			int direction;
			int direction2;
			if (indexnow != usedList.Count - 1 && indexnow != 0) {
				Vector2 lastVect = usedList [indexnow - 1];
				Vector2 nextVect = usedList [indexnow + 1];

				direction = CalculateDirection (targetVect,nextVect);
				direction2 = CalculateDirection (lastVect,targetVect);

			} else if (indexnow == usedList.Count - 1) {
				Vector2 lastVect = usedList [indexnow - 1];

				direction2 = CalculateDirection (lastVect,targetVect);
				direction = direction2;
			} else if (indexnow == 0) {
				Vector2 nextVect = usedList [indexnow + 1];
				direction = CalculateDirection (targetVect,nextVect);
				direction2 = direction;
			} else {
				direction = 0;
				direction2 = 0;
			}

			Tile newTile = new Tile ();
			newTile._direction = direction;

			string tileName = newTile.GetScene (direction,direction2);
//			Debug.Log (tileName);

			GameObject newPart = (GameObject)Instantiate (Resources.Load ("LevelParts/" + tileName), new Vector3(targetVect.x*distSize,targetVect.y*distSize),Quaternion.identity);
			pathList.Add (tileName);
		}
	}
		

	private Vector2 RandomWalk(Vector2 inputVect){
		int randomDir = Random.Range (0, 4);
		Vector2 returnVal;
		//up
		if (randomDir == 0) {
			returnVal = new Vector2 (inputVect.x, inputVect.y+1);
			return returnVal;
		}
		//right
		if (randomDir == 1) {
			returnVal = new Vector2 (inputVect.x+1, inputVect.y);
			return returnVal;
		}
		//down
		if (randomDir == 2) {
			returnVal = new Vector2 (inputVect.x, inputVect.y-1);
			return returnVal;

		}
		//left
		if (randomDir == 3) {
			returnVal = new Vector2 (inputVect.x - 1, inputVect.y);
			return returnVal;
		} else {
			returnVal = Vector2.zero;
			return returnVal;
		}
	}

	private int CalculateDirection (Vector2 inputOne, Vector2 inputTwo){
		int direction;
		if (inputOne.y < inputTwo.y) {
			direction = 0;
			return direction;
		} else if (inputOne.x < inputTwo.x) {
			direction = 1;
			return direction;
		} else if (inputOne.y > inputTwo.y) {
			direction = 2;
			return direction;
		} else if (inputOne.x > inputTwo.x) {
			direction = 3;
			return direction;
		} else {
			direction = 0;
			return direction;
		}
	}


	void Awake(){
		RandomSet ();
		InstantiateParts();
	}

	public Vector2[] ReturnVects(){
		return usedList.ToArray();
	}

	public string[] ReturnStrings(){
		return pathList.ToArray();
	}




}
