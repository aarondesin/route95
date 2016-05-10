using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;

public class WorldManager : MonoBehaviour {

	public struct Wave {
		public Vector3 origin;
		public float range;
		public float progress;
		public float width;
		public float height;
		public float speed;

		public bool active;

		public Wave (Vector3 pos, float r, float w, float h, float s) {
			origin = pos;
			range = r;
			progress = 0f;
			width = w;
			height = h;
			speed = s;
			active = true;
		}

		public void Update () {
			progress += speed * Time.deltaTime;
			if (progress >= 1f) active = false;
		}

		public Vector2 pixelOrigin (int chunkRes, float chunkSize) {
			return new Vector2 (
				Mathf.RoundToInt((origin.x-chunkSize/2f) * (float)chunkRes / chunkSize),
				Mathf.RoundToInt((origin.z-chunkSize/2f) * (float)chunkRes / chunkSize)
			);
		}
	}

	public static WorldManager instance;

	public int loadsToDo;

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

	//[Tooltip("Maxmimum number of decorations.")]
	private int maxDecorations; //= DEFAULT_MAX_DECORATIONS;

	//[Tooltip("Individual maximums for each decoration group.")]
	//public Decoration.GroupInfo[] initialMaxActive;

	private int numDecorations;
	//private Dictionary<Decoration.Group, int> maxActive;
	public Decoration.GroupInfo vegetationGroup;
	public Decoration.GroupInfo roadSignGroup;
	public Decoration.GroupInfo rockGroup;
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

	public float decorationDensity = 1f;

	public Mesh grassModel;
	public Material vegetationMaterial;
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

	public Texture2D waveTexture;
	List<Wave> waves = new List<Wave>();
	List<Wave> waveDeletes = new List<Wave>();

	[SerializeField]
	float waveProgress = 1f;
	public float waveSpeed = 0.1f;


	//
	[Header("Road Settings")]
	//

	[NonSerialized]
	public Road road;

	[Tooltip("Width of generated road.")]
	[Range(1f, 20f)]
	public float roadWidth = DEFAULT_ROAD_WIDTH;

	[Tooltip("Height of generated road.")]
	[Range(0.1f, 1.0f)]
	public float roadHeight = DEFAULT_ROAD_HEIGHT;

	[Tooltip("Radius within which to extend road.")]
	[Range(100f, 2000f)]
	public float roadExtendRadius = DEFAULT_ROAD_EXTEND_RADIUS;

	public float roadVariance = 0.4f;
	public float roadSlope = 0.0015f;

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

	[Tooltip("Flare texture to use for the sun.")]
	public Flare sunFlare;

	private GameObject moon;
	Light moonLight;

	[Tooltip("Nighttime intensity of the moon.")]
	[Range(0.1f, 1.5f)]
	public float maxMoonIntensity;

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

	private float[] freqDataArray;

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

		freqDataArray = new float[freqArraySize];

		terrain = new DynamicTerrain ();
		wind = UnityEngine.Random.insideUnitSphere;

		road = CreateRoad();

		numDecorations = 0;

		timeOfDay = UnityEngine.Random.Range(0, 2*Mathf.PI);
		sun = CreateSun();
		sunLight = sun.Light();
		moon = CreateMoon();
		moonLight = moon.Light();

		lightningStriker.SetActive(false);
		rainEmitter.SetRate(0f);
		shootingStarTemplate.SetActive(false);

		waveTexture = new Texture2D (terrain.vertexmap.width, terrain.vertexmap.width);

		loadsToDo = chunkLoadRadius * chunkLoadRadius + 
			(doDecorate ? maxDecorations + decorationPaths.Count : 0);

