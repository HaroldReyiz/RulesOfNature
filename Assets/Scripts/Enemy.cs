using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour 
{
	[ Header( "Attributes" ) ]
	public	float				health = 100.0f;
	public  float				moveSpeed;

	[ Header( "Path Finding" ) ]
	public	PathFinder			pathFinder;

	private List< Location >	path;
	private int		            waypointIdx;

	/// Unity Callbacks.
	private void Start()
	{
		path        = pathFinder.enemyPaths[ transform.position ];
		waypointIdx = 1;
	}
	private void Update()
	{
		if( waypointIdx < path.Count && Vector3.Distance( transform.position, path[ waypointIdx ].pos ) < 0.1f )
		{
			waypointIdx++;
		}

		// TODO: If goal reached, attack and take one life from the human ("goal").

		if( waypointIdx < path.Count )
		{
			Vector3 dir = ( path[ waypointIdx ].pos - transform.position ).normalized;
			transform.position += dir * moveSpeed * Time.deltaTime;
		}
	}

	/// Other Methods.
	public void TakeDamage( float amount )
	{
		health -= amount;

		if( health <= 0.0f )
		{
			Die();
		}
	}
	private void Die()
	{
		Destroy( gameObject );
	}
}