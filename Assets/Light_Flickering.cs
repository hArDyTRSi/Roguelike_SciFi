using UnityEngine;
using System.Collections;

public class Light_Flickering : MonoBehaviour
{
//-------------------------------------------------------------------------------------------------
//--- Public Fields

public float baseIntensity = 0.75f;
public float baseRange = 15.0f;
public float initialSpeed = 5.0f;

//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
//+++ Private Fields

private bool negative = true;

private float speed = 1.0f;
private float rangeModifier;
private float intensityModifier;
private float minIntensity;
private float maxIntensity;
private float destIntensity;

//#################################################################################################
//### UnityEngine

void Start()
{
	// calc everything the class needs to know about the intensity-values
	intensityModifier = baseIntensity / 5.0f;
	rangeModifier = baseRange / 5.0f;
	minIntensity = baseIntensity - intensityModifier;
	maxIntensity = baseIntensity + intensityModifier;

	// set start destination value
	destIntensity = 0.5f;
}


void Update()
{
	// sub/add intensity
	light.intensity = negative
			? light.intensity - speed * Time.deltaTime
			: light.intensity + speed * Time.deltaTime;

	// sub/add range
	light.range = negative
			? light.range - rangeModifier * speed * Time.deltaTime
			: light.range + rangeModifier * speed * Time.deltaTime;


	// check if destination-intensity is reached, if so calc new random values
	if(negative ? light.intensity < minIntensity : light.intensity > maxIntensity)
	{
		// calc new random value for range
//		destRange = Random.Range(baseRange - rangeModifier, baseRange + rangeModifier);

		// calc new random value for intensity
		destIntensity = Random.Range(baseIntensity - intensityModifier, baseIntensity + intensityModifier);

		// determine if new value is smaller or bigger than the actual one, set negative-flag accordingly
		negative = destIntensity < light.intensity ? true : false;
		
		// calc new random speed value
		speed = Random.Range(0.1f * initialSpeed, initialSpeed);
	}
}

//****************************************************************************************************
//*** Functions


}
