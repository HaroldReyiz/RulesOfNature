using UnityEngine;

public class Bullet : MonoBehaviour 
{
	//// Fields ////
	[ Header( "Attributes" ) ]
	public  float       m_MoveSpeed;
	public  float       m_Damage;

	[ HideInInspector ]
	public  Enemy		m_Target;

	//// Unity Callbacks ////
	private void Start()
	{
		
	}
	private void Update()
	{
		if( m_Target == null )
		{
			Destroy( this );
			gameObject.SetActive( false );
			enabled = false;
			return;
		}

		if( Vector3.Distance( transform.position, m_Target.transform.position ) > 0.2f )
		{
			Vector3 dir = ( m_Target.transform.position - transform.position ).normalized;
			transform.position += dir * m_MoveSpeed * Time.deltaTime;
			transform.rotation = Quaternion.LookRotation( dir );
		}
		else
		{
			CollideWithTarget();
		}
	}

	//// Other methods ////
	private void CollideWithTarget()
	{
		m_Target.TakeDamage( m_Damage );
		Destroy( this );
		gameObject.SetActive( false );
		enabled = false;
	}
}