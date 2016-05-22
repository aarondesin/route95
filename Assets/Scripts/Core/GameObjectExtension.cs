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

	public static void SetParent (this GameObject obj, RectTransform parent) {
		RectTransform tr = obj.RectTransform();
		Vector3 scale = tr.localScale;
		tr.SetParent (parent);
		tr.localScale = scale;
	}

	public static void AnchorAtPoint (this GameObject obj, float x, float y) {
		obj.RectTransform().AnchorAtPoint (new Vector2 (x, y));
	}

	public static void SetSize2D (this GameObject obj, float x, float y) {
		obj.RectTransform().sizeDelta = new Vector2 (x, y);
	}

	public static void SetSideWidth (this GameObject obj, float width) {
		obj.RectTransform().sizeDelta = new Vector2 (width, width);
	}

	public static void SetPosition2D (this GameObject obj, float x, float y) {
		obj.RectTransform().anchoredPosition = new Vector2 (x, y);
	}

	public static void SetTextAlignment (this GameObject obj, TextAnchor align) {
		obj.Text().alignment = align;
	}

	public static void SetFontSize (this GameObject obj, int size) {
		obj.Text().fontSize = size;
	}

	#endregion
	#region RectTransform Extensions

	public static void SetSideWidth (this RectTransform tr, float width) {
		tr.sizeDelta = new Vector2 (width, width);
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
