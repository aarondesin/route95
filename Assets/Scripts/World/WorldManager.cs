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
		[SerializeField]
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

		bool _initialDecoration = true;

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
        [Range(1, 50)]
		[SerializeField]
        int _decorationDensity = 25;

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
        int _rainDensity;

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

		/// <summary>
		/// Width of generated road.
		/// </summary>
        [Tooltip("Width of generated road.")]
        [Range(1f, 20f)]
		[SerializeField]
        float _roadWidth = DEFAULT_ROAD_WIDTH;

		/// <summary>
		/// Height of generated road.
		/// </summary>
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

		/// <summary>
		/// Number of mesh subdivisions per road segment.
		/// </summary>
        [Tooltip("Number of mesh subdivisions per road segment.")]
		[SerializeField]
        int _roadStepsPerCurve = DEFAULT_ROAD_STEPS_PER_CURVE;

		/// <summary>
		/// Distance from player for road to try to extend.
		/// </summary>
        float _roadExtendRadius = DEFAULT_ROAD_EXTEND_RADIUS;

        /// <summary>
		/// Distance from player for road to cleanup points.
		/// </summary>
        float _roadCleanupRadius;

		/// <summary>
		/// Radius within which to place road.
		/// </summary>
        [Tooltip("Radius within which to place road.")]
		[SerializeField]
        float _roadPlacementDistance = DEFAULT_ROAD_PLACEMENT_DISTANCE;

		/// <summary>
		/// Percentage radius of road placement distance within which to place road.
		/// </summary>
        [Tooltip("Percentage radius of road placement distance within which to place road.")]
		[SerializeField]
        float _roadVariance = DEFAULT_ROAD_VARIANCE;

		/// <summary>
		/// Max road slope per world unit of distance.
		/// </summary>
        [Tooltip("Max road slope per world unit of distance.")]
		[SerializeField]
        float _roadMaxSlope = DEFAULT_ROAD_MAX_SLOPE;

		[SerializeField]
		float _roadClearDistance;

		/// <summary>
		/// Reference to the road object.
		/// </summary>
        [NonSerialized]
        Road _road;

		/// <summary>
		/// Material to use for road.
		/// </summary>
        [Tooltip("Material to use for road.")]
		[SerializeField]
        Material _roadMaterial;

        //-----------------------------------------------------------------------------------------------------------------
        [Header("Day/Night Cycle Settings")]

		/// <summary>
		/// Global time scale for day/night cycle.
		/// </summary>
        [Tooltip("Global time scale for day/night cycle.")]
        [Range(0.001f, 0.1f)]
		[SerializeField]
        float _timeScale = DEFAULT_TIME_SCALE;

		/// <summary>
		/// Current time of day.
		/// </summary>
        [Tooltip("Current time of day.")]
        [Range(0f, 2f * Mathf.PI)]
		[SerializeField]
        float _timeOfDay;

		/// <summary>
		/// Daytime intensity of the sun.
		/// </summary>
        [Tooltip("Daytime intensity of the sun.")]
        [Range(0f, 8f)]
		[SerializeField]
        float _maxSunIntensity;

		/// <summary>
		/// Nighttime intensity of the sun.
		/// </summary>
        [Tooltip("Nighttime intensity of the sun.")]
        [Range(0f, 8f)]
		[SerializeField]
        float _minSunIntensity;

		/// <summary>
		/// Axis of sun intensity oscillation.
		/// </summary>
        float _sunIntensityAxis;

		/// <summary>
		/// Amplitude of sun intensity oscillation.
		/// </summary>
        float _sunIntensityAmplitude;

		/// <summary>
		/// Flare texture to use for the sun.
		/// </summary>
        [Tooltip("Flare texture to use for the sun.")]
		[SerializeField]
        Flare _sunFlare;

		/// <summary>
		/// Nighttime intensity of the moon.
		/// </summary>
        [Tooltip("Nighttime intensity of the moon.")]
        [Range(0f, 8f)]
		[SerializeField]
        float _maxMoonIntensity;

		/// <summary>
		/// Daytime intensity of the moon.
		/// </summary>
        [Tooltip("Daytime intensity of the moon.")]
        [Range(0f, 8f)]
		[SerializeField]
        float _minMoonIntensity;

		/// <summary>
		/// Axis of moon intensity oscillation.
		/// </summary>
        float _moonIntensityAxis;

		/// <summary>
		/// Amplitude of moon intensity oscillation.
		/// </summary>
        float _moonIntensityAmplitude;

		/// <summary>
		/// Sprites to randomize for the moon.
		/// </summary>
        [Tooltip("Sprites to randomize for the moon.")]
		[SerializeField]
        List<Sprite> _moonSprites;

		/// <summary>
		/// Current primary color.
		/// </summary>
        [Tooltip("Current primary color.")]
        [SerializeField]
        Color _primaryColor;

		/// <summary>
		/// Current secondary color.
		/// </summary>
        [Tooltip("Current secondary color.")]
        [SerializeField]
        Color _secondaryColor;

		/// <summary>
		/// Primary color cycle.
		/// </summary>
        [Tooltip("Primary color cycle.")]
		[SerializeField]
        Gradient _primaryColors;

		/// <summary>
		/// Secondary color cycle.
		/// </summary>
        [Tooltip("Secondary color cycle.")]
		[SerializeField]
        Gradient _secondaryColors;

		/// <summary>
		/// Skybox transition cycle.
		/// </summary>
        [Tooltip("Skybox transition cycle.")]
		[SerializeField]
        Gradient _skyboxFade;

        //-----------------------------------------------------------------------------------------------------------------
        [Header("Performance Settings")]

		/// <summary>
		/// Maximum number of chunk updates per cycle.
		/// </summary>
        [Tooltip("Maximum number of chunk updates per cycle.")]
        [Range(1, 16)]
		[SerializeField]
        int _chunkUpdatesPerCycle = DEFAULT_CHUNK_UPDATES_PER_CYCLE;

		/// <summary>
		/// Resolution used in frequency spectrum analysis. Must be a power of 2.
		/// </summary>
        [Tooltip("Resolution used in frequency spectrum analysis. Must be a power of 2.")]
		[SerializeField]
        int _frequencySampleSize = DEFAULT_FREQ_ARRAY_SIZE;

		/// <summary>
		/// FFT window to use when sampling music frequencies.
		/// </summary>
        [Tooltip("FFT window to use when sampling music frequencies.")]
		[SerializeField]
        FFTWindow _frequencyFFTWindow;

		/// <summary>
		/// Maximum number of decorations to place per update cycle.
		/// </summary>
        [Tooltip("Maximum number of decorations to place per update cycle.")]
        [Range(10, 200)]
		[SerializeField]
        int _decorationsPerStep = DEFAULT_DECORATIONS_PER_STEP;

		/// <summary>
		/// Maximum number of grass particles to place per chunk.
		/// </summary>
        [Tooltip("Maximum number of grass particles to place per chunk.")]
        [Range(0, 100)]
		[SerializeField]
        int _grassPerChunk;

		/// <summary>
		/// The accuracy used in road distance checks.
		/// </summary>
        [Tooltip("The accuracy used in road distance checks.")]
        [Range(1f, 500f)]
		[SerializeField]
        float _roadPathCheckResolution = DEFAULT_ROAD_PATH_CHECK_RESOLUTION;

		/// <summary>
		/// Time at which loading started.
		/// </summary>
        float _loadStartTime;

		/// <summary>
		/// Is WorldManager loaded?
		/// </summary>
        bool _loaded = false;

		/// <summary>
		/// Is terrain loaded?
		/// </summary>
        bool _loadedTerrain = false;

		/// <summary>
		/// 
		/// </summary>
        bool _hasRandomized = false;

        #endregion
        #region Unity Callbacks

        new void Awake() {
            base.Awake();

            _maxDecorations = _roadSignGroup.maxActive + _rockGroup.maxActive + _vegetationGroup.maxActive;
            //Debug.Log(maxDecorations);

            _wind = UnityEngine.Random.insideUnitSphere;

            //roadPlacementDistance = chunkSize * 2f;
            _roadCleanupRadius = _chunkSize * (_chunkLoadRadius);
            _roadPlacementDistance = _chunkSize * 0.4f;
            _roadExtendRadius = _chunkSize * (_chunkLoadRadius / 2);
            _road = CreateRoad();

            _numDecorations = 0;
            _decorationPool = new ObjectPool<Decoration>();

            _timeOfDay = UnityEngine.Random.Range(0, 2 * Mathf.PI);
            _sunIntensityAmplitude = (_maxSunIntensity - _minSunIntensity) / 2f;
            _sunIntensityAxis = _minSunIntensity + _sunIntensityAmplitude;

            _moonIntensityAmplitude = (_maxMoonIntensity - _maxMoonIntensity) / 2f;
            _moonIntensityAxis = _minMoonIntensity + _moonIntensityAmplitude;

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
            if (_loadedTerrain && !_hasRandomized)
                Load();
            if (_loaded && _road.IsLoaded) {

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
                _rainDensity = Mathf.FloorToInt(_minRainDensity + rd * (_maxRainDensity - _minRainDensity));
                _rainEmitter.SetRate(_rainDensity);

                _starEmitter.SetRate(0.5f * _starEmissionRate * -Mathf.Sin(_timeOfDay) + _starEmissionRate / 2f);
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

		public float TimeOfDay {
			get { return _timeOfDay; }
			set { _timeOfDay = value; }
		}

		public Road Road { get { return _road; } }

		public float RoadWidth { get { return _roadWidth; } }

		public float RoadHeight { get { return _roadHeight; } }

		public float RoadSlope { get { return _roadSlope; } } 

		public float RoadMaxSlope {
			get { return _roadMaxSlope; }
			set { _roadMaxSlope = value; }
		}

		public float RoadCleanupRadius { get { return _roadCleanupRadius; } }

		public float RoadExtendRadius { get { return _roadExtendRadius; } }

		public float RoadPlacementDistance { get { return _roadPlacementDistance; } }

		public float RoadPathCheckResolution { get { return _roadPathCheckResolution; } }

		public float RoadVariance {
			get { return _roadVariance; }
			set { _roadVariance = value; }
		}

		public int RoadStepsPerCurve { get { return _roadStepsPerCurve; } }

		public float RoadClearDistance { get { return _roadClearDistance; } }

		public FFTWindow FrequencyFFTWindow { get { return _frequencyFFTWindow; } }

		public int FrequencySampleSize { get { return _frequencySampleSize; } }

		public Material TerrainMaterial { get { return _terrainMaterial; } }

		public Material TerrainDebugMaterial { get { return _terrainDebugMaterial; } }

		public GameObject GrassEmitterPrefab { get { return _grassEmitterPrefab; } }

		public ParticleSystem DecorationParticleEmitter { get { return _decorationParticleEmitter; } }

		public int DecorationsPerStep { get { return _decorationsPerStep; } }

		public int GrassPerChunk { get { return _grassPerChunk; } }

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

		public int DecorationDensity {
			get { return _decorationDensity; }
			set { _decorationDensity = value; }
		}

		#endregion
		#region WorldManager Methods

		public void Load() {

            Camera.main.GetComponent<SunShafts>().sunTransform = Sun.Instance.transform;

            // Get start time
            _loadStartTime = Time.realtimeSinceStartup;

            // Start by loading chunks
            DynamicTerrain.Instance.DoLoadChunks();
        }

        public void FinishLoading() {

            _loaded = true;

            // Print time taken
            Debug.Log("WorldManager.Load(): finished in " + (Time.realtimeSinceStartup - _loadStartTime).ToString("0.0000") + " seconds.");

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

                    if (_numDecorations == _maxDecorations && !_loaded) {
                        FinishLoading();
                        yield return null;
                    }

                    if (Time.realtimeSinceStartup - startTime > GameManager.LoadingTargetDeltaTime) {
                        startTime = Time.realtimeSinceStartup;
                        if (!_loaded) GameManager.Instance.ReportLoaded(numLoaded);
                        numLoaded = 0;
						yield return null;
                    }

                }
                else {
                    yield return null;
                }


            }

        }

        void UpdateColor() {

            float progress = _timeOfDay / (Mathf.PI * 2f);

            _primaryColor = _primaryColors.Evaluate(progress);
            _secondaryColor = _secondaryColors.Evaluate(progress);

            Sun.Instance.Light.intensity = _sunIntensityAxis + _sunIntensityAmplitude * Mathf.Sin(_timeOfDay);
            Sun.Instance.Light.color = _primaryColor;
            Sun.Instance.ShadowCaster.intensity = Sun.Instance.Light.intensity / 2f;
            Sun.Instance.ShadowCaster.color = Sun.Instance.Light.color;

            Moon.Instance.Light.color = Color.white;
            Moon.Instance.Light.intensity = _moonIntensityAxis + _moonIntensityAmplitude * Mathf.Cos(_timeOfDay - (Mathf.PI / 2f));
            Moon.Instance.ShadowCaster.intensity = Moon.Instance.Light.intensity / 4f;
            Moon.Instance.ShadowCaster.color = Moon.Instance.Light.color;

            RenderSettings.fogColor = _secondaryColor;
            RenderSettings.ambientLight = _secondaryColor;

            RenderSettings.skybox.SetFloat("_Value", _skyboxFade.Evaluate(progress).a);

            if (Spectrum2.Instance != null)
				Spectrum2.Instance.SetColor (_primaryColor);
        }

        private void UpdateTime() {
            _timeOfDay += _timeScale * Time.deltaTime;
            while (_timeOfDay > (2 * Mathf.PI)) { //clamp timeOfDay between 0 and 2PI)
                _timeOfDay -= 2 * Mathf.PI;
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

        Road CreateRoad() {

            // Create road object
            GameObject roadObj = new GameObject("Road",
                typeof(MeshFilter),
                typeof(MeshRenderer),
                typeof(Road)
            );

            // Change renderer properties
            MeshRenderer roadRenderer = roadObj.GetComponent<MeshRenderer>();
            roadRenderer.sharedMaterial = _roadMaterial;
            roadRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;

            // Pass on road properties
            Road rd = roadObj.GetComponent<Road>();

            return rd;
        }

        bool DecorateRandom(Chunk chunk, GameObject decorationPrefab, bool createNew) {

			// Check for invalid chunk
            if (chunk == null) {
                Debug.LogError("WorldManager.DecorateRandom(): invalid chunk!");
                return false;
            }

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
            if (DynamicTerrain.Instance.VertexMap.VertexAt(nearestVertex).NoDecorations) {
                return false;
            }

            // Roll based on density
            float density = decorationPrefab.GetComponent<Decoration>().Density;
            float spawnThreshold = (float)density / DynamicTerrain.Instance.NumActiveChunks * _decorationDensity;
			bool roll = Mathf.PerlinNoise(coordinate.x, coordinate.y) < spawnThreshold || !_loaded;

            // If roll succeeded
            if (!createNew || roll) {

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
            } else {
				bool valid = CheckDecoration (decoration.GetComponent<Decoration>());
				if (valid)_decorations.Add(decoration);
            }
        }

		bool CheckDecoration (Decoration decoration) {
			if (decoration.DistributionType == Decoration.Distribution.None) {
				Debug.LogError ("Decoration " + decoration.name + " has no distribution type!");
				return false;
			}

			if (decoration.Density <= 0f) {
				Debug.LogError ("Decoration " + decoration.name + " has a zero or negative distribution!");
				return false;
			}

			if (decoration.DecoGroup == Decoration.Group.None) {
				Debug.LogError ("Decoration " + decoration.name + " has no group!");
				return false;
			}

			return true;
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
            if (!v.Locked && !v.NearRoad) v.SmoothHeight(v.Height + _heightScale / 32f, 0.95f);

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

	public void ForceBulldoze (float start, float end) {
			_road.DoBulldoze (start, end);
		}

    public void DebugTerrain() {
        DynamicTerrain.Instance.SetDebugColors(DynamicTerrain.DebugColors.Constrained);
    }

    public void PrintVertexMap() {
        VertexMap vmap = DynamicTerrain.Instance.VertexMap;
        string log = "";
        for (int i = vmap.XMin; i <= vmap.XMax; i++) {
            for (int j = vmap.YMin; j <= vmap.YMax; j++) {
                Vertex vert = vmap.VertexAt(i, j);
                log += "[" + (vert != null ? (vert.Height < 0f ? "-" : " ") + vert.Height.ToString("000") : "    ") + "]";
            }
            log += "\n";
        }
        System.IO.File.WriteAllText(Application.persistentDataPath + "/vmap.txt", log);
    }

    #endregion

}
}
