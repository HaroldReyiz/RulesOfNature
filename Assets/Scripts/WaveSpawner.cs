using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObserverPattern;

//// Singleton Class ////
/// <summary>
/// Observes enemies to keep track of their count etc.
/// Is observed by GameManager so that the GameManager can be notified when there are no more enemies left (game over).
/// </summary>
public class WaveSpawner : MonoBehaviour, IObserver, IObservable
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
			public float      m_TimeBetweenSpawns = 1.0f;
		}

		public	EnemyTypeWaveAttributes[]	m_EnemyTypes;
		public  float						m_TimeToNextWave;
	}

	[ HideInInspector ]
	public static   WaveSpawner					INSTANCE;
	public			WaveProperties[]			m_Waves;
	public          List< Enemy >				m_ActiveEnemies;

	private         Dictionary< int, Enemy >	m_EnemyDict;
	private         int							m_RemainingEnemyCount = 0;
	private         int							m_WaveIdx = 0;

	private static  Vector3                     SPAWNPOINT_VIS_OFFSET = new Vector3( 0.0f, 0.5f, 0.0f );
	private static  Vector3                     SPAWNPOINT_VIS_SIZE   = new Vector3( 1.0f, 1.0f, 1.0f );

	// Other.
	private         List< IObserver >			m_Observers = new List< IObserver >();

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

		m_ActiveEnemies = new List< Enemy >();
		m_EnemyDict     = new Dictionary< int, Enemy >();

		// Cache the "Enemy" components of each enemy gameobject in a dictionary beforehand.
		for( int waveIdx = 0; waveIdx < m_Waves.Length; waveIdx++ )
		{
			WaveProperties wave = m_Waves[ waveIdx ];
			for( int enemyTypeIdx = 0; enemyTypeIdx < wave.m_EnemyTypes.Length; enemyTypeIdx++ )
			{
				WaveProperties.EnemyTypeWaveAttributes enemyType = wave.m_EnemyTypes[ enemyTypeIdx ];
				for( int enemyIdx = 0; enemyIdx < enemyType.m_EnemyCount; enemyIdx++ )
				{
					GameObject enemyGO = PoolsManager.Spawn( enemyType.m_EnemyPrefab, Vector3.zero, Quaternion.identity );
					int enemyID = enemyGO.GetInstanceID();
					m_EnemyDict.Add( enemyID, enemyGO.GetComponent< Enemy >() );
				}
			}
		}

		// Enemy components are cached, despawn enemy gameobjects for now.
		foreach( Enemy enemy in m_EnemyDict.Values )
		{
			PoolsManager.Despawn( enemy.gameObject );
		}

		// Send the first wave.
		Invoke( "SpawnNextWave", 0.0f );
	}
	private void OnDrawGizmosSelected()
	{
		// Spawnpoint Visualization.
		for( int waveIdx = 0 ; waveIdx < m_Waves.Length ; waveIdx++ )
		{
			WaveProperties wave = m_Waves[ waveIdx ];
			for( int enemyTypeIdx = 0 ; enemyTypeIdx < wave.m_EnemyTypes.Length ; enemyTypeIdx++ )
			{
				WaveProperties.EnemyTypeWaveAttributes enemyType = wave.m_EnemyTypes[ enemyTypeIdx ];
				if( waveIdx == m_WaveIdx )
				{
					Gizmos.color = Color.red;
				}
				else
				{
					Gizmos.color = Color.white;
				}
				Gizmos.DrawWireCube( enemyType.m_SpawnPosition + SPAWNPOINT_VIS_OFFSET, SPAWNPOINT_VIS_SIZE );
			}
		}
	}

	//// IObserver Interface ////
	void IObserver.OnNotify( int enemyID )
	{
		OnEnemyDied( enemyID );
	}

	//// IObservable Interface ////
	void IObservable.Subscribe( IObserver observer )
	{
		if( !m_Observers.Contains( observer ) )
		{
			m_Observers.Add( observer );
		}
	}
	void IObservable.Unsubscribe( IObserver observer )
	{
		if( m_Observers.Contains( observer ) )
		{
			m_Observers.Remove( observer );
		}
	}

	//// Other Methods ////
	private void OnEnemyDied( int enemyID )
	{
		PoolsManager.Despawn( m_EnemyDict[ enemyID ].gameObject );
		m_ActiveEnemies.Remove( m_EnemyDict[ enemyID ] );
		m_RemainingEnemyCount--;

		if( m_RemainingEnemyCount == 0 )
		{
			if( m_WaveIdx == m_Waves.Length - 1 ) // All enemies are dead. Notify someone!
			{
				Notify();
				enabled = false;
				return;
			}

			m_WaveIdx++;
			Invoke( "SpawnNextWave", m_Waves[ m_WaveIdx - 1 ].m_TimeToNextWave );
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
			// Spawn an enemy of this type each m_TimeBetweenSpawns seconds.
			GameObject enemyGO = PoolsManager.Spawn( enemyType.m_EnemyPrefab, enemyType.m_SpawnPosition, Quaternion.identity );
			m_ActiveEnemies.Add( m_EnemyDict[ enemyGO.GetInstanceID() ] );
			( m_EnemyDict[ enemyGO.GetInstanceID() ] as IObservable ).Subscribe( this );
			yield return new WaitForSeconds( enemyType.m_TimeBetweenSpawns );
		}
	}
	private void Notify()
	{
		foreach( IObserver observer in m_Observers )
		{
			observer.OnNotify( GetInstanceID() ); // ID of the WaveSpawner component, not the gameObject itself!.
		}

		m_Observers.Clear(); // No need for further notifications.
	}
}