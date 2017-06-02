using System.Collections;
using System;
using UnityEngine;

[Serializable]
public class TileSprite{
	public string Name;
	public Sprite TileImage;
	public Tiles TileType;

	public TileSprite(){
		Name = "Unset";
		TileImage = new Sprite();
		TileType = Tiles.Unset;
	}

	public TileSprite(string name, Sprite image, Tiles tile){
		Name = name;
		TileImage = image;
		TileType = tile;
	}
}