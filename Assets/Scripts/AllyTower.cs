using UnityEngine;
using ObserverPattern;
using System.Collections.Generic;
using System.Collections;

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

	// Attributes.
	private         float		    m_AttackInterval;
	private         float			m_CooldownTimer;

	// Components.
	private         Animator        m_Animator;

	// Other.
	private			Enemy			m_Target;
	private         List< Enemy >   m_ActiveEnemies;

	//// Properties ////
	private			bool			TargetInRange
	{
		get { return Vector3.Distance( transform.position, m_Target.transform.position ) <= m_Range; }
	}
	private			bool			OnCooldown
	{
		get { return m_CooldownTimer > 0.0f; }
	}

	//// Unity Callbacks ////
	private void Start()
	{
		m_AttackInterval = 1.0f / m_AttacksPerSecond;
		m_CooldownTimer	 = 0.0f;

		m_Animator		 = GetComponent< Animator >();

		m_ActiveEnemies  = FindObjectOfType< WaveSpawner >().m_ActiveEnemies;

		// Start the "AcquireTarget/AttackWhenReady" cycle by checking for targets every tenth of a second.
		InvokeRepeating( "AcquireTarget", 0.0f, 0.1f );
	}
	private void Update()
	{
	}
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere( transform.position, m_Range );
	}

	//// IObserver Interface ////
	void IObserver.OnNotify( int param )
	{
		m_Target = null; // Let it go, let it go...
	}

	//// Other Methods ////
	// This method looks for new targets. When it finds one, it cancels its InvokeRepeating & fires a new one for
	// "AttackWhenReady" instead. 
	// "AttackWhenReady" does the same when its target is dead or out of range. Thus they form a cycle.
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
					( m_Target as IObservable ).Subscribe( this ); // Track the enemy to know when it dies.
					minDistance = distance;

					CancelInvoke( "AcquireTarget" ); // Stop looking for new targets.
					InvokeRepeating( "AttackWhenReady", 0.0f, 0.1f ); // Start attacking!
				}
			}
		}
	}
	private void Attack()
	{
		// Emit a bullet. Bullet will do the actual damage when it collides with the enemy.
		Quaternion orientation = Quaternion.LookRotation( ( m_Target.transform.position - transform.position ).normalized );
		GameObject bulletGO = PoolsManager.Spawn( m_BulletPrefab, transform.position, orientation );
		bulletGO.GetComponent< Bullet >().m_Target = m_Target;
	}
	// This method looks starts attacking the target. When the target is dead or out of range, it cancels its 
	// InvokeRepeating & fires a new one for "AcquireTarget" instead. 
	// "AcquireTarget" does the same when it finds a new target. Thus they form a cycle.
	private void AttackWhenReady()
	{
		if( m_Target == null || !TargetInRange )
		{
			CancelInvoke( "AttackWhenReady" ); // Stop attacking.
			InvokeRepeating( "AcquireTarget", 0.0f, 0.1f ); // Start checking for new targets every tenth of a second.
		}
		else if( !OnCooldown )
		{
			m_Animator.SetTrigger( "Attacking" );
			Attack();
			//m_Animator.SetBool( "Attacking", false );
			StartCoroutine( "Cooldown" );
		}
	}
	IEnumerator Cooldown()
	{
		m_CooldownTimer = m_AttackInterval;
		float deltaTime = m_CooldownTimer / 10.0f;
		while( OnCooldown )
		{
			yield return new WaitForSecondsRealtime( deltaTime ); // No need to do this every frame, do it only 10 times.
			m_CooldownTimer -= deltaTime;
		}
	}
}