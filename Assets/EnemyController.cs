using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
//-------------------------------------------------------------------------------------------------
//--- Public Fields

public float minSpeed = 0.5f;
public float maxSpeed = 2.5f;
public float rotateSpeed = 2.0f;
public float closestDistance = 1.0f;
public float outOfRangeDistance = 5.0f;

//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//+++ Private Fields

bool attackPlayer = false;

float moveSpeed;

Transform model;

GameObject player;

//#################################################################################################
//### UnityEngine

void Start()
{
	// cache Player
	player = GameObject.FindGameObjectWithTag("Player");

	// Look at Player at Startup
//	transform.LookAt(player.transform.position);

	// cache child to trigger animations
	model = transform.GetChild(0);

	// set random Move-Speed
	moveSpeed = Random.Range(minSpeed, maxSpeed);
}


void Update()
{
	// keep model at the same height-level! (workaround for rigidbody-issues!)
	model.transform.position = new Vector3(model.transform.position.x, 0.0f, model.transform.position.z);


	if(attackPlayer)
	{
		float distToPlayer = Vector3.Distance(transform.position, player.transform.position);

		if(distToPlayer >= outOfRangeDistance)
		{
			attackPlayer = false;
			model.animation.Stop();
			return;
		}

		// avoid rotation on z-axis
		Vector3 antiRotatePosition = new Vector3(transform.position.x, 0.0f, transform.position.z);
		// Look at Player
		transform.rotation = Quaternion.Slerp(
				transform.rotation,
//				Quaternion.LookRotation(player.transform.position - transform.position),
				Quaternion.LookRotation(player.transform.position - antiRotatePosition),
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
//	Debug.Log(c.name);
	
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
