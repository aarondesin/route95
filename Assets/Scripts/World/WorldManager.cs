// WorldManager.cs
// ©2016 Team 95

using Route95.Core;
using Route95.Music;
using Route95.UI;

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering;

using UnityStandardAssets.ImageEffects;

namespace Route95.World {

    /// <summary>
    /// Manager for all things world-related.
    /// </summary>
    public class WorldManager : SingletonMonoBehaviour<WorldManager> {

        #region WorldManager Enums

        /// <summary>
        /// Modes to generate chunks around player.
        /// </summary>
        public enum ChunkGenerationMode {
            Square,
            Circular
        }

        #endregion
        #region Vars

		/// <summary>
		/// GameObject to use to show vertices.
		/// </summary>
		[Tooltip("GameObject to use to show vertices.")]
		[SerializeField]
        GameObject _vertexIndicator;

		/// <summary>
		/// Number of load operations to perform.
		/// </summary>
        int _loadOpsToDo;

		/// <summary>
		/// Default chunk size (world units).
		/// </summary>
        const float DEFAULT_CHUNK_SIZE = 100;

		/// <summary>
		/// Default number of vertices per chunk edge/
		/// </summary>
        const int DEFAULT_CHUNK_RESOLUTION = 8;

		/// <summary>
		/// Default player radius to load chunks (chunks).
		/// </summary>
        const int DEFAULT_CHUNK_LOAD_RADIUS = 4;

        /// <summary>
		/// Default terrain height scale (world units).
		/// </summary>
        const float DEFAULT_HEIGHT_SCALE = 800f;

		/// <summary>
		/// Default player radius to update vertices (world units).
		/// </summary>
        const float DEFAULT_VERTEX_UPDATE_DISTANCE = 600f;

        /// <summary>
		/// Default hard decoration limit.
		/// </summary>
        const int DEFAULT_MAX_DECORATIONS = 1000;

		/// <summary>
		/// Default max number of decorations to place each cycle.
		/// </summary>
        const int DEFAULT_DECORATIONS_PER_STEP = 100;

        /// <summary>
		/// Default road width (world units).
		/// </summary>
        const float DEFAULT_ROAD_WIDTH = 10f;

		/// <summary>
		/// Default road height (world units).
		/// </summary>
        const float DEFAULT_ROAD_HEIGHT = 0.2f;

		/// <summary>
		/// Default ratio of road top plane to bottom plane (percent).
		/// </summary>
        const float DEFAULT_ROAD_SLOPE = 0.9f;

		/// <summary>
		/// Default player radius to extend road (world units).
		/// </summary>
        const float DEFAULT_ROAD_EXTEND_RADIUS = 1000f;

		/// <summary>
		/// Default number of road mesh subdivision steps per segment.
		/// </summary>
        const int DEFAULT_ROAD_STEPS_PER_CURVE = 100;

		/// <summary>
		/// Default limit on road slope per world unit.
		/// </summary>
        const float DEFAULT_ROAD_MAX_SLOPE = 0.0015f;

		/// <summary>
		/// Default distance to place road (world units).
		/// </summary>
        const float DEFAULT_ROAD_PLACEMENT_DISTANCE = 30f;

		/// <summary>
		/// Default radius of road placement circle (percent).
		/// </summary>
        const float DEFAULT_ROAD_VARIANCE = 0.4f;

        /// <summary>
		/// Default time scale multiplier.
		/// </summary>
        const float DEFAULT_TIME_SCALE = 0.01f;

        /// <summary>
		/// Default number of chunks to update per cycle.
		/// </summary>
        const int DEFAULT_CHUNK_UPDATES_PER_CYCLE = 4;

		/// <summary>
		/// Default size of frequency data array (power of 2).
		/// </summary>
        const int DEFAULT_FREQ_ARRAY_SIZE = 256;
		
		/// <summary>
		/// Default resolution at which to check the road.
		/// </summary> 
        const float DEFAULT_ROAD_PATH_CHECK_RESOLUTION = 4f;

        //-----------------------------------------------------------------------------------------------------------------
        [Header("Chunk Settings")]

		/// <summary>
		/// The length of one side of a chunk.
		/// </summary>
        [Tooltip("The length of one side of a chunk.")]
        [Range(1f, 200f)]
		[SerializeField]
        float _chunkSize = DEFAULT_CHUNK_SIZE;

		/// <summary>
		/// The number of vertices along one side of a chunk.
		/// </summary>
        [Tooltip("The number of vertices along one side of a chunk.")]
        [Range(2, 32)]
        [SerializeField]
        int _chunkResolution = DEFAULT_CHUNK_RESOLUTION;

		/// <summary>
		/// The radius around the player within which to draw chunks.
		/// </summary>
        [Tooltip("The radius around the player within which to draw chunks.")]
        [Range(4, 32)]
		[SerializeField]
        int _chunkLoadRadius = DEFAULT_CHUNK_LOAD_RADIUS;

		/// <summary>
		/// Mode to generate chunks.
		/// </summary>
        [Tooltip("Mode to generate chunks.")]
		[SerializeField]
        ChunkGenerationMode _chunkGenerationMode = ChunkGenerationMode.Circular;

        //-----------------------------------------------------------------------------------------------------------------
        [Header("Terrain Settings")]

