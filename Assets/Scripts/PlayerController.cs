//using UnityEditor;
using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class PlayerController : MonoBehaviour
{

public int moveSpeed = 100;

//-------------------------------------------------------------------------------------------------

Vector3 velocity = Vector3.zero;

//Camera cam;

//#################################################################################################

/*
void Start()
{
//	cam = Camera.main;
}
*/

void Update()
{
//	velocity = new Vector3(Input.GetAxis("Horizontal"), -9.81f, Input.GetAxis("Vertical"));
	velocity = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));

	velocity = Quaternion.Euler(0, 45, 0) * velocity;

	velocity *= moveSpeed;
	velocity *= Time.deltaTime;

	ClampVelocity();
	
	transform.position += velocity;
}

/*
void FixedUpdate()
{
//	transform.position += velocity * moveSpeed * Time.deltaTime;
}
*/

//#################################################################################################

void ClampVelocity()
{
	// set half-size of player-collider
	float playerRadius = 0.125f;
//	float playerRadius = player.collider.size.x;	// sth like that
	
	// set offset to the inside of the player to avoid overstepping
	float insideOffset = 0.05f;

	// set Rays per Direction
	int rayCount = 5;

	// vertical
	float closestHit = Mathf.Abs(velocity.z) + insideOffset;
	float sign = Mathf.Sign(velocity.z);

	for(int z = -rayCount+1; z < rayCount; z++)
	{
		Ray rayZ = new Ray(
			transform.position + sign * Vector3.forward * (playerRadius - insideOffset) + Vector3.right * (z * playerRadius) / rayCount,
			sign * Vector3.forward);
		RaycastHit hitZ = new RaycastHit();
		
		if(Physics.Raycast(rayZ, out hitZ, Mathf.Abs(velocity.z) + insideOffset))
		{
//			if(hitZ.distance - insideOffset < closestHit)
			if(hitZ.distance < closestHit)
			{
//				closestHit = hitZ.distance - insideOffset;
				closestHit = hitZ.distance;
			}
		}


		Debug.DrawLine(
		transform.position + sign * Vector3.forward * (playerRadius - insideOffset) + Vector3.right * (z * playerRadius) / rayCount,
		transform.position + sign * Vector3.forward * (playerRadius + closestHit - insideOffset) + Vector3.right * (z * playerRadius) / rayCount,
		Color.magenta);
	}

	if(Mathf.Abs(velocity.z) > 0.0f)
	{
		velocity.z = sign * (closestHit - insideOffset);
	}

//-----------------------------------------------------------------------------------------------

	// horizontal
	closestHit = Mathf.Abs(velocity.x) + insideOffset;
	sign = Mathf.Sign(velocity.x);

	for(int x = -rayCount+1; x < rayCount; x++)
	{
		Ray rayX = new Ray(
			transform.position + sign * Vector3.right * (playerRadius - insideOffset) + Vector3.forward * (x * playerRadius) / rayCount,
			sign * Vector3.right);
		RaycastHit hitX = new RaycastHit();
		
		if(Physics.Raycast(rayX, out hitX, Mathf.Abs(velocity.x) + insideOffset))
		{
			if(hitX.distance < closestHit)
			{
				closestHit = hitX.distance;
			}
		}

		Debug.DrawLine(
			transform.position + sign * Vector3.right * (playerRadius - insideOffset) + Vector3.forward * (x * playerRadius) / rayCount,
				transform.position + sign * Vector3.right * (playerRadius + closestHit - insideOffset) + Vector3.forward * (x * playerRadius) / rayCount,
			Color.cyan);
	}

	if(Mathf.Abs(velocity.x) > 0.0f)
	{
		velocity.x = sign * (closestHit - insideOffset);
	}
}


}
