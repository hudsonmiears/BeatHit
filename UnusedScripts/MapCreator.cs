using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCreator : MonoBehaviour {

	public int gridCountX = 10;
	public int gridCountY = 10;

	public float tileSizeX = 5;
	public float tileSizeY = 5;

	private float gridSizeX;
	private float gridSizeY;

	public int maxTileCount = 16;

	public Vector3 startPoint;
	Vector3 movementVector;

	int currentTileCount = 0;
//	List<Vector3> tiles;

	public GameObject tile;
	Dictionary<Vector3,bool> tilesUsed;

	void Start(){
		generateTileGrid ();
		for (int i = 0; i < maxTileCount; i++) {
			generateMap ();
		}
		Debug.Log (tilesUsed);
	}

	void generateTileGrid (){
		gridSizeX = gridCountX * tileSizeX;
		gridSizeY = gridCountY * tileSizeY;
	
		startPoint = new Vector3 (gridSizeX / 2, gridSizeY /2);
		currentTileCount += 1;

		tilesUsed = new Dictionary<Vector3, bool> ();
		for (int i = 0; i < gridCountX; i++){
			for (int j = 0; j < gridSizeY; j++){
				tilesUsed.Add (new Vector3(i*tileSizeX,j*tileSizeY), false);
			}
		}
	}

//	void indexGrid(){
//		
//	}

//	bool isValidTile(Vector3 questionPosition){
//		if (quest
//	}

	void generateMap(){
		
		Vector3 currentPosition = startPoint;

		int directionInt = Random.Range (0, 4);

		//Down
		if (directionInt == 0){
			movementVector = new Vector3 (currentPosition.x, currentPosition.y - tileSizeY);
		}
		//Left
		if (directionInt == 1){
			movementVector = new Vector3 (currentPosition.x-tileSizeX, currentPosition.y);

		}
		//Up
		if (directionInt == 2){
			movementVector = new Vector3 (currentPosition.x, currentPosition.y + tileSizeY);

		}
		//Right
		if (directionInt == 3){
			movementVector = new Vector3 (currentPosition.x+tileSizeX, currentPosition.y);

		}

		if (!tilesUsed.ContainsKey (movementVector)) {
			GameObject newTile = (GameObject)Instantiate (tile, movementVector, Quaternion.identity);
			tilesUsed.Add (movementVector, true);
		}
		if (tilesUsed[movementVector] != true){
			GameObject newTile = (GameObject)Instantiate (tile, movementVector, Quaternion.identity);
			tilesUsed [movementVector] = true;
		}
	}
}
