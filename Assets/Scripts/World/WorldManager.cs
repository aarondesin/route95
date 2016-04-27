using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;

public class WorldManager : MonoBehaviour {
	public static WorldManager instance;

	#region WorldManager Defaults

	// Chunk vars
	const float DEFAULT_CHUNK_SIZE = 100;
	const int DEFAULT_CHUNK_RESOLUTION = 8;
	const int DEFAULT_CHUNK_LOAD_RADIUS = 8;

	// Terrain vars
	const float DEFAULT_HEIGHT_SCALE = 800f;
	const float DEFAULT_VERTEX_UPDATE_DISTANCE = 600f;

	// Decoration vars
	const int DEFAULT_MAX_DECORATIONS = 1000;
	const int DEFAULT_DECORATIONS_PER_STEP = 100;

	// Road vars
	const float DEFAULT_ROAD_WIDTH = 10f;
	const float DEFAULT_ROAD_HEIGHT = 0.2f;
	const float DEFAULT_ROAD_EXTEND_RADIUS = 1000f;

	// Day/night cycle vars
	const float DEFAULT_TIME_SCALE = 0.01f;

	static Color DEFAULT_PRIMARY_DAY_COLOR       = new Color (1.00f, 1.00f, 1.00f, 1.00f);
	static Color DEFAULT_SECONDARY_DAY_COLOR     = new Color (0.54f, 0.72f, 0.87f, 1.00f);

	static Color DEFAULT_PRIMARY_SUNSET_COLOR    = new Color (1.00f, 1.00f, 0.00f, 1.00f);
	static Color DEFAULT_SECONDARY_SUNSET_COLOR  = new Color (1.00f, 0.50f, 0.00f, 1.00f);

	static Color DEFAULT_PRIMARY_NIGHT_COLOR     = new Color (0.03f, 0.05f, 0.08f, 1.00f);
	static Color DEFAULT_SECONDARY_NIGHT_COLOR   = new Color (0.06f, 0.07f, 0.18f, 1.00f);

	static Color DEFAULT_PRIMARY_SUNRISE_COLOR   = new Color (1.00f, 1.00f, 0.00f, 1.00f);
	static Color DEFAULT_SECONDARY_SUNRISE_COLOR = new Color (1.00f, 0.50f, 0.00f, 1.00f);

	// Performance vars
	const int DEFAULT_CHUNK_UPDATES_PER_CYCLE = 4;
	const int DEFAULT_FREQ_ARRAY_SIZE = 256;
	const float DEFAULT_ROAD_PATH_CHECK_RESOLUTION = 4f;

	#endregion
	#region WorldManager Vars

	//
	[Header("Chunk Settings")]
	//

	[Tooltip("The length of one side of a chunk.")]
	[Range(50f, 200f)]
	public float chunkSize = DEFAULT_CHUNK_SIZE;

	[Tooltip("The number of vertices along one side of a chunk.")]
	[Range(2, 16)]
	public int chunkResolution = DEFAULT_CHUNK_RESOLUTION;

	[Tooltip("The radius around the player within which to draw chunks.")]
	[Range(2, 16)]
	public int chunkLoadRadius = DEFAULT_CHUNK_LOAD_RADIUS;


	//
	[Header("Terrain Settings")]
	//

	[Tooltip("Height scale of generated terrain.")]
	[Range(100f, 1000f)]
	public float heightScale = DEFAULT_HEIGHT_SCALE;

	[Tooltip("The distance from the player at which vertices should update.")]
	[Range(100f, 1000f)]
	public float vertexUpdateDistance = DEFAULT_VERTEX_UPDATE_DISTANCE;

	[Tooltip("Current wind vector.")]
	public Vector3 wind;

	[Tooltip("Material to use for terrain.")]
	public Material terrainMaterial;

	private DynamicTerrain terrain;


	//
	[Header("Decoration Settings")]
	//

	[Tooltip("Enable/disable decoration.")]
	public bool doDecorate = true;

	[Tooltip("Maxmimum number of decorations.")]
	public int maxDecorations = DEFAULT_MAX_DECORATIONS;

	[Tooltip("Individual maximums for each decoration group.")]
	public Decoration.GroupInfo[] initialMaxActive;