		/// <summary>
		/// Height scale of generated terrain.
		/// </summary>
        [Tooltip("Height scale of generated terrain.")]
        [Range(100f, 1000f)]
		[SerializeField]
        float _heightScale = DEFAULT_HEIGHT_SCALE;

		/// <summary>
		/// The distance from the player at which vertices should update.
		/// </summary>
        [Tooltip("The distance from the player at which vertices should update.")]
        [Range(100f, 1000f)]
		[SerializeField]
        float _vertexUpdateDistance = DEFAULT_VERTEX_UPDATE_DISTANCE;

		/// <summary>
		/// Material to use for terrain.
		/// </summary>
        [Tooltip("Material to use for terrain.")]
		[SerializeField]
        Material _terrainMaterial;

		/// <summary>
		/// Material to use when debugging terrain.
		/// </summary>
        [Tooltip("Material to use when debugging terrain.")]
        Material _terrainDebugMaterial;

        //-----------------------------------------------------------------------------------------------------------------
        [Header("Physics Settings")]

		/// <summary>
		/// Current wind vector.
		/// </summary>
        [Tooltip("Current wind vector.")]
		[SerializeField]
        Vector3 _wind;

        //-----------------------------------------------------------------------------------------------------------------
        [Header("Decoration Settings")]

		/// <summary>
		/// Enable/disable decoration.
		/// </summary>
        [Tooltip("Enable/disable decoration.")]
		[SerializeField]
        bool _doDecorate = true;

		/// <summary>
		/// Total max decorations.
		/// </summary>
        [SerializeField]
        [Tooltip("Total max decorations.")]
        int _maxDecorations;

		/// <summary>
		/// Current number of active decorations.
		/// </summary>
        [SerializeField]
        [Tooltip("Current number of active decorations.")]
        int _numDecorations;

		/// <summary>
		/// Decoration group info for vegetation.
		/// </summary>
        [Tooltip("Decoration group info for vegetation.")]
		[SerializeField]
        Decoration.GroupInfo _vegetationGroup;

		/// <summary>
		/// Decoration group info for road signs.
		/// </summary>
        [Tooltip("Decoration group info for road signs.")]
		[SerializeField]
        Decoration.GroupInfo _roadSignGroup;

		/// <summary>
		/// Decoration group info for rocks.
		/// </summary>
        [Tooltip("Decoration group info for rocks.")]
		[SerializeField]
        Decoration.GroupInfo _rockGroup;

        /// <summary>
		/// List of all active decorations.
		/// </summary>
        List<GameObject> _decorations = new List<GameObject>();

		/// <summary>
		/// List of load paths for decoration prefabs/
		/// </summary>
        List<string> _decorationPaths = new List<string>() {
			"Prefabs/Decoration_50mph",
			"Prefabs/Decoration_50mph45night",
			"Prefabs/Decoration_65MPH",
			"Prefabs/Decoration_70MPH",
			"Prefabs/Decoration_75mph",
			"Prefabs/Decoration_Agave01",
			"Prefabs/Decoration_BarrelCactus",
			"Prefabs/Decoration_Boulder01",
			"Prefabs/Decoration_Boulder02",
			"Prefabs/Decoration_Boulder03",
			"Prefabs/Decoration_Chevron",
			"Prefabs/Decoration_JoshuaTree01",
			"Prefabs/Decoration_PassWithCare",
			"Prefabs/Decoration_Saguaro",
			"Prefabs/DynamicDecoration_Tumbleweed01"
		};

		/// <summary>
		/// Decoration pool to use.
		/// </summary>
        ObjectPool<Decoration> _decorationPool;

		/// <summary>
		/// Current global decoration density.
		/// </summary>
        [Tooltip("Current global decoration density.")]
        [Range(0f, 2f)]
		[SerializeField]
        float _decorationDensity = 1f;

		/// <summary>
		/// Mesh to use for grass particles.
		/// </summary>
        [Tooltip("Mesh to use for grass particles.")]
		[SerializeField]
        Mesh _grassModel;

		/// <summary>
		/// Material to use for vegetation decorations.
		/// </summary>
        [Tooltip("Material to use for vegetation decorations.")]
		[SerializeField]
        Material _vegetationMaterial;

		/// <summary>
		/// Template to use for grass particle emitters.
		/// </summary>
        [Tooltip("Template to use for grass particle emitters.")]
		[SerializeField]
        GameObject _grassEmitterPrefab;

		/// <summary>
		/// 
		/// </summary>
		[SerializeField]
        ParticleSystem _decorationParticleEmitter;

        //-----------------------------------------------------------------------------------------------------------------
        [Header("Effects Settings")]

		/// <summary>
		/// Base intensity of lightning effects.
		/// </summary>
        [Tooltip("Base intensity of lightning effects.")]
        [Range(0.5f, 2f)]
		[SerializeField]
        float _baseLightningIntensity = 1.5f;

		/// <summary>
		/// GameObject to use for lightning strikes.
		/// </summary>
        [Tooltip("GameObject to use for lightning strikes.")]
		[SerializeField]
        GameObject _lightningStriker;

		/// <summary>
		/// GameObject to use for in-cloud lightning flashes.
		/// </summary>
        [Tooltip("GameObject to use for in-cloud lightning flashes.")]
		[SerializeField]
        GameObject _lightningFlash;

