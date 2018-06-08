using UnityEngine;

public class PoolInitializer : MonoBehaviour 
{
	//// Fields ////
	[System.Serializable ]
	public class PoolableObject
	{
		public  GameObject  m_PrefabToBePooled;
		public  int         m_StartingQuantity = 3;
	}

	public  PoolableObject[]  m_ObjectsToBePooled;

	//// Unity Callbacks ////
	private void Awake()
	{
		// Initialize each pool with given starting quantities.
		foreach( PoolableObject poolableObj in m_ObjectsToBePooled )
		{
			PoolManager.Preload( poolableObj.m_PrefabToBePooled, poolableObj.m_StartingQuantity );
		}
	}
	
	//// Other Methods ////
}