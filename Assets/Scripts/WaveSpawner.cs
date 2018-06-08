using System.Collections;
using UnityEngine;

//// Singleton Class ////
public class WaveSpawner : MonoBehaviour 
{
	//// Fields ////
	[ System.Serializable ]
	public class WaveProperties
	{
		[ System.Serializable ]
		public class EnemyTypeWaveAttributes
		{
			public GameObject m_EnemyPrefab;
			public int        m_EnemyCount;
			public Vector3    m_SpawnPosition;
		}

		public	EnemyTypeWaveAttributes[]	m_EnemyTypes;
		public  float						m_TimeToNextWave;
	}

	[ HideInInspector ]
	public static   WaveSpawner			INSTANCE;
	public			WaveProperties[]	m_Waves;

	private         int                 m_RemainingEnemyCount = 0;
	private         int                 m_WaveIdx = 0;

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
			Debug.Log( "More than one instance of the singleton class WaveSpawner found!" );
		}

		// Send the first wave.
		Invoke( "SpawnNextWave", 0.0f );
	}
	// TODO: Visualize spawnpoints via OnDrawGizmoXXX callbacks.

	//// Other Methods ////
	public void OnEnemyDied()
	{
		m_RemainingEnemyCount--;
		if( m_RemainingEnemyCount == 0 )
		{
			m_WaveIdx++;
			Invoke( "SpawnNextWave", m_Waves[ m_WaveIdx ].m_TimeToNextWave );
		}
	}
	private void SpawnNextWave()
	{
		for( int enemyTypeIdx = 0; enemyTypeIdx < m_Waves[ m_WaveIdx ].m_EnemyTypes.Length; enemyTypeIdx++ )
		{
			StartCoroutine( SpawnEnemiesOfType( enemyTypeIdx ) );
		}
	}
	private IEnumerator SpawnEnemiesOfType( int enemyTypeIdx )
	{
		WaveProperties.EnemyTypeWaveAttributes enemyType = m_Waves[ m_WaveIdx ].m_EnemyTypes[ enemyTypeIdx ];
		m_RemainingEnemyCount += enemyType.m_EnemyCount;
		for( int enemyIdx = 0; enemyIdx < enemyType.m_EnemyCount; enemyIdx++ )
		{
			// Spawn an enemy of this type each 2 seconds.
			PoolManager.Spawn( enemyType.m_EnemyPrefab, enemyType.m_SpawnPosition, Quaternion.identity );
			yield return new WaitForSeconds( 2.0f );
		}
	}
}