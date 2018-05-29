using System.Collections.Generic;

public class Graph< Location >
{
	public Dictionary< Location, Location[] > edges = new Dictionary< Location, Location[] >();

	public Location[] Neighbors( Location id )
	{
		return edges[ id ];
	}
};