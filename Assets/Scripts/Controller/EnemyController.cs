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

public Color displayActiveColor;
public Color displayInactiveColor;

//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//+++ Private Fields

bool attackPlayer = false;

float moveSpeed;

Transform model;

GameObject player;
//PlayerController playerController;

bool mobDisplayActive = false;
GameObject mobDisplay;
GUITexture mobDisplayGUITexture = null;

//GenerateFloors floor;

GameObject mapDisplayParent;

//#################################################################################################
//### UnityEngine

void Start()
{
	// cache Player
	player = GameObject.FindGameObjectWithTag("Player");

	// cache PlayerController
//	playerController = player.GetComponent<PlayerController>();

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
//	mobDisplay.SetActive(false);

	// cache GUITexture component from mobDisplay
	mobDisplayGUITexture = mobDisplay.GetComponent<GUITexture>();
	// set color to transparent (Minimap off at Startup)
	mobDisplayGUITexture.color = displayInactiveColor;


	// cache level
//	floor = GameObject.FindGameObjectWithTag("Level").GetComponent<GenerateFloors>();

}


void Update()
{

	// ATTACK Mode
	if(attackPlayer)
	{
		// get distance to Player
		float distToPlayer = Vector3.Distance(transform.position, player.transform.position);

		// stop moving if Player is too far away (player escaped)
		if(distToPlayer >= outOfRangeDistance)
		{
			// deactivate Mob-Display on Minimap
			if(mobDisplayActive)
			{
//				mobDisplay.SetActive(false);
				mobDisplayGUITexture.color = displayInactiveColor;
				mobDisplayActive = false;
			}

			attackPlayer = false;
			model.animation.Stop();

			return;
		}

		if(Global.global.mapOpened)
//		if(playerController.mapOpened)
		{
			// activate Mob-Display on Minimap
			if(!mobDisplayActive)
			{
//				mobDisplay.SetActive(true);
				mobDisplayGUITexture.color = displayActiveColor;
				mobDisplayActive = true;
			}

			//TODO: check for errors in other ratios, strange numbers here, where do they come from?
			// set mob-position on Map
			mobDisplay.transform.position = new Vector3(
				//	xOff + 0.5f + ((transform.position.x - Global.global.floorSizeX / 2.0f) / Global.global.floorSizeX) * xMul,
				//	xOff + 0.5f + ((transform.position.z - Global.global.floorSizeZ / 2.0f) / Global.global.floorSizeZ) * (Camera.main.aspect * xMul),
				0.005f + 0.5f + ((transform.position.x - Global.global.floorSizeX / 2.0f) / Global.global.floorSizeX) * 0.5425f,
				0.005f + 0.5f + ((transform.position.z - Global.global.floorSizeZ / 2.0f) / Global.global.floorSizeZ) * (Camera.main.aspect * 0.5425f),
				0.0f);
		}
		else
		{
			// deactivate mobDisplay on MiniMap
			if(mobDisplayActive)
			{
//				mobDisplay.SetActive(false);
				mobDisplayGUITexture.color = displayInactiveColor;
				mobDisplayActive = false;
			}
		}


//-----------------------------------------------------------------------------------------------

		// MOB-Control

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

		//move towards player
		if(distToPlayer > closestDistance)
		{
			transform.position -= (transform.position - player.transform.position).normalized * moveSpeed * Time.deltaTime;
		}

		//TODO: remove RigidBodies and test for Walls as with the player! RayCasts....
		//HACK: keep model at the same height-level! (workaround for rigidbody-issues!)
		model.transform.position = new Vector3(model.transform.position.x, 0.0f, model.transform.position.z);

	}

}


void OnTriggerEnter(Collider collider)
{
	if(collider.tag == "AttackRadius")
	{
		attackPlayer = true;
		model.animation.Play();
	}
}

//****************************************************************************************************
//*** Functions


}
