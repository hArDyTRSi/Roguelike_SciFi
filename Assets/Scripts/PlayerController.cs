using UnityEngine;
using System.Collections;

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
	velocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
	
	//TODO: clip velocity*movespeed*time.deltatime
	// via rays shot in the direction of velocity x and z
	
	velocity = Quaternion.Euler(0, 45, 0) * velocity;
	transform.position += Vector3.right * velocity.x * moveSpeed * Time.deltaTime;
	transform.position += Vector3.forward * velocity.z * moveSpeed * Time.deltaTime;
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


}
