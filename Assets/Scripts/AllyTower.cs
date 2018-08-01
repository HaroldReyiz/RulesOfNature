using UnityEngine;
using ObserverPattern;
using System.Collections.Generic;

/// <summary>
/// Observes an enemy (current target) so that it can know when the enemy died (and got deactivated) and move on.
/// </summary>
public class AllyTower : MonoBehaviour, IObserver
{
	//// Fields ////
	[ Header( "Attributes" ) ]
	public			string			m_TowerName;
	public          float		    m_AttacksPerSecond = 1.0f;
	public          float           m_Range;

	[ Header( "Unity Setup" ) ]
	public          GameObject      m_BulletPrefab;

	private         float		    m_AttackInterval;
	private			Enemy			m_Target;
	private         bool            m_IsAttacking = false;
	private         List< Enemy >   m_ActiveEnemies;

	//// Unity Callbacks ////
	private void Start()
	{
		m_AttackInterval = 1.0f / m_AttacksPerSecond;
		m_ActiveEnemies = FindObjectOfType< WaveSpawner >().m_ActiveEnemies;
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

	//// IObserver Interface ////
	void IObserver.OnNotify( int param )
	{
		m_Target = null; // You got to let him go!
	}

	//// Other Methods ////
	private void AcquireTarget()
	{
		float minDistance = float.PositiveInfinity;
		foreach( Enemy enemy in m_ActiveEnemies )
		{
			if( enemy.isActiveAndEnabled )
			{
				float distance = Vector3.Distance( transform.position, enemy.transform.position );
				if( distance < minDistance && distance <= m_Range && enemy.gameObject.activeSelf )
				{
					m_Target = enemy;
					( m_Target as IObservable ).Subscribe( this );
					minDistance = distance;
				}
			}
		}
	}
	private void Attack()
	{
		if( m_Target == null )
		{
			return;
		}

		Quaternion orientation = Quaternion.LookRotation( ( m_Target.transform.position - transform.position ).normalized );
		GameObject bulletGO = PoolsManager.Spawn( m_BulletPrefab, transform.position, orientation );
		bulletGO.GetComponent< Bullet >().m_Target = m_Target;
	}
}