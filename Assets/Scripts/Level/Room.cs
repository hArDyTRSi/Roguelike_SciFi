using UnityEngine;
//using System.Collections;

public class Room
{
//-------------------------------------------------------------------------------------------------
//--- Public Fields

public int sizeX;
public int sizeZ;
public int offsetX;
public int offsetZ;

public int pillarAmount;
public int[,] pillarPositions;

//public int enemiesAmount;
//public int[,] enemiesPositions;

public bool isConnected = false;

public int lightsAmount;

//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//+++ Private Fields

int roomMinSizeX = 3;
int roomMinSizeZ = 3;
int roomMaxSizeX = 20;
int roomMaxSizeZ = 20;

//#################################################################################################
//### Constructor

public Room(int floorSizeX, int floorSizeZ)
{
	sizeX = Random.Range(roomMinSizeX, roomMaxSizeX);
	sizeZ = Random.Range(roomMinSizeZ, roomMaxSizeZ);
	offsetX = Random.Range(1, floorSizeX - sizeX);
	offsetZ = Random.Range(1, floorSizeZ - sizeZ);
	lightsAmount = Random.Range(0, 4);
	pillarAmount = Random.Range(0, (int)((sizeX * sizeZ) / 10));
//	enemiesAmount = Random.Range(0, (int)((sizeX * sizeZ) / 10));

	pillarPositions = new int[pillarAmount, 2];
	for(int p=0; p< pillarAmount; p++)
	{
		pillarPositions[p, 0] = Random.Range(offsetX + 1, offsetX + sizeX - 1);
		pillarPositions[p, 1] = Random.Range(offsetZ + 1, offsetZ + sizeZ - 1);
	}


//	MakeRoom();
}
/*
public Room(int x, int z, int offx, int offz, int l, int p)
{
	sizeX = x;
	sizeZ = z;
	offsetX = offx;
	offsetZ = offz;
	lightAmount = l;
	pillarAmount = p;

//	MakeRoom();
}
*/
//****************************************************************************************************
//*** Functions
/*
void MakeRoom()
{
	for(int x=0; x<sizeX; x++)
	{
		for(int z=0; z<sizeZ; z++)
		{

		}
	}
}
*/
}
