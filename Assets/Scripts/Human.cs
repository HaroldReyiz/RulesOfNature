using System.Collections.Generic;
using ObserverPattern;
using UnityEngine;

/// <summary>
/// Is observed by the GameManager so that it can know when the human dies and end the game (game over).
/// </summary>
public class Human : MonoBehaviour, IObservable 
{
	//// Fields ////
	[ Header( "Attributes" ) ]
	public          float               m_StartHealth = 100.0f;

	[ Header( "Unity Setup" ) ]
	public          Material[]          m_Materials; // 0: Idle, 1: Run01, 2: Run02.

	// Attributes.
	private         float               m_Health;

	// Other.
	private         List< IObserver >   m_Observers = new List< IObserver >();

	//// Unity Callbacks ////
	private void Start()
	{
		m_Health = m_StartHealth;
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
		Debug.Log( "Human health left:" + m_Health );

		if( m_Health <= 0.0f )
		{
			Die();
		}
	}
	private void Die()
	{
		if( !gameObject.activeSelf )
		{
			return;
		}

		Debug.Log( string.Format( "Human died.", name ) );
		gameObject.SetActive( false );
		Notify();
	}
	private void Notify()
	{
		foreach( IObserver observer in m_Observers )
		{
			observer.OnNotify( GetInstanceID() ); // ID of the Human component, not the gameObject itself!.
		}

		m_Observers.Clear(); // No need for further notifications.
	}
}