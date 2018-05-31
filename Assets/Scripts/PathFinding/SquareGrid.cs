using System.Collections.Generic;
using UnityEngine;

public class SquareGrid : IWeightedGraph< Location >
{
	public int width, height;
	public HashSet< Location > obstacles                   = new HashSet< Location >();
	public Dictionary< int, HashSet< Location > > costMaps = new Dictionary< int, HashSet< Location > >();

	private static readonly Location[] DIRS = new []
	{
		new Location(  1,  0 ),
		new Location(  0, -1 ),
		new Location( -1,  0 ),
		new Location(  0,  1 )
	};

	public SquareGrid( int width, int height )
	{
		this.width  = width;
		this.height = height;
	}
	public bool InBounds( Location id )
	{
		return 0 <= id.x && id.x < width && 0 <= id.y && id.y < height;
	}
	public bool Passable( Location id )
	{
		return !obstacles.Contains( id );
	}
	public float Cost( Location loc )
	{
		foreach( var costMap in costMaps )
		{
			if( costMap.Value.Contains( loc ) )
			{
				return costMap.Key;
			}
		}
		return 1; // Default cost is 1.
	}
	public IEnumerable< Location > Neighbors( Location id )
	{
		foreach( var dir in DIRS )
		{
			Location next = new Location( id.x + dir.x, id.y + dir.y );
			if( InBounds( next ) && Passable( next ) )
			{
				yield return next;
			}
		}
	}
}