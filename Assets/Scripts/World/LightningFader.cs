// LightningFader.cs
// ©2016 Team 95

using UnityEngine;

namespace Route95.World {

	/// <summary>
	/// Class to handle fading of lightning.
	/// </summary>
	public class LightningFader : MonoBehaviour {

		#region LightningFader Vars

		/// <summary>
		/// Reference to light component.
		/// </summary>
		Light _light;

		/// <summary>
		/// Reference to sprite renderer.
		/// </summary>
		SpriteRenderer _renderer;

		/// <summary>
		/// Lightning fade speed (seconds).
		/// </summary>
		[SerializeField]
		[Tooltip("Lightning fade speed (seconds).")]
		float _fadeSpeed;

		#endregion
		#region Unity Callbacks

		void Awake() {
			// Init vars
			_light = gameObject.GetComponent<Light>();
			_renderer = gameObject.GetComponent<SpriteRenderer>();
		}

		void Update() {
			float baseIntensity = WorldManager.Instance.BaseLightningIntensity;

			if (_light.intensity > 0f) _light.intensity -= baseIntensity * (Time.deltaTime / _fadeSpeed);
			else {
				gameObject.SetActive(false);
				return;
			}

			Color temp = Color.white;
			temp.a = _light.intensity;
			_renderer.color = temp;
		}

		#endregion
	}
}
