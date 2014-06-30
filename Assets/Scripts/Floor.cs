using UnityEngine;
//using System.Collections;
using System.Collections.Generic;

public class Floor
{

public byte[,] blockMap;

public List<Room> rooms = new List<Room>();

//-------------------------------------------------------------------------------------------------

int sizeX = 3;
int sizeZ = 3;

int roomAmount = 100;


//#################################################################################################

public Floor(int x, int z)
{
	sizeX = x;
	sizeZ = z;
	blockMap = new byte[x, z];
	MakeFloor(x, z);
}

//#################################################################################################
	
void MakeFloor(int sizeX, int sizeZ)
{
	for(int x=0; x<sizeX; x++)
	{
		for(int z=0; z<sizeZ; z++)
		{
			blockMap[x, z] = 254;
		}
	}

	MakeRooms();
	MakeConnections();
	OutlineWithWalls();
//	CleanUp();
}

void MakeRooms()
{
	rooms.Add(new Room(sizeX, sizeZ));

	for(int i=0; i<roomAmount-1; i++)
	{
		Room room = new Room(sizeX, sizeZ);
		if(!IsOverlapping(room))
		{
			rooms.Add(room);
		}
	}

	foreach(Room r in rooms)
	{
		for(int x=0; x<r.sizeX; x++)
		{
			for(int z=0; z<r.sizeZ; z++)
			{
				int newX = x + r.offsetX;
				int newZ = z + r.offsetZ;

				bool pillar = false;
				// check for pillars, if pillar keep wall
				for(int p=0; p< r.pillarAmount; p++)
				{
					if(newX == r.pillarPositions[p, 0] && newZ == r.pillarPositions[p, 1])
					{
						pillar = true;
						break;
					}
				}
				if(!pillar)
				{
					blockMap[newX, newZ] = 255;
				}
			}
		}
	}
	//TODO: Add lights to room/scene

}

bool IsOverlapping(Room r)
{
	for(int i=0; i<rooms.Count; i++)
	{
		if(r.offsetX > rooms[i].offsetX + rooms[i].sizeX)
		{
			continue;
		}
		if(r.offsetX + r.sizeX < rooms[i].offsetX)
		{
			continue;
		}
		if(r.offsetZ > rooms[i].offsetZ + rooms[i].sizeZ)
		{
			continue;
		}
		if(r.offsetZ + r.sizeZ < rooms[i].offsetZ)
		{
			continue;
		}
		return true;
	}

	return false;
}

void MakeConnections()
{
	for(int i=0; i<rooms.Count; i++)
	{
		if(!rooms[i].isConnected)
		{
			int j = Random.Range(1, rooms.Count);
			
			MakeCorridor(rooms[i], rooms[(i + j) % rooms.Count]);
		}
	}
}

void MakeCorridor(Room r1, Room r2)
{
	r1.isConnected = true;
	r2.isConnected = true;

	int r1X = (int)(r1.offsetX + r1.sizeX / 2);
	int r1Z = (int)(r1.offsetZ + r1.sizeZ / 2);
	int r2CenterX = (int)(r2.offsetX + r2.sizeX / 2);
	int r2CenterZ = (int)(r2.offsetZ + r2.sizeZ / 2);

	while(r1X != r2CenterX)
	{
		blockMap[r1X, r1Z] = 255;
		r1X += r1X < r2CenterX ? 1 : -1;
	}
	while(r1Z != r2CenterZ)
	{
		blockMap[r1X, r1Z] = 255;
		r1Z += r1Z < r2CenterZ ? 1 : -1;
	}
}


void OutlineWithWalls()
{
	for(int x=0; x<sizeX; x++)
	{
		for(int z=0; z<sizeZ; z++)
		{
			if(blockMap[x, z] == 254 && (
				blockMap[Mathf.Clamp(x + 1, 0, sizeX - 1), Mathf.Clamp(z + 1, 0, sizeZ - 1)] == 255 ||
				blockMap[Mathf.Clamp(x + 1, 0, sizeX - 1), Mathf.Clamp(z - 1, 0, sizeZ - 1)] == 255 ||
				blockMap[Mathf.Clamp(x - 1, 0, sizeX - 1), Mathf.Clamp(z + 1, 0, sizeZ - 1)] == 255 ||
				blockMap[Mathf.Clamp(x - 1, 0, sizeX - 1), Mathf.Clamp(z - 1, 0, sizeZ - 1)] == 255 ||

				blockMap[Mathf.Clamp(x + 1, 0, sizeX - 1), Mathf.Clamp(z, 0, sizeZ - 1)] == 255 ||
				blockMap[Mathf.Clamp(x - 1, 0, sizeX - 1), Mathf.Clamp(z, 0, sizeZ - 1)] == 255 ||
				blockMap[Mathf.Clamp(x, 0, sizeX - 1), Mathf.Clamp(z + 1, 0, sizeZ - 1)] == 255 ||
				blockMap[Mathf.Clamp(x, 0, sizeX - 1), Mathf.Clamp(z - 1, 0, sizeZ - 1)] == 255)
				)
			{
				blockMap[x, z] = 0;
			}
		}
	}

}
/*
void CleanUp()
{
	for(int x=0; x<sizeX; x++)
	{
		for(int z=0; z<sizeZ; z++)
		{
			if(blockMap[x, z] == 254)
			{
				blockMap[x, z] = 255;
			}
		}
	}
}
*/



}
