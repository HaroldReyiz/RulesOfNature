using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Uses A* to determine for each enemy the shortest path around towers, towards the human (the "Goal").
/// </summary>
public class PathFinder : MonoBehaviour 
{
	public	GameObject									nodesParent;
	public  int											gridWidth;
	public  int											gridHeight;
	public  Location									goal;

	[ HideInInspector ]
	public  Dictionary< Vector3, List< Location > >		enemyPaths;

	private Dictionary< Vector3, AStarSearch >			searchResults;
	private SquareGrid									grid;
	private Dictionary< Location, Node >				nodesDict;

	/// Unity Callbacks.
	private void Awake() // Use awake method so that all enemy paths are ready by the time start methods are being called.
	{
		// Get human spawnpoint (a.k.a. "Goal").
		GameObject humanGO = GameObject.FindGameObjectWithTag( "Human" );
		Vector3 humanSpawnpoint = humanGO.transform.position;
		goal = new Location( humanSpawnpoint );

		// Get enemy spawnpoints (a.k.a. "Start(s)").
		GameObject[] enemyGOs = GameObject.FindGameObjectsWithTag( "Enemy" );
		Vector3[] enemySpawnpoints = new Vector3[ enemyGOs.Length ];
		for( int enIdx = 0; enIdx < enemySpawnpoints.Length; enIdx++ )
		{
			enemySpawnpoints[ enIdx ] = enemyGOs[ enIdx ].transform.position;
		}

		// Get ally towers.
		GameObject[] allyTowerGOs = GameObject.FindGameObjectsWithTag( "AllyTower" );
		Vector3[] allyTowerCoords = new Vector3[ allyTowerGOs.Length ];
		for( int alyIdx = 0; alyIdx < allyTowerCoords.Length; alyIdx++ )
		{
			allyTowerCoords[ alyIdx ] = allyTowerGOs[ alyIdx ].transform.position;
		}

		// Inialize enemy paths, AStarSearch(es) & the grid.
		enemyPaths = new Dictionary<Vector3, List<Location>>( enemySpawnpoints.Length );
		searchResults = new Dictionary<Vector3, AStarSearch>( enemySpawnpoints.Length );
		grid = new SquareGrid( gridWidth, gridHeight );

		// Initialize nodes dictionary.
		Node[] nodes = nodesParent.GetComponentsInChildren<Node>();
		nodesDict = new Dictionary<Location, Node>( nodes.Length );

		foreach( Node node in nodes )
		{
			nodesDict.Add( new Location( node.transform.position ), node );
		}

		// Mark ally towers with NodeType.AllyTower.
		foreach( Vector3 allyTowerCoord in allyTowerCoords )
		{
			nodesDict[ new Location( allyTowerCoord ) ].type = NodeType.AllyTower;
		}

		// Initialize costmaps, obstacles etc. for the grid.
		ProcessNodeTypes();

		// Use A* for each enemy and determine shortest paths towards the goal.
		for( int enIdx = 0; enIdx < enemySpawnpoints.Length; enIdx++ )
		{
			Vector3 enemySpawnpoint = enemySpawnpoints[ enIdx ];
			searchResults.Add( enemySpawnpoint, new AStarSearch( grid, new Location( enemySpawnpoint ), goal ) );
		}

		ReconstructPaths();
	}

	/// Other methods.
	private void ProcessNodeTypes()
	{
		foreach( var nodeLocPair in nodesDict )
		{
			Node node = nodeLocPair.Value;
			switch( node.type )
			{
				case NodeType.Empty:
					if( !grid.costMaps.ContainsKey( node.cost ) )
					{
						grid.costMaps.Add( node.cost, new HashSet<Location>() );
					}
					grid.costMaps[ node.cost ].Add( new Location( node.transform.position ) );
					break;
				case NodeType.AllyTower:
					grid.obstacles.Add( new Location( node.transform.position ) );
					break;
				case NodeType.AllyUnits:
					// TODO: Add this as a new waypoint so that the enemies walk towards ally units and engage them ?
					break;
				case NodeType.Human:
					// TODO: Is this a cost or an obstacle?
					break;
				default:
					break;
			}
		}
	}
	private void ReconstructPaths()
	{
		foreach( var pair in searchResults )
		{
			enemyPaths.Add( pair.Key, ReconstructPath( pair.Value, true ) );
		}
	}
	private List< Location > ReconstructPath( AStarSearch searchResult, bool display = false )
	{
		Location pointer = searchResult.goal;
		List< Location > path = new List< Location >();

		while( !searchResult.cameFrom[ pointer ].Equals( searchResult.start ) )
		{
			path.Add( searchResult.cameFrom[ pointer ] );
			pointer = searchResult.cameFrom[ pointer ]; // Increment the location pointer.
		}

		path.Add( searchResult.cameFrom[ pointer ] ); // Add the start node to the path.
		path.Reverse();
		return path;
	}
}