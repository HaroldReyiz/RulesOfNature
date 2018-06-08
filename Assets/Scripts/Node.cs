using UnityEngine.EventSystems;
using UnityEngine;

public class Node : MonoBehaviour
{
	//// Fields ////

	//// Unity Callbacks ////
	private void Start()
	{
	}
	private void Update()
	{
	}
	private void OnMouseDown()
	{
		if( EventSystem.current.IsPointerOverGameObject() ) // Check if we clicked on an UI object.
		{
			return;
		}

		if( BuildManager.INSTANCE.IsInBuildMode /*&& player has enoug money */ )
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
		BuildManager.INSTANCE.DeactivateSelectionGizmos();
	}

	//// Other Methods ////
}