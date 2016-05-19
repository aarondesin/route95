using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Class to fade all maskable graphics on and in a GameObject
/// </summary>
public class Fadeable : MonoBehaviour {

	#region Fadeable Vars

	public bool startFaded = true;          // If true, graphic will start faded
	public float fadeSpeed = 0.05f;         // Speed at which to perform fade
	public bool fadeAllChildren = false;    // If true, will fade all graphics in children as well
	public bool disableAfterFading = false; // If true, will disable GameObject after fade is complete

	float alpha;                                       // Current fade alpha
	Dictionary<MaskableGraphic, Color> originalColors; // Stores references to each graphic and its original color

	#endregion
	#region Unity Callbacks

	public void Awake () {

		// Build dicts of original colors
		originalColors = new Dictionary<MaskableGraphic, Color>();
		if (fadeAllChildren) {
			List<MaskableGraphic> allGraphics = GetComponentsInChildren<MaskableGraphic>().ToList<MaskableGraphic>();
			foreach (MaskableGraphic graphic in allGraphics) originalColors.Add (graphic, graphic.color);
		} else if (GetComponent<MaskableGraphic>() != null)
			originalColors.Add (GetComponent<MaskableGraphic>(), GetComponent<MaskableGraphic>().color);
			
		// Initially fade if necessary
		alpha = (startFaded ? 0f : 1f);
		foreach (MaskableGraphic graphic in originalColors.Keys) {
			Color newColor = originalColors[graphic];
			newColor.a *= alpha;
			graphic.color = newColor;
		}
	}

	#endregion
	#region Fadeable Methods

	/// <summary>
	/// Starts fading the object.
	/// </summary>
	public void Fade () {
		StopCoroutine ("DoUnFade");
		if (!gameObject.activeSelf) return;
		StartCoroutine("DoFade");
	}

	/// <summary>
	/// Starts unfading the object.
	/// </summary>
	public void UnFade () {
		StopCoroutine("DoFade");
		if (!gameObject.activeSelf) return; 
		StartCoroutine("DoUnFade");
	}

	/// <summary>
	/// Fades an object.
	/// </summary>
	/// <returns></returns>
	IEnumerator DoFade () {
		while (true) {
			if (alpha >= fadeSpeed && originalColors != null) {
				alpha -= fadeSpeed;

				foreach (MaskableGraphic graphic in originalColors.Keys) {
					Color newColor = originalColors[graphic];
					newColor.a *= alpha;
					graphic.color = newColor;
				}
					
				yield return 0;
			} else {
				if (disableAfterFading) gameObject.SetActive(false);
				yield break;
			}
		}
	}

	/// <summary>
	/// Unfades an object.
	/// </summary>
	/// <returns></returns>
	IEnumerator DoUnFade () {
		while (true) {
			if (alpha <= 1f-fadeSpeed && originalColors != null) {
				alpha += fadeSpeed;

				foreach (MaskableGraphic graphic in originalColors.Keys) {
					Color newColor = originalColors[graphic];
					newColor.a *= alpha;
					graphic.color = newColor;
				}

				yield return 0;
			} else {
				yield break;
			}
		}
	}

	#endregion
}
