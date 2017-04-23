using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the level generation
/// </summary>
public class LevelGenerator : MonoBehaviour {

	public GameObject wallTile;
	public GameObject borderWall;
	public GameObject playerObject;
	public GameObject hazardObject;
	public GameObject pickUpObject;
	public GameObject keyItem;
	public GameObject exitObject;
	public GameObject backgroundPlant;

	//Dimensions of the level in tiles (dimensions of one tile are the same as the dimensions of one borderTile)
	private int levelHeight = 30;
	private int levelWidth = 25;

	//2-dimensional array representing the level
	private TileType[,] levelGrid;  //[columns, rows] , [width, height]
	private float tileHeight;
	private float tileWidth;

	private float levelActualWidth;
	private float levelActualHeight;


	/// <summary>
	/// Enum for the different things that one tile in the level, that is one cell in levelGrid-array, can contain
	/// "Reserved" can be used for reserving a tile for some pattern, so that other patterns can no longer alter these tiles.
	/// Probabilistic tiles are either a hazard (20%), pickup (20%), wall tile (40%), or empty tile (20%) randomly
	/// </summary>
	private enum TileType
	{
		Empty, ReservedEmpty,
		Spawn, Exit,
		Wall, ReservedWall, 
		UnbreakableWall, ReservedUnbreakable,
		PickUp, ReservedPickUp,
		KeyItem, ReservedKeyItem,
		Hazard, ReservedHazard,
		Probabilistic, ReservedProbabilistic,
		Plant, ReservedPlant,
		TestValue //Test value used to check wether a cell has been "reserved"
	};
		

	// Use this for initialization
	void Start () 
	{
		tileWidth = borderWall.GetComponent<BoxCollider2D> ().size.x;
		tileHeight = borderWall.GetComponent<BoxCollider2D> ().size.y;
		GenerateLevel (levelWidth, levelHeight);
	}

	public float GetActualWidth()
	{
		return levelActualWidth;
	}

	public float GetActualHeight()
	{
		return levelActualHeight;
	}

	public int GetLevelWidth()
	{
		return levelWidth;
	}

	public int GetLevelHeight()
	{
		return levelHeight;
	}
		

	/// <summary>
	/// Destroys all the child objects, clearing the level
	/// </summary>
	private void ClearLevel()
	{
		foreach (Transform block in transform) 
		{
			Destroy (block.gameObject);	
		}
	}

	/// <summary>
	/// Prints the level grid to the debug log, with the positions and types of each cell
	/// </summary>
	private void PrintLevelGrid()
	{
		for (int i = 0; i < levelGrid.GetLength (0); i++) 
		{
			for (int j = 0; j < levelGrid.GetLength (1); j++) 
			{				
				Debug.Log("At " + i + "-" + j + ": " + levelGrid [i, j]);	
			}
		}
	}
		

	/// <summary>
	/// Creates the outer walls and sets all the other tiles to be empty.
	/// </summary>
	private void CreateBaseLevel()
	{
		//Checking column by column
		for (int i = 0; i < levelGrid.GetLength (0); i++) 
		{
			//Rows
			for (int j = 0; j < levelGrid.GetLength (1); j++) 
			{
				//If first or last column, every tile is a wall
				if (i == 0 || i == levelGrid.GetLength (0) - 1)
				{
					levelGrid [i, j] = TileType.ReservedUnbreakable;
				}
				//If first or last row, every tile is a wall
				else if (j == 0 || j == levelGrid.GetLength (1) - 1)
				{
					levelGrid [i, j] = TileType.ReservedUnbreakable;
				}
				//Else every tile is empty
				else 
				{
					levelGrid [i, j] = TileType.Empty;
				}
			}
		}
	}

