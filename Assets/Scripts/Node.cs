using UnityEngine;

public enum NodeType
{
	invalid, free, ally, enemy, human // TODO: Add different terrain types with different traversal costs.
}

public class Node : MonoBehaviour
{
	public			NodeType    type;

	private static  Vector3     GIZMO_OFFSET = new Vector3( 0.0f,  0.75f, 0.0f );
	private static  Vector3     GIZMO_SCALE  = new Vector3( 0.95f, 0.95f, 0.95f );

	private void OnDrawGizmos()
	{
		if( type == NodeType.human )
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireCube( transform.position + GIZMO_OFFSET, GIZMO_SCALE );
		}
		else if( type == NodeType.ally )
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube( transform.position + GIZMO_OFFSET, GIZMO_SCALE );
		}
		else if( type == NodeType.enemy )
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube( transform.position + GIZMO_OFFSET, GIZMO_SCALE );
		}
	}
}