		/// <summary>
		/// Star particle emitter.
		/// </summary>
        [Tooltip("Star particle emitter.")]
		[SerializeField]
        ParticleSystem _starEmitter;

		/// <summary>
		/// Natural star emission rate.
		/// </summary>
        [Tooltip("Natural star emission rate.")]
		[SerializeField]
        float _starEmissionRate = 6f;

		/// <summary>
		/// Template to use to instantiate shooting stars.
		/// </summary>
        [Tooltip("Template to use to instantiate shooting stars.")]
		[SerializeField]
        GameObject _shootingStarTemplate;

		/// <summary>
		/// Cloud particle emitter.
		/// </summary>
        [Tooltip("Cloud particle emitter.")]
		[SerializeField]
        ParticleSystem _cloudEmitter;

		/// <summary>
		/// Minimum number of cloud particles.
		/// </summary>
        [Tooltip("Minimum number of cloud particles.")]
		[SerializeField]
        float _minCloudDensity;

		/// <summary>
		/// Maximum number of cloud particles.
		/// </summary>
        [Tooltip("Maximum number of cloud particles.")]
		[SerializeField]
        float _maxCloudDensity;

		/// <summary>
		/// 
		/// </summary>
        [SerializeField]
        int _cloudDensity;

		/// <summary>
		/// Rain particle emitter.
		/// </summary>
        [Tooltip("Rain particle emitter.")]
		[SerializeField]
        ParticleSystem _rainEmitter;

		/// <summary>
		/// Minimum number of rain particles.
		/// </summary>
        [Tooltip("Minimum number of rain particles.")]
		[SerializeField]
        float _minRainDensity;

		/// <summary>
		/// Maximuim number of rain particles.
		/// </summary>
        [Tooltip("Maximuim number of rain particles.")]
        float _maxRainDensity;

		/// <summary>
		/// 
		/// </summary>
        [SerializeField]
        int rainDensity;

		/// <summary>
		/// Number of active shakers.
		/// </summary>
        [Tooltip("Number of active shakers.")]
        int _shakers;

		/// <summary>
		/// All car exhaust puff emitters.
		/// </summary>
        [Tooltip("All car exhaust puff emitters.")]
        List<ParticleSystem> _exhaustEmitters;

        //-----------------------------------------------------------------------------------------------------------------
        [Header("Road Settings")]

        [Tooltip("Width of generated road.")]
        [Range(1f, 20f)]
		[SerializeField]
        float _roadWidth = DEFAULT_ROAD_WIDTH;

        [Tooltip("Height of generated road.")]
        [Range(0.1f, 1.0f)]
		[SerializeField]
        float _roadHeight = DEFAULT_ROAD_HEIGHT;

		/// <summary>
		/// Ratio of top of road to bottom.
		/// </summary>
        [Tooltip("Ratio of top of road to bottom.")]
		[SerializeField]
        float _roadSlope = DEFAULT_ROAD_SLOPE;

        [Tooltip("Number of mesh subdivisions per road segment.")]
        public int roadStepsPerCurve = DEFAULT_ROAD_STEPS_PER_CURVE;

        [NonSerialized]
        public float roadExtendRadius = DEFAULT_ROAD_EXTEND_RADIUS;

        [NonSerialized]
        public float roadCleanupRadius;

        [Tooltip("Radius within which to place road.")]
        public float roadPlacementDistance = DEFAULT_ROAD_PLACEMENT_DISTANCE;

        [Tooltip("Percentage radius of road placement distance within which to place road.")]
        public float roadVariance = DEFAULT_ROAD_VARIANCE;

        [Tooltip("Max road slope per world unit of distance.")]
        public float roadMaxSlope = DEFAULT_ROAD_MAX_SLOPE;

		/// <summary>
		/// Reference to the road object.
		/// </summary>
        [NonSerialized]
        Road _road;

        [Tooltip("Material to use for road.")]
        public Material roadMaterial;

        //-----------------------------------------------------------------------------------------------------------------
        [Header("Day/Night Cycle Settings")]

        [Tooltip("Global time scale for day/night cycle.")]
        [Range(0.001f, 0.1f)]
        public float timeScale = DEFAULT_TIME_SCALE;

        [Tooltip("Current time of day.")]
        [Range(0f, 2f * Mathf.PI)]
        public float timeOfDay;

        public GameObject sun;                               // Sun object
        Light sunLight;                                      // Sun object's light

        [Tooltip("Scale to use for the sun.")]
        public float sunScale;

        [Tooltip("Daytime intensity of the sun.")]
        [Range(0f, 8f)]
        public float maxSunIntensity;

        [Tooltip("Nighttime intensity of the sun.")]
        [Range(0f, 8f)]
        public float minSunIntensity;

        float sunIntensityAxis;                              // Axis of sun intensity oscillation
        float sunIntensityAmplitude;                         // Amplitude of sun intensity oscillation

        [Tooltip("Flare texture to use for the sun.")]
        public Flare sunFlare;

        public GameObject moon;                              // Moon object
        Light moonLight;                                     // Moon object's light

        [Tooltip("Scale to use for the moon.")]
        public float moonScale;

        [Tooltip("Nighttime intensity of the moon.")]
        [Range(0f, 8f)]
        public float maxMoonIntensity;

