using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour 
{
	public	float	health = 100.0f;
	public  float   moveSpeed;

	private void Start()
	{
	}

	private void Update()
	{
		// TODO: Follow the path determined by pathfinding algorithm here.
	}

	public void TakeDamage( float amount )
	{
		health -= amount;

		if( health <= 0.0f )
		{
			Die();
		}
	}

	private void Die()
	{
		Destroy( gameObject );
	}
}