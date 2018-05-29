using System.Collections.Generic;
using UnityEngine;

public class BreadthFirstSearch
{
	public static void Search( Graph< string > graph, string start )
	{
		var frontier = new Queue< string >();
		frontier.Enqueue( start );

		var visited = new HashSet< string >();
		visited.Add( start );

		while( frontier.Count > 0 )
		{
			var current = frontier.Dequeue();

			Debug.LogFormat( "Visiting {0}", current );
			foreach( var next in graph.Neighbors( current ) )
			{
				if( !visited.Contains( next ) )
				{
					frontier.Enqueue( next );
					visited.Add( next );
				}
			}
		}
	}
}