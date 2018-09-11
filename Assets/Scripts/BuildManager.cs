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
	public          Camera                              m_Cam;
	public			AllyTower[]                         m_AllyTowerPrefabs;
	public          NavMeshSurface						m_NavMeshSurface;
	public          GameObject							m_BuildStartPosEffect;
	public          GameObject							m_BuildEndPosEffect;
	public          GameObject                          m_Grid;

	private         Dictionary< string, GameObject >    m_TowerDict;
	private         string                              m_TowerToBuild;
	private         bool                                m_MultipleBuildMode;
	private         Vector3                             m_BuildStartPos;
	private         Vector3                             m_BuildEndPos;
	private         Vector3                             m_BuildDirection;
	private         int                                 m_NumTowersToBuild;
	private         bool                                m_InBuildMode;
	private         Material                            m_GridMaterial;
	private static  Vector3								VERTICAL_OFFSET = new Vector3( 0.0f, 0.25f, 0.0f );

	//// Properties ////
	[ HideInInspector ]
	public bool IsInBuildMode
	{
		get { return m_InBuildMode; }
	}
	[HideInInspector]
	public bool MultipleBuildMode
	{
		get { return m_MultipleBuildMode; }
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

		m_GridMaterial = m_Grid.GetComponentInChildren< MeshRenderer >().material;
	}
	private void Update()
	{
		// Exit build mode if ESC was pressed.
		if( Input.GetKeyDown( KeyCode.Escape ) )
		{
			m_InBuildMode = false;
			DeactivateSelectionUIAndFX();
		}

		if( Input.GetKeyDown( KeyCode.Alpha1 ) )
		{
			m_InBuildMode = true;
			ActivateSelectionUIAndFX();
			m_TowerToBuild = "Ally Tower 1";
		}

		if( Input.GetKeyDown( KeyCode.Alpha2 ) )
		{
			m_InBuildMode = true;
			ActivateSelectionUIAndFX();
			m_TowerToBuild = "Ally Tower 2";
		}

		if( Input.GetKeyDown( KeyCode.Alpha3 ) )
		{
			m_InBuildMode = true;
			ActivateSelectionUIAndFX();
			m_TowerToBuild = "Ally Tower 3";
		}

		if( m_InBuildMode )
		{
			SetRevealShaderParameters();
		}
	}

	//// Other Methods ////
	public void SelectTowerToBuild( string towerTypeName )
	{
		m_TowerToBuild = towerTypeName;
		m_InBuildMode = true;
		ActivateSelectionUIAndFX();
	}
	public void BuildTower( Vector3 buildPosition )
	{
		if( MultipleBuildMode )
		{
			for( int nodeIdx = 0 ; nodeIdx < m_NumTowersToBuild ; nodeIdx++ )
			{
				// Spawn an ally tower on this node.
				PoolsManager.Spawn( m_TowerDict[ m_TowerToBuild ],
									m_BuildStartPos + m_BuildDirection * nodeIdx + VERTICAL_OFFSET,
									m_TowerDict[ m_TowerToBuild ].transform.rotation );
			}

			m_BuildStartPos = m_BuildEndPos; // Update build start position so that it starts from the last position.
		}
		else
		{
			// Spawn an ally tower on this node.
			PoolsManager.Spawn( m_TowerDict[ m_TowerToBuild ], buildPosition + VERTICAL_OFFSET,
								m_TowerDict[ m_TowerToBuild ].transform.rotation );
		}

		// Update the nav mesh.
		m_NavMeshSurface.UpdateNavMesh( m_NavMeshSurface.navMeshData );

		m_BuildStartPosEffect.SetActive( false );
		m_BuildEndPosEffect.SetActive( false );
	}
	public void SetBuildStartPosition( Vector3 startPos )
	{
		m_BuildStartPos = startPos;

		m_BuildStartPosEffect.transform.position = m_BuildStartPos;
		m_BuildStartPosEffect.SetActive( true );
	}
	// Computes the build end position by projecting the mouse coordinates onto the closer of the x and z axes.
	public void SetBuildEndPosition( Vector3 currentNodePos )
	{
		float deltaX = currentNodePos.x - m_BuildStartPos.x;
		float deltaZ = currentNodePos.z - m_BuildStartPos.z;
		int absDeltaX = ( int )( Mathf.Abs( deltaX ) + 0.5f ) + 1;
		int absDeltaZ = ( int )( Mathf.Abs( deltaZ ) + 0.5f ) + 1;
		m_BuildDirection = ( absDeltaX > absDeltaZ ? deltaX * Vector3.right : deltaZ * Vector3.forward ).normalized;
		m_NumTowersToBuild = Mathf.Max( absDeltaX, absDeltaZ );

		m_BuildEndPos = m_BuildStartPos + m_BuildDirection * ( m_NumTowersToBuild - 1 );

		m_BuildEndPosEffect.transform.position = m_BuildEndPos;
		m_BuildEndPosEffect.SetActive( true );
	}
	public void EnableMultiBuildMode()
	{
		m_MultipleBuildMode = true;
	}
	public void DisableMultiBuildMode()
	{
		m_MultipleBuildMode = false;

		m_BuildStartPosEffect.SetActive( false );
		m_BuildEndPosEffect.SetActive( false );
	}
	private void ActivateSelectionUIAndFX()
	{	
		// Set active the actual grid.
		m_Grid.SetActive( true );
	}
	private void DeactivateSelectionUIAndFX()
	{
		m_BuildStartPosEffect.SetActive( false );
		m_BuildEndPosEffect.SetActive( false );
		m_Grid.SetActive( false );
	}
	private void SetRevealShaderParameters()
	{
		Ray ray = m_Cam.ScreenPointToRay( Input.mousePosition );
		RaycastHit hit;
		if( Physics.Raycast( ray, out hit ) )
		{
			m_GridMaterial.SetFloat( "_PlayerXPos", ( hit.point.x ) / 32.0f );
			m_GridMaterial.SetFloat( "_PlayerZPos", ( hit.point.z ) / 32.0f );
		}
	}
}