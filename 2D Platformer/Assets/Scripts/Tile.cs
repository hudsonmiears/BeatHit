using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Tile {

	public int _direction; // 0 for up, 1 for right, 2 for down, 3 for left.
	public int _exitNumber; // number of exits
	public List<string> _exitPlacement = new List<string>(); // string list for exit placement: parameters left, right, middle
	public int _cornerType; // 0 for connection up>right; 1 for connection up>left; 2 for connection right>up;
	// 3 for connection right>down; 4 for connection left>up; 5 for connection left>down; 6 for connection down>right;
	// 7 for connection down>left; 8 for not a corner

	public bool _unassigned;
	public string _levelName;

	public GameObject _tileObject; // Make Empty GameObject
	public Vector3 _transformPos; // For setting the position of the tile.


	public Tile(){
		_direction = 0;
		_exitNumber = 0;
		_exitPlacement.Add ("left");
		_exitPlacement.Add ("right");
		_exitPlacement.Add ("middle");
		_cornerType = 8;
		_tileObject = new GameObject();
		_transformPos = new Vector3 (0, 0, 0);
		_tileObject.transform.position = _transformPos;
		_unassigned = true;
		_levelName = GetScene (_direction,_cornerType);
	}

	public Tile(int direction, int exitNumber, List<string> exitPlacement, int horizOrVert, Vector3 transformPos, int tileTypeSize = 0, int cornerType = 8, bool unassigned = false){
		_direction = direction;
		_exitNumber = exitNumber;
		_exitPlacement = exitPlacement;
		_cornerType = cornerType;
		_tileObject = new GameObject();
		_transformPos = transformPos;
		_tileObject.transform.position = _transformPos;
		_unassigned = unassigned;
		_levelName = GetScene (_direction, _cornerType);
	}

	public string GetScene(int direction, int direction2){
		int direction3;
		if (direction == direction2) {
			direction3 = direction;
		} else {
			direction3 = direction2;
		}
		string levelName = (
		                       direction + "_" +
			direction2 + "_" + Random.Range(0,2));
		return levelName;
	}
}
