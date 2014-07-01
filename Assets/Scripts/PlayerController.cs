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


void Start()
{
//	cam = Camera.main;
}


void Update()
{
//	if(transform.position.y < 0.1f)
//	{
	transform.position = new Vector3(transform.position.x, 0.1f, transform.position.z);
//	rigidbody.velocity = new Vector3(0, 0, 0);
//	}

//	velocity = new Vector3(Input.GetAxis("Horizontal"), -9.81f, Input.GetAxis("Vertical"));
	velocity = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));

	velocity = Quaternion.Euler(0, 45, 0) * velocity;
	
//	velocity *= moveSpeed;
//	velocity *= Time.deltaTime;

	ClampVelocity();

	transform.position += velocity * moveSpeed * Time.deltaTime;

//	velocity *= moveSpeed;
//	velocity *= Time.deltaTime;

//	transform.position += Vector3.right * velocity.x * moveSpeed * Time.deltaTime;
//	transform.position += Vector3.forward * velocity.z * moveSpeed * Time.deltaTime;
//	transform.position += Vector3.right * velocity.x;
//	transform.position += Vector3.up * velocity.y;
//	transform.position += Vector3.forward * velocity.z;
}

/*
void FixedUpdate()
{
	Vector3 newVelocity = new Vector3(velocity.x * moveSpeed, 0, velocity.y * moveSpeed);
	newVelocity = Quaternion.Euler(0, 45, 0) * newVelocity;
//		newVelocity = Quaternion.Euler(0, cam.transform.rotation.y, 0) * newVelocity;
	rigidbody.AddForce(newVelocity);
}
*/
//#################################################################################################

void ClampVelocity()
{
	Vector3 newVelocity = velocity;

	float extraDistance = 0.00000001f;

	// vertical
	float closestHit = Mathf.Abs(velocity.z) + extraDistance;
	
	//TODO: fix sub-optimal ray-testing!!! more rays? 
	// HACK: maybe Euler(0,45,0) * Vector3.forward ???
	float sign = Mathf.Sign(velocity.z);
	Ray rayZ = new Ray(transform.position + sign * Vector3.forward * 0.125f, sign * Vector3.forward);
	RaycastHit hitZ = new RaycastHit();
		
	if(Physics.Raycast(rayZ, out hitZ, Mathf.Abs(velocity.z) + extraDistance))
	{
		closestHit = hitZ.distance;
	}

	newVelocity.z = Mathf.Clamp(velocity.z, -closestHit, closestHit);
	
//	Gizmos.DrawLine(transform.position + sign * Vector3.forward * 0.125f,
//			transform.position + sign * Vector3.forward * (0.125f + closestHit));
	

	// horizontal
	closestHit = Mathf.Abs(velocity.x) + extraDistance;
		
	sign = Mathf.Sign(velocity.x);
	Ray rayX = new Ray(transform.position + sign * Vector3.right * 0.125f, sign * Vector3.right);
	RaycastHit hitX = new RaycastHit();
		
	if(Physics.Raycast(rayX, out hitX, Mathf.Abs(velocity.x) + extraDistance))
	{
		closestHit = hitX.distance;
	}

	newVelocity.x = Mathf.Clamp(velocity.x, -closestHit, closestHit);

//	Gizmos.DrawLine(transform.position + sign * Vector3.right * 0.125f,
//		transform.position + sign * Vector3.right * (0.125f + closestHit));
/*
	// downwards
	//TODO: rewrite code to only check for downwards
	closestHit = Mathf.Abs(velocity.y);
		
	sign = Mathf.Sign(velocity.y);
	Ray rayY = new Ray(transform.position + sign * Vector3.up * 0.25f, sign * Vector3.up);
	RaycastHit hitY = new RaycastHit();
		
	if(Physics.Raycast(rayY, out hitY, Mathf.Abs(velocity.y) + 0.0001f))
	{
		closestHit = hitY.distance;
	}
		
	newVelocity.y = Mathf.Clamp(velocity.y, -closestHit, closestHit);

//	Gizmos.DrawLine(transform.position + sign * Vector3.right * 0.125f,
//		transform.position + sign * Vector3.right * (0.125f + closestHit));
*/
	velocity = newVelocity;
}


}