		terrainMaterial.SetFloat("_WaveProgress", 1f);
	}

	// Update is called once per frame
	void Update () {
		if (loadedTerrain && !hasRandomized) 
			Load();
		if (loaded && road.generated) {

			terrain.Update(freqDataArray);
			UpdateTime();
			UpdateColor();
			AttemptDecorate();
			Vector3 dWind = UnityEngine.Random.insideUnitSphere;
			wind += dWind * 0.5f;
			wind.Normalize();
			cloudEmitter.maxParticles = Mathf.Clamp(100 + Mathf.FloorToInt((float)shakers/(float)(MusicManager.instance.beatsElapsedInCurrentSong+1)*75f), 100, 300);
			rainEmitter.SetRate((float)shakers/(float)(MusicManager.instance.beatsElapsedInCurrentSong+1)*100f);
			starEmitter.SetRate(0.5f*starEmissionRate*-Mathf.Sin(timeOfDay)+starEmissionRate/2f);

			/*if (waveProgress < 1f) {
				Vector4 pos = PlayerMovement.instance.transform.position;
				pos.w = 1f;
				waveProgress += waveSpeed;
				terrainMaterial.SetVector("_WaveOrigin", pos);
				SetWaveProgress(waveProgress);
			}

			if (waves.Count > 0) {

				foreach (Wave wave in waves) {
					if (!wave.active) waveDeletes.Add(wave);
					else wave.Update();
				}

				foreach (Wave wave in waveDeletes) {
					waves.Remove(wave);
				}




			}
			RenderWaveTexture();*/
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

			if (Time.realtimeSinceStartup - startTime > 1f / Application.targetFrameRate) {
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

				if (Time.realtimeSinceStartup - startTime > 1f/Application.targetFrameRate) {
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

		
			
		sunLight.intensity = maxSunIntensity/2f * Mathf.Sin (timeOfDay) + maxSunIntensity/2f;
		sunLight.color = primaryColor;

		moonLight.color = Color.white;
		moonLight.intensity = maxMoonIntensity/2f * Mathf.Cos (progress) + maxMoonIntensity/2f;

		RenderSettings.fogColor = secondaryColor;

		RenderSettings.skybox.SetFloat("_Value", skyboxFade.Evaluate(progress).a);

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
    
	bool AttemptDecorate () {
		if (doDecorate && DynamicTerrain.instance.activeChunks.Count != 0 && road.generated) {
			for (int i=0; i<decorationsPerStep && numDecorations < maxDecorations; i++) {

				// Pick a random decoration and decorate with it
				GameObject decoration = decorations[UnityEngine.Random.Range(0, decorations.Count)];
				Decoration deco = decoration.GetComponent<Decoration>();


				//if (Decoration.numDecorations == null) Decoration.numDecorations = new Dictionary<Decoration.Group, int>();
				//if (!Decoration.numDecorations.ContainsKey(deco.group))
					//Decoration.numDecorations.Add (deco.group, 0);
				//if (Decoration.numDecorations[deco.group] < maxActive[deco.group]) {
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
						return DecorateRandom (chunk, decoration);
					case Decoration.Distribution.Roadside:
						float bezierProg = UnityEngine.Random.Range (PlayerMovement.instance.progress, 1f);
						return DecorateRoadside (bezierProg, decoration);
					case Decoration.Distribution.CloseToRoad:
						return DecorateRandom (terrain.RandomCloseToRoadChunk(), decoration);
					}
					//numDecorations++;
				}
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
		GameObject roadObj = new GameObject ("Road",
			typeof (MeshFilter),
			typeof (MeshRenderer),
			typeof (Road)
		);
		roadObj.GetComponent<MeshRenderer>().material = roadMaterial;
		roadObj.GetComponent<MeshRenderer>().reflectionProbeUsage = ReflectionProbeUsage.Off;
		roadObj.GetComponent<Road>().width = roadWidth;
		roadObj.GetComponent<Road>().height = roadHeight;

		return roadObj.GetComponent<Road>();
	}

	bool DecorateRandom (Chunk chunk, GameObject decoration) {
		//chunk.UpdateCollider();
		if (chunk == null) return false;
		Vector2 coordinate = new Vector2 (
			chunk.x*chunkSize+UnityEngine.Random.Range(-chunkSize/2f, chunkSize/2f),
			chunk.y*chunkSize+UnityEngine.Random.Range(-chunkSize/2f, chunkSize/2f)
		);

		if (Mathf.PerlinNoise (coordinate.x, coordinate.y) < decoration.GetComponent<Decoration>().density / terrain.activeChunks.Count * decorationDensity) {
			//Debug.Log(coordinate);
			IntVector2 nearestVertex = Chunk.ToNearestVMapCoords(coordinate.x, coordinate.y);
			//Debug.Log(nearestVertex.ToString());
			//if (UnityEngine.Random.Range(0,100) == 0) Debug.Log(coordinate + " maps to "+nearestVertex.ToString());
			if (!terrain.vertexmap.IsConstrained (nearestVertex)) {
				RaycastHit hit;
				float y = 0f;
				if (Physics.Raycast(new Vector3 (coordinate.x, heightScale, coordinate.y), Vector3.down,out hit, Mathf.Infinity)) {
					//Debug.Log("bap");
					y = hit.point.y;
				}
					
				GameObject newDecoration = 
					(GameObject)Instantiate (decoration, new Vector3 (coordinate.x, y, coordinate.y), Quaternion.Euler (0f, 0f, 0f));
				Decoration deco = newDecoration.GetComponent<Decoration>();
				deco.Randomize();
				if (!deco.dynamic) newDecoration.transform.parent = chunk.chunk.transform;
				numDecorations++;
				//Decoration.numDecorations[decoration.GetComponent<Decoration>().group]++;
				terrain.vertexmap.RegisterDecoration (nearestVertex, newDecoration);
				if (deco.group == Decoration.Group.Rocks) {
					rockGroup.numActive++;
				} else if (deco.group == Decoration.Group.Vegetation) {
					vegetationGroup.numActive++;
				}
				//Debug.Log("placed");
				return true;
			}
		}

		return false;
	}

	bool DecorateRoadside (float prog, GameObject decoration) {
		int side = UnityEngine.Random.Range (0, 2); // 0 = player side, 1 = other side
		Vector3 point = road.GetPoint(prog);
		Vector3 coordinate = 
			point + road.BezRight(point) * roadWidth * 2f * (side == 0 ? 1 : -1);
		RaycastHit hit;
		if (Physics.Raycast(new Vector3 (coordinate.x, heightScale, coordinate.y), Vector3.down, out hit, Mathf.Infinity))
			coordinate.y = hit.point.y;
		else coordinate.y = 0f;
		GameObject newDecoration = 
			(GameObject)Instantiate (decoration, coordinate, Quaternion.Euler (0f, 0f, 0f));
		Decoration deco = newDecoration.GetComponent<Decoration>();
		deco.Randomize();

		Vector3 rot = Quaternion.FromToRotation (coordinate+ road.GetDirection(prog), coordinate ).eulerAngles + new Vector3 (-90f,90f,0f);
		newDecoration.transform.rotation = Quaternion.Euler(rot);
	
		if (side == 0) newDecoration.transform.Rotate (new Vector3 (0f, 180f, 0f), Space.World);
		//newDecoration.transform.parent = road.gameObject.transform;
		newDecoration.transform.parent = terrain.chunkmap[Mathf.FloorToInt(coordinate.x/chunkSize)][Mathf.FloorToInt(coordinate.z/chunkSize)].chunk.transform;
		numDecorations++;
		//Decoration.numDecorations[decoration.GetComponent<Decoration>().group]++;
		if (deco.group == Decoration.Group.RoadSigns) {
			roadSignGroup.numActive++;
		}
		//road.AddDecoration(newDecoration, prog);
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
		if (deco == null) return;
		Decoration d = deco.GetComponent<Decoration>();
		//Decoration.numDecorations[d.group]--;
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
		Destroy(deco);
	}

	public void DecNumDeco(int n) {
		numDecorations -= n;
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

	public void StartWave () {
		SetWaveProgress(0f);
	}

	public void SetWaveProgress (float p) {
		terrainMaterial.SetFloat("_WaveProgress", p);
	}

	public void MakeWave (Vector3 pos, float strength, float speed) {
		waves.Add(
			new Wave(pos, strength * strength,
				strength / 2f, strength, speed) 
		);
	}

	public void RenderWaveTexture () {
		//waveTexture = Texture2D.blackTexture;
		if (waveTexture.width != terrain.vertexmap.width)
			waveTexture.Resize (terrain.vertexmap.width, terrain.vertexmap.width);

		for (int i=0; i<waveTexture.width; i++) {
			for (int j=0; j<waveTexture.height; j++) {
				
				//if (waveTexture.GetPixel(i,j).r > 0f) continue;
				waveTexture.SetPixel(i,j, Color.black);

				Vector2 pix = new Vector2 ((float)i, (float)j);
				foreach (Wave wave in waves) {
					if (!wave.active) continue;
					Vector2 center = wave.pixelOrigin(chunkResolution, chunkSize);
					float dist = Vector2.Distance (center, pix);
					if (dist <= wave.range + wave.width && dist >= wave.range - wave.width) {
						//float val = 
						waveTexture.SetPixel(i,j,Color.white);
					}
				}
			}
		}

		terrainMaterial.SetTexture ("_WaveTexture", waveTexture);
			
	}
		
}