	/// <summary>
	/// Instantiates the level based on the levelGrid, and names the objects created based on the position.
	/// Also sets the created objects as the children of the level.
	/// </summary>
	private void InstantiateLevel()
	{
		GameObject wall;
		GameObject exit;
		GameObject border;
		GameObject hazard;
		GameObject pickUp;
		GameObject key;
		GameObject plant;

		Vector2 tilePosition;
		Vector2 groundObjectPos;

		for (int i = 0; i < levelGrid.GetLength (0); i++) 
		{
			for (int j = 0; j < levelGrid.GetLength (1); j++) 
			{
				tilePosition = new Vector2 (i * tileWidth, j * tileHeight);
				groundObjectPos = new Vector2 (i * tileWidth, j * tileHeight - 0.1f);

				switch (levelGrid[i,j]) 
				{
				case TileType.UnbreakableWall:
				case TileType.ReservedUnbreakable:
					border = Instantiate (borderWall, tilePosition, borderWall.transform.rotation);
					border.name = "Unbreakable block " + i + "-" + j;
					border.transform.SetParent (gameObject.transform);
					break;
				case TileType.Wall:
				case TileType.ReservedWall:
					wall = Instantiate (wallTile, tilePosition, wallTile.transform.rotation);
					wall.name = "Wall " + i + "-" + j;
					wall.transform.SetParent (gameObject.transform);
					break;
				case TileType.Spawn:
					playerObject.transform.position = tilePosition;
					playerObject.SetActive (true);
					playerObject.GetComponent<PlayerController> ().setStartingHeight (playerObject.transform.position.y);
					break;
				case TileType.Exit:
					exit = Instantiate (exitObject, tilePosition, exitObject.transform.rotation);
					exit.name = "The exit";
					exit.transform.SetParent (gameObject.transform);
					break;
				case TileType.Hazard:
				case TileType.ReservedHazard:
					hazard = Instantiate (hazardObject, tilePosition, hazardObject.transform.rotation);
					hazard.name = "Hazard " + i + "-" + j;
					hazard.transform.SetParent (gameObject.transform);
					break;
				case TileType.PickUp:
				case TileType.ReservedPickUp:
					pickUp = Instantiate (pickUpObject, tilePosition, pickUpObject.transform.rotation);
					pickUp.name = "Pick up " + i + "-" + j;
					pickUp.transform.SetParent (gameObject.transform);
					break;
				case TileType.Plant:
				case TileType.ReservedPlant:
					plant = Instantiate (backgroundPlant, groundObjectPos, backgroundPlant.transform.rotation);
					plant.name = "BG plant " + i + "-" + j;
					plant.transform.SetParent (gameObject.transform);
					break;
				case TileType.KeyItem:
				case TileType.ReservedKeyItem:
					key = Instantiate (keyItem, groundObjectPos, keyItem.transform.rotation);
					key.name = "Key item " + i + "-" + j;
					key.transform.SetParent (gameObject.transform);
					break;

				//Probabilistic tiles are either a hazard (20%), pickup (20%), wall tile (40%), or empty tile (20%) randomly
				case TileType.Probabilistic:
				case TileType.ReservedProbabilistic:
					float randomValue = Random.value;
					if (randomValue < 0.20f)
					{
						goto case TileType.Hazard;
					}
					if (randomValue >= 0.20f && randomValue < 0.40f)
					{
						goto case TileType.PickUp;
					}
					if (randomValue >= 0.40f && randomValue < 0.80f)
					{
						goto case TileType.Wall;
					}
					break;
				default:
					break;
				}
			}
		}
	}

	/// <summary>
	/// Checks if spesified cell is taken by reserved tiletype
	/// </summary>
	/// <returns><c>true</c>if the tile is reserved<c>false</c> otherwise.</returns>
	/// <param name="column">Column of the level array</param>
	/// <param name="row">Row of the level array</param>
	private bool IsReserved(int column, int row)
	{
		return !PlaceTileType (column, row, TileType.TestValue);
	}

	/// <summary>
	/// Tries to place a certain type of tile to the levelGrid, but fails if the spot has been reserved
	/// </summary>
	/// <returns><c>true</c>, if tile type was placed, <c>false</c> if the spot was reserved and no new kind of tile was placed</returns>
	/// <param name="column">Column of the levelGrid</param>
	/// <param name="row">Row of the levelGrid</param>
	/// <param name="type">Type of the tile to be placed</param>
	private bool PlaceTileType(int column, int row, TileType type)
	{
		if (column >= levelWidth || column < 0 || row >= levelHeight || row < 0)
		{
			Debug.Log ("<color=#ff0000ff>Tried to place a tile outside of the levelGrid</color>");
			return false;
		}
		
		//If attempting to replace reserved or border or spawn tile with other tiles, do nothing and return false
		if (levelGrid[column, row] == TileType.ReservedUnbreakable || levelGrid[column, row] == TileType.ReservedWall
			|| levelGrid[column, row] == TileType.ReservedProbabilistic || levelGrid[column, row] == TileType.ReservedPickUp
			|| levelGrid[column, row] == TileType.ReservedEmpty || levelGrid[column, row] == TileType.ReservedPlant
			|| levelGrid[column, row] == TileType.Spawn ||  levelGrid[column, row] == TileType.Exit)
		{
			//Debug.Log ("Failed at " + column + ":" + row + " with type " + type);
			return false;
		}

		//By using the test value, the level array won't be altered
		if (type == TileType.TestValue)
		{
			return true;
		}

		levelGrid [column, row] = type;
		return true;
	}