        [Tooltip("Daytime intensity of the moon.")]
        [Range(0f, 8f)]
        public float minMoonIntensity;

        float moonIntensityAxis;                             // Axis of moon intensity oscillation
        float moonIntensityAmplitude;                        // Amplitude of moon intensity oscillation

        [Tooltip("Sprites to randomize for the moon.")]
        public List<Sprite> moonSprites;

        [Tooltip("Current primary color.")]
        [SerializeField]
        private Color primaryColor;

        [Tooltip("Current secondary color.")]
        [SerializeField]
        private Color secondaryColor;

        [Tooltip("Primary color cycle.")]
        public Gradient primaryColors;

        [Tooltip("Secondary color cycle.")]
        public Gradient secondaryColors;

        [Tooltip("Skybox transition cycle.")]
        public Gradient skyboxFade;

        //-----------------------------------------------------------------------------------------------------------------
        [Header("Performance Settings")]

        [Tooltip("Maximum number of chunk updates per cycle.")]
        [Range(1, 16)]
		[SerializeField]
        int _chunkUpdatesPerCycle = DEFAULT_CHUNK_UPDATES_PER_CYCLE;

        [Tooltip("Resolution used in frequency spectrum analysis. Must be a power of 2.")]
		[SerializeField]
        int _frequencySampleSize = DEFAULT_FREQ_ARRAY_SIZE;

        LineRenderer visualizer;                                // Frequency visualizer

        [Tooltip("FFT window to use when sampling music frequencies.")]
		[SerializeField]
        FFTWindow _frequencyFFTWindow;

        [Tooltip("Maximum number of decorations to place per update cycle.")]
        [Range(10, 200)]
        public int decorationsPerStep = DEFAULT_DECORATIONS_PER_STEP;

        [Tooltip("Maximum number of grass particles to place per chunk.")]
        [Range(0, 100)]
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

        new void Awake() {
            base.Awake();

            _maxDecorations = _roadSignGroup.maxActive + _rockGroup.maxActive + _vegetationGroup.maxActive;
            //Debug.Log(maxDecorations);

            _wind = UnityEngine.Random.insideUnitSphere;

            //roadPlacementDistance = chunkSize * 2f;
            roadCleanupRadius = _chunkSize * (_chunkLoadRadius);
            roadPlacementDistance = _chunkSize * 0.4f;
            roadExtendRadius = _chunkSize * (_chunkLoadRadius / 2);
            _road = CreateRoad();

            _numDecorations = 0;
            _decorationPool = new ObjectPool<Decoration>();

            timeOfDay = UnityEngine.Random.Range(0, 2 * Mathf.PI);
            CreateSun();
            sunLight = sun.GetComponent<Light>();
            sunIntensityAmplitude = (maxSunIntensity - minSunIntensity) / 2f;
            sunIntensityAxis = minSunIntensity + sunIntensityAmplitude;

            CreateMoon();
            moonLight = moon.GetComponent<Light>();
            moonIntensityAmplitude = (maxMoonIntensity - maxMoonIntensity) / 2f;
            moonIntensityAxis = minMoonIntensity + moonIntensityAmplitude;

            //RenderSettings.ambientMode = AmbientMode.Flat;
            //RenderSettings.ambientIntensity = 0.5f;

            _lightningStriker.SetActive(false);
            _rainEmitter.SetRate(0f);
            _shootingStarTemplate.SetActive(false);

            _terrainMaterial.SetFloat("_WaveProgress", 1f);
        }

        void Start() {
            _loadOpsToDo = _chunkLoadRadius * _chunkLoadRadius +
                (_doDecorate ? _maxDecorations + _decorationPaths.Count : 0) +
                DynamicTerrain.Instance.LoadOpsToDo;

			Spectrum2.Instance.enabled = false;
        }

        // Update is called once per frame
        void Update() {
            if (loadedTerrain && !hasRandomized)
                Load();
            if (loaded && _road.IsLoaded) {

                //terrain.Update(freqDataArray);
                UpdateTime();
                UpdateColor();
                if (_doDecorate) {
                    AttemptDecorate();
                    Vector3 dWind = UnityEngine.Random.insideUnitSphere;
                    _wind += dWind * Time.deltaTime;
                    _wind.Normalize();
                }

                float cd = _shakers / (Riff.MAX_BEATS * 4f);
                _cloudDensity = Mathf.FloorToInt(_minCloudDensity + cd * (_maxCloudDensity - _minCloudDensity));
                _cloudEmitter.maxParticles = _cloudDensity;

                float rd = _shakers / (Riff.MAX_BEATS * 2f);
                rainDensity = Mathf.FloorToInt(_minRainDensity + rd * (_maxRainDensity - _minRainDensity));
                _rainEmitter.SetRate(rainDensity);

                _starEmitter.SetRate(0.5f * _starEmissionRate * -Mathf.Sin(timeOfDay) + _starEmissionRate / 2f);
            }

        }

		#endregion
		#region Properties

		public GameObject VertexIndicator { get { return _vertexIndicator; } }

		public int LoadOpsToDo { get { return _loadOpsToDo; } }

		public float ChunkSize { get { return _chunkSize; } }

		/// <summary>
        /// Returns the chunk resolution (read-only).
        /// </summary>
        public int ChunkResolution { get { return _chunkResolution; } }

