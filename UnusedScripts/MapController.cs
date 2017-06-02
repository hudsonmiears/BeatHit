using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapController : MonoBehaviour {

	[Serializable]
	public class Count{
		public int minimum;
		public int maximum;

		public Count (int min, int max){
			minimum = min;
			maximum = max;
		}
	}

	WalkVars walkVars;

	public int columns = 8;
	public int rows = 8;
	public Count cellCount = new Count (8, 16);
//	public GameObject exit;
	public Transform gridCellTransform;
	public GameObject[] gridCells;


	public int fillCount = 8;
//	List<Vector3> used;

	private Transform gridHolder;
	private List<Vector3> gridPositions = new List<Vector3>();

	void InitializeList(){
		gridPositions.Clear();

		for (int x = 1; x < columns; x++) {
			for (int y = 1; y < rows; y++) {
				gridPositions.Add (new Vector3 (x, y, 0f));
			}
		}
	}

	void GridSetup(){
		gridHolder = new GameObject ("Grid").transform;

		for (int x = 1; x < columns; x++) {
			for (int y = 1; y < rows; y++) {
				GameObject toInstantiate = gridCells [Random.Range (0, gridCells.Length)];

				GameObject instance = Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;

				instance.transform.SetParent (gridHolder);
			}
		}
	}

//	void SetFlagFalse(Vector3 flagPlace){
//		if (){}
//	}

	void RandomWalk(){
		InitializeList ();
//		walkVars.Reset ();
//		Vector3 startingPos = new Vector3 (0, 0, 0);
//		Vector3 currentPos = startingPos;
//		Vector3 nextPos = new Vector3 (0,0,0);
//		Vector3 startingPos = walkVars.;
		List<Vector3> filledPos = walkVars.filledPos;
		filledPos.Add(new Vector3(0,0,0));
		Debug.Log (filledPos);

		Vector3 currentPos = walkVars.currentPos;
		Vector3 nextPos = walkVars.nextPos;

		if (filledPos == null) {
			walkVars.Reset ();
		}

		int randomDir = Random.Range (0, 4);


		if (randomDir == 0) {
			walkVars.oldPos = currentPos;
			nextPos = new Vector3 (walkVars.oldPos.x, walkVars.oldPos.y - 5);
			currentPos = nextPos;
			walkVars.currentPos = nextPos;
			if (!filledPos.Contains(nextPos)){
				filledPos.Add (walkVars.currentPos);
				GameObject gridCellNew = GameObject.CreatePrimitive (PrimitiveType.Quad);
//				gridCellNew.AddComponent<TextMesh> ();
				gridCellNew.transform.position = nextPos;
			}
		}
		if (randomDir == 1) {
			walkVars.oldPos = currentPos;
			nextPos = new Vector3 (walkVars.oldPos.x - 5, walkVars.oldPos.y);
			currentPos = nextPos;
			currentPos = nextPos;
			if (!filledPos.Contains(nextPos)){
				filledPos.Add (walkVars.currentPos);
				GameObject gridCellNew = GameObject.CreatePrimitive (PrimitiveType.Quad);
//				gridCellNew.AddComponent<TextMesh> ();
				gridCellNew.transform.position = nextPos;
			}
		}
		if (randomDir == 2) {
			walkVars.oldPos = currentPos;
			nextPos = new Vector3 (walkVars.oldPos.x, walkVars.oldPos.y + 5);
			currentPos = nextPos;
			walkVars.currentPos = nextPos;
			if (!filledPos.Contains(nextPos)){
				filledPos.Add (walkVars.currentPos);
				GameObject gridCellNew = GameObject.CreatePrimitive (PrimitiveType.Quad);
//				gridCellNew.AddComponent<TextMesh> ();
				gridCellNew.transform.position = nextPos;
			}
		}
		if (randomDir == 3) {
			walkVars.oldPos = currentPos;
			nextPos = new Vector3 (walkVars.oldPos.x + 5, walkVars.oldPos.y);
			currentPos = nextPos;
			walkVars.currentPos = nextPos;
			if (!filledPos.Contains(nextPos)){
				filledPos.Add (walkVars.currentPos);
				GameObject gridCellNew = GameObject.CreatePrimitive (PrimitiveType.Quad);
//				gridCellNew.AddComponent<TextMesh> ();
				gridCellNew.transform.position = nextPos;
			}
		}
	}
//	void InstantiateQuads(){
//		for (int x = 1; x < columns; x++) {
//			for (int y = 1; y < rows; y++) {
//				if (
//			}
//		}
//	}

	void OnDrawGizmos(){
//		for (i = 1; i < walkVars.filledPos.Count; i++){
		Gizmos.color = new Color (1, 0, 0, 0.5f);
		Gizmos.DrawCube(walkVars.currentPos,gridCellTransform.transform.localScale);

	}


	void Start () {
		GameObject gridCell = GameObject.CreatePrimitive (PrimitiveType.Quad);
		gridCell.AddComponent<TextMesh> ();
		gridCell.transform.position = new Vector3 (0, 0, 0);
		for (int i = 1; i < fillCount * 2; i++) {
			RandomWalk ();
			if (!walkVars.filledPos.Contains (walkVars.currentPos)) {
				walkVars.filledPos.Add (walkVars.currentPos);
				Instantiate (gridCell);
				gridCell.transform.position = walkVars.currentPos;
			}
		}
		Debug.Log ("filledPos: ");
		for (int i = 1; i < walkVars.filledPos.Count; i++) {
			Debug.Log (walkVars.filledPos[i]);
		}
	}


	public struct WalkVars{
		public List<Vector3> filledPos;
		public Vector3 currentPos;
		public Vector3 nextPos;
		public Vector3 oldPos;

		public void Reset(){
			filledPos = new List<Vector3>();
			currentPos = Vector3.zero;
			nextPos = Vector3.zero;
			oldPos = Vector3.zero;
		}
	}
}
