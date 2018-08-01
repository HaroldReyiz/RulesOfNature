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
	public          Mesh[]              m_Meshes; // 0: Idle, 1: Run01, 2: Run02.
	public          Material[]          m_Materials; // 0: Idle, 1: Run01, 2: Run02.
	public			float				m_AnimFrequency = 0.2f;

	// Attributes.
	private         float               m_Health;
	private         bool                m_IsAttacking = false;
	private         float               m_AttackInterval;

	// Other.
	private         MeshFilter			m_MeshFilter;
	private         MeshRenderer		m_MeshRenderer;
	private         float               m_AnimTimer;
	private         int                 m_AnimIdx = 0; // 0: Idle, 1: Run01, 2: Run02.
	private         List< IObserver >	m_Observers = new List< IObserver >();

	//// Unity Callbacks ////
	private void Start()
	{
		m_Health         = m_StartHealth;
		m_AttackInterval = 1.0f / m_AttacksPerSecond;

		m_AnimTimer      = m_AnimFrequency;

		m_Goal           = GameObject.FindGameObjectWithTag( "Goal" );
		m_Agent.speed    = m_MoveSpeed;
		m_MeshFilter     = GetComponent< MeshFilter >();
		m_MeshRenderer   = GetComponent< MeshRenderer >();
	}
	private void Update()
	{
		m_Agent.SetDestination( m_Goal.transform.position );

		// Check if we've reached the destination (human).
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

		if( m_Agent.velocity.sqrMagnitude > 0.005f ) // Walking animation.
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
		else if( m_AnimIdx != 0 ) // Idle animation.
		{
			m_AnimIdx = 0;
			UpdateAnimation();
		}
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

	//// Other Methods ////
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
	private void UpdateAnimation()
	{
		m_MeshFilter.mesh       = m_Meshes[ m_AnimIdx ];
		m_MeshRenderer.material = m_Materials[ m_AnimIdx ];
	}
	private void Notify()
	{
		foreach( IObserver observer in m_Observers )
		{
			observer.OnNotify( gameObject.GetInstanceID() ); // ID of the gameObject itself.
		}

		m_Observers.Clear(); // No need for further notifications.
	}
	private void ResetAttributes() // Call this when an enemy is spawned from the pool.
	{
		m_Health = m_StartHealth;
	}
}