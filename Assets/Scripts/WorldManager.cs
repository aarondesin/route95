using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;

public class WorldManager : MonoBehaviour {
	public static WorldManager instance;
	public float CHUNK_SIZE; //derived from TERRAIN_SIZE/TERRAIN_RESOLUTION
	public int CHUNK_RESOLUTION; //number of vertices per side of chunk
	public int LOADED_CHUNK_RADIUS; //number of CHUNK_SIZEs a chunk can be to load or not be unloaded
	public float VERT_HEIGHT_SCALE; //scales the heights of the vertices from the LinInt data
	public float VERT_UPDATE_DISTANCE; //the distance at which vertices should update
	public float SMOOTH_FACTOR;
	public Material TERRAIN_MATERIAL; //material used for terrain
	public bool DO_RANDOM_HEIGHT_MAPS; //will deform terrain with random height maps
	public int FREQ_ARRAY_SIZE; //must be a power of 2
	public float ROAD_RADIUS; //radius in unity units in which to spawn road
	public float ROAD_WIDTH;
	public float ROAD_HEIGHT;
	public Material roadMaterial;

	public bool DO_DECORATE;
	public int MAX_DECORATIONS;
	public int DECORATIONS_PER_STEP;
    public float MAX_DECORATION_HEIGHT;
	public float MIN_DECORATION_HEIGHT;
	[SerializeField]
	private int numDecorations;
	public List<string> decorationPaths = new List<string>() {
		"Prefabs/Decoration_Saguaro",
        "Prefabs/Decoration_BarrelCactus"
	};
	public List<GameObject> decorations = new List<GameObject>();

	public float TIME_SCALE;
	public float LIGHT_X_SCALE;
	public float LIGHT_Y_SCALE;
	public float LIGHT_Z_SCALE;

	public GameObject player;
	public AudioListener audioOut;

	private DynamicTerrain terrain;
	private float[] freqDataArray;
	public GameObject road;
	private GameObject sun;
	private GameObject moon;

	float startLoadTime;
    bool loaded = false;
	//bool loadedDecorations = false;
	//bool decorated = false;

