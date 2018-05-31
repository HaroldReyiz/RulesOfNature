using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Location
{
	// Implementation notes: I am using the default Equals but it can be slow. 
	// You'll probably want to override both Equals and GetHashCode in a real project.

	private readonly Vector2Int coords;
	public int x { get { return coords.x; } }
	public int y { get { return coords.y; } }
	public Vector3 pos
	{
		get { return new Vector3( x, 0.5f, y ); }
	}

	public Location( Vector2Int vec2I )
	{
		coords = vec2I;
	}
	public Location( Vector2 vec2 )
	{
		coords = new Vector2Int( ( int )vec2.x, ( int )vec2.y );
	}
	public Location( Vector3Int vec3I )
	{
		coords = new Vector2Int( vec3I.x, vec3I.z );
	}
	public Location( Vector3 vec3 )
	{
		coords = new Vector2Int( ( int )vec3.x, ( int )vec3.z );
	}
	public Location( int x, int y )
	{
		coords = new Vector2Int( x, y );
	}
}