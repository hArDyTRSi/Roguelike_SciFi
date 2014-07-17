//using UnityEditor;
using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class PlayerController : MonoBehaviour
{
//-------------------------------------------------------------------------------------------------
//--- Public Fields

//TODO: remove!
//public float xOff;
//public float xMul;

//public bool mapOpened = false;

public int moveSpeed = 100;
public int rotationSpeed = 100;

public GameObject mapDisplayPlayer;
public Color displayActiveColor;
public Color displayInactiveColor;

//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//+++ Private Fields

Vector3 velocity = Vector3.zero;

//Camera cam;

GameObject mapDisplay;
GameObject playerDisplay;
GUITexture playerDisplayGUITexture = null;
GameObject playerModel;

//GenerateFloors floor;

GameObject mapDisplayParent;


//#################################################################################################
//### UnityEngine

void Start()
{
//	cam = Camera.main;
	playerModel = GameObject.FindGameObjectWithTag("PlayerModel");

	// cache Map-Display
	mapDisplay = GameObject.FindGameObjectWithTag("MapDisplay");
	mapDisplay.SetActive(false);

	// cache Parent-GameObject (folder) for instantiated playerDisplay
	mapDisplayParent = GameObject.FindGameObjectWithTag("GUI");

	// Instantiate and cache mapDisplay of Player
//	playerDisplay = GameObject.FindGameObjectWithTag("PlayerDisplay");
	playerDisplay = Instantiate(mapDisplayPlayer) as GameObject;
	playerDisplay.transform.parent = mapDisplayParent.transform;
//	playerDisplay.SetActive(false);

	// cache GUITexture component from mobDisplay
	playerDisplayGUITexture = playerDisplay.GetComponent<GUITexture>();
	// set color to transparent (Minimap off at Startup)
	playerDisplayGUITexture.color = displayInactiveColor;


	// cache level
//	floor = GameObject.FindGameObjectWithTag("Level").GetComponent<GenerateFloors>();
}


void Update()
{
	// Map-Display
	if(Input.GetButtonDown("Map"))
	{
//		mapOpened = !mapOpened;

//		if(mapDisplay.activeSelf)
		if(Global.global.mapOpened)
//		if(mapOpened)
		{
			mapDisplay.SetActive(false);
//			playerDisplay.SetActive(false);
			playerDisplayGUITexture.color = displayInactiveColor;
			Global.global.mapOpened = false;
//			mapOpened = false;

		}
		else
		{
			mapDisplay.SetActive(true);
//			playerDisplay.SetActive(true);
			playerDisplayGUITexture.color = displayActiveColor;
			Global.global.mapOpened = true;
//			mapOpened = true;
		}
	}

	//TODO: check for errors in other ratios, strange numbers here, where do they come from?
	if(Global.global.mapOpened)
//	if(mapOpened)
	{
		// set player-position on Map
		playerDisplay.transform.position = new Vector3(
//			xOff + 0.5f + ((transform.position.x - Global.global.floorSizeX / 2.0f) / Global.global.floorSizeX) * xMul,
//			xOff + 0.5f + ((transform.position.z - Global.global.floorSizeZ / 2.0f) / Global.global.floorSizeZ) * (Camera.main.aspect * xMul),
			0.005f + 0.5f + ((transform.position.x - Global.global.floorSizeX / 2.0f) / Global.global.floorSizeX) * 0.5425f,
			0.005f + 0.5f + ((transform.position.z - Global.global.floorSizeZ / 2.0f) / Global.global.floorSizeZ) * (Camera.main.aspect * 0.5425f),
			0.0f);
	}


//-----------------------------------------------------------------------------------------------

	// PLAYER-Control

//	velocity = new Vector3(Input.GetAxis("Horizontal"), -9.81f, Input.GetAxis("Vertical"));
	velocity = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));

	velocity = Quaternion.Euler(0, 45, 0) * velocity;

	velocity *= moveSpeed;
	velocity *= Time.deltaTime;
	
	ClampVelocity();
	
	transform.position += velocity;

	// Rotation
	if(velocity != Vector3.zero)
	{
		playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, Quaternion.LookRotation(velocity), rotationSpeed * Time.deltaTime);
	}

}

/*
void FixedUpdate()
{
//	transform.position += velocity * moveSpeed * Time.deltaTime;
}
*/

//****************************************************************************************************
//*** Functions

void ClampVelocity()
{
	// set half-size of player-collider
	float playerRadius = 0.125f;
//	float playerRadius = player.collider.size.x;	// sth like that
	
	// set offset to the inside of the player to avoid overstepping
	float insideOffset = 0.1f;

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
