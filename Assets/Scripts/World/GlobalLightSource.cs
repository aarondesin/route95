// GlobalLightSource.cs
// ©2016 Team 95

using UnityEngine;

namespace Route95.World {

	/// <summary>
	/// Base class for large light sources (sun and moon).
	/// </summary>
	public abstract class GlobalLightSource<T> : SingletonMonoBehaviour<T> where T: MonoBehaviour {

		#region Vars

		/// <summary>
		/// Reference to this light component.
		/// </summary>
		protected Light _light;

		/// <summary>
		/// Light to use for shadow casting.
		/// </summary>
		[SerializeField]
		[Tooltip("Light to use for shadow casting.")]
        protected Light _shadowCaster;

		/// <summary>
		/// Orbit radius.
		/// </summary>
		[SerializeField]
		[Tooltip("Orbit radius.")]
        protected float _radius;

		/// <summary>
		/// Scale.
		/// </summary>
		[Tooltip("Scale.")]
		[SerializeField]
        protected float _scale;

		/// <summary>
		/// Target for the sun to point at: the car or the origin.
		/// </summary>
        protected Vector3 _target;

		#endregion
		#region Unity Callbacks

		new protected void Awake () {
			base.Awake();

			// Init vars
			_light = GetComponent<Light>();
		}

		protected void Start () {
			transform.SetParent(PlayerMovement.Instance.transform);
			transform.localScale = new Vector3(_scale, _scale, _scale);
		}

		void Update() {
            UpdateTransform();
        }

		#endregion
		#region Properties

		public Light Light { get { return _light; } }

		public Light ShadowCaster { get { return _shadowCaster; } }

		#endregion
		#region Methods

		protected void UpdateTransform() {
            _target = PlayerMovement.Instance.transform.position;

            float newX = -_radius * Mathf.Cos(WorldManager.Instance.TimeOfDay);
            float newY = -_radius * Mathf.Sin(WorldManager.Instance.TimeOfDay);
            float newZ = -_radius * Mathf.Cos(WorldManager.Instance.TimeOfDay + Mathf.PI / 5);
            transform.position = new Vector3(newX, newY, newZ);

            transform.LookAt(_target);
        }

		#endregion
	}
}
