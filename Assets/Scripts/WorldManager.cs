using UnityEngine;
using System.Collections;

public class WorldManager : MonoBehaviour {
	public int SIZE;
	public int LINEAR_RESOLUTION;
	public Material TERRAIN_MATERIAL;

	public float TIME_SCALE;
	public float LIGHT_X_SCALE = 200;
	public float LIGHT_Y_SCALE = 200;
	public float LIGHT_Z_SCALE = 200;

	private DynamicTerrain terrain;
	private GameObject sun;
	private GameObject moon;

	// Use this for initialization
	void Start () {
		terrain = new DynamicTerrain (SIZE, LINEAR_RESOLUTION, TERRAIN_MATERIAL);

		sun = new GameObject ("Sun");
		sun.AddComponent<Light> ();
		sun.AddComponent<Sun> ();
		if (TIME_SCALE != 0) 
			sun.GetComponent<Sun> ().setTimeScale (TIME_SCALE);
		sun.GetComponent<Sun> ().setPosScales (LIGHT_X_SCALE, LIGHT_Y_SCALE, LIGHT_Z_SCALE);
		sun.GetComponent<Light> ().shadows = LightShadows.Soft;

		//Do something else with the moon.  Not an orbiting directional light, maybe one
		//that is stationary.
		/*
		moon = new GameObject ("Moon");
		moon.AddComponent<Light> ();
		moon.AddComponent<Moon> ();
		if (TIME_SCALE != 0)
			moon.GetComponent<Moon> ().setTimeScale (TIME_SCALE);
		moon.GetComponent<Moon> ().setPosScales (LIGHT_X_SCALE, LIGHT_Y_SCALE, LIGHT_Z_SCALE);
		moon.GetComponent<Light> ().shadows = LightShadows.Soft;
		*/
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
