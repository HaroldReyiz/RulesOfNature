using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class InitialNavMeshGeneration : MonoBehaviour 
{
	//// Fields ////
	public  NavMeshSurface				m_NavMeshSurface;

	private List< NavMeshBuildSource >  m_Sources;

	//// Unity Callbacks ////
	private void Start()
	{
		m_NavMeshSurface.BuildNavMesh();
	}

	//// Other Methods ////
}