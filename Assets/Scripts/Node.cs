using UnityEngine.EventSystems;
using UnityEngine;

public class Node : MonoBehaviour
{
	//// Fields ////
	public  bool    m_IsOccupiedByTower = false;

	//// Unity Callbacks ////
	private void OnMouseDown()
	{
		if( m_IsOccupiedByTower /*|| EventSystem.current.IsPointerOverGameObject()*/ )
		{
			return;
		}

		if( Input.GetButtonUp( "Fire3" ) ) // Fire3 = Left-Shift Button on PC.
		{
			BuildManager.INSTANCE.DisableMultiBuildMode();
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

		if( BuildManager.INSTANCE.IsInBuildMode /*&& player has enough money */ )
		{
			if( Input.GetButtonDown( "Fire3" ) ) // Fire3 = Left-Shift Button on PC.
			{
				BuildManager.INSTANCE.EnableMultiBuildMode();
				BuildManager.INSTANCE.SetBuildStartPosition( transform.position );
			}

			if( Input.GetButton( "Fire3" ) ) // Fire3 = Left-Shift Button on PC.
			{
				BuildManager.INSTANCE.SetBuildEndPosition( transform.position );
				// TODO: Draw a ui of line of nodes to build on.
			}
			else
			{
				BuildManager.INSTANCE.DisableMultiBuildMode();
			}
		}
	}
	
	//// Other Methods ////
}