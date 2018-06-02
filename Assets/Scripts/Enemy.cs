using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour 
{
	//// Fields ////
	[ Header( "Attributes" ) ]
	public	float			m_health = 100.0f;
	public  float			m_moveSpeed;

	[ Header( "Unity Setup" ) ]
	public  GameObject		m_goal;
	public  NavMeshAgent    m_Agent;

	//// Unity Callbacks ////
	private void Start()
	{
	}
	private void Update()
	{
		m_Agent.SetDestination( m_goal.transform.position );
	}

	//// Other methods ////
}