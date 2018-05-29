using System.Collections.Generic;

public struct Location
{
	// Implementation notes: I am using the default Equals but it can be slow. 
	// You'll probably want to override both Equals and GetHashCode in a real project.

	public readonly int x, y;
	public Location( int x, int y )
	{
		this.x = x;
		this.y = y;
	}
}
public class SquareGrid : IWeightedGraph< Location >
{
	public int width, height;
	public HashSet< Location > walls   = new HashSet< Location >();
	public HashSet< Location > forests = new HashSet< Location >();

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
		return !walls.Contains( id );
	}
	public float Cost( Location a, Location b )
	{
		return forests.Contains( b ) ? 5 : 1;
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