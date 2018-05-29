using System;
using System.Collections.Generic;

public class PriorityQueue< T >
{
	// I'm using an unsorted array for this example, but ideally this would be a binary heap. 
	// binary heap class examples:
	// * https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp
	// * http://visualstudiomagazine.com/articles/2012/11/01/priority-queues-with-c.aspx
	// * http://xfleury.github.io/graphsearch.html
	// * http://stackoverflow.com/questions/102398/priority-queue-in-net

	private List< KeyValuePair< T, double > > elements = new List< KeyValuePair< T, double > >();

	public int Count
	{
		get { return elements.Count; }
	}

	public void Enqueue( T item, double priority )
	{
		elements.Add( new KeyValuePair< T, double >( item, priority ) );
	}
	public T Dequeue()
	{
		int bestIndex = 0;

		for( int i = 0 ; i < elements.Count ; i++ )
		{
			if( elements[ i ].Value < elements[ bestIndex ].Value )
			{
				bestIndex = i;
			}
		}

		T bestItem = elements[ bestIndex ].Key;
		elements.RemoveAt( bestIndex );
		return bestItem;
	}
}