using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
//-------------------------------------------------------------------------------------------------
//--- Public Fields

public float moveSpeed = 2.0f;
public float rotateSpeed = 2.0f;
public float closestDistance = 1.0f;
public float outOfRangeDistance = 5.0f;

//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//+++ Private Fields

bool attackPlayer = false;

Transform model;

GameObject player;

//#################################################################################################
//### UnityEngine

void Start()
{
	// cache Player
	player = GameObject.FindGameObjectWithTag("Player");

	// Look at Player at Startup
	transform.LookAt(player.transform.position);

	// cache child to trigger animations
	model = transform.GetChild(0);
}


void Update()
{
	// Look at Player
//	transform.LookAt(player.transform.position);

	if(attackPlayer)
	{
		float distToPlayer = Vector3.Distance(transform.position, player.transform.position);

		if(distToPlayer >= outOfRangeDistance)
		{
			attackPlayer = false;
			model.animation.Stop();
			return;
		}

		// Look at Player
		transform.rotation = Quaternion.Slerp(
				transform.rotation,
				Quaternion.LookRotation(player.transform.position - transform.position),
				rotateSpeed * Time.deltaTime
		);

		//TODO: remove RigidBodies and test for Walls as with the player! RayCasts....
		//move towards player
		if(distToPlayer > closestDistance)
		{
			transform.position -= (transform.position - player.transform.position).normalized * moveSpeed * Time.deltaTime;
		}
	}

}


void OnTriggerEnter(Collider c)
{
	if(c.tag == "AttackRadius")
	{
		attackPlayer = true;
		model.animation.Play();
	}
//	Debug.Log("Trigger");
}

//****************************************************************************************************
//*** Functions


}
