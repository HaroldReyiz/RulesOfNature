using UnityEngine.EventSystems;
using UnityEngine;

public class Node : MonoBehaviour
{
	//// Fields ////

	//// Unity Callbacks ////
	private void OnMouseDown()
	{
		if( EventSystem.current.IsPointerOverGameObject() ) // Check if we clicked on an UI object.
		{
			return;
		}

		if( BuildManager.INSTANCE.IsInBuildMode /*&& player has enough money */ )
		{
			BuildManager.INSTANCE.BuildTower( transform.position );
		}
	}
	private void OnMouseOver()
	{
		if( Input.GetMouseButton( 0 ) && BuildManager.INSTANCE.IsInBuildMode /*&& player has enough money */ )
		{
			BuildManager.INSTANCE.BuildTower( transform.position );
		}
	}
	private void OnMouseEnter()
	{
		if( EventSystem.current.IsPointerOverGameObject() ) // Check if we clicked on an UI object.
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