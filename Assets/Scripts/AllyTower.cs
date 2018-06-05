using UnityEngine;

public class AllyTower : MonoBehaviour 
{
	//// Fields ////
	[ Header( "Attributes" ) ]
	public			string			m_TowerName;
	public          float		    m_AttacksPerSecond = 1.0f;
	public          float           m_Range;

	[ Header( "Unity Setup" ) ]
	public          GameObject      m_BulletPrefab;

	private         float		    m_AttackInterval;
	private			Enemy[]			m_Enemies;
	private			Enemy			m_Target;
	private         bool            m_IsAttacking;

	//// Unity Callbacks ////
	private void Start()
	{
		m_AttackInterval = 1.0f / m_AttacksPerSecond;

		m_Enemies = FindObjectsOfType< Enemy >();
	}
	private void Update()
	{
		if( m_Target == null )
		{
			CancelInvoke( "Attack" );
			m_IsAttacking = false;
			AcquireTarget();
		}
		else if( !m_IsAttacking )
		{
			InvokeRepeating( "Attack", 0.0f, m_AttackInterval );
			m_IsAttacking = true;
		}
	}
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere( transform.position, m_Range );
	}

	//// Other methods ////
	private void AcquireTarget()
	{
		float minDistance = float.PositiveInfinity;
		foreach( Enemy enemy in m_Enemies )
		{
			if( enemy != null )
			{
				float distance = Vector3.Distance( transform.position, enemy.transform.position );
				if( distance < minDistance && distance <= m_Range && enemy.gameObject.activeSelf )
				{
					m_Target = enemy;
					minDistance = distance;
				}
			}
		}
	}
	private void Attack()
	{
		Vector3 dir = m_Target.transform.position - transform.position;
		GameObject bulletGO = Instantiate( m_BulletPrefab, transform.position, Quaternion.LookRotation( dir ) );
		bulletGO.GetComponent< Bullet >().m_Target = m_Target;
	}
}