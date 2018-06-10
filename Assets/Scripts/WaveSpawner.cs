﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ObserverPattern;
using System;

//// Singleton Class ////
public class WaveSpawner : MonoBehaviour, IObserver
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
	public static   WaveSpawner					INSTANCE;
	public			WaveProperties[]			m_Waves;
	public          List< Enemy >				m_ActiveEnemies;

	private         Dictionary< int, Enemy >	m_EnemyDict;
	private         int							m_RemainingEnemyCount = 0;
	private         int							m_WaveIdx = 0;

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
	// TODO: Visualize spawnpoints via OnDrawGizmoXXX callbacks.

	//// IObserver Interface ////
	void IObserver.OnNotify( int enemyID )
	{
		// Enemy died. Remove it from active enemies list.
		m_ActiveEnemies.Remove( m_EnemyDict[ enemyID ] );
	}

	//// Other Methods ////
	public void OnEnemyDied()
	{
		m_RemainingEnemyCount--;
		if( m_RemainingEnemyCount == 0 )
		{
			if( m_WaveIdx == m_Waves.Length - 1 )
			{
				Debug.Log( "Game over! You won!" );
				enabled = false;
				return;
			}

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
			GameObject enemyGO = PoolsManager.Spawn( enemyType.m_EnemyPrefab, enemyType.m_SpawnPosition, Quaternion.identity );
			m_ActiveEnemies.Add( m_EnemyDict[ enemyGO.GetInstanceID() ] );
			yield return new WaitForSeconds( 2.0f );
		}
	}
}