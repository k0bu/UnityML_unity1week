using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMapGenerator : MonoBehaviour {
	
	public Transform tilePrefab;
	public Transform obstaclePrefab;
	public Transform pointSourcePrefab;
	private Vector2 mapSize;
	private int pointSourceNumber;
	
	[Range(0,1)]
	public float outlinePercent;
	//[Range(0,1)]
	private float obstaclePercent;

	List<Coord> allTileCoords;
	Queue<Coord> shuffledTileCoords;
	private Queue<Coord> accessibleTileCoords;

	private int seed;
	Coord mapCentre;
	
	public void StartGenerateMap(int gridSize, float percentObstacles, int numberPoint) {
		// var r = new System.Random();
		// seed = r.Next();
		seed = 0;

		mapSize.x = gridSize;
		mapSize.y = gridSize;
		obstaclePercent = percentObstacles;
		pointSourceNumber = numberPoint;
		
		accessibleTileCoords = new Queue<Coord>();
		GenerateMap ();
		
	}
	
	public void GenerateMap() {

		allTileCoords = new List<Coord> ();
		for (int x = 0; x < mapSize.x; x ++) {
			for (int y = 0; y < mapSize.y; y ++) {
				allTileCoords.Add(new Coord(x,y));
			}
		}
		shuffledTileCoords = new Queue<Coord> (FisherYatesShuffle.ShuffleArray (allTileCoords.ToArray (), seed));
		mapCentre = new Coord ((int)mapSize.x / 2, (int)mapSize.y / 2);

		string holderName = "Generated Map";
		if (transform.Find (holderName)) {
			DestroyImmediate (transform.Find (holderName).gameObject);
		}
		
		Transform mapHolder = new GameObject (holderName).transform;
		mapHolder.parent = transform;
		
		for (int x = 0; x < mapSize.x; x ++) {
			for (int y = 0; y < mapSize.y; y ++) {
				Vector3 tilePosition = CoordToPosition(x,y);
				Transform newTile = Instantiate (tilePrefab, tilePosition, Quaternion.Euler (Vector3.right * 90));
				newTile.localScale = Vector3.one * (1 - outlinePercent);
				newTile.parent = mapHolder;
			}
		}

		bool[,] obstacleMap = new bool[(int)mapSize.x,(int)mapSize.y];

		int obstacleCount = (int)(mapSize.x * mapSize.y * obstaclePercent);
		int currentObstacleCount = 0;

		for (int i =0; i < obstacleCount; i ++) {
			Coord randomCoord = GetRandomCoord();
			obstacleMap[randomCoord.x,randomCoord.y] = true;
			currentObstacleCount ++;

			if (randomCoord != mapCentre && MapIsFullyAccessible(obstacleMap, currentObstacleCount)) {
				Vector3 obstaclePosition = CoordToPosition(randomCoord.x,randomCoord.y);

				Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition 
					+ new Vector3( 0,obstaclePrefab.localScale.x * .5f,0), Quaternion.identity);
				newObstacle.localScale += new Vector3(-0.15f, Random.Range(3,8), -0.15f);
				newObstacle.parent = mapHolder;
			}
			else {
				obstacleMap[randomCoord.x,randomCoord.y] = false;
				currentObstacleCount --;
			}
		}
		
		PointSourceGenerator(mapHolder);
		
	}

	bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount) {
		bool[,] mapFlags = new bool[obstacleMap.GetLength(0),obstacleMap.GetLength(1)];
		Queue<Coord> queue = new Queue<Coord> ();
		queue.Enqueue (mapCentre);
		mapFlags [mapCentre.x, mapCentre.y] = true;

		int accessibleTileCount = 1;

		while (queue.Count > 0) {
			Coord tile = queue.Dequeue();
			accessibleTileCoords.Enqueue(tile);
			
			for (int x = -1; x <= 1; x ++) {
				for (int y = -1; y <= 1; y ++) {
					int neighbourX = tile.x + x;
					int neighbourY = tile.y + y;
					if (x == 0 || y == 0) {
						if (neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) 
						&& neighbourY >= 0 
						&& neighbourY < obstacleMap.GetLength(1)) {
							if (!mapFlags[neighbourX,neighbourY] && !obstacleMap[neighbourX,neighbourY]) {
								mapFlags[neighbourX,neighbourY] = true;
								queue.Enqueue(new Coord(neighbourX,neighbourY));
								accessibleTileCount ++;
							}
						}
					}
				}
			}
		}

		int targetAccessibleTileCount = (int)(mapSize.x * mapSize.y - currentObstacleCount);
		if (targetAccessibleTileCount != accessibleTileCount) {
			accessibleTileCoords.Clear();
		}
		return targetAccessibleTileCount == accessibleTileCount;
	}

	private void PointSourceGenerator(Transform mapHolder) {
		//shuffled accessibleTileCoords
		accessibleTileCoords = new Queue<Coord> (FisherYatesShuffle.ShuffleArray (accessibleTileCoords.ToArray (), seed));
		for (int i = 0; i < pointSourceNumber; i++) {
			Coord randomAccessibleTile = accessibleTileCoords.Dequeue();
            Vector3 pointSourcePosition = CoordToPosition(randomAccessibleTile.x,randomAccessibleTile.y);
            		
            		
        	Transform newPoint = Instantiate(pointSourcePrefab, pointSourcePosition + new Vector3(0,.5f,0), Quaternion.identity);
            newPoint.parent = mapHolder;
		}
		
	}

	Vector3 CoordToPosition(int x, int y) {
		return new Vector3 (-mapSize.x / 2 + 0.5f + x, 0, -mapSize.y / 2 + 0.5f + y);
	}

	public Coord GetRandomCoord() {
		Coord randomCoord = shuffledTileCoords.Dequeue ();
		shuffledTileCoords.Enqueue (randomCoord);
		return randomCoord;
	}

	public struct Coord {
		public int x;
		public int y;

		public Coord(int _x, int _y) {
			x = _x;
			y = _y;
		}

		public static bool operator ==(Coord c1, Coord c2) {
			return c1.x == c2.x && c1.y == c2.y;
		}

		public static bool operator !=(Coord c1, Coord c2) {
			return !(c1 == c2);
		}

	}
}
