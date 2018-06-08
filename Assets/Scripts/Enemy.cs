using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour 
{
	//// Fields ////
	[ Header( "Attributes" ) ]
	public	float			m_health = 100.0f;
	public  float			m_moveSpeed;

	[ Header( "Unity Setup" ) ]
	public  GameObject		m_goal;
	public  NavMeshAgent    m_Agent;

	//// Unity Callbacks ////
	private void Start()
	{
	}
	private void Update()
	{
		m_Agent.SetDestination( m_goal.transform.position );
	}

	//// Other Methods ////
	public void TakeDamage( float amount )
	{
		m_health -= amount;

		if( m_health <= 0.0f )
		{
			Die();
		}
	}
	private void Die()
	{
		Debug.Log( string.Format( "Enemy {0} died.", name ) );
		Destroy( this );
		gameObject.SetActive( false );
		enabled = false;
	}
}