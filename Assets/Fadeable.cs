using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Fadeable : MonoBehaviour {

	public bool startFaded = true;
	public float fadeSpeed = 0.05f;
	public bool fadeAllChildren = false;
	public bool disableAfterFading = false;

	float alpha;
	Dictionary<MaskableGraphic, Color> originalColors;

	public void Start () {
		originalColors = new Dictionary<MaskableGraphic, Color>();
		if (fadeAllChildren) {
			List<MaskableGraphic> allGraphics = GetComponentsInChildren<MaskableGraphic>().ToList<MaskableGraphic>();
			foreach (MaskableGraphic graphic in allGraphics) originalColors.Add (graphic, graphic.color);
		} else originalColors.Add (GetComponent<MaskableGraphic>(), GetComponent<MaskableGraphic>().color);
			
		alpha = (startFaded ? 0f : 1f);
		foreach (MaskableGraphic graphic in originalColors.Keys) {
			Color newColor = originalColors[graphic];
			newColor.a *= alpha;
			graphic.color = newColor;
		}
	}

	public void Fade () {
		StopCoroutine ("DoUnFade");
		StartCoroutine("DoFade");
	}

	public void UnFade () {
		StopCoroutine("DoFade");
		StartCoroutine("DoUnFade");
	}

	IEnumerator DoFade () {
		while (alpha >= fadeSpeed && originalColors != null) {
			alpha -= fadeSpeed;

			foreach (MaskableGraphic graphic in originalColors.Keys) {
				Color newColor = originalColors[graphic];
				newColor.a *= alpha;
				graphic.color = newColor;
			}
				
			yield return 0;
		}

		if (disableAfterFading) gameObject.SetActive(false);
	}

	IEnumerator DoUnFade () {
		while (alpha <= 1f-fadeSpeed && originalColors != null) {
			alpha += fadeSpeed;

			foreach (MaskableGraphic graphic in originalColors.Keys) {
				Color newColor = originalColors[graphic];
				newColor.a *= alpha;
				graphic.color = newColor;
			}

			yield return 0;
		}
	}
}
