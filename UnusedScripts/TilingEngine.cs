﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean;

public class TilingEngine : MonoBehaviour {

	public List<TileSprite> TileSprites;
	public Vector2 MapSize;
	public Sprite DefaultImage;
	public GameObject TileContainerPrefab;
	public GameObject TilePrefab;
	public Vector2 CurrentPosition;
	public Vector2 ViewPortSize;

	private GameObject controller;
	private GameObject _tileContainer;
	private TileSprite[,] _map;
	private List<GameObject> _tiles = new List<GameObject>();

	private TileSprite FindTile(Tiles tile){
		foreach (TileSprite tileSprite in TileSprites) {
			if (tileSprite.TileType == tile)
				return tileSprite;
		}
		return null;
	}
		
	private void DefaultTiles(){
		//Fill the map with tiles which are unset;
		for (var y = 0; y < MapSize.y - 1; y++) {
			for (var x = 0; x < MapSize.x - 1; x++){
				_map [x, y] = new TileSprite ("unset", DefaultImage, Tiles.Unset);
			}
		} 
	}

	private void SetTiles(){
		//Set the tiles;
		var index = 0;
		for (var y = 0; y - MapSize.y; y++){
			for (var x = 0; x - MapSize.x; x++) {
				_map [x, y] = new TileSprite (TileSprites [index].Name, TileSprites [index].TileImage, TileSprites [index].TileType);
				index++;
				if (index > TileSprites.Count - 1)
					index = 0;
			}
		}
	}

	private void AddTilesToWorld(){
		foreach (GameObject o in _tiles) {
			LeanPool.Despawn (o);
		}
		_tiles.Clear();
		LeanPool.Despawn (_tileContainer);
		_tileContainer = LeanPool.Spawn (TileContainerPrefab);
		var tileSize = .64f;
		var viewOffsetX = ViewPortSize.x / 2f;
		var viewOffsetY = ViewPortSize.y / 2f;

		for (var y = -viewOffsetY; y < viewOffsetY; y++) {
			for (var x = -viewOffsetX; x < viewOffsetX; x++) {
				var tX = x * tileSize;
				var tY = y * tileSize;

				var iX = x + CurrentPosition.x;
				var iY = y + CurrentPosition.y;

				if (iX < 0)
					continue;
				if (iY < 0)
					continue;

				if (iX > MapSize.x - 2)
					continue;
				if (iY > MapSize.y - 2)
					continue;

				var t = LeanPool.Spawn (TilePrefab);
				t.transform.position = new Vector3 (tX, tY, 0);
				t.transform.SetParent (_tileContainer.transform);
				var renderer = t.GetComponent<SpriteRenderer> ();
				renderer.sprite = _map [(int)x + (int)CurrentPosition.x, (int)y + (int)CurrentPosition.y].TileImage;
				_tiles.Add (t);
			}
		}
	}

	public void Start(){
		controller = GameObject.Find ("Controller");
		_map = new TileSprite[(int)MapSize.x, (int)MapSize.y];

		DefaultTiles ();
		SetTiles ();
	}

	private void Update(){
		AddTilesToWorld ();
	}
}
