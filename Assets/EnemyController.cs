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

public GameObject mapDisplayMob;


//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//+++ Private Fields

bool attackPlayer = false;

float moveSpeed;

Transform model;

GameObject player;
PlayerController playerController;

GameObject mobDisplay;

GenerateFloors floor;

GameObject mapDisplayParent;

//#################################################################################################
//### UnityEngine

void Start()
{
	// cache Player
	player = GameObject.FindGameObjectWithTag("Player");

	playerController = player.GetComponent<PlayerController>();

	// Look at Player at Startup
//	transform.LookAt(player.transform.position);

	// cache child to trigger animations
	model = transform.GetChild(0);

	// set random Move-Speed
	moveSpeed = Random.Range(minSpeed, maxSpeed);

	// cache Parent-GameObject (folder) for instantiated mobDisplays
	mapDisplayParent = GameObject.FindGameObjectWithTag("GUI");

	// Instantiate and cache mapDisplay of Mob
//	mobDisplay = GameObject.FindGameObjectWithTag("PlayerDisplay");
	mobDisplay = Instantiate(mapDisplayMob) as GameObject;
	mobDisplay.transform.parent = mapDisplayParent.transform;
	mobDisplay.SetActive(false);

	// cache level
	floor = GameObject.FindGameObjectWithTag("Level").GetComponent<GenerateFloors>();


}


void Update()
{

	if(attackPlayer)
	{
		// get distance to Player
		float distToPlayer = Vector3.Distance(transform.position, player.transform.position);

		// stop moving if Player is too far away (feige Sau, da!)
		if(distToPlayer >= outOfRangeDistance)
		{
			attackPlayer = false;
			model.animation.Stop();

			// deactivate Mob-Display on Minimap
			mobDisplay.SetActive(false);

			return;
		}

		if(playerController.mapOpened)
		{
			// activate Mob-Display on Minimap
			mobDisplay.SetActive(true);

			//TODO: check for errors in other ratios, strange numbers here, where do they come from?
			// set mob-position on Map
			mobDisplay.transform.position = new Vector3(
				//	xOff + 0.5f + ((transform.position.x - floor.floorSizeX / 2.0f) / floor.floorSizeX) * xMul,
				//	xOff + 0.5f + ((transform.position.z - floor.floorSizeZ / 2.0f) / floor.floorSizeZ) * (Camera.main.aspect * xMul),
				0.005f + 0.5f + ((transform.position.x - floor.floorSizeX / 2.0f) / floor.floorSizeX) * 0.5425f,
				0.005f + 0.5f + ((transform.position.z - floor.floorSizeZ / 2.0f) / floor.floorSizeZ) * (Camera.main.aspect * 0.5425f),
				0.0f);
		}
		else
		{
			mobDisplay.SetActive(false);
		}



		// avoid rotation on z-axis
		Vector3 antiRotatePosition = new Vector3(transform.position.x, 0.0f, transform.position.z);
		Vector3 antiRotatePositionPlayer = new Vector3(player.transform.position.x, 0.0f, player.transform.position.z);
		// Look at Player
		transform.rotation = Quaternion.Slerp(
				transform.rotation,
//				Quaternion.LookRotation(player.transform.position - transform.position),
//				Quaternion.LookRotation(player.transform.position - antiRotatePosition),
				Quaternion.LookRotation(antiRotatePositionPlayer - antiRotatePosition),
				rotateSpeed * Time.deltaTime
		);

		//TODO: remove RigidBodies and test for Walls as with the player! RayCasts....
		//move towards player
		if(distToPlayer > closestDistance)
		{
			transform.position -= (transform.position - player.transform.position).normalized * moveSpeed * Time.deltaTime;
		}

		//HACK: keep model at the same height-level! (workaround for rigidbody-issues!)
		model.transform.position = new Vector3(model.transform.position.x, 0.0f, model.transform.position.z);

	}

}


void OnTriggerEnter(Collider c)
{
	if(c.tag == "AttackRadius")
	{
		attackPlayer = true;
		model.animation.Play();

		// activate Mob-Display on Minimap
//		mobDisplay.SetActive(true);
	}
}

//****************************************************************************************************
//*** Functions


}
