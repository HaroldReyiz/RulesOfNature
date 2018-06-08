using UnityEngine;

public class PoolInitializer : MonoBehaviour 
{
	//// Fields ////
	[ System.Serializable ]
	public class PoolableObject
	{
		public  GameObject  m_PrefabToBePooled;
		public  int         m_StartingQuantity = 3;
	}

	public  PoolableObject[]  m_ObjectsToBePooled;

	//// Unity Callbacks ////
	private void Start()
	{
		// Initialize each pool with given starting quantities.
		foreach( PoolableObject poolableObj in m_ObjectsToBePooled )
		{
			PoolManager.Preload( poolableObj.m_PrefabToBePooled, poolableObj.m_StartingQuantity );
		}

		// Get enemy types and quantities from WaveSpawner and preload them.
		for( int waveIdx = 0; waveIdx < WaveSpawner.INSTANCE.m_Waves.Length; waveIdx++ )
		{
			WaveSpawner.WaveProperties wave = WaveSpawner.INSTANCE.m_Waves[ waveIdx ];
			for( int enemyTypeIdx = 0; enemyTypeIdx < wave.m_EnemyTypes.Length; enemyTypeIdx++ )
			{
				WaveSpawner.WaveProperties.EnemyTypeWaveAttributes enemyType = wave.m_EnemyTypes[ enemyTypeIdx ];
				PoolManager.Preload( enemyType.m_EnemyPrefab, enemyType.m_EnemyCount );
			}
		}
	}
	
	//// Other Methods ////
}