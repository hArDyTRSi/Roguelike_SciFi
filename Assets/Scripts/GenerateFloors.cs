using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenerateFloors : MonoBehaviour
{

public float TEST = 0.5f;

// TODO: make private
public int actualFloor = 1;
public GameObject player;
// 

public int floorSizeX = 64;
public int floorSizeZ = 64;

public GameObject tileBlock;
public GameObject groundBlock;

public Material[] materials;
public Material[] materialsGround;

//TODO: check if List has to be public
public List<GameObject> lights = new List<GameObject>();
public GameObject[] lightPrefabs;

//-------------------------------------------------------------------------------------------------

bool playerPositioned = false;
//GameObject player = null;
Floor activeFloor = null;

//#################################################################################################

void Awake()
{
	if(activeFloor != null)
	{
		RemoveLevel();
	}
}


void Start()
{
//	player = GameObject.FindGameObjectWithTag("Player");
	if(activeFloor != null)
	{
		MakeLevel();
	}
}
/*	
void Update()
{
	
}
*/
//#################################################################################################


public void RemoveLevel()
{
	RemoveLightSources();

	GameObject[] go = GameObject.FindGameObjectsWithTag("TileBlock");
	foreach(GameObject g in go)
	{
		DestroyImmediate(g);
	}
}


public void MakeLevel()
{
	RemoveLevel();

	activeFloor = new Floor(floorSizeX, floorSizeZ);

	// HACK: TEST!!!	
//	activeFloor.blockMap[Random.Range(0, floorSizeX - 1), Random.Range(0, floorSizeZ - 1)] = 1;

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


void MakeGround()
{
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
	playerPositioned = false;
	while(!playerPositioned)
	{
		int x = Random.Range(1, floorSizeX);
		int z = Random.Range(1, floorSizeZ);

		if(activeFloor.blockMap[x, z] == 255)
		{		
			player.transform.position = new Vector3(x, 0.1f, z);
//			player.rigidbody.velocity = new Vector3(0, 0, 0);
			playerPositioned = true;
		}
	}
}


void SetLightSources()
{
	foreach(Room r in activeFloor.rooms)
	{
//		int lightColor = Random.Range(0, lightPrefabs.Length);

		for(int p=0; p< r.lightsAmount; p++)
		{
			int xOrZ = Random.Range(0, 2);
			int lOrR = Random.Range(0, 2);

			int lightColor = Random.Range(0, lightPrefabs.Length);


			Vector3 lightPosition = 
			new Vector3(
				xOrZ == 0 ? Random.Range(r.offsetX + 0.5f, r.offsetX + r.sizeX - 0.5f) : lOrR == 0 ? r.offsetX - 0.5f : r.offsetX + r.sizeX - 0.5f,
				0.25f,
				xOrZ == 1 ? Random.Range(r.offsetZ + 0.5f, r.offsetZ + r.sizeZ - 0.5f) : lOrR == 0 ? r.offsetZ - 0.5f : r.offsetZ + r.sizeZ - 0.5f);

			GameObject l = Instantiate(lightPrefabs[lightColor],
				lightPosition,
				Quaternion.identity) as GameObject;
			lights.Add(l);
		}
	}
}

void RemoveLightSources()
{
	foreach(GameObject l in lights)
	{
		DestroyImmediate(l);
	}

	lights.Clear();
}
}
