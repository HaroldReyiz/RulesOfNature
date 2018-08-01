using UnityEngine.EventSystems;
using UnityEngine;

public class Node : MonoBehaviour
{
	//// Fields ////
	public  bool    m_IsOccupiedByTower = false;

	//// Unity Callbacks ////
	private void OnMouseDown()
	{
		if( m_IsOccupiedByTower || EventSystem.current.IsPointerOverGameObject() ) // Check if we clicked on an UI object.
		{
			return;
		}

		if( BuildManager.INSTANCE.IsInBuildMode /*&& player has enough money */ )
		{
			BuildManager.INSTANCE.BuildTower( transform.position );
			m_IsOccupiedByTower = true;
		}
	}
	private void OnMouseOver()
	{
		if( m_IsOccupiedByTower )
		{
			return;
		}

		if( Input.GetMouseButton( 0 ) && BuildManager.INSTANCE.IsInBuildMode /*&& player has enough money */ )
		{
			BuildManager.INSTANCE.BuildTower( transform.position );
			m_IsOccupiedByTower = true;
		}
	}
	private void OnMouseEnter()
	{
		if( m_IsOccupiedByTower || EventSystem.current.IsPointerOverGameObject() ) // Check if we clicked on an UI object.
		{
			return;
		}

		if( BuildManager.INSTANCE.IsInBuildMode )
		{
			BuildManager.INSTANCE.ActivateSelectionGizmos( transform.position );
		}
	}
	private void OnMouseExit()
	{
	}

	//// Other Methods ////
}