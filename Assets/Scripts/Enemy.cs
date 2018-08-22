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
	private         MeshFilter			m_MeshFilter;
	private         MeshRenderer		m_MeshRenderer;
	private         Animator            m_Animator;

	// IObservable Related.	
	private         List< IObserver >	m_Observers = new List< IObserver >();

	//// Unity Callbacks ////
	private void Start()
	{
		m_Health         = m_StartHealth;
		m_AttackInterval = 1.0f / m_AttacksPerSecond;

		m_Goal           = GameObject.FindGameObjectWithTag( "Goal" );
		m_Agent.speed    = m_MoveSpeed;

		m_MeshFilter     = GetComponent< MeshFilter   >();
		m_MeshRenderer   = GetComponent< MeshRenderer >();
		m_Animator		 = GetComponent< Animator     >();
	}
	private void Update()
	{
		m_Agent.SetDestination( m_Goal.transform.position );

		// Check if we've reached the "goal" (human).
		if( !m_Agent.pathPending )
		{
			if( m_Agent.remainingDistance <= m_Agent.stoppingDistance )
			{
				if( !m_Agent.hasPath || m_Agent.velocity.sqrMagnitude == 0f )
				{
					if( !m_IsAttacking )
					{
						InvokeRepeating( "AttackHuman", 0.0f, m_AttackInterval );
						m_IsAttacking = true;
					}
				}
			}
		}
	}
	private void LateUpdate()
	{
		Animate();
	}
	private void OnEnable()
	{
		ResetAttributes();
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

	//// Combat Related ////
	public void TakeDamage( float amount )
	{
		if( !gameObject.activeSelf )
		{
			return;
		}

		m_Health -= amount;

		if( m_Health <= 0.0f )
		{
			Die();
		}
	}
	private void AttackHuman()
	{
		GameManager.INSTANCE.m_Human.TakeDamage( m_AttackDamage );
	}
	private void Die()
	{
		if( !gameObject.activeSelf )
		{
			return;
		}

		Debug.Log( string.Format( "Enemy {0} died.", name ) );
		Notify();
	}
	private void ResetAttributes() // Call this when an enemy is spawned from the pool.
	{
		m_Health = m_StartHealth;
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