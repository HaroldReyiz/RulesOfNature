using UnityEngine;
using UnityEngine.AI;
using ObserverPattern;
using System.Collections.Generic;

/// <summary>
/// Is observed by:
///		- AllyTower (to let the AllyTower know when the enemy dies and gets deactivated),
///		- WaveSpawner (so that it can keep track of ALL enemies and let GameManager know when there are no more enemies left).
/// </summary>
public class Enemy : MonoBehaviour, IObservable
{
	//// Fields ////
	[ Header( "Attributes" ) ]
	public			float               m_StartHealth      = 100.0f;
	public			float				m_MoveSpeed        = 3.5f;
	public          float               m_AttackRange      = 5.0f;
	public          float               m_AttackDamage     = 10.0f;
	public          float				m_AttacksPerSecond = 1.0f;

	[ Header( "Unity Setup" ) ]
	public static	GameObject			m_Goal;
	public			NavMeshAgent		m_Agent;

	// Attributes.
	private         float               m_Health;
	private         bool                m_IsAttacking = false;
	private         float               m_AttackInterval;

	// Components.
	private         Animator            m_Animator;

	// Other.
	private         bool                m_SpawnedForTheFirstTime = true;

	// IObservable Related.	
	private         List< IObserver >	m_Observers = new List< IObserver >();

	//// Unity Callbacks ////
	private void Start()
	{
		m_Animator		 = GetComponent< Animator >();

		m_Goal           = GameObject.FindGameObjectWithTag( "Goal" );

		m_Agent.SetDestination( m_Goal.transform.position );
		InvokeRepeating( "AttackWhenReady", 0.0f, 0.1f ); // Check if we reached the human (and start attacking) every 0.1 sec.

		m_SpawnedForTheFirstTime = false;
	}
	private void Update()
	{
	}
	private void LateUpdate()
	{
		Animate();
	}
	private void OnEnable()
	{
		ResetAttributes(); // Health, movespeed etc.

		if( !m_SpawnedForTheFirstTime )
		{
			m_Agent.SetDestination( m_Goal.transform.position );
			// Check if we reached the human (and start attacking) every 0.1 sec.
			InvokeRepeating( "AttackWhenReady", 0.0f, 0.1f );
		}
	}
	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere( transform.position, m_AttackRange);
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
	private void Notify()
	{
		foreach( IObserver observer in m_Observers )
		{
			observer.OnNotify( gameObject.GetInstanceID() ); // ID of the gameObject itself.
		}

		m_Observers.Clear(); // No need for further notifications.
	}

	//// Combat ////
	public void TakeDamage( float amount )
	{
		m_Health -= amount;

		if( m_Health <= 0.0f && m_Health + amount > 0.0f ) // Second check is there to make sure the enemy only dies once.
		{
			Die();
		}
	}
	private void AttackHuman()
	{
		GameManager.INSTANCE.m_Human.TakeDamage( m_AttackDamage );
	}
	private void AttackWhenReady()
	{
		// Check if we've reached the "goal" (human).
		if( !m_IsAttacking )
		{
			if( !m_Agent.pathPending )
			{
				if( m_Agent.remainingDistance <= m_Agent.stoppingDistance )
				{
					if( !m_Agent.hasPath || m_Agent.velocity.sqrMagnitude == 0f )
					{
						InvokeRepeating( "AttackHuman", 0.0f, m_AttackInterval );
						m_IsAttacking = true;
					}
				}
			}
		}
	}
	private void Die()
	{
		Debug.Log( string.Format( "Enemy {0} died.", name ) );
		Notify();
		CancelInvoke();
	}
	private void ResetAttributes() // Call this when an enemy is spawned/respawned from the pool.
	{
		m_Health         = m_StartHealth;
		m_AttackInterval = 1.0f / m_AttacksPerSecond;

		m_Agent.speed    = m_MoveSpeed;
	}

	//// Animation ////
	private void Animate()
	{
		if( m_Agent.velocity.sqrMagnitude > 0.005f ) // Run animation.
		{
			m_Animator.SetBool( "Running", true );
		}
		else // Idle animation.
		{
			m_Animator.SetBool( "Running", false );
		}
	}
}