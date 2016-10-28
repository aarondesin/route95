// Decoration.cs
// ©2016 Team 95

using Route95.Core;

using System.Collections;

using UnityEngine;

namespace Route95.World {

    /// <summary>
    /// Class to store decoration data.
    /// </summary>
    public class Decoration : MonoBehaviour, IPoolable {

        #region Decoration Enums

        /// <summary>
        /// Type of decoration distribution.
        /// </summary>
        public enum Distribution {
            Random,     // Truly random, based on density
            Roadside,   // For signs, placed alongside road facing either direction
            CloseToRoad // Placed close to road (good for small objects)
        }

        /// <summary>
        /// Group to assign decoration to.
        /// </summary>
        public enum Group {
            None,
            Vegetation,
            Rocks,
            RoadSigns
        }

        #endregion
        #region Decoration Structs

        /// <summary>
        /// Info for relevant decoration group.
        /// </summary>
        [System.Serializable]
        public struct GroupInfo {
            public Group group;
            public int numActive;
            public int maxActive;
        }

        #endregion
        #region Decoration Vars

		/// <summary>
		/// Decoration group.
		/// </summary>
        [Tooltip("Decoration group.")]
		[SerializeField]
        Group _group = Group.None;

		/// <summary>
		/// Is this decoration dynamic/physics-enabled?
		/// </summary>
        [Tooltip("Is this decoration dynamic/physics-enabled?")]
		[SerializeField]
        bool _dynamic = false;

		/// <summary>
		/// Average number of this decoration per chunk.
		/// </summary>
        [Tooltip("Average number of this decoration per chunk.")]
		[SerializeField]
        float _density;

		/// <summary>
		/// Distribution type to use for this decoration.
		/// </summary>
        [Tooltip("Distribution type to use for this decoration.")]
		[SerializeField]
        Distribution _distribution;

		/// <summary>
		/// Base position offset to use when placing.
		/// </summary>
        [Tooltip("Base position offset to use when placing.")]
		[SerializeField]
        Vector3 _positionOffset;

		/// <summary>
		/// Base rotation offset to use when placing.
		/// </summary>
        [Tooltip("Base rotation offset to use when placing.")]
		[SerializeField]
        Vector3 _rotationOffset;

		/// <summary>
		/// Range of randomization in height.
		/// </summary>
        [Tooltip("Range of randomization in height.")]
		[SerializeField]
        Vector2 _heightRange;

		/// <summary>
		/// Range of randomization in width.
		/// </summary>
        [Tooltip("Range of randomization in width.")]
		[SerializeField]
        Vector2 _widthRange;

		/// <summary>
		/// Range of randomization in pitch.
		/// </summary>
        [Tooltip("Range of randomization in pitch.")]
		[SerializeField]
        Vector2 _pitchRange;

		/// <summary>
		/// Range of randomization in yaw.
		/// </summary>
        [Tooltip("Range of randomization in yaw.")]
		[SerializeField]
        Vector2 _yawRange;
	
		/// <summary>
		/// Range of randomization in roll.
		/// </summary>
        [Tooltip("Range of randomization in roll.")]
		[SerializeField]
        Vector2 _rollRange;

		/// <summary>
		/// Height animation curve.
		/// </summary>
		[Tooltip("Height animation curve.")]
		[SerializeField]
        AnimationCurve _heightAnimation;

		/// <summary>
		/// Width animation curve.
		/// </summary>
		[Tooltip("Width animation curve.")]
		[SerializeField]
        AnimationCurve _widthAnimation;

		/// <summary>
		/// Animation speed.
		/// </summary>
		[Tooltip("Animation speed.")]
		[SerializeField]
        float _animationSpeed = 0.5f;

		/// <summary>
		/// Scale backup.
		/// </summary>
        Vector3 _normalScale;

		/// <summary>
		/// Particle system.
		/// </summary>
        ParticleSystem _partSystem;

        #endregion
        #region Unity Callbacks

        void Awake() {
            _normalScale = transform.localScale;
            _partSystem = ((GameObject)Instantiate(WorldManager.Instance.decorationParticleEmitter.gameObject)).GetComponent<ParticleSystem>();
            _partSystem.gameObject.transform.parent = transform.parent;
        }

        void FixedUpdate() {
            if (_dynamic) {

                // Remove if decoration fell below
                if (transform.position.y < -WorldManager.Instance.heightScale)
                    WorldManager.Instance.RemoveDecoration(gameObject);

                // Push with wind
                GetComponent<Rigidbody>().AddForce(WorldManager.Instance.wind);
            }
        }

        #endregion
        #region IPoolable Implementations

        void IPoolable.OnPool() {
            transform.parent = null;
            gameObject.SetActive(false);
        }

        void IPoolable.OnDepool() {
            gameObject.SetActive(true);
            Randomize();
        }

		#endregion
		#region Properties

		public Group DecoGroup { get { return _group; } }

		public Distribution DistributionType { get { return _distribution; } }

		public bool Dynamic { get { return _dynamic; } }

		public float Density { get { return _density; } }

		public Vector3 PositionOffset { get { return _positionOffset; } }

		#endregion
		#region Methods

		// Starts with base position/rotation, and adds variance
		public void Randomize() {
            transform.localScale = _normalScale;
            transform.position += _positionOffset;
            transform.rotation = Quaternion.Euler(_rotationOffset.x, _rotationOffset.y, _rotationOffset.z);

            // Randomize scale (width and height)
            transform.localScale = new Vector3(
                transform.localScale.x * Random.Range(_widthRange[0], _widthRange[1]),
                transform.localScale.y * Random.Range(_heightRange[0], _heightRange[1]),
                transform.localScale.z * Random.Range(_widthRange[0], _widthRange[1])
            );

            // Randomize rotation
            transform.Rotate(new Vector3(
                Random.Range(_pitchRange[0], _pitchRange[1]),
                Random.Range(_yawRange[0], _yawRange[1]),
                Random.Range(_rollRange[0], _rollRange[1])
            ), Space.World);

            _partSystem.transform.position = transform.position;
            _partSystem.Play();

            StartCoroutine(Animate());
        }

        IEnumerator Animate() {
            Vector3 baseScale = transform.localScale;
            float progress = 0f;
            while (progress < 1f) {
                Vector3 scale = new Vector3(
                    baseScale.x * _widthAnimation.Evaluate(progress),
                    baseScale.y * _heightAnimation.Evaluate(progress),
                    baseScale.z * _widthAnimation.Evaluate(progress)
                );
                transform.localScale = scale;
                progress += _animationSpeed * Time.deltaTime;
                yield return null;
            }
            yield break;
        }

        #endregion
    }
}
