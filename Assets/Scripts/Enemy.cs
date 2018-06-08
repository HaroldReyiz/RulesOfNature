using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour 
{
	//// Fields ////
	[ Header( "Attributes" ) ]
	public			float			m_Health = 100.0f;
	public			float			m_MoveSpeed;

	[ Header( "Unity Setup" ) ]
	public static	GameObject		m_Goal;
	public			NavMeshAgent    m_Agent;

	//// Unity Callbacks ////
	private void Start()
	{
		m_Goal = GameObject.FindGameObjectWithTag( "Goal" );
	}
	private void Update()
	{
		m_Agent.SetDestination( m_Goal.transform.position );
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
		PoolManager.Despawn( gameObject );
		WaveSpawner.INSTANCE.OnEnemyDied();
	}
}