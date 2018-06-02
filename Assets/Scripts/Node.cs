using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.AI;

public class Node : MonoBehaviour
{
	//// Fields ////
	[ Header( "Unity Setup" ) ]
	public  GameObject		m_allyTowerPrefab;
	public  NavMeshSurface  m_navMeshSurface;

	//// Unity Callbacks ////
	private void Start()
	{
	}
	private void Update()
	{
	}
	private void OnMouseDown()
	{
		// TODO: Uncomment this when UI (and therefore an EventSystem object) is added to the project.
		//if( EventSystem.current.IsPointerOverGameObject() ) // Check if we clicked on an UI object.
		//{
		//	return;
		//}

		// Create a new ally tower on this node.
		Instantiate( m_allyTowerPrefab, transform.position + new Vector3( 0.0f, 0.5f, 0.0f ), Quaternion.identity );

		// Update the nav mesh.
		m_navMeshSurface.UpdateNavMesh( m_navMeshSurface.navMeshData ); 
	}

	//// Other methods ////
}