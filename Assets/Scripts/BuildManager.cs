using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//// Singleton Class ////
public class BuildManager : MonoBehaviour 
{
	//// Fields ////
	[ HideInInspector ]
	public static   BuildManager                        INSTANCE;

	[ Header( "Unity Setup" ) ]
	public			AllyTower[]                         m_AllyTowerPrefabs;
	public          NavMeshSurface						m_NavMeshSurface;
	public          Transform							m_AllyTowersParent;
	public          GameObject							m_NodeSelectedParticleEffect;
	public          GameObject							m_PartialGrid;

	private         Dictionary< string, GameObject >    m_TowerDict;
	private         string                              m_TowerToBuild;
	private         bool                                m_InBuildMode;
	private static  Vector3								PARTICLE_EFFECT_VERTICAL_OFFSET = new Vector3( 0.0f, 0.7f, 0.0f );

	//// Properties ////
	[ HideInInspector ]
	public bool IsInBuildMode
	{
		get { return m_InBuildMode; }
	}

	//// Unity Callbacks ////
	private void Start()
	{
		// Make sure only one instance is active at a time.
		if( INSTANCE == null )
		{
			INSTANCE = this;
		}
		else
		{
			Debug.Log( "More than one instance of the singleton class BuildManager found!" );
		}

		// Initialize each tower name-prefab pair.
		m_TowerDict = new Dictionary< string, GameObject >( m_AllyTowerPrefabs.Length );
		for( int towerIdx = 0; towerIdx < m_AllyTowerPrefabs.Length; towerIdx++ )
		{
			m_TowerDict.Add( m_AllyTowerPrefabs[ towerIdx ].m_TowerName, m_AllyTowerPrefabs[ towerIdx ].gameObject );
		}
	}
	private void Update()
	{
		// Exit build mode if ESC was pressed.
		if( Input.GetKeyDown( KeyCode.Escape ) )
		{
			m_InBuildMode = false;
			DeactivateSelectionGizmos();
		}
	}
	
	//// Other Methods ////
	public void ActivateSelectionGizmos( Vector3 nodePos )
	{
		// Move the preselected particle effect here, play it.
		m_NodeSelectedParticleEffect.transform.position = nodePos + PARTICLE_EFFECT_VERTICAL_OFFSET;
		m_NodeSelectedParticleEffect.SetActive( true );

		// Move and set active the partial grid.
		m_PartialGrid.transform.position = nodePos + PARTICLE_EFFECT_VERTICAL_OFFSET;
		m_PartialGrid.SetActive( true );
	}
	public void DeactivateSelectionGizmos()
	{
		m_NodeSelectedParticleEffect.SetActive( false );
		m_PartialGrid.SetActive( false );
	}
	public void SelectTowerToBuild( string towerTypeName )
	{
		m_TowerToBuild = towerTypeName;
		m_InBuildMode = true;
	}
	public void BuildTower( Vector3 buildPosition )
	{
		// Create a new ally tower on this node.
		Instantiate( m_TowerDict[ m_TowerToBuild ], buildPosition + new Vector3( 0.0f, 0.5f, 0.0f ), Quaternion.identity,
					 m_AllyTowersParent );

		// Update the nav mesh.
		m_NavMeshSurface.UpdateNavMesh( m_NavMeshSurface.navMeshData );
	}
}