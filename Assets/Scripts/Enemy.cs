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

	// Attributes.
	private         float               m_Health;

	// Other.
	private         List< IObserver >	m_Observers;

	//// Unity Callbacks ////
	private void Start()
	{
		m_Health      = m_StartHealth;

		m_Goal        = GameObject.FindGameObjectWithTag( "Goal" );
		m_Agent.speed = m_MoveSpeed;
		m_Observers   = new List< IObserver >();
	}
	private void Update()
	{
		m_Agent.SetDestination( m_Goal.transform.position );
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
		Debug.Log( string.Format( "Enemy {0} died.", name ) );
		PoolsManager.Despawn( gameObject );
		WaveSpawner.INSTANCE.OnEnemyDied();
		Notify();
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