	private int numDecorations;
	private Dictionary<Decoration.Group, int> maxActive;
	[NonSerialized]
	public List<GameObject> decorations = new List<GameObject>();
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
		"Prefabs/Decoration_Saguaro",
		"Prefabs/DynamicDecoration_Tumbleweed01"
	};


	//
	[Header("Road Settings")]
	//

	[NonSerialized]
	public Road road;

	[Tooltip("Width of generated road.")]
	[Range(1f, 20f)]
	public float roadWidth = DEFAULT_ROAD_HEIGHT;

	[Tooltip("Height of generated road.")]
	[Range(0.1f, 0.5f)]
	public float roadHeight = DEFAULT_ROAD_HEIGHT;

	[Tooltip("Radius within which to extend road.")]
	[Range(100f, 2000f)]
	public float roadExtendRadius = DEFAULT_ROAD_EXTEND_RADIUS;

	[Tooltip("Material to use for road.")]
	public Material roadMaterial;


	//
	[Header("Day/Night Cycle Settings")]
	//

	[Tooltip("Global time scale for day/night cycle.")]
	[Range(0.001f, 0.1f)]
	public float timeScale = DEFAULT_TIME_SCALE;

	[Tooltip("Current time of day.")]
	[Range(0f, 2f*Mathf.PI)]
	public float timeOfDay;

	private GameObject sun;

	[Tooltip("Daytime intensity of the sun.")]
	[Range(0.1f, 1.5f)]
	public float maxSunIntensity;

	[Tooltip("Flare texture to use for the sun.")]
	public Flare sunFlare;

	private GameObject moon;

	[Tooltip("Nighttime intensity of the moon.")]
	[Range(0.1f, 1.5f)]
	public float maxMoonIntensity;

	[Tooltip("Sprites to randomize for the moon.")]
	public List<Sprite> moonSprites;

	private Color primaryColor;
	private Color secondaryColor;

	public Color primaryDayColor       = DEFAULT_PRIMARY_DAY_COLOR;
	public Color secondaryDayColor     = DEFAULT_SECONDARY_DAY_COLOR;
	public Color primarySunsetColor    = DEFAULT_PRIMARY_SUNSET_COLOR;
	public Color secondarySunsetColor  = DEFAULT_SECONDARY_SUNSET_COLOR;
	public Color primaryNightColor     = DEFAULT_PRIMARY_NIGHT_COLOR;
	public Color secondaryNightColor   = DEFAULT_SECONDARY_NIGHT_COLOR;
	public Color primarySunriseColor   = DEFAULT_PRIMARY_SUNRISE_COLOR;
	public Color secondarySunriseColor = DEFAULT_SECONDARY_SUNRISE_COLOR;


	//
	[Header("Performance Settings")]
	//

	[Tooltip("Maximum number of chunk updates per cycle.")]
	[Range(1,16)]
	public int chunkUpdatesPerCycle = DEFAULT_CHUNK_UPDATES_PER_CYCLE;

	[Tooltip("Resolution used in frequency spectrum analysis. Must be a power of 2.")]
	public int freqArraySize = DEFAULT_FREQ_ARRAY_SIZE;

	private float[] freqDataArray;

	[Tooltip("Maximum number of decorations to place per update cycle.")]
	[Range(10, 200)]
	public int decorationsPerStep = DEFAULT_DECORATIONS_PER_STEP;

	[Tooltip("The accuracy used in road distance checks.")]
	[Range(1f, 100f)]
	public float roadPathCheckResolution = DEFAULT_ROAD_PATH_CHECK_RESOLUTION;

	float startLoadTime;
	bool loaded = false;
	public bool loadedTerrain = false;
	bool hasRandomized = false;



	#endregion
	#region Unity Callbacks

	// Use this for initialization
	void Start () {
		instance = this;



		freqDataArray = new float[freqArraySize];

		terrain = new DynamicTerrain ();
		wind = UnityEngine.Random.insideUnitSphere;

		road = CreateRoad ();

		numDecorations = 0;
		maxActive = new Dictionary<Decoration.Group, int>();
		foreach (Decoration.GroupInfo groupSize in initialMaxActive) {
			maxActive[groupSize.group] = groupSize.maxActive;
		}

		timeOfDay = UnityEngine.Random.Range(0, 2*Mathf.PI);
		sun = CreateSun();
		moon = CreateMoon();
	}

	// Update is called once per frame
	void Update () {
		if (loadedTerrain && !hasRandomized) 
			Load();
		if (loaded) {

			terrain.Update(freqDataArray);
			UpdateTime();
			UpdateColor();
			AttemptDecorate();
		}

	}

	#endregion

	public void Load () {
		startLoadTime = Time.realtimeSinceStartup;
		GameManager.instance.ChangeLoadingMessage("Loading world...");
		//terrain.update();

			
		if (doDecorate) {
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
		while (numDecorations < maxDecorations) {
			for (int i=0; i<decorationsPerStep && numDecorations < maxDecorations; i++) {
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
		hasRandomized = true;
	}
		
	void UpdateColor() {
		//Light light = this.GetComponent<Light>();

		// Sunrise to noon
		if ((timeOfDay >= 0) && (timeOfDay < (Mathf.PI / 2))) {
			Camera.main.GetComponent<SunShafts>().sunTransform = sun.transform;
			float lerpValue = timeOfDay / (Mathf.PI / 2);
			primaryColor = Color.Lerp (primarySunriseColor, primaryDayColor, lerpValue);
			secondaryColor = Color.Lerp (secondarySunriseColor, secondaryDayColor, lerpValue);
		// Noon to sunset
		} else if ((timeOfDay >= (Mathf.PI / 2)) && (timeOfDay < Mathf.PI)) {
			Camera.main.GetComponent<SunShafts>().sunTransform = sun.transform;
			float lerpValue = (timeOfDay - Mathf.PI / 2) / (Mathf.PI / 2);
			primaryColor = Color.Lerp (primaryDayColor, primarySunsetColor, lerpValue);
			secondaryColor = Color.Lerp (secondaryDayColor, secondarySunsetColor, lerpValue);

		// Sunset to night
		} else if ((timeOfDay >= Mathf.PI) && (timeOfDay < ((3f/2f) * Mathf.PI))){
			Camera.main.GetComponent<SunShafts>().sunTransform = moon.transform;
			float lerpValue = (timeOfDay - Mathf.PI) / (Mathf.PI / 2);
			primaryColor = Color.Lerp (primarySunsetColor, primaryNightColor, lerpValue);
			secondaryColor = Color.Lerp (secondarySunsetColor, secondaryNightColor, lerpValue);

		// Night to sunrise
		} else if ((timeOfDay >= ((3f/2f) * Mathf.PI)) && (timeOfDay < (2 * Mathf.PI))){
			Camera.main.GetComponent<SunShafts>().sunTransform = moon.transform;
			float lerpValue = (timeOfDay - ((3f/2f) * Mathf.PI)) / (Mathf.PI / 2);
			primaryColor = Color.Lerp (primaryNightColor, primarySunriseColor, lerpValue);
			secondaryColor = Color.Lerp (secondaryNightColor, secondarySunriseColor, lerpValue);
		}

		sun.GetComponent<Light>().intensity = (timeOfDay >= 0f && timeOfDay <= Mathf.PI) ? maxSunIntensity : 0f;
		sun.GetComponent<Light>().color = primaryColor;
		moon.GetComponent<Light>().color = Color.white;
		moon.GetComponent<Light>().intensity = (timeOfDay >= Mathf.PI && timeOfDay <= 2f*Mathf.PI) ? maxMoonIntensity : 0f;
		RenderSettings.fogColor = secondaryColor;

		RenderSettings.skybox.SetFloat("_Value", Mathf.Clamp01(AngularDistance(timeOfDay,-Mathf.PI/2f)));
		if (Spectrum2.instance != null) {
			Color temp = new Color (sun.GetComponent<Light> ().color.r, sun.GetComponent<Light> ().color.g, sun.GetComponent<Light> ().color.b, Spectrum2.instance.opacity);
			Spectrum2.instance.GetComponent<LineRenderer> ().SetColors (temp, temp);
			Spectrum2.instance.GetComponent<LineRenderer> ().material.color = temp;
		}
	}

	private void UpdateTime() {
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
		for (int i=0; i<maxDecorations; i++) {
			AttemptDecorate ();
		}
	}
    
	void AttemptDecorate () {
		if (doDecorate && DynamicTerrain.instance.activeChunks.Count != 0) {
			for (int i=0; i<decorationsPerStep && numDecorations < maxDecorations; i++) {

				// Pick a random decoration and decorate with it
				GameObject decoration = decorations[UnityEngine.Random.Range(0, decorations.Count)];
				Decoration deco = decoration.GetComponent<Decoration>();
				if (Decoration.numDecorations == null) Decoration.numDecorations = new Dictionary<Decoration.Group, int>();
				if (!Decoration.numDecorations.ContainsKey(deco.group))
					Decoration.numDecorations.Add (deco.group, 0);
				if (Decoration.numDecorations[deco.group] < maxActive[deco.group]) {
					switch (deco.distribution) {
					case Decoration.Distribution.Random:
						Chunk chunk = terrain.RandomChunk ();
						DecorateRandom (chunk, decoration);
						break;
					case Decoration.Distribution.Roadside:
						float bezierProg = UnityEngine.Random.Range (PlayerMovement.instance.progress, 1f);
						DecorateRoadside (bezierProg, decoration);
						break;
					case Decoration.Distribution.CloseToRoad:
						DecorateRandom (terrain.RandomCloseToRoadChunk(), decoration);
						break;
					}
					Decoration.numDecorations[deco.group]++;
				}
			}
		}
	}
    
	GameObject CreateSun(){
		GameObject sun = new GameObject ("Sun");
		sun.AddComponent<Light> ();
		sun.AddComponent<Sun> ();
		//sun.GetComponent<Sun> ().setPosScales (LIGHT_X_SCALE, LIGHT_Y_SCALE, LIGHT_Z_SCALE);
		sun.GetComponent<Light> ().shadows = LightShadows.Soft;
		sun.GetComponent<Light>().flare = sunFlare;
		return sun;
	}

	GameObject CreateMoon(){
		GameObject moon = new GameObject ("Moon");
		moon.AddComponent<Light> ();
		moon.AddComponent<Moon> ();
		//moon.GetComponent<Moon> ().setPosScales (LIGHT_X_SCALE, LIGHT_Y_SCALE, LIGHT_Z_SCALE);
		moon.GetComponent<Light> ().shadows = LightShadows.Soft;
		moon.AddComponent<SpriteRenderer>();
		moon.GetComponent<SpriteRenderer>().sprite = moonSprites[UnityEngine.Random.Range(0,moonSprites.Count)];
		return  moon;
	}

	Road CreateRoad() {
		GameObject roadObj = new GameObject ("Road",
			typeof (MeshFilter),
			typeof (MeshRenderer),
			typeof (Road)
		);
		roadObj.GetComponent<MeshRenderer>().material = roadMaterial;
		roadObj.GetComponent<MeshRenderer>().reflectionProbeUsage = ReflectionProbeUsage.Off;
		road.width = roadWidth;
		road.height = roadHeight;

		return roadObj.GetComponent<Road>();
	}

	void DecorateRandom (Chunk chunk, GameObject decoration) {
		chunk.UpdateCollider();
		if (chunk == null) return;
		Vector2 coordinate = new Vector2 (
			chunk.x*chunkSize+UnityEngine.Random.Range(-chunkSize/2f, chunkSize/2f),
			chunk.y*chunkSize+UnityEngine.Random.Range(-chunkSize/2f, chunkSize/2f)
		);
		if (Mathf.PerlinNoise (coordinate.x, coordinate.y) < decoration.GetComponent<Decoration>().density) {
			if (!chunk.Constrained (new Vector3 (coordinate.x, 0f, coordinate.y))) {
				RaycastHit hit;
				float y = 0f;
				if (Physics.Raycast(new Vector3 (coordinate.x, heightScale, coordinate.y), Vector3.down,out hit, Mathf.Infinity)) {
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
		Road roadBez = road.GetComponent<Road>();
		Vector3 coordinate = 
			roadBez.Right(roadBez.GetPoint(prog)) * 1.1f * (side == 0 ? 1 : -1);
		RaycastHit hit;
		if (Physics.Raycast(new Vector3 (coordinate.x, heightScale, coordinate.y), Vector3.down, out hit, Mathf.Infinity))
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
		
}
