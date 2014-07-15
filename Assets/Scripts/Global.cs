using UnityEngine;
using System.Collections;

public class Global : MonoBehaviour
{
//-------------------------------------------------------------------------------------------------
//--- Public Fields

public static Global global;


public bool mapOpened;

public int floorSizeX = 64;
public int floorSizeZ = 64;


//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//+++ Private Fields


//#################################################################################################
//### Constructor

//Global()
void Awake()
{
	global = this;

	mapOpened = false;
}

//****************************************************************************************************
//*** Functions


}
