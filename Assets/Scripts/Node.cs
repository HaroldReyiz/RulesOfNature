using UnityEngine;

public enum NodeType
{
	Empty, AllyTower, AllyUnit, Human
}

public class Node : MonoBehaviour
{
	//// Fields ////
	public          NodeType    m_type;

	//// Unity Callbacks ////
	private void Start()
	{
	}
	private void Update()
	{
	}

	//// Other methods ////
}