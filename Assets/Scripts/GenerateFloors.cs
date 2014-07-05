using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//[ExecuteInEditMode]
public class GenerateFloors : MonoBehaviour
{
//-------------------------------------------------------------------------------------------------
//--- Public Fields

//public float TEST = 0.5f;

public bool keepEditorLevel = true;

// TODO: make private
public GameObject player;

public int floorSizeX = 64;
public int floorSizeZ = 64;

public GameObject tileBlock;
public GameObject groundBlock;

public Material[] materials;
public Material[] materialsGround;

public GameObject[] lightPrefabs;

//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//+++ Private Fields

bool playerPositioned = false;
//GameObject player = null;

Floor activeFloor = null;

public List<GameObject> lights = new List<GameObject>();

//#################################################################################################
//### UnityEngine
/*
void Awake()
{

}
*/

void Start()
{
//	player = GameObject.FindGameObjectWithTag("Player");
	
	// Make a new Floor (if no Editor-Test-Level generated already!)
	if(!keepEditorLevel)
	{
		MakeFloor();
	}
}

/*	
void Update()
{
	
}
*/
//****************************************************************************************************
//*** Functions

public void MakeFloor()
{
	// Remove last Floor (except theres an Editor-Test-Level generated already)
	RemoveFloor();

	// Instantiate a new Floor
	activeFloor = new Floor(floorSizeX, floorSizeZ);

	// Instantiate new Tile-Blocks based on new Floor-Data
	for(int x=0; x<floorSizeX; x++)
	{
		for(int z=0; z<floorSizeZ; z++)
		{
			byte tile = activeFloor.blockMap[x, z];
			if(tile < 254)
			{
				GameObject tb = Instantiate(tileBlock, new Vector3(x, 0, z), Quaternion.identity) as GameObject;
				tb.renderer.material = materials[tile];
			}
		}
	}

	MakeGround();

	SetPlayerPosition();

	SetLightSources();
}


public void RemoveFloor()
{
	// Remove all Lights
	RemoveLightSources();
		
	// Remove all Tile-Blocks
	GameObject[] go = GameObject.FindGameObjectsWithTag("TileBlock");
	foreach(GameObject g in go)
	{
		DestroyImmediate(g);
	}
		
	activeFloor = null;
}
	
	
void MakeGround()
{
	// Instantiate flat Tile-Blocks under Rooms based on new Floor-Data
	for(int x=0; x<floorSizeX; x++)
	{
		for(int z=0; z<floorSizeZ; z++)
		{
			byte tile = activeFloor.blockMap[x, z];
			if(tile == 255)
			{
				GameObject tb = Instantiate(groundBlock, new Vector3(x, -0.5f, z), Quaternion.identity) as GameObject;
				tb.renderer.material = materialsGround[0];
//				tb.renderer.material = materialsGround[(int)(actualFloor / 5)];
			}
		}
	}

}


void SetPlayerPosition()
{
	// Find a Spot to spawn and reposition Player once found one
	playerPositioned = false;
	while(!playerPositioned)
	{
		int x = Random.Range(1, floorSizeX);
		int z = Random.Range(1, floorSizeZ);

		if(activeFloor.blockMap[x, z] == 255)
		{		
			player.transform.position = new Vector3(x, 0.1f, z);
			playerPositioned = true;
		}
	}
}


void SetLightSources()
{
	// Instantiate Lights based on new Room-Data in new Floor-Data
	foreach(Room r in activeFloor.rooms)
	{
		// one color of lights per room
//		int lightColor = Random.Range(0, lightPrefabs.Length);

		for(int p=0; p< r.lightsAmount; p++)
		{
			int xOrZ = Random.Range(0, 2);
			int lOrR = Random.Range(0, 2);

			// every light has a random color
			int lightColor = Random.Range(0, lightPrefabs.Length);

			Vector3 lightPosition = 
			new Vector3(
				xOrZ == 0 ? Random.Range(r.offsetX + 0.6f, r.offsetX + r.sizeX - 0.6f) : lOrR == 0 ? r.offsetX - 0.4f : r.offsetX + r.sizeX - 0.6f,
				0.0f,
				xOrZ == 1 ? Random.Range(r.offsetZ + 0.6f, r.offsetZ + r.sizeZ - 0.6f) : lOrR == 0 ? r.offsetZ - 0.4f : r.offsetZ + r.sizeZ - 0.6f);

			GameObject l = Instantiate(lightPrefabs[lightColor],
				lightPosition,
				Quaternion.identity) as GameObject;
			
			// Add new Light to List of Lights
			lights.Add(l);
		}
	}
}

void RemoveLightSources()
{
	// Destroy all lights in List

	//TODO: FIX Light Removal !
/*	
	GameObject[] lights = GameObject.FindGameObjectsWithTag("FloorLight");

	foreach(GameObject l in lights)
	{
		Debug.Log(l.name);
		DestroyImmediate(l);
//		Destroy(l);
	}
*/
	foreach(GameObject l in lights)
	{
		DestroyImmediate(l);
	}
	
	// Delete all entries on List
	lights.Clear();

}

}
