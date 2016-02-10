using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldManager : MonoBehaviour {
	public float CHUNK_SIZE;
	public int CHUNK_RESOLUTION;
	public int LOADED_CHUNK_RADIUS;
	public Material TERRAIN_MATERIAL;

	public bool DO_DECORATE;
	public int MAX_DECORATIONS;
	private int numDecorations;
	public List<string> decorationPaths = new List<string>() {
		"Prefabs/Decoration_Saguaro"
	};
	public List<GameObject> decorations = new List<GameObject>();

	public float TIME_SCALE;
	public float LIGHT_X_SCALE;
	public float LIGHT_Y_SCALE;
	public float LIGHT_Z_SCALE;

	public GameObject player;

	private DynamicTerrain terrain;
	private GameObject sun;
	private GameObject moon;

	// Use this for initialization
	void Start () {
		numDecorations = 0;
		foreach (string path in decorationPaths) {
			LoadDecoration (path);
		}
		terrain = new DynamicTerrain (player, CHUNK_SIZE, CHUNK_RESOLUTION, TERRAIN_MATERIAL, LOADED_CHUNK_RADIUS);

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
		if (DO_DECORATE && numDecorations < MAX_DECORATIONS) {
			Decorate (terrain.RandomChunk(), decorations[Random.Range(0, decorations.Count)]);
		}
	}
		
	// Attempts to place a single decoration
	// May be called more than once
	void Decorate (Chunk chunk, GameObject decoration) {
		//Debug.Log("yee");
		Vector2 coordinate = new Vector2 (
			chunk.getCoordinate().x*CHUNK_SIZE+UnityEngine.Random.Range(-CHUNK_SIZE/2f, CHUNK_SIZE/2f),
			chunk.getCoordinate().y*CHUNK_SIZE+UnityEngine.Random.Range(-CHUNK_SIZE/2f, CHUNK_SIZE/2f)
		);
		if (Mathf.PerlinNoise (coordinate.x, coordinate.y) < decoration.GetComponent<Decoration>().density) {
			GameObject newDecoration = 
				(GameObject)Instantiate (decoration, new Vector3 (coordinate.x, 0f, coordinate.y), Quaternion.Euler (0f, 0f, 0f));
			newDecoration.GetComponent<Decoration>().Randomize();
			numDecorations++;
		}
	}

	void LoadDecoration (string path) {
		GameObject decoration = (GameObject) Resources.Load(path);
		if (decoration == null) {
			Debug.LogError ("Failed to load decoration at "+path);
		} else {
			Debug.Log("Loaded "+path);
			decorations.Add(decoration);
		}
	}

		
}
