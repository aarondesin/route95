using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;

public class WorldManager : MonoBehaviour {

	#region WorldManager Vars

	public static WorldManager instance;

	public int loadsToDo;

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
	const float DEFAULT_ROAD_SLOPE = 0.9f;
	const float DEFAULT_ROAD_EXTEND_RADIUS = 1000f;
	const int DEFAULT_ROAD_STEPS_PER_CURVE = 100;
	const float DEFAULT_ROAD_MAX_SLOPE = 0.0015f;
	const float DEFAULT_ROAD_PLACEMENT_DISTANCE = 30f;
	const float DEFAULT_ROAD_VARIANCE = 0.4f;

	// Day/night cycle vars
	const float DEFAULT_TIME_SCALE = 0.01f;

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
	[Range(1f, 200f)]
	public float chunkSize = DEFAULT_CHUNK_SIZE;

	[Tooltip("The number of vertices along one side of a chunk.")]
	[Range(2, 32)]
	public int chunkResolution = DEFAULT_CHUNK_RESOLUTION;

	[Tooltip("The radius around the player within which to draw chunks.")]
	[Range(1, 32)]
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

	[Tooltip("Material to use for terrain.")]
	public Material terrainMaterial;
	public Material terrainDebugMaterial;

	[NonSerialized]
	public DynamicTerrain terrain;

	//
	[Header("Physics Settings")]
	//

	[Tooltip("Current wind vector.")]
	public Vector3 wind;


	//
	[Header("Decoration Settings")]
	//

	[Tooltip("Enable/disable decoration.")]
	public bool doDecorate = true;

	[SerializeField]
	private int maxDecorations;

	[SerializeField]
	private int numDecorations;

	public Decoration.GroupInfo vegetationGroup;
	public Decoration.GroupInfo roadSignGroup;
	public Decoration.GroupInfo rockGroup;

	[NonSerialized]
	public List<GameObject> decorations = new List<GameObject>();