		public int ChunkLoadRadius {
			get { return _chunkLoadRadius; }
			set { _chunkLoadRadius = value; }
		}

		public int ChunkUpdatesPerCycle { get { return _chunkUpdatesPerCycle; } }

		public ChunkGenerationMode ChunkGenMode {
			get { return _chunkGenerationMode; }
			set { _chunkGenerationMode = value; }
		}

		public float VertexUpdateDistance { get { return _vertexUpdateDistance; } }

		public float HeightScale { get { return _heightScale; } }

		public Road Road { get { return _road; } }

		public float RoadWidth { get { return _roadWidth; } }

		public float RoadHeight { get { return _roadHeight; } }

		public float RoadSlope { get { return _roadSlope; } } 

		public FFTWindow FrequencyFFTWindow { get { return _frequencyFFTWindow; } }

		public int FrequencySampleSize { get { return _frequencySampleSize; } }

		public Material TerrainMaterial { get { return _terrainMaterial; } }

		public Material TerrainDebugMaterial { get { return _terrainDebugMaterial; } }

		public GameObject GrassEmitterPrefab { get { return _grassEmitterPrefab; } }

		public ParticleSystem DecorationParticleEmitter { get { return _decorationParticleEmitter; } }

		public Vector3 Wind { get { return _wind; } }

		public float BaseLightningIntensity { get { return _baseLightningIntensity; } }

		public int Shakers {
			get { return _shakers; }
			set { _shakers = value; }
		}

		public bool DoDecorate {
			get { return _doDecorate; }
			set { _doDecorate = value; }
		}

		public float DecorationDensity {
			get { return _decorationDensity; }
			set { _decorationDensity = value; }
		}

		#endregion
		#region WorldManager Methods

		public void Load() {

            Camera.main.GetComponent<SunShafts>().sunTransform = sun.transform;

            // Get start time
            startLoadTime = Time.realtimeSinceStartup;

            // Start by loading chunks
            DynamicTerrain.Instance.DoLoadChunks();
        }

        public void FinishLoading() {

            loaded = true;

            // Print time taken
            Debug.Log("WorldManager.Load(): finished in " + (Time.realtimeSinceStartup - startLoadTime).ToString("0.0000") + " seconds.");

            Spectrum2.Instance.enabled = true;

            // Call GameManager to finish loading
            GameManager.Instance.FinishLoading();
        }
        public void DoLoadRoad() {
            _road.DoLoad();
        }

        public void DoLoadDecorations() {
            StartCoroutine("LoadDecorations");
        }

        IEnumerator LoadDecorations() {
            LoadingScreen.Instance.SetLoadingMessage("Loading decorations...");
            float startTime = Time.realtimeSinceStartup;
            int numLoaded = 0;

            foreach (string path in _decorationPaths) {
                LoadDecoration(path);
                numLoaded++;

                if (Time.realtimeSinceStartup - startTime > GameManager.TargetDeltaTime) {
                    yield return null;
                    startTime = Time.realtimeSinceStartup;
                    GameManager.Instance.ReportLoaded(numLoaded);
                    numLoaded = 0;
                }
            }

            if (_decorations.Count == _decorationPaths.Count)
                DoDecoration();
            yield return null;
        }

        public void DoDecoration() {
            StartCoroutine("DecorationLoop");
        }

        IEnumerator DecorationLoop() {

            List<string> loadMessages = new List<string>() {
            "Planting cacti...",
            "Placing doodads...",
            "Landscaping...",
        };

            LoadingScreen.Instance.SetLoadingMessage(loadMessages.Random());
            float startTime = Time.realtimeSinceStartup;
            int numLoaded = 0;

            while (true) {
                if (_numDecorations < _maxDecorations) {
                    numLoaded += (AttemptDecorate() ? 1 : 0);

                    if (_numDecorations == _maxDecorations && !loaded) {
                        FinishLoading();
                        yield return null;
                    }

                    if (Time.realtimeSinceStartup - startTime > GameManager.TargetDeltaTime) {
                        yield return null;
                        startTime = Time.realtimeSinceStartup;
                        if (!loaded) GameManager.Instance.ReportLoaded(numLoaded);
                        numLoaded = 0;
                    }

                }
                else {
                    yield return null;
                }


            }

        }

        void UpdateColor() {

            float progress = timeOfDay / (Mathf.PI * 2f);

            primaryColor = primaryColors.Evaluate(progress);
            secondaryColor = secondaryColors.Evaluate(progress);

            sunLight.intensity = sunIntensityAxis + sunIntensityAmplitude * Mathf.Sin(timeOfDay);
            sunLight.color = primaryColor;
            sun.GetComponent<Sun>().ShadowCaster.intensity = sunLight.intensity / 2f;
            sun.GetComponent<Sun>().ShadowCaster.color = sunLight.color;

            moonLight.color = Color.white;
            moonLight.intensity = moonIntensityAxis + moonIntensityAmplitude * Mathf.Cos(timeOfDay - (Mathf.PI / 2f));
            moon.GetComponent<Moon>().ShadowCaster.intensity = moonLight.intensity / 4f;
            moon.GetComponent<Moon>().ShadowCaster.color = moonLight.color;

            RenderSettings.fogColor = secondaryColor;
            RenderSettings.ambientLight = secondaryColor;

            RenderSettings.skybox.SetFloat("_Value", skyboxFade.Evaluate(progress).a);

            if (Spectrum2.Instance != null)
				Spectrum2.Instance.SetColor (primaryColor);
        }

