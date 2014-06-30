using UnityEditor;
using UnityEngine;
//using System.Collections;

[CustomEditor(typeof(GenerateFloors))]
public class GenerateFloorsInspector : Editor
{

//-------------------------------------------------------------------------------------------------

//#################################################################################################

public override void OnInspectorGUI()
{
	DrawDefaultInspector();

	if(GUILayout.Button("ReDraw"))
	{
		GenerateFloors gf = (GenerateFloors)target;
		gf.MakeLevel();
	}
	
	if(GUILayout.Button("Remove"))
	{
		GenerateFloors gf = (GenerateFloors)target;
		gf.RemoveLevel();
	}
}

//#################################################################################################

}
