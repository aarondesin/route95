// ShootingStar.cs
// ©2016 Team 95

using UnityEngine;

namespace Route95.World {

	/// <summary>
	/// Class to handle shooting stars.
	/// </summary>
	public class ShootingStar : MonoBehaviour {

		#region Vars

		/// <summary>
		/// Maximum velocity.
		/// </summary>
		const float MAX_V = 100f;

		/// <summary>
		/// Gradient to plot alpha over time.
		/// </summary>
		[SerializeField]
		[Tooltip("Gradient to plot alpha over time.")]
		Gradient _alpha;

		/// <summary>
		/// Reference to this object's sprite renderer.
		/// </summary>
		SpriteRenderer _renderer;

		/// <summary>
		/// Minimum lifetime.
		/// </summary>
		[SerializeField]
		[Tooltip("Minimum lifetime.")]
		float _minLifetime;

		/// <summary>
		/// Maximum lifetime.
		/// </summary>
		[SerializeField]
		[Tooltip("Maximum lifetime.")]
		float _maxLifetime;

		/// <summary>
		/// Chosen lifetime.
		/// </summary>
		float _lifetime;

		/// <summary>
		/// Current lifetime left.
		/// </summary>
		float _life;

		/// <summary>
		/// Movement vector.
		/// </summary>
		Vector3 _v;

		#endregion
		#region Unity Callbacks

		void Awake () {
			// Init vars
			_renderer = GetComponent<SpriteRenderer>();
			_life = 0f;
			_lifetime = Random.Range(_minLifetime, _maxLifetime);
			_v = new Vector3(Random.Range(-MAX_V, MAX_V), Random.Range(-MAX_V, MAX_V), Random.Range(-MAX_V, MAX_V));
		}

		void Update() {
			_life += Time.deltaTime;
			if (_life >= 1f) Destroy(gameObject);
			else {
				transform.position += _v * Time.deltaTime;
				transform.LookAt(Camera.main.transform);
				_v.y += Physics.gravity.y * Time.deltaTime;

				Color color = _renderer.color;
				color.a = _alpha.Evaluate(_life / _lifetime).a;
				_renderer.color = color;
			}
		}

		#endregion
	}
}
