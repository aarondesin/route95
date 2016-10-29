// PlayerMovement.cs
// ©2016 Team 95

using Route95.Core;
using Route95.Music;
using Route95.UI;

using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Route95.World {

	/// <summary>
	/// Ckass to handle player movement.
	/// </summary>
    public class PlayerMovement : SingletonMonoBehaviour<PlayerMovement> {

		#region Vars

		/// <summary>
		/// Car headlights.
		/// </summary>
		[Tooltip("Car headlights.")]
		[SerializeField]
		List<Light> _headlights;

		/// <summary>
		/// Tempo to velocity ratio.
		/// </summary>
        const float DIST_PER_BEAT = 0.0012f;

		/// <summary>
		/// Particle emission rate to distance ratio.
		/// </summary>
        const float PARTICLES_PER_DIST = 100f;

		/// <summary>
		/// Distance to look ahead.
		/// </summary>
        const float LOOK_AHEAD_DIST = 0.01f;

		/// <summary>
		/// 
		/// </summary>
        bool _moving = false;

        bool _lightsOn = false;

		/// <summary>
		/// Progress along road.
		/// </summary>
        float _progress = 0f;

		/// <summary>
		/// Dust emitters.
		/// </summary>
        List<ParticleSystem> _particles;

		/// <summary>
		/// 
		/// </summary>
        float _minVelocity;

		/// <summary>
		/// 
		/// </summary>
        float _maxVelocity;

		/// <summary>
		/// 
		/// </summary>
        float _velocity = 0f;

		/// <summary>
		/// 
		/// </summary>
        const float VELOCITY_TO_ROTATION = 10000f;

		/// <summary>
		/// 
		/// </summary>
        Vector3 _target;

		/// <summary>
		/// 
		/// </summary>
        float _offsetH = 0f;

		/// <summary>
		/// 
		/// </summary>
        float _velocityOffset = 0f;

		/// <summary>
		/// 
		/// </summary>
        List<ReflectionProbe> _reflectionProbes;

		/// <summary>
		/// 
		/// </summary>
        bool _initialized = false;

		/// <summary>
		/// 
		/// </summary>
        float _dOffset;

		GameObject _FRWheel;
		GameObject _FLWheel;
		GameObject _RRWheel;
		GameObject _RLWheel;

		#endregion
		#region Unity Callbacks

		// Use this for initialization
		new void Awake() {
			// Init vars
            _FRWheel = GameObject.FindGameObjectWithTag("FrontRightWheel");
			_FLWheel = GameObject.FindGameObjectWithTag("FrontLeftWheel");
			_RRWheel = GameObject.FindGameObjectWithTag("RearRightWheel");
			_RLWheel = GameObject.FindGameObjectWithTag("RearLeftWheel");

            //prevPosition = Vector3.zero;
            _reflectionProbes = GetComponentsInChildren<ReflectionProbe>().ToList();

            _target = new Vector3(0f, 0f, 0f);

            _progress = 0f;
            StopMoving();

            _minVelocity = MusicManager.TempoToFloat[Tempo.Slowest] * DIST_PER_BEAT;
            _maxVelocity = MusicManager.TempoToFloat[Tempo.Fastest] * DIST_PER_BEAT;

            GetComponent<AudioSource>().volume = 0.0f;

            _dOffset = 0f;
        }

        void Start() {
			UIManager.Instance.onSwitchToPlaylistMenu.AddListener(()=> {
				StopMoving();
			});

			UIManager.Instance.onSwitchToLiveMode.AddListener(StartMoving);

			UIManager.Instance.onSwitchToPostplayMode.AddListener(StopMoving);
        }

		// Update is called once per frame
        void FixedUpdate() {
            if (!_initialized)
                Initialize();

            if (Sun.Instance != null) {
                if (_moving && !GameManager.Instance.Paused) {

					Road road = WorldManager.Instance.Road;

                    _dOffset += (Mathf.PerlinNoise(Random.Range(0f, 1f), 0f) - 0.5f);
                    _velocityOffset = Mathf.Clamp(_velocityOffset + _dOffset, _minVelocity, _maxVelocity);

                    _velocity = MusicManager.TempoToFloat[MusicManager.Instance.Tempo] * DIST_PER_BEAT + _velocityOffset;

                    _progress += _velocity * Time.fixedDeltaTime / road.CurveCount;
                    if (_progress >= 1f)
                        _progress = 1f;

                    _offsetH += (Mathf.PerlinNoise(Random.Range(0f, 1f), 0f) - Random.Range(0f, _offsetH)) * Time.deltaTime;
                    Vector3 offset = new Vector3(_offsetH, 2.27f + WorldManager.Instance.RoadHeight, 0f);
                    Vector3 point = road.GetPoint(_progress);
                    Vector3 ahead = point + road.GetVelocity(_progress);
                    transform.position = point + offset -
                        road.BezRight(point) * WorldManager.Instance.RoadWidth / 3f;

                    transform.LookAt(ahead);
                    float rotation = _velocity * VELOCITY_TO_ROTATION * Time.deltaTime;

                    _FLWheel.transform.Rotate(new Vector3(rotation, 0f, 0f), Space.Self);
                    _FRWheel.transform.Rotate(new Vector3(rotation, 0f, 0f), Space.Self);
                    _RLWheel.transform.Rotate(new Vector3(rotation, 0f, 0f), Space.Self);
                    _RRWheel.transform.Rotate(new Vector3(rotation, 0f, 0f), Space.Self);

                    foreach (ParticleSystem particle in _particles) {
                        var emission = particle.emission;
                        var rate = emission.rate;
                        rate.constantMax = _velocity * PARTICLES_PER_DIST;
                        emission.rate = rate;
                    }

                    GetComponent<AudioSource>().pitch = Mathf.Clamp(0.75f + (_velocity - _minVelocity) / (_maxVelocity - _minVelocity), 0.75f, 1.75f);

                }

                _lightsOn = (WorldManager.Instance.timeOfDay > (Mathf.PI * (7f / 8f))
                    || WorldManager.Instance.timeOfDay <= Mathf.PI * (1f / 8f));
				foreach (Light light in _headlights) light.enabled = _lightsOn;
            }
        }

        void OnDrawGizmosSelected() {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(_target, 1f);
        }

		#endregion
		#region Properties

		public bool Moving { get { return _moving; } }

		public float Progress {
			get { return _progress; }
			set { _progress = value; }
		}

		#endregion
		#region Methods

		public void StartMoving() {
            _moving = true;
            foreach (ParticleSystem ps in _particles) ps.Play();
        }

        public void StopMoving() {
            _moving = false;
            foreach (ParticleSystem ps in _particles) ps.Pause();
            GetComponent<AudioSource>().Stop();
        }

        public void DisableReflections() {
            foreach (ReflectionProbe probe in _reflectionProbes)
                probe.enabled = false;
        }

        public void EnableReflections() {
            foreach (ReflectionProbe probe in _reflectionProbes)
                probe.enabled = true;
        }

        void Initialize() {
            _initialized = true;
        }
    }

	#endregion
}
