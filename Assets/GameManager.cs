using System;
using ObserverPattern;
using UnityEngine;

//// Singleton Class ////
/// <summary>
/// Observes:
///		- WaveSpawner (so that the GameManager can know when there are no more enemies and call GameOver()),
///		- Human (so that the GameManager can know when the human dies and call GameOver()).
/// </summary>
public class GameManager : MonoBehaviour, IObserver
{
	//// Fields ////
	[ HideInInspector ]
	public static   GameManager		INSTANCE;
	public          Human           m_Human;

	//// Unity Callbacks ////
	private void Start()
	{
		// Make sure only one instance is active at a time.
		if( INSTANCE == null )
		{
			INSTANCE = this;
		}
		else
		{
			Debug.Log( "More than one instance of the singleton class GameManager found!" );
		}

		( m_Human				as IObservable ).Subscribe( this );
		( WaveSpawner.INSTANCE	as IObservable ).Subscribe( this );
	}

	//// IObserver Interface ////
	void IObserver.OnNotify( int param )
	{
		if( param == m_Human.GetInstanceID() )
		{
			// If the notifying observable entity is the human, that means human died. So the player lost.
			GameOver( false );
		}
		else if( param == WaveSpawner.INSTANCE.GetInstanceID() )
		{
			// If the notifying observable entity is the WaveSpawner, that means all enemies died. So the player won.
			GameOver( true );
		}
	}

	//// Other Methods ////
	private void GameOver( bool playerWon )
	{
		if( playerWon )
		{
			Debug.Log( "Player won!" );
		}
		else
		{
			Debug.Log( "Player lost!" );
		}

		Time.timeScale = 0.0f;
	}
}