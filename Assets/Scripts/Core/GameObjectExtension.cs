using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Class with extension methods for GameObjects and Components.
/// </summary>
public static class GameObjectExtension {

	#region GameObject Extensions

	/// <summary>
	/// Quick reference to the GameObject's RectTransform component.
	/// </summary>
	/// <param name="obj"></param>
	/// <returns></returns>
	public static RectTransform RectTransform (this GameObject obj) {
		return obj.GetComponent<RectTransform>();
	}

	/// <summary>
	/// Quick reference to the GameObject's Button component.
	/// </summary>
	/// <param name="obj"></param>
	/// <returns></returns>
	public static Button Button (this GameObject obj) {
		return obj.GetComponent<Button>();
	}

	/// <summary>
	/// Quick reference to the GameObject's Text component.
	/// </summary>
	/// <param name="obj"></param>
	/// <returns></returns>
	public static Text Text (this GameObject obj) {
		return obj.GetComponent<Text>();
	}

	/// <summary>
	/// Quick reference to the GameObject's Image component.
	/// </summary>
	/// <param name="obj"></param>
	/// <returns></returns>
	public static Image Image (this GameObject obj) {
		return obj.GetComponent<Image>();
	}

	/// <summary>
	/// Quick reference to the GameObject's SpriteRenderer component.
	/// </summary>
	/// <param name="obj"></param>
	/// <returns></returns>
	public static SpriteRenderer SpriteRenderer (this GameObject obj) {
		return obj.GetComponent<SpriteRenderer>();
	}

	/// <summary>
	/// Quick reference to the GameObject's ShowHide component.
	/// </summary>
	/// <param name="obj"></param>
	/// <returns></returns>
	public static ShowHide ShowHide (this GameObject obj) {
		return obj.GetComponent<ShowHide>();
	}

	/// <summary>
	/// Quick reference to the GameObject's Light component.
	/// </summary>
	/// <param name="obj"></param>
	/// <returns></returns>
	public static Light Light (this GameObject obj) {
		return obj.GetComponent<Light>();
	}

	#endregion
	#region ParticleSystem Extensions

	/// <summary>
	/// Sets the emission rate of a particle system.
	/// </summary>
	/// <param name="sys"></param>
	/// <param name="newRate"></param>
	public static void SetRate (this ParticleSystem sys, float newRate) {
		ParticleSystem.EmissionModule temp = sys.emission;
		ParticleSystem.MinMaxCurve curve = temp.rate;
		curve = new ParticleSystem.MinMaxCurve(newRate);
		temp.rate = curve;
	}

	#endregion
}
