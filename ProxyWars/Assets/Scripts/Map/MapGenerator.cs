using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {

	Main main;
	Map map;
	MapSettings ms;

	private int counter = 1;

	void Awake () {
		main = Util.GetMain ();
		map = Util.GetMap ();
		ms = main.ms;
	}

	public void GenerateNewMap () {
		spawnTiles ();
		spawnCities ();
		spawnConnections ();
		ensureConnectivity ();
		//ensureBiConnectedness();
		setCamera ();
	}

	private void ensureBiConnectedness()
	{
		foreach(City city in map.Cities)
		{
			//Do djikstras to each city, if the path goes through the opposite city on the way, then we need to make a new connection to a node that is on the way to the targeted starting city 
			BFSWithPassThroughCheck(city, map.PlayerStartCity, map.EnemyStartCity);
			//Do djikstras to the right city

			//Check if it goes through the closest city on the way

			//Do djikstras to the left city
			
			//Check if it goes through the closest city on the way

		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="u">The start city</param>
	/// <param name="v">The end city</param>
	/// <param name="onTheWayCity">The city that we are deciding is on the path or not</param>
	/// <returns></returns>
	private bool BFSWithPassThroughCheck(City u, City v, City onTheWayCity)
	{
		Dictionary<City, City> prev = new Dictionary<City, City>();
		Dictionary<City, bool> flag = new Dictionary<City, bool>();

		List<City> cities = u.GetConnectedCities();
		foreach(City city in map.Cities)
		{
			prev.Add(city, null);
			flag.Add(city, false);
		}
		Queue<City> queue = new Queue<City>();
		flag[u] = true;
		queue.Enqueue(u);
		while(queue.Count > 0)
		{
			City c = queue.Dequeue();
			foreach(City city in c.GetConnectedCities())
			{
				if(flag[city] == false)
				{
					flag[city] = true;
					prev[city] = c;
					queue.Enqueue(city);
				}
			}
		}

		//Trace back
		City traceBack = v;
		while(prev[traceBack] != null)
		{
			if(traceBack == onTheWayCity)
			{
				//return false;
			}
		}

		return false;
	}

	private void spawnTiles () {
		map.TileGrid = new Tile[main.ms.MapWidth, main.ms.MapHeight];
		map.Width = main.ms.MapWidth;
		map.Height = main.ms.MapHeight;

		for (int i = 0; i < main.ms.MapWidth; i++) {
			for (int j = 0; j < main.ms.MapHeight; j++) {
				GameObject go = (GameObject) GameObject.Instantiate (main.l.TilePrefab, new Vector3 (i, j, Main.Z_TILE_BG), Quaternion.identity);
				go.GetComponent<SpriteRenderer> ().color = main.l.BG_TerrainColor;
				go.transform.SetParent (GameObject.Find ("Tile Container").transform, false);

				Tile tile = go.GetComponent<Tile> ();
				map.TileGrid[i,j] = tile;
				tile.X = i;
				tile.Y = j;
			}
		}
	}

	private void spawnCities () {

		// Place start cities
		int playerSpawnY = map.Height / 2 + Random.Range (-1, 2);
		int enemySpawnY = map.Height / 2 + Random.Range (-1, 2);

		map.PlayerStartCity = placeCity (0, playerSpawnY, Faction.Player);
		map.EnemyStartCity = placeCity (map.Width - 1, enemySpawnY, Faction.Enemy);

		int citiesLeftToSpawn = ms.NumCities - 2;	// Accounting for start cities

		// Build list of columns not yet spawned it (one for each half of the map)
		List<int> LeftColumnsNotSpawnedIn = getLeftColumnsList ();
		List<int> RightColumnsNotSpawnedIn = getRightColumnsList ();
		bool leftColumn = false;

		while (citiesLeftToSpawn > 0) {

			// If all valid columns have been spawned in, refill the list
			if (LeftColumnsNotSpawnedIn.Count == 0) {
				LeftColumnsNotSpawnedIn = getLeftColumnsList ();
			}
			if (RightColumnsNotSpawnedIn.Count == 0) {
				RightColumnsNotSpawnedIn = getRightColumnsList ();
			}

			// Choose a random column on the left or right side
			int spawnColumn;

			if (leftColumn) {
				spawnColumn = LeftColumnsNotSpawnedIn [Random.Range (0, LeftColumnsNotSpawnedIn.Count)];
				LeftColumnsNotSpawnedIn.Remove (spawnColumn);
			} 
			else {
				spawnColumn = RightColumnsNotSpawnedIn [Random.Range (0, RightColumnsNotSpawnedIn.Count)];
				RightColumnsNotSpawnedIn.Remove (spawnColumn);
			}
				
			// Build list of rows within the chosen column that don't have cities and will be at least min distance away from any other city
			List<int> RowsToSpawnIn = new List<int> ();
			for (int i = 0; i < map.Height; i++) {
				Tile tile = map.TileGrid [spawnColumn, i];

				if (!tile.HasCity () && 
					!isTooCloseToOtherCities (tile, ms.MinDistanceBetweenCities)) 
				{
					RowsToSpawnIn.Add (i);
				}
			}

			// Spawn the city in a random row from the list
			if (RowsToSpawnIn.Count > 0) {
				placeCity (spawnColumn, RowsToSpawnIn[Random.Range (0, RowsToSpawnIn.Count)], Faction.Neutral);
			}

			// Whether or not spawning was successful, ensure the next city will spawn on the opposite side, and decrement cities to spawn
			// (This means that it will not necessarily always spawn the exact # of cities you set)
			// (But if that happens the parameters were probably bad)
			leftColumn = !leftColumn;
			citiesLeftToSpawn--;
		}
	}
		
	private void spawnConnections () {
		bool leftCity = true;

		// Build a dictionary which has lists of all the cities in each column
		Dictionary<int, List<City>> unconnectedCities = new Dictionary<int, List<City>> ();
		foreach (City nextCity in map.Cities) {
			int x = nextCity.GetTile ().X;

			if (unconnectedCities.ContainsKey (x)) {
				unconnectedCities [x].Add (nextCity);
			} 
			else {
				List<City> newList = new List<City> ();
				newList.Add (nextCity);
				unconnectedCities.Add (x, newList);
			}
		}

		while (unconnectedCities.Count > 0) {

			// Pick a random city from the leftmost or rightmost column with any unconnected cities
			City cityToConnect = map.Cities[0];
			int columnX = 0;

			if (leftCity) {
				columnX = map.Width;
				foreach (int key in unconnectedCities.Keys) {
					if (key < columnX) {
						columnX = key;
					}
				}
				List<City> columnList = unconnectedCities [columnX];
				cityToConnect = columnList [Random.Range (0, columnList.Count)];
			}
			else {
				columnX = -1;
				foreach (int key in unconnectedCities.Keys) {
					if (key > columnX) {
						columnX = key;
					}
					List<City> columnList = unconnectedCities [columnX];
					cityToConnect = columnList [Random.Range (0, columnList.Count)];
				}
			}

			// Connect to the two closest cities (unless there is already a connection)
			bool drewAnyConnection = false;

			Tile tile = cityToConnect.GetTile ();
			List<City> closestCities = getClosestCities (tile);
			if (closestCities.Count == 1) {
				List<City> secondClosestCities = getClosestCities (tile, closestCities);
				closestCities.Add (secondClosestCities[Random.Range (0, secondClosestCities.Count)]);
			}

			foreach (City otherCity in closestCities) {
				if (!cityToConnect.IsConnectedToCity (otherCity)) {
					connectCities (cityToConnect, otherCity);
					drewAnyConnection = true;
				}
			}

			// If the closest two cities were already connected, connect to the next-closest that is not already connected to this
			// (if you already had 5 connections, you probably don't need any more)
			int safety = 5;
			while (!drewAnyConnection  && safety > 0) {
				List<City> nextClosestCities = getClosestCities (tile, closestCities);
				City otherCity = nextClosestCities [Random.Range (0, nextClosestCities.Count)];
				closestCities.Add (otherCity);
				safety--;

				if (!cityToConnect.IsConnectedToCity (otherCity)) {
					connectCities (cityToConnect, otherCity);
					drewAnyConnection = true;
				}
			}

			// Clean up			
			unconnectedCities[columnX].Remove (cityToConnect);
			if (unconnectedCities[columnX].Count == 0) {
				unconnectedCities.Remove (columnX);
			}
		}
	}

	private void ensureConnectivity () {

		// Build a list of cities connected to the player and enemy start cities (player network and enemy network)
		List<City> playerNetwork = getNetwork (map.PlayerStartCity);
		List<City> enemyNetwork = getNetwork (map.EnemyStartCity);

		// Build list of networks not connected to either of these
		List<List<City>> otherNetworks = new List<List<City>> ();
		List<City> citiesInNetworks = new List<City> ();

		foreach (City c in map.Cities) {
			if (!playerNetwork.Contains (c) && !enemyNetwork.Contains (c) && !citiesInNetworks.Contains (c)) {

				List<City> newNetwork = getNetwork (c);
				otherNetworks.Add (newNetwork);

				foreach (City networkCity in newNetwork) {
					citiesInNetworks.Add (networkCity);
				}
			}
		}

		// For each other network...
		foreach (List<City> network in otherNetworks) {

			int numConnections = 2;
			for (int i = 0; i < numConnections; i++) {

				// Find the closest X connections between any city in that list and the player or enemy network
				List<List<City>> playerAndEnemyNetworks = new List<List<City>> ();
				playerAndEnemyNetworks.Add (playerNetwork);
				playerAndEnemyNetworks.Add (enemyNetwork);

				List<PossibleConnection> actualClosestConnections = 
					getClosestPossibleConnections (network, playerAndEnemyNetworks, numConnections);

				// Make those connections
				foreach (PossibleConnection pc in actualClosestConnections) {
					connectCities (pc.city1, pc.city2);
				}
			}
		}

		// If player and enemy start cities are not connected, make the 3 closest possible connections between them
		// (would be nice to ensure multiple paths between player and enemy start cities no matter what, but dunno how to do that)

		// Player and enemy networks may have changed by now, so check them again
		playerNetwork = getNetwork (map.PlayerStartCity);
		enemyNetwork = getNetwork (map.EnemyStartCity);
		int numPlayerEnemyConnections = 3;

		if (!playerNetwork.Contains (map.EnemyStartCity)) {
				
			// Find closest X connections between updated player and enemy networks
			List<PossibleConnection> possiblePlayerEnemyConnections = 
				getClosestPossibleConnections (playerNetwork, enemyNetwork, numPlayerEnemyConnections);

			// Make those connections
			// Could maybe use a check to ensure not connecting player + enemy start cities, but that's extremely unlikely
			foreach (PossibleConnection pc in possiblePlayerEnemyConnections) {
				connectCities (pc.city1, pc.city2);
			}
		}
	}

	private List<City> getNetwork (City city) {
		List<City> network = new List<City> ();
		List<City> citiesToCheck = new List<City> ();

		network.Add (city);
		citiesToCheck.Add (city);

		while (citiesToCheck.Count > 0) {
			City nextCity = citiesToCheck [0];
			List<City> connectedCities = nextCity.GetConnectedCities ();

			foreach (City c in connectedCities) {
				if (!network.Contains (c)) {
					network.Add (c);
					citiesToCheck.Add (c);
				}
			}

			citiesToCheck.RemoveAt (0);
		}

		return network;
	}

	private List<PossibleConnection> getClosestPossibleConnections (List<City> network1, List<List<City>> otherNetworks, int connections) {
		List<PossibleConnection> possibleConnections = new List<PossibleConnection> ();

		int furthestIncludedDistance = 0;

		foreach (City city1 in network1) {
			foreach (List<City> network2 in otherNetworks) {
				foreach (City city2 in network2) {

					int distance = getDistanceBetweenTiles (city1.GetTile (), city2.GetTile ());

					// If we don't have enough connections yet, just add this one in
					if (possibleConnections.Count < connections) {
						PossibleConnection pc = new PossibleConnection (city1, city2, distance);
						possibleConnections.Add (pc);
						furthestIncludedDistance = getFurthestDistance (possibleConnections);

					}

					// Otherwise, if the new one is shorter than the furthest, remove the furthest existing one and add this one in
					// If there are multiple "furthest existing ones" then choose one at random
					else if (distance < furthestIncludedDistance) {
						possibleConnections = removeRandomFurthestConnection (possibleConnections, furthestIncludedDistance);
						PossibleConnection pc = new PossibleConnection (city1, city2, distance);
						possibleConnections.Add (pc);
						furthestIncludedDistance = getFurthestDistance (possibleConnections);
					}

					// Otherwise, if the new one is the same distance as the furthest...
					// 50% chance to remove the furthest existing one and add this one in
					else if (distance == furthestIncludedDistance) {
						int r = Random.Range (0, 2);
						if (r == 0) {
							possibleConnections = removeRandomFurthestConnection (possibleConnections, furthestIncludedDistance);
							PossibleConnection pc = new PossibleConnection (city1, city2, distance);
							possibleConnections.Add (pc);
						}
					}
				}
			}
		}

		return possibleConnections;
	}

	private int getFurthestDistance (List<PossibleConnection> pcs) {
		int furthestDistance = 0;

		foreach (PossibleConnection pc in pcs) {
			if (pc.distance > furthestDistance) {
				furthestDistance = pc.distance;
			}
		}

		return furthestDistance;
	}

	private List<PossibleConnection> removeRandomFurthestConnection (List<PossibleConnection> pcs) {		
		return removeRandomFurthestConnection (pcs, getFurthestDistance (pcs));
	}

	private List<PossibleConnection> removeRandomFurthestConnection (List<PossibleConnection> pcs, int furthestDistance) {
		List<PossibleConnection> optionsToDestroy = new List<PossibleConnection> (); 
		foreach (PossibleConnection pcDestroy in pcs) {
			if (pcDestroy.distance == furthestDistance) {
				optionsToDestroy.Add (pcDestroy);
			}
		}

		PossibleConnection toDestroy = optionsToDestroy [Random.Range (0, optionsToDestroy.Count)];
		pcs.Remove (toDestroy);
		return pcs;
	}

	private List<PossibleConnection> getClosestPossibleConnections (List<City> network1, List<City> network2, int connections) {
		List<List<City>> otherNetworks = new List<List<City>> ();
		otherNetworks.Add (network2);
		return getClosestPossibleConnections (network1, otherNetworks, connections);
	}
		
	private bool isTooCloseToOtherCities (Tile tile, int minDistance) {
		City city = getClosestCities (tile) [0];
		return getDistanceBetweenTiles (tile, city.GetTile ()) < minDistance;
	}

	private void connectCities (City city1, City city2) {
		city1.AddConnection (city2);
		city2.AddConnection (city1);
		drawConnection (city1, city2);
	}

	private void drawConnection (City city1, City city2) {
		GameObject go = (GameObject)GameObject.Instantiate (main.l.ConnectionPrefab, GameObject.Find ("Connection Container").transform);
		LineRenderer lr = go.GetComponent<LineRenderer> ();
		Vector3[] positions = new Vector3[] {
			city1.transform.position,
			city2.transform.position
		};

		positions[0].z = Main.Z_CONNECTION;
		positions[1].z = Main.Z_CONNECTION;

		lr.SetPositions (positions);
	}

	private List<City> getClosestCities (Tile tile) {
		List<City> exclude = new List<City> ();
		return getClosestCities (tile, exclude); 
	}

	private List<City> getClosestCities (Tile tile, City cityToExclude) {
		List<City> exclude = new List<City> ();
		exclude.Add (cityToExclude);
		return getClosestCities (tile, exclude); 
	}

	private List<City> getClosestCities (Tile tile, List<City> citiesToExclude) {

		// Note: If multiple cities are equally close, returns one of the closest cities at random

		int closestDistance = 1000;
		List<City> closestCities = new List<City> ();

		List<City> exclude = citiesToExclude;
		if (tile.HasCity ()) {
			exclude.Add (tile.GetCity ());
		}

		foreach (City otherCity in map.Cities) {

			if (!exclude.Contains (otherCity)) {
				int distance = getDistanceBetweenTiles (tile, otherCity.GetTile ());

				if (distance < closestDistance) {
					closestDistance = distance;
					closestCities = new List<City> ();
					closestCities.Add (otherCity);
				} 
				else if (distance == closestDistance) {
					closestCities.Add (otherCity);
				}
			}
		}

		if (closestCities.Count == 0) {
			Debug.LogError ("Something went horribly wrong when finding the closest city");
		}

		return closestCities;
	}

	private int getDistanceBetweenTiles (Tile tile1, Tile tile2) {
		return Mathf.Abs (tile2.X - tile1.X) + Mathf.Abs (tile2.Y - tile1.Y);
	}

	public struct PossibleConnection {
		public City city1;
		public City city2;
		public int distance;

		public PossibleConnection (City c1, City c2, int d) {
			city1 = c1;
			city2 = c2;
			distance = d;
		}
	}

	private List<int> getLeftColumnsList () {
		List<int> LeftColumnsNotSpawnedIn = new List<int> ();

		for (int i = 1; i < (map.Width - 1) / 2; i++) {
			LeftColumnsNotSpawnedIn.Add (i);
		}

		return LeftColumnsNotSpawnedIn;
	}

	private List<int> getRightColumnsList () {
		List<int> RightColumnsNotSpawnedIn = new List<int> ();

		for (int i = (map.Width - 1) / 2 + 1; i < map.Width - 1; i++) {
			RightColumnsNotSpawnedIn.Add (i);
		}

		return RightColumnsNotSpawnedIn;
	}

	private City placeCity (int x, int y, Faction f) {
		GameObject go = (GameObject)GameObject.Instantiate (main.l.CityPrefab, new Vector3 (x, y, Main.Z_CITY), Quaternion.identity);
		go.transform.SetParent (GameObject.Find ("City Container").transform, false);
		City city = go.GetComponent<City> ();
		city.Setup ();
		map.Cities.Add (city);
		city.SetTile (map.TileGrid [x, y]);
		city.SetOwner (f);
		city.cityId = counter;
		counter++;
		if (f != Faction.Neutral) {
			city.IM.SetCompleteControl (f);
		}
		return city;
	}
		
	private void setCamera () {
		Vector3 centerOfMap = new Vector3 (map.Width / 2, map.Height / 2, Main.Z_CAMERA);
		Camera.main.transform.position = centerOfMap;
		Camera.main.orthographicSize = (map.Width > map.Height) ? map.Width / 2 + 1 : map.Height / 2 + 1;
	}
}