	// Use this for initialization
	void Start () {
		instance = this;
		numDecorations = 0;

		terrain = new DynamicTerrain (player, CHUNK_SIZE, CHUNK_RESOLUTION, TERRAIN_MATERIAL, LOADED_CHUNK_RADIUS, VERT_UPDATE_DISTANCE, VERT_HEIGHT_SCALE, SMOOTH_FACTOR);

		createSun();
		audioOut = Camera.main.GetComponent<AudioListener> ();
		freqDataArray = new float[FREQ_ARRAY_SIZE];

		createRoad ();
		if (ROAD_RADIUS == 0) {
			ROAD_RADIUS = 1000f;
		}
		road.GetComponent<Bezier> ().ROAD_RADIUS = ROAD_RADIUS;

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

	public void Load () {
		startLoadTime = Time.realtimeSinceStartup;
		GameManager.instance.ChangeLoadingMessage("Loading world...");
		terrain.update(freqDataArray);
		if (DO_DECORATE) {
			StartCoroutine("LoadDecorations");
			StartCoroutine("DoDecorate");
		}
        NotifyLoadingDone();
	}

	void LoadDecorations () {
		foreach (string path in decorationPaths) {
			LoadDecoration (path);
			GameManager.instance.IncrementLoadProgress();
		}
	}

	void DoDecorate () {
		while (numDecorations < MAX_DECORATIONS) {
			for (int i=0; i<DECORATIONS_PER_STEP && numDecorations < MAX_DECORATIONS; i++) {
					//Debug.Log("dick");
				AttemptDecorate ();
				GameManager.instance.IncrementLoadProgress();
				//yield return null;
			}
		}
	}

	void NotifyLoadingDone () {
			Debug.Log("WorldManager.Load(): finished in "+(Time.realtimeSinceStartup-startLoadTime).ToString("0.0000")+" seconds.");
			GameManager.instance.LoadNext();
		loaded = true;
	}

	// Update is called once per frame
	void Update () {
		if (loaded) {
			terrain.update(freqDataArray);
			AttemptDecorate();
		}

	}
    
	void AttemptDecorate () {
		if (DO_DECORATE) {
			for (int i=0; i<DECORATIONS_PER_STEP && numDecorations < MAX_DECORATIONS; i++) {

				// Pick a random decoration and decorate with it
				GameObject decoration = decorations[Random.Range(0, decorations.Count)];
				switch (decoration.GetComponent<Decoration>().distribution) {
				case DecorationDistribution.Random:
					Chunk chunk = terrain.RandomChunk ();
					while (terrain.activeRoadChunksContains (chunk)) {
						chunk = terrain.RandomChunk ();
					}
					DecorateRandom (chunk, decoration);
					break;
				case DecorationDistribution.Roadside:
					break;
				case DecorationDistribution.CloseToRoad:
					DecorateRandom (terrain.RandomCloseToRoadChunk(), decoration);
					break;



				}
			}
		}
	}
    
	void createSun(){
		GameObject sun = new GameObject ("Sun");
		sun.AddComponent<Light> ();
		sun.AddComponent<Sun> ();
		if (TIME_SCALE != 0) 
			sun.GetComponent<Sun> ().setTimeScale (TIME_SCALE);
		sun.GetComponent<Sun> ().setPosScales (LIGHT_X_SCALE, LIGHT_Y_SCALE, LIGHT_Z_SCALE);
		sun.GetComponent<Light> ().shadows = LightShadows.Soft;
		Camera.main.GetComponent<SunShafts>().sunTransform = sun.transform;
	}

	void createRoad(){
		road = new GameObject ("Road");
		road.AddComponent<MeshFilter> ();
		//road.AddComponent<MeshCollider> ();
		road.AddComponent<MeshRenderer>();
		road.GetComponent<MeshRenderer>().material = roadMaterial;
		road.AddComponent<Bezier> ();
		road.GetComponent<Bezier>().ROAD_WIDTH = ROAD_WIDTH;
		road.GetComponent<Bezier>().ROAD_HEIGHT = ROAD_HEIGHT;
	}

	void DecorateRandom (Chunk chunk, GameObject decoration) {
		Vector2 coordinate = new Vector2 (
			chunk.getCoordinate().x*CHUNK_SIZE+UnityEngine.Random.Range(-CHUNK_SIZE/2f, CHUNK_SIZE/2f),
			chunk.getCoordinate().y*CHUNK_SIZE+UnityEngine.Random.Range(-CHUNK_SIZE/2f, CHUNK_SIZE/2f)
		);
		if (Mathf.PerlinNoise (coordinate.x, coordinate.y) < decoration.GetComponent<Decoration>().density) {
			if (!chunk.Constrained (new Vector3 (coordinate.x, 0f, coordinate.y))) {
				RaycastHit hit;
				float y = 0f;
				if (Physics.Raycast(new Vector3 (coordinate.x, MAX_DECORATION_HEIGHT, coordinate.y), Vector3.down,out hit, Mathf.Infinity)) {
					//Debug.Log("bap");
					y = hit.point.y;
				}
					
				GameObject newDecoration = 
					(GameObject)Instantiate (decoration, new Vector3 (coordinate.x, y, coordinate.y), Quaternion.Euler (0f, 0f, 0f));
				newDecoration.GetComponent<Decoration>().Randomize();
				newDecoration.transform.parent = chunk.chunk.transform;
				numDecorations++;
			}
		}
	}

	void LoadDecoration (string path) {
		GameObject decoration = (GameObject) Resources.Load(path);
		if (decoration == null) {
			Debug.LogError ("Failed to load decoration at "+path);
		} else {
			//Debug.Log("Loaded "+path);
			decorations.Add(decoration);
			GameManager.instance.IncrementLoadProgress();
		}
	}

	public void DecNumDeco(int n) {
		numDecorations -= n;
	}

	public bool Constrained (Vector3 pos) {
		return (pos.x < 6f && pos.x > -6f);
	}
}
