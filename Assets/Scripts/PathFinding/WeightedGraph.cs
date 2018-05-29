using System.Collections.Generic;

// A* needs only a WeightedGraph and a location type L, and does *not* have to be a grid.
public interface IWeightedGraph< L >
{
	float Cost( L a, L b );
	IEnumerable< L > Neighbors( L id );
}