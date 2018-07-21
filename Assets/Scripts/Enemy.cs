using UnityEngine;
using UnityEngine.AI;
using ObserverPattern;
using System.Collections.Generic;

public class Enemy : MonoBehaviour, IObservable
{
	//// Fields ////
	[ Header( "Attributes" ) ]
	public			float               m_StartHealth = 100.0f;
	public			float				m_MoveSpeed = 3.5f;

	[ Header( "Unity Setup" ) ]
	public static	GameObject			m_Goal;
	public			NavMeshAgent		m_Agent;
	public          Mesh[]              m_Meshes; // 0: Idle, 1: Run01, 2: Run02.
	public          Material[]          m_Materials; // 0: Idle, 1: Run01, 2: Run02.
	public			float				m_AnimFrequency = 0.2f;

	// Attributes.
	private         float               m_Health;

	// Other.
	private         MeshFilter			m_MeshFilter;
	private         MeshRenderer		m_MeshRenderer;
	private         float               m_AnimTimer;
	private         int                 m_AnimIdx = 0; // 0: Idle, 1: Run01, 2: Run02.
	private         List< IObserver >	m_Observers;

	//// Unity Callbacks ////
	private void Start()
	{
		m_Health       = m_StartHealth;

		m_AnimTimer	   = m_AnimFrequency;

		m_Goal         = GameObject.FindGameObjectWithTag( "Goal" );
		m_Agent.speed  = m_MoveSpeed;
		m_MeshFilter   = GetComponent< MeshFilter >();
		m_MeshRenderer = GetComponent< MeshRenderer >();
		m_Observers    = new List< IObserver >();
	}
	private void Update()
	{
		m_Agent.SetDestination( m_Goal.transform.position );
		if( m_Agent.velocity.sqrMagnitude > 0.005f )
		{
			if( m_AnimTimer <= 0.0f )
			{
				m_AnimIdx = ( m_AnimIdx + 1 ) % 3;
				UpdateAnimation();
				m_AnimTimer = m_AnimFrequency;
			}
			else
			{
				m_AnimTimer -= Time.deltaTime;
			}
		}
		else if( m_AnimIdx != 0 )
		{
			m_AnimIdx = 0;
			UpdateAnimation();
		}
	}
	private void OnEnable()
	{
		ResetAttributes();
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
	public void TakeDamage( float amount )
	{
		m_Health -= amount;

		if( m_Health <= 0.0f )
		{
			Die();
		}
	}
	private void Die()
	{
		if( !gameObject.activeSelf )
		{
			return;
		}

		Debug.Log( string.Format( "Enemy {0} died.", name ) );
		PoolsManager.Despawn( gameObject );
		WaveSpawner.INSTANCE.OnEnemyDied();
		Notify();
	}
	private void UpdateAnimation()
	{
		m_MeshFilter.mesh       = m_Meshes[ m_AnimIdx ];
		m_MeshRenderer.material = m_Materials[ m_AnimIdx ];
	}
	private void Notify()
	{
		foreach( IObserver observer in m_Observers )
		{
			observer.OnNotify( GetInstanceID() );
		}

		m_Observers.Clear(); // No need for further notifications.
	}
	private void ResetAttributes() // Call this when an enemy is spawned from the pool.
	{
		m_Health = m_StartHealth;
	}
}