using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Uses A* to determine for each enemy the shortest path around towers, towards the human (the "Goal").
/// </summary>
public class PathFinder : MonoBehaviour 
{
	public	GameObject						nodesParent;
	public  int								gridWidth;
	public  int								gridHeight;

	private Location						goal;
	private List< AStarSearch >				searchResults;
	private SquareGrid						grid;
	private Dictionary< Location, Node >	nodesDict;

	/// Unity Callbacks.
	private void Start()
	{
		// Get human spawnpoint (a.k.a. "Goal").
		GameObject humanGO = GameObject.FindGameObjectWithTag( "Human" );
		Vector3 humanSpawnpoint = humanGO.transform.position;
		goal = new Location( humanSpawnpoint );

		// Get enemy spawnpoints (a.k.a. "Start(s)").
		GameObject[] enemyGOs = GameObject.FindGameObjectsWithTag( "Enemy" );
		Vector3[] enemySpawnpoints = new Vector3[ enemyGOs.Length ];
		for( int enIdx = 0 ; enIdx < enemySpawnpoints.Length ; enIdx++ )
		{
			enemySpawnpoints[ enIdx ] = enemyGOs[ enIdx ].transform.position;
		}

		// Get ally towers.
		GameObject[] allyTowerGOs = GameObject.FindGameObjectsWithTag( "AllyTower" );
		Vector3[] allyTowerCoords = new Vector3[ allyTowerGOs.Length ];
		for( int alyIdx = 0 ; alyIdx < allyTowerCoords.Length ; alyIdx++ )
		{
			allyTowerCoords[ alyIdx ] = allyTowerGOs[ alyIdx ].transform.position;
		}

		// Inialize AStarSearch(es) & the grid.
		searchResults = new List< AStarSearch >( enemySpawnpoints.Length );
		grid          = new SquareGrid( gridWidth, gridHeight );

		// Initialize nodes dictionary.
		Node[] nodes  = nodesParent.GetComponentsInChildren< Node >();
		nodesDict     = new Dictionary< Location, Node >( nodes.Length );

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
		foreach( var nodeLocPair in nodesDict )
		{
			Node node = nodeLocPair.Value;
			switch( node.type )
			{
				case NodeType.Empty:
					if( !grid.costMaps.ContainsKey( node.cost ) )
					{
						grid.costMaps.Add( node.cost, new HashSet< Location >() );
					}
					grid.costMaps[ node.cost ].Add( new Location( node.transform.position ) );
					break;
				case NodeType.AllyTower:
					grid.obstacles.Add( new Location( node.transform.position ) );
					break;
				case NodeType.AllyUnits:
					// TODO: Is this a cost or an obstacle?
					break;
				case NodeType.Human:
					// TODO: Is this a cost or an obstacle?
					break;
				default:
					break;
			}
		}

		// Use A* for each enemy and determine shortest paths towards the goal.
		foreach( Vector3 enemySpawnpoint in enemySpawnpoints )
		{
			searchResults.Add( new AStarSearch( grid, new Location( enemySpawnpoint ), goal ) );
		}

		DisplayAllEnemyPaths( goal );
	}

	/// Other methods.
	private void DisplayAllEnemyPaths( Location goal )
	{
		foreach( AStarSearch searchResult in searchResults )
		{
			ReconstructPath( searchResult, true );
		}
	}
	private List< Location > ReconstructPath( AStarSearch searchResult, bool display = false )
	{
		Location pointer = searchResult.goal;
		List< Location > path = new List< Location >();

		Vector3 nodeBefore = searchResult.cameFrom[ searchResult.cameFrom[ searchResult.goal ] ].pos;
		Vector3 nodeAfter  = searchResult.goal.pos;

		while( !searchResult.cameFrom[ pointer ].Equals( searchResult.start ) )
		{
			if( display )
			{
				nodesDict[ searchResult.cameFrom[ pointer ] ].VisualizePathNode( nodeBefore, nodeAfter );
			}

			path.Add( searchResult.cameFrom[ pointer ] );
			pointer = searchResult.cameFrom[ pointer ]; // Increment the location pointer.

			nodeBefore = searchResult.cameFrom[ searchResult.cameFrom[ pointer ] ].pos;
			nodeAfter  = pointer.pos;
		}

		path.Add( searchResult.cameFrom[ pointer ] ); // Add the start node to the path.
		nodeBefore = searchResult.cameFrom[ pointer ].pos;
		nodesDict[ searchResult.cameFrom[ pointer ] ].VisualizePathNode( nodeBefore, nodeAfter );
		path.Reverse();
		return path;
	}
}