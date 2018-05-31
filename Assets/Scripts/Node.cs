using UnityEngine;

public enum NodeType
{
	empty, allyTower, allyUnits, human // TODO: Add different terrain types with different traversal costs.
}

public class Node : MonoBehaviour
{
	public			NodeType    type;
	public          int         cost;

	// Path Visualization.
	private         bool        isPathNode;
	private         Vector3     nodeBefore, nodeAfter;

	private static  Vector3     GIZMO_OFFSET = new Vector3( 0.0f,  0.75f, 0.0f );
	private static  Vector3     GIZMO_SCALE  = new Vector3( 0.95f, 0.95f, 0.95f );

	/// Unity Callbacks.
	private void OnDrawGizmos()
	{
		if( type == NodeType.human )
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireCube( transform.position + GIZMO_OFFSET, GIZMO_SCALE );
		}
		else if( type == NodeType.allyTower )
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube( transform.position + GIZMO_OFFSET, GIZMO_SCALE );
		}
		else if( type == NodeType.allyUnits )
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawWireCube( transform.position + GIZMO_OFFSET, GIZMO_SCALE );
		}
		else // NodeType.empty.
		{
			if( isPathNode )
			{
				Gizmos.color = Color.magenta;
				// Draw main lines.
				Vector3 posAbove = transform.position + Vector3.up * 0.75f;
				Vector3 dirNext = ( this.nodeAfter  - posAbove ).normalized;
				Vector3 dirPrev = ( this.nodeBefore - posAbove ).normalized;
				Gizmos.DrawLine( posAbove + ( dirPrev * transform.localScale.x / 2.0f ),
								 posAbove + ( dirNext * transform.localScale.x / 2.0f ) );

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
	}

	/// Other Methods.
	public void VisualizePathNode( Vector3 nodeBefore, Vector3 nodeAfter )
	{
		this.nodeBefore = nodeBefore;
		this.nodeAfter  = nodeAfter;
		this.isPathNode = true;
	}
}