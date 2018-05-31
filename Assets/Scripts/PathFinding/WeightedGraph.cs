using System.Collections.Generic;

// A* needs only a WeightedGraph and a location type L, and does *not* have to be a grid.
public interface IWeightedGraph< L >
{
	float Cost( L loc );
	IEnumerable< L > Neighbors( L id );
}