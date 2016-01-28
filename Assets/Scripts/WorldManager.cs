using UnityEngine;
using System.Collections;

public class WorldManager : MonoBehaviour {
	public int SIZE;
	public int LINEAR_RESOLUTION;
	public Material TERRAIN_MATERIAL;

	private DynamicTerrain terrain;

	// Use this for initialization
	void Start () {
		terrain = new DynamicTerrain (SIZE, LINEAR_RESOLUTION, TERRAIN_MATERIAL);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