        private void UpdateTime() {
            timeOfDay += timeScale * Time.deltaTime;
            while (timeOfDay > (2 * Mathf.PI)) { //clamp timeOfDay between 0 and 2PI)
                timeOfDay -= 2 * Mathf.PI;
            }
        }

        bool AttemptDecorate() {

            // Pick a random decoration and decorate with it
            GameObject decoration;
            bool createNew = false;
            if (_decorationPool.Empty) {
                decoration = _decorations[UnityEngine.Random.Range(0, _decorations.Count)];
                createNew = true;
            }
            else decoration = _decorationPool.Peek().gameObject;

            //if (!createNew) Debug.Log("old");

            Decoration deco = decoration.GetComponent<Decoration>();

            int numActive = 0;
            int maxActive = 0;
            switch (deco.DecoGroup) {
                case Decoration.Group.RoadSigns:
                    numActive = _roadSignGroup.numActive;
                    maxActive = _roadSignGroup.maxActive;
                    break;
                case Decoration.Group.Rocks:
                    numActive = _rockGroup.numActive;
                    maxActive = _rockGroup.maxActive;
                    break;
                case Decoration.Group.Vegetation:
                    numActive = _vegetationGroup.numActive;
                    maxActive = _vegetationGroup.maxActive;
                    break;
            }
            if (numActive < maxActive) {
                switch (deco.DistributionType) {
                    case Decoration.Distribution.Random:
                        Chunk chunk = DynamicTerrain.Instance.RandomChunk();
                        if (chunk == null) return false;
                        return DecorateRandom(chunk, decoration, createNew);
                    case Decoration.Distribution.Roadside:
                        float bezierProg = UnityEngine.Random.Range(PlayerMovement.Instance.Progress, 1f);
                        return DecorateRoadside(bezierProg, decoration, createNew);
                    case Decoration.Distribution.CloseToRoad:
                        Chunk chunk2 = DynamicTerrain.Instance.RandomCloseToRoadChunk();
                        if (chunk2 == null) return false;
                        return DecorateRandom(chunk2, decoration, createNew);
                }
            }

            return false;
        }

        /*GameObject CreateSun(){
            GameObject sun = new GameObject ("Sun",
                typeof (Light),
                typeof (Sun),
                typeof (LensFlare)
            );

            Light light = sun.GetComponent<Light>();
            light.shadows = LightShadows.Soft;
            light.flare = sunFlare;

            LensFlare flare = sun.GetComponent<LensFlare>();
            flare.flare = sunFlare;
            //flare.

            return sun;
        }*/

        void CreateSun() {

        }

        void CreateMoon() {
            moon.GetComponent<SpriteRenderer>().sprite =
                moonSprites[UnityEngine.Random.Range(0, moonSprites.Count)];
        }

        /*GameObject CreateMoon(){
            GameObject moon = new GameObject ("Moon",
                typeof (Light),
                typeof (Moon),
                typeof (SpriteRenderer)
            );

            Light light = moon.GetComponent<Light>();
            light.shadows = LightShadows.Soft;

            // Random moon phase
            moon.GetComponent<SpriteRenderer>().sprite = 
                moonSprites[UnityEngine.Random.Range(0,moonSprites.Count)];
            return  moon;
        }*/

        Road CreateRoad() {

            // Create road object
            GameObject roadObj = new GameObject("Road",
                typeof(MeshFilter),
                typeof(MeshRenderer),
                typeof(Road)
            );

            // Change renderer properties
            MeshRenderer roadRenderer = roadObj.GetComponent<MeshRenderer>();
            roadRenderer.sharedMaterial = roadMaterial;
            roadRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;

            // Pass on road properties
            Road rd = roadObj.GetComponent<Road>();

            return rd;
        }

