using UnityEngine.EventSystems;
using UnityEngine;

public class Node : MonoBehaviour
{
	//// Fields ////
	[ Header( "UI Setup" ) ]
	public          GameObject      m_NodeSelectedParticleEffect;
	public          GameObject      m_PartialGrid;

	private static	Vector3         PARTICLE_EFFECT_VERTICAL_OFFSET       = new Vector3( 0.0f, 0.7f, 0.0f );

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

		// Move the preselected particle effect here, play it.
		m_NodeSelectedParticleEffect.transform.position = transform.position + PARTICLE_EFFECT_VERTICAL_OFFSET;
		m_NodeSelectedParticleEffect.SetActive( true );

		// Move and set active the partial grid.
		m_PartialGrid.transform.position = transform.position + PARTICLE_EFFECT_VERTICAL_OFFSET;
		m_PartialGrid.SetActive( true );
	}
	private void OnMouseExit()
	{
		m_NodeSelectedParticleEffect.SetActive( false );
	}

	//// Other methods ////
}