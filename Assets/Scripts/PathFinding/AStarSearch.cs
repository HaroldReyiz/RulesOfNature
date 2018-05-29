using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/* NOTE about types: in the main article,
 * In the Python code I just use numbers for costs, heuristics, and priorities. 
 * In the C++ code I use a typedef for this, because you might want int or double or another type. 
 * In this C# code I use double for costs, heuristics, and priorities. You can use an int if you know your values are
 *	always integers, and you can use a smaller size number if you know the values are always small. */

public class AStarSearch
{
	public Dictionary< Location, Location > cameFrom  = new Dictionary< Location, Location >();
	public Dictionary< Location, float    > costSoFar = new Dictionary< Location, float    >();

	// Note: a generic version of A* would abstract over Location and also Heuristic
	static public float Heuristic( Location a, Location b )
	{
		return Math.Abs( a.x - b.x ) + Math.Abs( a.y - b.y );
	}
	public AStarSearch( IWeightedGraph< Location > graph, Location start, Location goal )
	{
		var frontier = new PriorityQueue< Location >();
		frontier.Enqueue( start, 0 );

		cameFrom[ start ] = start;
		costSoFar[ start ] = 0;

		while( frontier.Count > 0 )
		{
			var current = frontier.Dequeue();

			if( current.Equals( goal ) )
			{
				break;
			}

			foreach( var next in graph.Neighbors( current ) )
			{
				float newCost = costSoFar[ current ] + graph.Cost( current, next );
				if( !costSoFar.ContainsKey( next )
					|| newCost < costSoFar[ next ] )
				{
					costSoFar[ next ] = newCost;
					float priority = newCost + Heuristic( next, goal );
					frontier.Enqueue( next, priority );
					cameFrom[ next ] = current;
				}
			}
		}
	}
}