        bool DecorateRandom(Chunk chunk, GameObject decorationPrefab, bool createNew) {
            if (chunk == null) {
                Debug.LogError("WorldManager.DecorateRandom(): invalid chunk!");
                return false;
            }

            //if (!createNew) Debug.Log("old");

            // Pick a random coordinate
            Vector2 coordinate = new Vector2(
                chunk.X * _chunkSize + UnityEngine.Random.Range(-_chunkSize / 2f, _chunkSize / 2f),
                chunk.Y * _chunkSize + UnityEngine.Random.Range(-_chunkSize / 2f, _chunkSize / 2f)
            );

            // Find nearest vertex
            IntVector2 nearestVertex = Chunk.ToNearestVMapCoords(coordinate.x, coordinate.y);
            Vertex vert = DynamicTerrain.Instance.VertexMap.VertexAt(nearestVertex);
            if (vert == null) {
                Debug.LogError("WorldManager.DecorateRandom(): picked nonexistent vertex at " + nearestVertex.ToString());
                return false;
            }

            // Check if constrained
            if (DynamicTerrain.Instance.VertexMap.VertexAt(nearestVertex).noDecorations) {
                //Debug.Log(nearestVertex.ToString() + " was constrained, picked chunk "+chunk.name);
                return false;
            }

            // Roll based on density
            float density = decorationPrefab.GetComponent<Decoration>().Density;
            float spawnThreshold = density / DynamicTerrain.Instance.NumActiveChunks * _decorationDensity;

            // If roll succeeded
            if (!createNew || Mathf.PerlinNoise(coordinate.x, coordinate.y) < spawnThreshold) {

                // Instantiate or grab object
                GameObject decoration;
                if (createNew) decoration = (GameObject)Instantiate(decorationPrefab);
                else decoration = _decorationPool.Get().gameObject;
                Decoration deco = decoration.GetComponent<Decoration>();

                // Raycast down 
                RaycastHit hit;
                float y;
                Vector3 rayOrigin = new Vector3(coordinate.x, _heightScale, coordinate.y);
                if (Physics.Raycast(rayOrigin, Vector3.down, out hit, Mathf.Infinity)) y = hit.point.y;
                else y = 0f;

                // Transform decoration
                decoration.transform.position = new Vector3(coordinate.x, y, coordinate.y);

                // Parent decoration to chunk (if not dynamic)
                if (deco.Dynamic) decoration.transform.parent = DynamicTerrain.Instance.transform;
                else {
                    decoration.transform.parent = chunk.gameObject.transform;
                    chunk.AddDecoration(decoration);
                }

                // Register decoration
                _numDecorations++;
                DynamicTerrain.Instance.VertexMap.RegisterDecoration(nearestVertex, decoration);
                switch (deco.DecoGroup) {
                    case Decoration.Group.Rocks:
                        _rockGroup.numActive++;
                        break;
                    case Decoration.Group.Vegetation:
                        _vegetationGroup.numActive++;
                        break;
                }


                _decorationParticleEmitter.transform.position = decoration.transform.position;
                //ParticleSystem.EmitParams emitOverride = new ParticleSystem.EmitParams();
                //emitOverride.position = decoration.transform.position;
                //decorationParticleEmitter.Emit(emitOverride, 5);
                _decorationParticleEmitter.Emit(5);

                return true;
            }

            return false;
        }

        bool DecorateRoadside(float prog, GameObject decorationPrefab, bool createNew) {

            // Get road point
            Vector3 point = _road.GetPoint(prog);

            // Pick a road side
            int side = UnityEngine.Random.Range(0, 2); // 0 = player side, 1 = other side

            // Calculate coordinate
            Vector3 coordinate = point + _road.BezRight(point) *
                _roadWidth * UnityEngine.Random.Range(1.5f, 1.6f) * (side == 0 ? 1 : -1);

            // Find nearest chunk
            int chunkX = Mathf.RoundToInt((coordinate.x - _chunkSize / 2f) / _chunkSize);
            int chunkY = Mathf.RoundToInt((coordinate.z - _chunkSize / 2f) / _chunkSize);
            Chunk chunk = DynamicTerrain.Instance.ChunkAt(chunkX, chunkY);

            if (chunk == null) return false;

            // Raycast down
            RaycastHit hit;
            Vector3 rayOrigin = new Vector3(coordinate.x, _heightScale, coordinate.y);
            if (Physics.Raycast(rayOrigin, Vector3.down, out hit, Mathf.Infinity)) coordinate.y = hit.point.y;
            else coordinate.y = 0f;

            // Instantiate or grab decoration
            GameObject decoration;
            if (createNew) decoration =
                (GameObject)Instantiate(decorationPrefab, coordinate, Quaternion.Euler(Vector3.zero));
            else decoration = _decorationPool.Get().gameObject;

            // Randomize
            Decoration deco = decoration.GetComponent<Decoration>();
            deco.Randomize();

            // Point decoration in road direction
            Vector3 target = coordinate + _road.GetVelocity(prog);
            decoration.transform.LookAt(target, Vector3.up);
            decoration.transform.Rotate(-90f, side == 1 ? 180f : 0f, 0f);

            // Parent to nearest chunk
            decoration.transform.parent = chunk.gameObject.transform;
            chunk.AddDecoration(decoration);

            // Register
            _numDecorations++;
            if (deco.DecoGroup == Decoration.Group.RoadSigns) _roadSignGroup.numActive++;

            return true;
        }

        void LoadDecoration(string path) {
            GameObject decoration = (GameObject)Resources.Load(path);
            if (decoration == null) {
                Debug.LogError("Failed to load decoration at " + path);
            }
            else {
                //Debug.Log("Loaded "+path);
                _decorations.Add(decoration);
                //GameManager.Instance.IncrementLoadProgress();
            }
        }

        public void RemoveDecoration(GameObject deco) {
            Decoration d = deco.GetComponent<Decoration>();

            // Deparent decoration
            deco.transform.parent = null;


            // Deregister
            switch (d.DecoGroup) {
                case Decoration.Group.RoadSigns:
                    _roadSignGroup.numActive--;
                    break;
                case Decoration.Group.Rocks:
                    _rockGroup.numActive--;
                    break;
                case Decoration.Group.Vegetation:
                    _vegetationGroup.numActive--;
                    break;
            }
            _numDecorations--;

            // Pool decoration
            _decorationPool.Add(d);
        }

