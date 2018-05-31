using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour 
{
	public	GameObject						nodesParent;
	public	Vector3[]						enemySpawnpoints;
	public	Vector3							humanSpawnPoint;
	public  int								gridWidth;
	public  int								gridHeight;

	private Location						goal;
	private List< AStarSearch >				searchResults;
	private SquareGrid						grid;
	private Dictionary< Location, Node >	nodesDict;

	/// Unity Callbacks.
	private void Start()
	{
		goal          = new Location( humanSpawnPoint );
		searchResults = new List< AStarSearch >( enemySpawnpoints.Length );
		grid          = new SquareGrid( gridWidth, gridHeight );

		Node[] nodes  = nodesParent.GetComponentsInChildren< Node >();
		nodesDict     = new Dictionary< Location, Node >( nodes.Length );

		foreach( Node node in nodes )
		{
			nodesDict.Add( new Location( node.transform.position ), node );
		}

		foreach( var nodeLocPair in nodesDict )
		{
			Node node = nodeLocPair.Value;
			switch( node.type )
			{
				case NodeType.empty:
					if( !grid.costMaps.ContainsKey( node.cost ) )
					{
						grid.costMaps.Add( node.cost, new HashSet< Location >() );
					}
					grid.costMaps[ node.cost ].Add( new Location( node.transform.position ) );
					break;
				case NodeType.allyTower:
					grid.obstacles.Add( new Location( node.transform.position ) );
					break;
				case NodeType.allyUnits:
					// TODO: Is this a cost or an obstacle?
					break;
				case NodeType.human:
					// TODO: Is this a cost or an obstacle?
					break;
				default:
					break;
			}
		}

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