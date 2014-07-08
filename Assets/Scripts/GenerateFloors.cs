//using UnityEditor;
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

//public List<GameObject> lights = new List<GameObject>();
List<GameObject> lights = new List<GameObject>();

GameObject mapDisplay;

//#################################################################################################
//### UnityEngine
/*
void Awake()
{

}
*/

void Start()
{
	//cache Player	
//	player = GameObject.FindGameObjectWithTag("Player");

	// cache Map-Display
//	mapDisplay = GameObject.FindGameObjectWithTag("MapDisplay").GetComponent<GUITexture>();
	mapDisplay = GameObject.FindGameObjectWithTag("MapDisplay");

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

				// group as child into "LEVEL"-GameObject
				tb.transform.parent = this.transform;
			}
		}
	}

	MakeGround();

	SetPlayerPosition();

	SetLightSources();

	MakeMap();
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

				// group as child into "LEVEL"-GameObject
				tb.transform.parent = this.transform;
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

			// group as child into "LEVEL"-GameObject
			l.transform.parent = this.transform;

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


void MakeMap()
{
	// make new color-array
	Color[] colors = new Color[floorSizeX * floorSizeZ];
	
	// fill color-array according to map-data
	for(int x=0; x<floorSizeX; x++)
	{
		for(int z=0; z<floorSizeZ; z++)
		{
			byte pixel = activeFloor.blockMap[x, z];
			
			colors[x + z * floorSizeX] =
						pixel == 255 ? new Color(0.2f, 0.2f, 0.2f, 0.5f) :
						pixel == 254 ? new Color(0.0f, 0.0f, 0.0f, 0.0f) :
						new Color(1.0f, 1.0f, 1.0f, 0.5f);
		}
	}

	// make new Texture and set its pixels
	Texture2D newMap = new Texture2D(floorSizeX, floorSizeZ);
	newMap.filterMode = FilterMode.Point;
	newMap.SetPixels(colors);
	newMap.Apply();

	// set GUI-Texture
//	mapDisplay.SetActive(true);
	mapDisplay.guiTexture.pixelInset = new Rect(floorSizeX / 2, floorSizeZ / 2, floorSizeX, floorSizeZ);
	mapDisplay.guiTexture.texture = newMap;
	
	// rescale and reposition according to Screen.resolution
	mapDisplay.transform.position = new Vector3(0.5f, ((float)Screen.width / (float)Screen.height) * 0.25f, 0.0f);
	mapDisplay.transform.localScale = new Vector3(0.5f, ((float)Screen.width / (float)Screen.height) * 0.5f, 0.0f);

	// disabled as default
	mapDisplay.SetActive(false);
}
}
