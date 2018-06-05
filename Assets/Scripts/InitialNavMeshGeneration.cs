using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class InitialNavMeshGeneration : MonoBehaviour 
{
	//// Fields ////
	public  NavMeshSurface				m_navMeshSurface;

	private List< NavMeshBuildSource >  m_Sources;

	//// Unity Callbacks ////
	private void Start()
	{
		m_navMeshSurface.BuildNavMesh();
	}

	//// Other methods ////
}