	List<string> decorationPaths = new List<string>() {
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
		
	ObjectPool decorationPool;

	[Tooltip("Current global decoration density.")]
	[Range(0f,2f)]
	public float decorationDensity = 1f;

	[Tooltip("Mesh to use for grass particles.")]
	public Mesh grassModel;

	[Tooltip("Material to use for vegetation decorations.")]
	public Material vegetationMaterial;

	[Tooltip("Template to use for grass particle emitters.")]
	public GameObject grassEmitterTemplate;

	//
	[Header("Effects Settings")]
	//

	public float baseLightningIntensity = 1.5f;
	public GameObject lightningStriker;
	public GameObject lightningFlash;
	public GameObject shootingStarTemplate;

	public ParticleSystem cloudEmitter;
	public ParticleSystem rainEmitter;

	public List<ParticleSystem> exhaustEmitters;

	public float starEmissionRate = 6f;
	public ParticleSystem starEmitter;
	public int shakers;
	float rainDensity;


	//
	[Header("Road Settings")]
	//

	[Tooltip("Width of generated road.")]
	[Range(1f, 20f)]
	public float roadWidth = DEFAULT_ROAD_WIDTH;

	[Tooltip("Height of generated road.")]
	[Range(0.1f, 1.0f)]
	public float roadHeight = DEFAULT_ROAD_HEIGHT;

	public float roadSlope = DEFAULT_ROAD_SLOPE;

	public int roadStepsPerCurve = DEFAULT_ROAD_STEPS_PER_CURVE;

	[Tooltip("Radius within which to extend road.")]
	[Range(100f, 2000f)]
	public float roadExtendRadius = DEFAULT_ROAD_EXTEND_RADIUS;

	public float roadPlacementDistance = DEFAULT_ROAD_PLACEMENT_DISTANCE;

	public float roadVariance = DEFAULT_ROAD_VARIANCE;
	public float roadMaxSlope = DEFAULT_ROAD_MAX_SLOPE;

	[NonSerialized]
	public Road road;

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
	Light sunLight;

	[Tooltip("Daytime intensity of the sun.")]
	[Range(0.1f, 1.5f)]
	public float maxSunIntensity;
	public float minSunIntensity;
	float sunIntensityAxis;
	float sunIntensityAmplitude;

	[Tooltip("Flare texture to use for the sun.")]
	public Flare sunFlare;

	private GameObject moon;
	Light moonLight;

	[Tooltip("Nighttime intensity of the moon.")]
	[Range(0.1f, 1.5f)]
	public float maxMoonIntensity;
	public float minMoonIntensity;
	float moonIntensityAxis;
	float moonIntensityAmplitude;

	[Tooltip("Sprites to randomize for the moon.")]
	public List<Sprite> moonSprites;

	[SerializeField]
	private Color primaryColor;
	[SerializeField]
	private Color secondaryColor;

	public Gradient primaryColors;
	public Gradient secondaryColors;

	public Gradient skyboxFade;


	//
	[Header("Performance Settings")]
	//

	[Tooltip("Maximum number of chunk updates per cycle.")]
	[Range(1,16)]
	public int chunkUpdatesPerCycle = DEFAULT_CHUNK_UPDATES_PER_CYCLE;

	[Tooltip("Resolution used in frequency spectrum analysis. Must be a power of 2.")]
	public int freqArraySize = DEFAULT_FREQ_ARRAY_SIZE;

	LineRenderer visualizer;

	[Tooltip("FFT window to use when sampling music frequencies.")]
	public FFTWindow freqFFTWindow;

	[Tooltip("Maximum number of decorations to place per update cycle.")]
	[Range(10, 200)]
	public int decorationsPerStep = DEFAULT_DECORATIONS_PER_STEP;

	[Tooltip("Maximum number of grass particles to place per chunk.")]
	[Range(0,100)]
	public int grassPerChunk;

	[Tooltip("The accuracy used in road distance checks.")]
	[Range(1f, 500f)]
	public float roadPathCheckResolution = DEFAULT_ROAD_PATH_CHECK_RESOLUTION;

	float startLoadTime;
	bool loaded = false;
	public bool loadedTerrain = false;
	bool hasRandomized = false;



	#endregion
	#region Unity Callbacks

	// Use this for initialization
	void Awake () {
		instance = this;

		maxDecorations = roadSignGroup.maxActive + rockGroup.maxActive + vegetationGroup.maxActive;
		//Debug.Log(maxDecorations);

		terrain = new GameObject ("Terrain",
			typeof(DynamicTerrain)
		).GetComponent<DynamicTerrain> ();
		wind = UnityEngine.Random.insideUnitSphere;

		road = CreateRoad();

		numDecorations = 0;
		decorationPool = new ObjectPool();

		timeOfDay = UnityEngine.Random.Range(0, 2*Mathf.PI);
		sun = CreateSun();
		sunLight = sun.Light();
		sunIntensityAmplitude = (maxSunIntensity-minSunIntensity)/2f;
		sunIntensityAxis = minSunIntensity + sunIntensityAmplitude;

		moon = CreateMoon();
		moonLight = moon.Light();
		moonIntensityAmplitude = (maxMoonIntensity-maxMoonIntensity)/2f;
		moonIntensityAxis = minMoonIntensity + moonIntensityAmplitude;

		RenderSettings.ambientMode = AmbientMode.Flat;
		RenderSettings.ambientIntensity = 0.5f;

		lightningStriker.SetActive(false);
		rainEmitter.SetRate(0f);
		shootingStarTemplate.SetActive(false);

		loadsToDo = chunkLoadRadius * chunkLoadRadius + 
			(doDecorate ? maxDecorations + decorationPaths.Count : 0);

		terrainMaterial.SetFloat("_WaveProgress", 1f);
	}

	// Update is called once per frame
	void Update () {
		if (loadedTerrain && !hasRandomized) 
			Load();
		if (loaded && road.loaded) {

			//terrain.Update(freqDataArray);
			UpdateTime();
			UpdateColor();
			AttemptDecorate();
			Vector3 dWind = UnityEngine.Random.insideUnitSphere;
			wind += dWind * Time.deltaTime;
			wind.Normalize();
			cloudEmitter.maxParticles = Mathf.Clamp(100 + Mathf.FloorToInt((float)shakers/(float)(MusicManager.instance.beatsElapsedInCurrentSong+1)*75f), 100, 300);
			rainEmitter.SetRate((float)shakers/(float)(MusicManager.instance.beatsElapsedInCurrentSong+1)*100f);
			starEmitter.SetRate(0.5f*starEmissionRate*-Mathf.Sin(timeOfDay)+starEmissionRate/2f);
		}

	}

	#endregion

	public void Load () {

		// Get start time
		startLoadTime = Time.realtimeSinceStartup;

		// Start by loading chunks
		terrain.DoLoadChunks();
	}

	public void FinishLoading() {

		loaded = true;

		// Print time taken
		Debug.Log("WorldManager.Load(): finished in "+(Time.realtimeSinceStartup-startLoadTime).ToString("0.0000")+" seconds.");

		// Call GameManager to finish loading
		GameManager.instance.FinishLoading();
	}
	public void DoLoadRoad () {
		road.DoLoad();
	}

	public void DoLoadDecorations () {
		StartCoroutine("LoadDecorations");
	}

	IEnumerator LoadDecorations () {
		GameManager.instance.ChangeLoadingMessage("Loading decorations...");
		float startTime = Time.realtimeSinceStartup;
		int numLoaded = 0;

		foreach (string path in decorationPaths) {
			LoadDecoration (path);
			numLoaded++;

			if (Time.realtimeSinceStartup - startTime > GameManager.instance.targetDeltaTime) {
				yield return null;
				startTime = Time.realtimeSinceStartup;
				GameManager.instance.ReportLoaded(numLoaded);
				numLoaded = 0;
			}
		}

		if (decorations.Count == decorationPaths.Count)
			DoDecoration();
		yield return null;
	}

	public void DoDecoration () {
		StartCoroutine("DecorationLoop");
	}

	IEnumerator DecorationLoop () {
		if (!loaded) GameManager.instance.ChangeLoadingMessage("Decorating terrain...");
		float startTime = Time.realtimeSinceStartup;
		int numLoaded = 0;

		while (true) {
			if (numDecorations < maxDecorations) {
				numLoaded += (AttemptDecorate () ? 1 : 0);

				if (numDecorations == maxDecorations && !loaded) {
					FinishLoading();
					yield return null;
				}

				if (Time.realtimeSinceStartup - startTime > GameManager.instance.targetDeltaTime) {
					yield return null;
					startTime = Time.realtimeSinceStartup;
					if (!loaded) GameManager.instance.ReportLoaded(numLoaded);
					numLoaded = 0;
				}
			} else {
				yield return null;
			}

	
		}
			
	}
		
	void UpdateColor() {
		
		float progress = timeOfDay/(Mathf.PI*2f);

		primaryColor = primaryColors.Evaluate(progress);
		secondaryColor = secondaryColors.Evaluate(progress);

		sunLight.intensity = sunIntensityAxis + sunIntensityAmplitude * Mathf.Sin (timeOfDay);
		sunLight.color = primaryColor;

		moonLight.color = Color.white;
		moonLight.intensity = moonIntensityAxis + moonIntensityAmplitude * Mathf.Cos(timeOfDay-(Mathf.PI/2f));

		RenderSettings.fogColor = secondaryColor;
		RenderSettings.ambientLight = secondaryColor;

		RenderSettings.skybox.SetFloat("_Value", skyboxFade.Evaluate(progress).a);

		if (Spectrum2.instance != null) {

			if (visualizer == null) visualizer = Spectrum2.instance.GetComponent<LineRenderer>();

			Color temp = primaryColor;
			temp.a = Spectrum2.instance.opacity;

			visualizer.SetColors (temp, temp);
			visualizer.material.color = temp;
		}
	}

	private void UpdateTime() {
		timeOfDay += timeScale * Time.deltaTime;
		while (timeOfDay > (2 * Mathf.PI)) { //clamp timeOfDay between 0 and 2PI)
			timeOfDay -= 2 * Mathf.PI;
		}
	}
    
	bool AttemptDecorate () {

		// Pick a random decoration and decorate with it
		GameObject decoration;
		bool createNew = false;
		if (decorationPool.Empty) {
			decoration = decorations[UnityEngine.Random.Range(0, decorations.Count)];
			createNew = true;
		} else decoration = decorationPool.Peek();

		//if (!createNew) Debug.Log("old");
	
		Decoration deco = decoration.GetComponent<Decoration>();
	
		int numActive = 0;
		int maxActive = 0;
		switch (deco.group) {
		case Decoration.Group.RoadSigns:
			numActive = roadSignGroup.numActive;
			maxActive = roadSignGroup.maxActive;
			break;
		case Decoration.Group.Rocks:
			numActive = rockGroup.numActive;
			maxActive = rockGroup.maxActive;
			break;
		case Decoration.Group.Vegetation:
			numActive = vegetationGroup.numActive;
			maxActive = vegetationGroup.maxActive;
			break;
		}
		if (numActive < maxActive) {
			switch (deco.distribution) {
			case Decoration.Distribution.Random:
				Chunk chunk = terrain.RandomChunk ();
				return DecorateRandom (chunk, decoration, createNew);
			case Decoration.Distribution.Roadside:
				float bezierProg = UnityEngine.Random.Range (PlayerMovement.instance.progress, 1f);
				return DecorateRoadside (bezierProg, decoration, createNew);
			case Decoration.Distribution.CloseToRoad:
				return DecorateRandom (terrain.RandomCloseToRoadChunk(), decoration, createNew);
			}
		}
	
		return false;
	}
    
	GameObject CreateSun(){
		GameObject sun = new GameObject ("Sun");
		sun.AddComponent<Light> ();
		sun.AddComponent<Sun> ();
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

		// Create road object
		GameObject roadObj = new GameObject ("Road",
			typeof (MeshFilter),
			typeof (MeshRenderer),
			typeof (Road)
		);

		// Change renderer properties
		MeshRenderer roadRenderer = roadObj.GetComponent<MeshRenderer>();
		roadRenderer.material = roadMaterial;
		roadRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;

		// Pass on road properties
		Road rd = roadObj.GetComponent<Road>();
		rd.width = roadWidth;
		rd.height = roadHeight;

		return rd;
	}

	bool DecorateRandom (Chunk chunk, GameObject decorationPrefab, bool createNew) {
		if (chunk == null) {
			Debug.LogError("WorldManager.DecorateRandom(): invalid chunk!");
			return false;
		}

		//if (!createNew) Debug.Log("old");

		// Pick a random coordinate
		Vector2 coordinate = new Vector2 (
			chunk.x*chunkSize+UnityEngine.Random.Range(-chunkSize/2f, chunkSize/2f),
			chunk.y*chunkSize+UnityEngine.Random.Range(-chunkSize/2f, chunkSize/2f)
		);

		// Find nearest vertex
		IntVector2 nearestVertex = Chunk.ToNearestVMapCoords(coordinate.x, coordinate.y);
		Vertex vert = terrain.vertexmap.VertexAt (nearestVertex);
		if (vert == null) {
			Debug.LogError ("WorldManager.DecorateRandom(): picked nonexistent vertex at " + nearestVertex.ToString ());
			return false;
		}

		// Check if constrained
		if (terrain.vertexmap.VertexAt(nearestVertex).noDecorations) {
			//Debug.Log(nearestVertex.ToString() + " was constrained, picked chunk "+chunk.name);
			return false;
		}

		// Roll based on density
		float density = decorationPrefab.GetComponent<Decoration>().density;
		float spawnThreshold = density / terrain.NumActiveChunks * decorationDensity;

		// If roll succeeded
		if (!createNew || Mathf.PerlinNoise (coordinate.x, coordinate.y) < spawnThreshold) {

			// Instantiate or grab object
			GameObject decoration;
			if (createNew) decoration = (GameObject)Instantiate(decorationPrefab);
			else decoration = decorationPool.Get();
			Decoration deco = decoration.GetComponent<Decoration>();

			// Raycast down 
			RaycastHit hit;
			float y;
			Vector3 rayOrigin = new Vector3 (coordinate.x, heightScale, coordinate.y);
			if (Physics.Raycast(rayOrigin, Vector3.down,out hit, Mathf.Infinity)) y = hit.point.y;
			else y = 0f;

			// Transform decoration
			decoration.transform.position = new Vector3 (coordinate.x, y, coordinate.y);
					
			// Randomize decoration
			deco.Randomize();

			// Parent decoration to chunk (if not dynamic)
			if (!deco.dynamic) {
				decoration.transform.parent = chunk.gameObject.transform;
				chunk.decorations.Add(decoration);
			}

			// Register decoration
			numDecorations++;
			terrain.vertexmap.RegisterDecoration (nearestVertex, decoration);
			switch (deco.group) {
			case Decoration.Group.Rocks:
				rockGroup.numActive++;
				break;
			case Decoration.Group.Vegetation:
				vegetationGroup.numActive++;
				break;
			}
				
			return true;
		}

		return false;
	}

	bool DecorateRoadside (float prog, GameObject decorationPrefab, bool createNew) {

		// Get road point
		Vector3 point = road.GetPoint(prog);

		// Pick a road side
		int side = UnityEngine.Random.Range (0, 2); // 0 = player side, 1 = other side

		// Calculate coordinate
		Vector3 coordinate = point + road.BezRight(point) * 
			roadWidth * UnityEngine.Random.Range(1.5f, 1.6f) * (side == 0 ? 1 : -1);

		// Raycast down
		RaycastHit hit;
		Vector3 rayOrigin = new Vector3 (coordinate.x, heightScale, coordinate.y);
		if (Physics.Raycast(rayOrigin, Vector3.down, out hit, Mathf.Infinity)) coordinate.y = hit.point.y;
		else coordinate.y = 0f;

		// Instantiate or grab decoration
		GameObject decoration;
		if (createNew) decoration = 
			(GameObject)Instantiate(decorationPrefab, coordinate, Quaternion.Euler(Vector3.zero));
		else decoration = decorationPool.Get();

		// Randomize
		Decoration deco = decoration.GetComponent<Decoration>();
		deco.Randomize();

		// Point decoration in road direction
		decoration.transform.rotation = Quaternion.LookRotation (road.GetDirection(prog), Vector3.up);
		decoration.transform.Rotate(-90f, side == 1 ? 180f : 0f, 0f);

		// Parent to nearest chunk
		int chunkX = Mathf.RoundToInt((coordinate.x-chunkSize/2f)/chunkSize);
		int chunkY = Mathf.RoundToInt((coordinate.z-chunkSize/2f)/chunkSize);
		Chunk chunk = terrain.ChunkAt(chunkX,chunkY);
		decoration.transform.parent = chunk.gameObject.transform;
		chunk.decorations.Add(decoration);

		// Register
		numDecorations++;
		if (deco.group == Decoration.Group.RoadSigns) roadSignGroup.numActive++;

		return true;
	}

	void LoadDecoration (string path) {
		GameObject decoration = (GameObject) Resources.Load(path);
		if (decoration == null) {
			Debug.LogError ("Failed to load decoration at "+path);
		} else {
			//Debug.Log("Loaded "+path);
			decorations.Add(decoration);
			//GameManager.instance.IncrementLoadProgress();
		}
	}

	public void RemoveDecoration (GameObject deco) {
		Decoration d = deco.GetComponent<Decoration>();

		// Deparent decoration
		deco.transform.parent = null;
		//Debug.Log("removing " + deco.name);

		// Deregister
		switch (d.group) {
		case Decoration.Group.RoadSigns:
			roadSignGroup.numActive--;
			break;
		case Decoration.Group.Rocks:
			rockGroup.numActive--;
			break;
		case Decoration.Group.Vegetation:
			vegetationGroup.numActive--;
			break;
		}
		numDecorations--;

		// Pool decoration
		decorationPool.Add(deco);
	}

	/// <summary>
	/// Creates a lightning strike at a random point in view.
	/// </summary>
	public void LightningStrike (float strength) {

		// Find camera forward direction (flat)
		Vector3 forward = Camera.main.transform.forward;
		forward.y = 0f;
		forward.Normalize();

		// Define an offset
		Vector2 r = UnityEngine.Random.insideUnitCircle;
		Vector3 offset = new Vector3 (400f*r.x, 250f, 400f*r.y);

		// Pick a point
		Vector3 origin = PlayerMovement.instance.transform.position + forward * UnityEngine.Random.Range(0.9f, 1.1f) *
			vertexUpdateDistance + offset;

		// Play sound
		// TODO

		// Enable lightning striker and move to point
		lightningStriker.SetActive(true);
		lightningStriker.transform.position = origin;
		lightningStriker.Light().intensity = baseLightningIntensity*strength;
	}

	/// <summary>
	/// Creates a lightning flash within the clouds.
	/// </summary>
	/// <param name="strength">Strength.</param>
	public void LightningFlash (float strength) {

		// Find camera forward direction (flat)
		Vector3 forward = Camera.main.transform.forward;
		forward.y = 0f;
		forward.Normalize();

		// Define an offset
		Vector2 r = UnityEngine.Random.insideUnitCircle;
		Vector3 offset = new Vector3 (800f*r.x, 800f, 800f*r.y);

		// Pick a point
		Vector3 origin = PlayerMovement.instance.transform.position + forward * UnityEngine.Random.Range(1.9f, 2.1f) *
			vertexUpdateDistance + offset;

		// Play sound
		// TODO

		// Enable lightning flash and move to point
		lightningFlash.SetActive(true);
		lightningFlash.transform.position = origin;
		lightningFlash.Light().intensity = baseLightningIntensity*strength;
	}

	public void StarBurst () {
		starEmitter.Emit(UnityEngine.Random.Range(10,20));
	}

	public void ExhaustPuff () {
		foreach (ParticleSystem sys in exhaustEmitters) sys.Emit(1);
	}

	public void ShootingStar () {

		// Find camera forward direction (flat)
		Vector3 forward = Camera.main.transform.forward;
		forward.y = 0f;
		forward.Normalize();

		// Define an offset
		Vector2 r = UnityEngine.Random.insideUnitCircle;
		Vector3 offset = new Vector3 (1000f*r.x, 600f, 1000f*r.y);

		// Pick a point
		Vector3 origin = PlayerMovement.instance.transform.position + forward * UnityEngine.Random.Range(2.9f, 3.1f) *
			vertexUpdateDistance + offset;
		
		GameObject shootingStar = (GameObject)Instantiate(shootingStarTemplate, origin, Quaternion.identity);
		shootingStar.SetActive(true);
	}

	public void DeformRandom () {
		// Find camera forward direction (flat)
		Vector3 forward = Camera.main.transform.forward;
		forward.y = 0f;
		forward.Normalize();

		// Define an offset
		Vector2 r = UnityEngine.Random.insideUnitCircle;
		Vector3 offset = new Vector3 (400f*r.x, 0f, 400f*r.y);

		// Pick a point
		Vector3 origin = PlayerMovement.instance.transform.position + forward * UnityEngine.Random.Range(0.9f, 1.1f) *
			vertexUpdateDistance + offset;

		IntVector2 coords = Chunk.ToNearestVMapCoords(origin.x, origin.z);
		Vertex v = terrain.vertexmap.VertexAt(coords);
		if (v == null) v = terrain.vertexmap.AddVertex (coords);
		if (!v.locked) v.SmoothHeight (v.height + heightScale/32f, 0.95f);

		//Debug.Log(v.ToString());
	}

	public void DebugTerrain () {
		terrain.SetDebugColors(DynamicTerrain.DebugColors.Constrained);
	}
		
}
