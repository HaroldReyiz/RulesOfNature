using UnityEngine;

public enum NodeType
{
	Empty, AllyTower, AllyUnits, Human // TODO: Add different terrain types with different traversal costs.
}

public class Node : MonoBehaviour
{
	public			NodeType    type;
	public          int         cost;
}