	/*____________________________________________________________________________

					ONLY NEED TO EDIT THE SCRIPT BELOW THIS PART
	______________________________________________________________________________*/


	/// <summary>
	/// Generates the level
	/// </summary>
	/// <param name="width">Width of the level in tiles</param>
	/// <param name="height">Height of the level in tiles</param>
	public void GenerateLevel(int width, int height)
	{
		levelWidth = width;
		levelHeight = height;
		levelActualWidth = tileWidth * levelWidth;
		levelActualHeight = tileHeight * levelHeight;

		ClearLevel (); //Clear the level before creating a new one

		levelGrid = new TileType[width, height];


		CreateBaseLevel (); //Level borders
		CreatePatterns();

		//MakeFeasible ();
		InstantiateLevel ();
	}

	/// <summary>
	/// Makes the level passable.
	/// </summary>
	private void MakeFeasible()
	{
		//
	}

	/// <summary>
	/// Calls all the methods that create patterns using somewhat random values.
	/// </summary>
	private void CreatePatterns ()
	{
		//Note: Random.Range (1, levelWidth - 1) = Random column, excludes border walls
		//		Random.Range(1, levelHeight - 1) = Random row

		//Sets the spawn location randomly to the top side of the level.
		CreatePlayerSpawnPattern (Random.Range (1, levelWidth - 1), Random.Range (levelHeight - 3, levelHeight - 6));

		//Sets the exit to the bottom of the level
		CreateExitPattern (Random.Range (1, levelWidth - 1), 1);

		AddRandomTiles (35); //Comment this out if you want to, or make it scale with the level size, or do whatever...

		//Here you can add your own patterns, or modify the above patterns
		//createPlatformPattern(x1, y1);
		//createKeyLocationPattern (x2, y2);
	}

	/// <summary>
	/// Adds probablistic tiles to random locations
	/// </summary>
	/// <param name="amountOfTiles">Amount of tiles to be added</param>
	private void AddRandomTiles(int amountOfTiles)
	{
		for (int i = 0; i < amountOfTiles; i++) 
		{
			PlaceTileType (Random.Range (1, levelWidth - 1), Random.Range (1, levelHeight - 1), TileType.Probabilistic);
		}
	}



	/// <summary>
	/// Sets up a spawn location for the player in the levelGrid, and also for small spawn structure.
	/// If false, nothing is created
	/// </summary>
	/// <returns><c>true</c>, if player spawn pattern was created, <c>false</c> if the creation of the pattern failed.</returns>
	/// <param name="column">Spawn location column in the levelGrid</param>
	/// <param name="row">Spawn location row in the levelGrid</param>
	private bool CreatePlayerSpawnPattern(int column, int row)
	{
		if (column <= 0 || column >= levelWidth - 1)
		{
			Debug.Log ("<color=#ff0000ff>Spawn position cannot be on the first or last column of the level. Column: " + column + "</color>");
			return false;
		}

		if (row <= 0 || row >= levelHeight - 2)
		{
			Debug.Log ("<color=#ff0000ff>Spawn position cannot be at the very top or bottom. Row: " + row + "</color>");
			return false;
		}

		//If the supposed player position or the tile beneath the spawn position is reserved, return false
		if (IsReserved (column, row) || IsReserved (column, row - 1))
		{
			Debug.Log ("<color=#ff0000ff>Creating player spawn failed, tried to create on top of reserved tiles</color>");
			return false;
		}


		//Player spawn tile
		PlaceTileType (column, row, TileType.Spawn);

		//Creates ground blocks just below the player and to the sides
		PlaceTileType (column, row - 1, TileType.ReservedWall);
		PlaceTileType (column - 1, row - 1, TileType.ReservedWall);
		PlaceTileType (column + 1, row - 1, TileType.ReservedWall);
		PlaceTileType (column - 1, row, TileType.ReservedWall);
		PlaceTileType (column + 1, row, TileType.ReservedWall);

		//Makes sure that the player can jump out of the spawnstructure
		PlaceTileType (column - 1, row + 1, TileType.ReservedEmpty);
		PlaceTileType (column, row + 1, TileType.ReservedEmpty);
		PlaceTileType (column + 1, row + 1, TileType.ReservedEmpty);

		return true;
	}

	private bool CreateExitPattern(int column, int row)
	{
		PlaceTileType (column, row, TileType.Exit);
		PlaceTileType (column - 1, row, TileType.Plant); //Doesn't matter whether or not these plants exists so they are not reserved
		PlaceTileType (column + 1, row, TileType.Plant);
		return true;
	}
}