        /// <summary>
        /// Creates a lightning strike at a random point in view.
        /// </summary>
        public void LightningStrike(float strength) {

            // Find camera forward direction (flat)
            Vector3 forward = Camera.main.transform.forward;
            forward.y = 0f;
            forward.Normalize();

            // Define an offset
            Vector2 r = UnityEngine.Random.insideUnitCircle;
            Vector3 offset = new Vector3(400f * r.x, 250f, 400f * r.y);

            // Pick a point
            Vector3 origin = PlayerMovement.Instance.transform.position + forward * UnityEngine.Random.Range(0.9f, 1.1f) *
                _vertexUpdateDistance + offset;

            // Play sound
            // TODO

            // Enable lightning striker and move to point
            _lightningStriker.SetActive(true);
            _lightningStriker.transform.position = origin;
            _lightningStriker.GetComponent<Light>().intensity = _baseLightningIntensity * strength;
        }

        /// <summary>
        /// Creates a lightning flash within the clouds.
        /// </summary>
        /// <param name="strength">Strength.</param>
        public void LightningFlash(float strength) {

            // Find camera forward direction (flat)
            Vector3 forward = Camera.main.transform.forward;
            forward.y = 0f;
            forward.Normalize();

            // Define an offset
            Vector2 r = UnityEngine.Random.insideUnitCircle;
            Vector3 offset = new Vector3(800f * r.x, 800f, 800f * r.y);

            // Pick a point
            Vector3 origin = PlayerMovement.Instance.transform.position + forward * UnityEngine.Random.Range(1.9f, 2.1f) *
                _vertexUpdateDistance + offset;

            // Play sound
            // TODO

            // Enable lightning flash and move to point
            _lightningFlash.SetActive(true);
            _lightningFlash.transform.position = origin;
            _lightningFlash.GetComponent<Light>().intensity = _baseLightningIntensity * strength;
        }

        public void StarBurst() {
            _starEmitter.Emit(UnityEngine.Random.Range(10, 20));
        }

        public void ExhaustPuff() {
            foreach (ParticleSystem sys in _exhaustEmitters) sys.Emit(1);
        }

        public void ShootingStar() {

            // Find camera forward direction (flat)
            Vector3 forward = Camera.main.transform.forward;
            forward.y = 0f;
            forward.Normalize();

            // Define an offset
            Vector2 r = UnityEngine.Random.insideUnitCircle;
            Vector3 offset = new Vector3(1000f * r.x, 600f, 1000f * r.y);

            // Pick a point
            Vector3 origin = PlayerMovement.Instance.transform.position + forward * UnityEngine.Random.Range(2.9f, 3.1f) *
                _vertexUpdateDistance + offset;

            GameObject shootingStar = (GameObject)Instantiate(_shootingStarTemplate, origin, Quaternion.identity);
            shootingStar.SetActive(true);
        }

        public void DeformRandom() {
            // Find camera forward direction (flat)
            Vector3 forward = Camera.main.transform.forward;
            forward.y = 0f;
            forward.Normalize();

            // Define an offset
            Vector2 r = UnityEngine.Random.insideUnitCircle;
            Vector3 offset = new Vector3(400f * r.x, 0f, 400f * r.y);

            // Pick a point
            Vector3 origin = PlayerMovement.Instance.transform.position + forward * UnityEngine.Random.Range(0.9f, 1.1f) *
                _vertexUpdateDistance + offset;

            IntVector2 coords = Chunk.ToNearestVMapCoords(origin.x, origin.z);
            Vertex v = DynamicTerrain.Instance.VertexMap.VertexAt(coords);
            if (v == null) v = DynamicTerrain.Instance.VertexMap.AddVertex(coords);
            if (!v.locked && !v.nearRoad) v.SmoothHeight(v.height + _heightScale / 32f, 0.95f);

            //Debug.Log(v.ToString());
        }

        public void ShowWeatherEffect(Note note) {

            switch (note.PercussionType) {

                // If cymbal, create shooting star
                case Note.PercType.Cymbal:
                    ShootingStar();
                    break;
                
                // If hat, create stars
                case Note.PercType.Hat:
                    StarBurst();
                    break;
                
                // If kick or tom, cause lightning flash
                case Note.PercType.Kick:
                    LightningFlash(note.Volume);
                    break;
                case Note.PercType.Tom:
                    LightningFlash(0.75f * note.Volume);
                    break;
                
                // If shaker, increase rain density
                case Note.PercType.Shaker:
                    _shakers++;
                    break;

                // If snare, cause lightning strike
                case Note.PercType.Snare:
                    LightningStrike(note.Volume);
                    break;

                // If wood, create exhaust puff
                case Note.PercType.Wood:
                    ExhaustPuff();
                    break;
        }
    }

    public void DebugTerrain() {
        DynamicTerrain.Instance.SetDebugColors(DynamicTerrain.DebugColors.Constrained);
    }

    public void PrintVertexMap() {
        VertexMap vmap = DynamicTerrain.Instance.VertexMap;
        string log = "";
        for (int i = vmap.xMin; i <= vmap.xMax; i++) {
            for (int j = vmap.yMin; j <= vmap.yMax; j++) {
                Vertex vert = vmap.VertexAt(i, j);
                log += "[" + (vert != null ? (vert.height < 0f ? "-" : " ") + vert.height.ToString("000") : "    ") + "]";
            }
            log += "\n";
        }
        System.IO.File.WriteAllText(Application.persistentDataPath + "/vmap.txt", log);
    }

    #endregion

}
}
