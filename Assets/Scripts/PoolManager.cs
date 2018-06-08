///
/// Simple pooling for Unity.
///   Author: Martin "quill18" Glaude (quill18@quill18.com)
///   Extended: Simon "Draugor" Wagner (https://www.twitter.com/Draugor_/)
///	  Modified slightly (mostly code reformatting) by Burak Canik for use on game "RulesOfNature".
///	  Also renamed PoolManager.
///   Latest Version: https://gist.github.com/quill18/5a7cfffae68892621267
///   License: CC0 (http://creativecommons.org/publicdomain/zero/1.0/)
///   UPDATES:
///     2018-01-04: - Added Extension Method for Despawn on GameObjects 
///                 - Changed the Member Lookup so it doesn't require a PoolMemberComponent anymore.
///                     - for that i added a HashSet which contains all PoolMemberIDs  (HashSet has O(1) contains operator)
///                     - PoolMemberIDs are just ints from GameObject.getInstanceID() which are unique for the GameObject 
///                        over the runtime of the game
///                 - Changed PoolDictionary from (Prefab, Pool) to (int, Pool) using Prefab.GetInstanceID
/// 	2015-04-16: Changed Pool to use a Stack generic.
/// 
/// Usage:
/// 
///   There's no need to do any special setup of any kind.
/// 
///   Instead of calling Instantiate(), use this:
///       SimplePool.Spawn(somePrefab, somePosition, someRotation);
/// 
///   Instead of destroying an object, use this:
///       SimplePool.Despawn(myGameObject);
///   or this:
///       myGameObject.Despawn();
/// 
///   If desired, you can preload the pool with a number of instances:
///       SimplePool.Preload(somePrefab, 20);
/// 
/// Remember that Awake and Start will only ever be called on the first instantiation
/// and that member variables won't be reset automatically.  You should reset your
/// object yourself after calling Spawn().  (i.e. You'll have to do things like set
/// the object's HPs to max, reset animation states, etc...)
/// 
/// 
/// 

using UnityEngine;
using System.Collections.Generic;

//// Singleton Class ////
public class PoolManager
{
	//// Fields ////
	// You can avoid resizing of the Stack's internal data by setting this to a number equal to or greater to what you expect 
	// most of your pool sizes to be. Note, you can also use Preload() to set the initial size of a pool.
	// This can be handy if only some of your pools are going to be exceptionally large (for example, your bullets.)
	private const	int							DEFAULT_POOL_SIZE = 3;
	private static	Dictionary< int, Pool >		m_Pools; // All of our pools

	//// Constructors ////
	/// <summary>
	/// The Pool class represents the pool for a particular prefab.
	/// </summary>
	class Pool
	{
		//// Fields ////
		public  readonly HashSet< int >			m_MemberIDs;

		private readonly Stack< GameObject >	m_InactiveStack;
		private readonly GameObject				m_PooledPrefab;  // The prefab that we are pooling

		//// Constructors ////
		public Pool( GameObject prefab, int initialQty )
		{
			m_MemberIDs     = new HashSet< int >();

			m_InactiveStack = new Stack< GameObject >( initialQty );
			m_PooledPrefab  = prefab;
		}

		//// Methods ////
		// Spawn an object from our pool
		public GameObject Spawn( Vector3 pos, Quaternion rot )
		{
			GameObject go;
			if( m_InactiveStack.Count == 0 ) // Pool empty, instantiate a new object.
			{
				go = GameObject.Instantiate( m_PooledPrefab, pos, rot );

				// Add this objects instance id to member IDs to keep track of it.
				m_MemberIDs.Add( go.GetInstanceID() );
			}
			else // Grab an existing object from the pool.
			{
				go = m_InactiveStack.Pop();

				if( go == null )
				{
					/* The inactive object we expected to find no longer exists. The most likely causes are:
					 * Someone calling Destroy() on our object
					 * A scene change (which will destroy all our objects).
					 * NOTE: This could be prevented with a DontDestroyOnLoad if you really don't want this.
					*/

					// No worries ,we'll just try the next one in our sequence. 
					return Spawn( pos, rot );
				}
			}

			go.transform.position = pos;
			go.transform.rotation = rot;
			go.SetActive( true );
			return go;
		}
		public void Despawn( GameObject obj ) // Return an object to the inactive pool.
		{
			obj.SetActive( false );

			m_InactiveStack.Push( obj );
		}
	}

	//// Unity Callbacks ////


	//// Other Methods ////
	/// <summary>
	/// Initialize our dictionary.
	/// </summary>
	private static void Initialize( GameObject prefab = null, int qty = DEFAULT_POOL_SIZE )
	{
		if( m_Pools == null )
		{
			m_Pools = new Dictionary< int, Pool >();
		}
		if( prefab != null )
		{
			int prefabID = prefab.GetInstanceID();
			if( !m_Pools.ContainsKey( prefabID ) )
			{
				m_Pools[ prefabID ] = new Pool( prefab, qty ); 
			}
		}
	}
	/// <summary>
	/// If you want to preload a few copies of an object at the start of a scene, you can use this. 
	/// Really not needed unless you're going to go from zero instances to 100+ very quickly.
	/// Could technically be optimized more, but in practice the Spawn/Despawn sequence is going to be pretty darn quick and
	/// this avoids code duplication.
	/// </summary>
	public static void Preload( GameObject prefab, int qty = 1 )
	{
		Initialize( prefab, qty );

		// Make an array to grab the objects we're about to pre-spawn.
		GameObject[] objects = new GameObject[ qty ];
		for( int objIdx = 0; objIdx < qty; objIdx++ )
		{
			objects[ objIdx ] = Spawn( prefab, Vector3.zero, Quaternion.identity );
		}

		// Now despawn them all.
		for( int objIdx = 0; objIdx < qty; objIdx++ )
		{
			Despawn( objects[ objIdx ] );
		}
	}
	/// <summary>
	/// Spawns a copy of the specified prefab (instantiating one if required).
	/// NOTE: Remember that Awake() or Start() will only run on the very first spawn and that member variables won't get reset.  
	/// OnEnable will run after spawning, but remember that toggling IsActive will also call that function.
	/// </summary>
	public static GameObject Spawn( GameObject prefab, Vector3 pos, Quaternion rot )
	{
		Initialize( prefab );

		return m_Pools[ prefab.GetInstanceID() ].Spawn( pos, rot );
	}

	/// <summary>
	/// Despawn the specified gameobject back into its pool.
	/// </summary>
	public static void Despawn( GameObject obj )
	{
		Pool poolToBeDespawned = null;

		foreach( Pool pool in m_Pools.Values )
		{
			if( pool.m_MemberIDs.Contains( obj.GetInstanceID() ) )
			{
				poolToBeDespawned = pool;
				break;
			}
		}

		if( poolToBeDespawned == null )
		{
			Debug.Log( "Object '" + obj.name + "' wasn't spawned from a pool. Destroying it instead." );
			GameObject.Destroy( obj );
		}
		else
		{
			poolToBeDespawned.Despawn( obj );
		}
	}
}