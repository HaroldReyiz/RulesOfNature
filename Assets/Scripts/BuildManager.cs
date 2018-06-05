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

	private         Dictionary< string, GameObject >    m_TowerDict;
	private         string                              m_TowerToBuild;
	private         bool                                m_InBuildMode;

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
			m_TowerDict.Add( m_AllyTowerPrefabs[ towerIdx ].m_name, m_AllyTowerPrefabs[ towerIdx ].gameObject );
		}
	}
	private void Update()
	{
	}
	
	//// Other methods ////
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