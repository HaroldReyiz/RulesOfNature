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
	private void OnDrawGizmos()
	{
		if( path == null ) // This prevents the null reference exception when the editor is open but game is not playing.
		{
			return;
		}

		for( int pathIdx = 1; pathIdx < path.Count; pathIdx++ )
		{
			Vector3 nodeBefore  = path[ pathIdx - 1 ].pos;
			Vector3 nodeCurrent = path[ pathIdx		].pos;
			Vector3 nodeAfter   = pathIdx == path.Count - 1 ? pathFinder.goal.pos : path[ pathIdx + 1 ].pos;

			Gizmos.color = pathIdx < waypointIdx ? Color.black : Color.magenta;

			// Draw main lines.
			Vector3 posAbove = nodeCurrent + Vector3.up * 0.75f;
			Vector3 dirNext = ( nodeAfter  - posAbove ).normalized;
			Vector3 dirPrev = ( nodeBefore - posAbove ).normalized;
			Gizmos.DrawLine( posAbove + ( dirPrev * 0.5f ),
							 posAbove + ( dirNext * 0.5f ) );

			// Draw arrows.
			Vector3 dir = ( dirNext - dirPrev ).normalized;

			Vector3 arrowStart = posAbove   + dirNext * 0.5f;
			Vector3 arrowEnd   = arrowStart - dir * 0.5f;
			Vector3 rightArrow = arrowEnd - arrowStart, leftArrow = arrowEnd - arrowStart;
			// Right arrow.
			rightArrow = Quaternion.Euler( 0.0f, +35.0f, 0.0f ) * rightArrow;
			Gizmos.DrawLine( arrowStart, arrowStart + rightArrow );
			// Left arrow.
			leftArrow  = Quaternion.Euler( 0.0f, -35.0f, 0.0f ) * leftArrow;
			Gizmos.DrawLine( arrowStart, arrowStart + leftArrow );
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