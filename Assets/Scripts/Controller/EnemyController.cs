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

Vector3 velocity = Vector3.zero;

Transform model;

GameObject player;
//PlayerController playerController;

bool mobDisplayActive = false;
GameObject mobDisplay;
GUITexture mobDisplayGUITexture = null;

//GenerateFloors floor;

GameObject mapDisplayParent;

float mobRadius = 1.0f;
float insideOffset = 0.1f;


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



	// set half-size of mob-collider
//	mobRadius = collider.transform.localScale.x / 2.0f;
	mobRadius = gameObject.GetComponent<BoxCollider>().size.x / 2.0f;

	// set offset to the inside of the player to avoid overstepping
//	insideOffset = collider.transform.localScale.x / 4.0f;
	insideOffset = gameObject.GetComponent<BoxCollider>().size.x / 4.0f;

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
			velocity = -(transform.position - player.transform.position).normalized * moveSpeed * Time.deltaTime;
				
			ClampVelocity();

//			transform.position -= (transform.position - player.transform.position).normalized * moveSpeed * Time.deltaTime;
			transform.position += velocity;
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

void ClampVelocity()
{
	// set minimum offset to objects
	float minDist = 0.001f;

	// set Rays per Direction
	int rayCount = 5;
		
	if(Mathf.Abs(velocity.z) > 0.0f)
	{
		// vertical
		float closestHit = Mathf.Abs(velocity.z) + insideOffset;
		float sign = Mathf.Sign(velocity.z);
			
		for(int z = -rayCount+1; z < rayCount; z++)
		{
			Ray rayZ = new Ray(
					transform.position + sign * Vector3.forward * (mobRadius - insideOffset) + Vector3.right * (z * mobRadius) / rayCount,
					sign * Vector3.forward);
			RaycastHit hitZ = new RaycastHit();
				
			if(Physics.Raycast(rayZ, out hitZ, Mathf.Abs(velocity.z) + insideOffset))
			{
//			if(hitZ.distance - insideOffset < closestHit)
				if(hitZ.distance < closestHit)
				{
					if(hitZ.collider.tag == "AttackRadius")
					{
						continue;
					}


//				closestHit = hitZ.distance - insideOffset;
					closestHit = hitZ.distance;
				}
			}
				
			/*
		Debug.DrawLine(
		transform.position + sign * Vector3.forward * (playerRadius - insideOffset) + Vector3.right * (z * playerRadius) / rayCount,
		transform.position + sign * Vector3.forward * (playerRadius + closestHit - insideOffset) + Vector3.right * (z * playerRadius) / rayCount,
		Color.magenta);
*/
		}
			
			
		velocity.z = Mathf.Clamp(sign * (closestHit - insideOffset), -closestHit + minDist, closestHit - minDist);
		//		velocity.z = sign * Mathf.Clamp(closestHit - insideOffset, 0.0f, closestHit - minDist);
		//		velocity.z = sign * (closestHit - insideOffset);
		//		if(sign * closestHit < minDist)
		//		{
		//			velocity.z -= sign * minDist;
		//		}
			
	}
		
	//-----------------------------------------------------------------------------------------------
		
	if(Mathf.Abs(velocity.x) > 0.0f)
	{
		// horizontal
		float closestHit = Mathf.Abs(velocity.x) + insideOffset;
		float sign = Mathf.Sign(velocity.x);
			
		for(int x = -rayCount+1; x < rayCount; x++)
		{
			Ray rayX = new Ray(
					transform.position + sign * Vector3.right * (mobRadius - insideOffset) + Vector3.forward * (x * mobRadius) / rayCount,
					sign * Vector3.right);
			RaycastHit hitX = new RaycastHit();
				
			if(Physics.Raycast(rayX, out hitX, Mathf.Abs(velocity.x) + insideOffset))
			{
				if(hitX.collider.tag == "AttackRadius")
				{
					continue;
				}

				if(hitX.distance < closestHit)
				{
					closestHit = hitX.distance;
				}
			}
			/*
			Debug.DrawLine(
			transform.position + sign * Vector3.right * (playerRadius - insideOffset) + Vector3.forward * (x * playerRadius) / rayCount,
				transform.position + sign * Vector3.right * (playerRadius + closestHit - insideOffset) + Vector3.forward * (x * playerRadius) / rayCount,
			Color.cyan);
*/
		}
			
		velocity.x = Mathf.Clamp(sign * (closestHit - insideOffset), -closestHit + minDist, closestHit - minDist);
		//		velocity.x = sign * Mathf.Clamp(closestHit - insideOffset, 0.0f, closestHit - minDist);
		//		velocity.x = sign * (closestHit - insideOffset);
		//		if(closestHit < minDist)
		//		{
		//			velocity.x -= sign * minDist;
		//		}
			
	}
}

}
