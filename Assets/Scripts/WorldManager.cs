using UnityEngine;
using UnityEngine.Rendering;
using System;
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
	private Dictionary<DecorationGroup, int> maxActive;
	[NonSerialized]
	public List<string> decorationPaths = new List<string>() {
		"Prefabs/Decoration_75MPH",
		"Prefabs/Decoration_Agave01",
		"Prefabs/Decoration_BarrelCactus",
		"Prefabs/Decoration_Boulder01",
		"Prefabs/Decoration_Boulder02",
		"Prefabs/Decoration_Boulder03",
		"Prefabs/Decoration_Chevron",
		"Prefabs/Decoration_JoshuaTree01",
		"Prefabs/Decoration_Saguaro"
	};
	public List<GameObject> decorations = new List<GameObject>();

	Vector2 roadDistance;

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

	[Range(0f, 2f*Mathf.PI)]
	public float timeOfDay;
	private float timeScale = 1;

	public Flare sunFlare;

	private Color primaryColor;
	private Color secondaryColor;

	public Color primaryDayColor;
	public Color secondaryDayColor;

	public Color primarySunsetColor;
	public Color secondarySunsetColor;

	public Color primaryNightColor;
	public Color secondaryNightColor;

	public Color primarySunriseColor;
	public Color secondarySunriseColor;

	public float maxSunIntensity;
	public float minSunIntensity;

	float startLoadTime;
    bool loaded = false;
	public bool loadedTerrain = false;
	//bool loadedDecorations = false;
	//bool decorated = false;

	public DecoGroupMaxSize[] initialMaxActive;

	// Use this for initialization
	void Start () {
		instance = this;
		numDecorations = 0;

		terrain = new DynamicTerrain (player, CHUNK_SIZE, CHUNK_RESOLUTION, TERRAIN_MATERIAL, LOADED_CHUNK_RADIUS, VERT_UPDATE_DISTANCE, VERT_HEIGHT_SCALE, SMOOTH_FACTOR);

		timeOfDay = UnityEngine.Random.Range(0, 2*Mathf.PI);
		sun = createSun();
		audioOut = Camera.main.GetComponent<AudioListener> ();
		freqDataArray = new float[FREQ_ARRAY_SIZE];

		createRoad ();
		if (ROAD_RADIUS == 0) {
			ROAD_RADIUS = 1000f;
		}
		road.GetComponent<Bezier> ().generateRoadRadius = ROAD_RADIUS;
		roadDistance  = new Vector2 (road.GetComponent<Bezier>().width * 1.1f, road.GetComponent<Bezier>().width * 1.2f);
		maxActive = new Dictionary<DecorationGroup, int>();
		foreach (DecoGroupMaxSize groupSize in initialMaxActive) {
			maxActive[groupSize.group] = groupSize.maxActive;
		}

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

	public void setTimeScale(float t = 1) {
		timeScale = t;
	}

	public void Load () {
		startLoadTime = Time.realtimeSinceStartup;
		GameManager.instance.ChangeLoadingMessage("Loading world...");
		terrain.update();

			
		if (DO_DECORATE) {
			LoadDecorations();
			InitialDecorate();
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
			terrain.update();
			updateTime();
			UpdateColor();
			AttemptDecorate();
		}

	}

	void UpdateColor() {
		//Light light = this.GetComponent<Light>();

		// Sunrise to noon
		if ((timeOfDay >= 0) && (timeOfDay < (Mathf.PI / 2))) {
			float lerpValue = timeOfDay / (Mathf.PI / 2);
			primaryColor = Color.Lerp (primarySunriseColor, primaryDayColor, lerpValue);
			secondaryColor = Color.Lerp (secondarySunriseColor, secondaryDayColor, lerpValue);
		// Noon to sunset
		} else if ((timeOfDay >= (Mathf.PI / 2)) && (timeOfDay < Mathf.PI)) {
			float lerpValue = (timeOfDay - Mathf.PI / 2) / (Mathf.PI / 2);
			primaryColor = Color.Lerp (primaryDayColor, primarySunsetColor, lerpValue);
			secondaryColor = Color.Lerp (secondaryDayColor, secondarySunsetColor, lerpValue);

		// Sunset to night
		} else if ((timeOfDay >= Mathf.PI) && (timeOfDay < ((3f/2f) * Mathf.PI))){
			float lerpValue = (timeOfDay - Mathf.PI) / (Mathf.PI / 2);
			primaryColor = Color.Lerp (primarySunsetColor, primaryNightColor, lerpValue);
			secondaryColor = Color.Lerp (secondarySunsetColor, secondaryNightColor, lerpValue);

		// Night to sunrise
		} else if ((timeOfDay >= ((3f/2f) * Mathf.PI)) && (timeOfDay < (2 * Mathf.PI))){
			float lerpValue = (timeOfDay - ((3f/2f) * Mathf.PI)) / (Mathf.PI / 2);
			primaryColor = Color.Lerp (primaryNightColor, primarySunriseColor, lerpValue);
			secondaryColor = Color.Lerp (secondaryNightColor, secondarySunriseColor, lerpValue);
		}

		sun.GetComponent<Light>().intensity = (timeOfDay >= 0f && timeOfDay <= Mathf.PI) ? maxSunIntensity : minSunIntensity;
		sun.GetComponent<Light>().color = primaryColor;
		RenderSettings.fogColor = secondaryColor;

		RenderSettings.skybox.SetFloat("_Value", Mathf.Clamp01(AngularDistance(timeOfDay,-Mathf.PI/2f)));
		if (Spectrum2.instance != null) {
			Color temp = new Color (sun.GetComponent<Light> ().color.r, sun.GetComponent<Light> ().color.g, sun.GetComponent<Light> ().color.b, Spectrum2.instance.opacity);
			Spectrum2.instance.GetComponent<LineRenderer> ().SetColors (temp, temp);
			Spectrum2.instance.GetComponent<LineRenderer> ().material.color = temp;
		}
	}

	private void updateTime() {
		timeOfDay += timeScale * Time.deltaTime;
		while (timeOfDay > (2 * Mathf.PI)) { //clamp timeOfDay between 0 and 2PI)
			timeOfDay -= 2 * Mathf.PI;
		}
	}

	float AngularDistance (float angle, float pos) {
		float d = angle - pos;
		while (d < -Mathf.PI)
			d += 2f * Mathf.PI;
		while (d > Mathf.PI)
			d -= 2f * Mathf.PI;
		return 1.0f-Mathf.Abs(5f*d/Mathf.PI/2.0f);
	}

	void InitialDecorate () {
		for (int i=0; i<MAX_DECORATIONS; i++) {
			AttemptDecorate ();
		}
	}
    
	void AttemptDecorate () {
		if (DO_DECORATE) {
			for (int i=0; i<DECORATIONS_PER_STEP && numDecorations < MAX_DECORATIONS; i++) {

				// Pick a random decoration and decorate with it
				GameObject decoration = decorations[UnityEngine.Random.Range(0, decorations.Count)];
				Decoration deco = decoration.GetComponent<Decoration>();
				if (Decoration.numDecorations == null) Decoration.numDecorations = new Dictionary<DecorationGroup, int>();
				if (!Decoration.numDecorations.ContainsKey(deco.group))
					Decoration.numDecorations.Add (deco.group, 0);
				if (Decoration.numDecorations[deco.group] < maxActive[deco.group]) {
					switch (deco.distribution) {
					case DecorationDistribution.Random:
						Chunk chunk = terrain.RandomChunk ();
						while (terrain.activeRoadChunksContains (chunk)) {
							chunk = terrain.RandomChunk ();
						}
						DecorateRandom (chunk, decoration);
						break;
					case DecorationDistribution.Roadside:
						float bezierProg = UnityEngine.Random.Range (PlayerMovement.instance.progress, 1f);
						DecorateRoadside (bezierProg, decoration);
						break;
					case DecorationDistribution.CloseToRoad:
						DecorateRandom (terrain.RandomCloseToRoadChunk(), decoration);
						break;
					}
					Decoration.numDecorations[deco.group]++;
				}
			}
		}
	}

	void UpdateFog () {
	}
		
    
	GameObject createSun(){
		GameObject sun = new GameObject ("Sun");
		sun.AddComponent<Light> ();
		sun.AddComponent<Sun> ();
		if (TIME_SCALE != 0) 
			setTimeScale (TIME_SCALE);
		sun.GetComponent<Sun> ().setPosScales (LIGHT_X_SCALE, LIGHT_Y_SCALE, LIGHT_Z_SCALE);
		sun.GetComponent<Light> ().shadows = LightShadows.Soft;
		Camera.main.GetComponent<SunShafts>().sunTransform = sun.transform;
		sun.GetComponent<Light>().flare = sunFlare;
		return sun;
	}

	void createRoad(){
		road = new GameObject ("Road");
		road.AddComponent<MeshFilter> ();
		//road.AddComponent<MeshCollider> ();
		road.AddComponent<MeshRenderer>();
		road.GetComponent<MeshRenderer>().material = roadMaterial;
		road.GetComponent<MeshRenderer>().reflectionProbeUsage = ReflectionProbeUsage.Off;
		road.AddComponent<Bezier> ();
		road.GetComponent<Bezier>().width = ROAD_WIDTH;
		road.GetComponent<Bezier>().height = ROAD_HEIGHT;
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

	void DecorateRoadside (float prog, GameObject decoration) {
		int side = UnityEngine.Random.Range (0, 2); // 0 = player side, 1 = other side
		Bezier roadBez = road.GetComponent<Bezier>();
		Vector3 coordinate = 
			roadBez.BezRight(roadBez.GetPoint(prog)) * UnityEngine.Random.Range (roadDistance.x, roadDistance.y) * (side == 0 ? 1 : -1);
		RaycastHit hit;
		if (Physics.Raycast(new Vector3 (coordinate.x, MAX_DECORATION_HEIGHT, coordinate.y), Vector3.down, out hit, Mathf.Infinity))
			coordinate.y = hit.point.y;
		else coordinate.y = 0f;
		GameObject newDecoration = 
			(GameObject)Instantiate (decoration, coordinate, Quaternion.Euler (0f, 0f, 0f));
		if (side == 0) newDecoration.transform.Rotate (new Vector3 (0f, 180f, 0f));
		newDecoration.GetComponent<Decoration>().Randomize();
		newDecoration.transform.parent = road.gameObject.transform;
		numDecorations++;
		roadBez.AddDecoration(newDecoration, prog);
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
