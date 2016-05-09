using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum TransitionType {
	Instant,
	Fade
}

public class ShowHide : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
	public List<GameObject> objects;
	public TransitionType transitionType = TransitionType.Instant;

	public float fadeSpeed;
	List<IEnumerator> activeFades;

	public void Show() {
		foreach (GameObject obj in objects) {
			switch (transitionType) {
			case TransitionType.Instant:
				obj.SetActive(true);
				break;
			case TransitionType.Fade:
				if (activeFades == null) activeFades = new List<IEnumerator>();
				IEnumerator temp = Fade(obj);
				activeFades.Add(temp);
				StartCoroutine (temp);
				break;
			}
		}
	}

	public void Hide () {
		foreach (GameObject obj in objects) {
			switch (transitionType) {
			case TransitionType.Instant:
				obj.SetActive(false);
				break;
			case TransitionType.Fade:
				foreach (IEnumerator fade in activeFades)
					StopCoroutine (fade);
				activeFades.Clear();
				break;
			}
		}
	}

	public void OnPointerEnter (PointerEventData eventData) {
		if (objects != null && InputManager.instance.selected == null) {
			Show();
		}
	}

	public void OnPointerExit (PointerEventData eventData) {
		if (objects != null && InputManager.instance.selected == null) {
			Hide();
		}
	}

	public IEnumerator Fade (GameObject target) {
		for (float a = 0f; a < 1.0f; a += fadeSpeed) {
			target.GetComponent<Image>().color = new Color (1, 1, 1f, a);
			yield return null;
		}
	}
}

public class UI : MonoBehaviour {

	public static GameObject MakeButton (string buttonName) {
		GameObject button = new GameObject (buttonName,
			typeof (RectTransform),
			typeof (CanvasRenderer),
			typeof (Button),
			typeof (ShowHide),
			typeof (Image)
		);
		RectTransform tr = button.GetComponent<RectTransform>();
		tr.localScale = Vector3.one;
		tr.localRotation = Quaternion.Euler(Vector3.zero);

		return button;
	}

	public static GameObject MakeTextButton (string buttonText) {
		GameObject button = MakeButton (buttonText);
		RectTransform button_tr = button.GetComponent<RectTransform>();
		GameObject text = MakeText (buttonText+"text");
		RectTransform text_tr = text.GetComponent<RectTransform>();
		Text text_text = text.GetComponent<Text>();
		text_tr.SetParent(button_tr);
		text_tr.sizeDelta = button_tr.sizeDelta;
		text_tr.localScale = button_tr.localScale;
		text_text.alignment = TextAnchor.MiddleCenter;
		text_text.text = buttonText;

		return button;
	}

	public static GameObject MakeText (string textName) {
		GameObject text = new GameObject (textName,
			typeof (RectTransform),
			typeof (CanvasRenderer),
			typeof (Text)
		);
		RectTransform tr = text.GetComponent<RectTransform>();
		tr.localScale = Vector3.one;
		tr.localRotation = Quaternion.Euler(Vector3.zero);
		Text txt = text.GetComponent<Text>();
		txt.resizeTextForBestFit = false;
		txt.fontStyle = FontStyle.Normal;
		return text;
	}

	public static GameObject MakeImage (string imageName) {
		GameObject image = new GameObject (imageName,
			typeof (RectTransform),
			typeof (CanvasRenderer),
			typeof (Image)
		);
		RectTransform tr = image.GetComponent<RectTransform>();
		tr.localScale = Vector3.one;
		tr.localRotation = Quaternion.Euler(Vector3.zero);
		return image;
	}
}

public static class UIExtension {
	/// <summary>
	/// Anchors at point.
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public static void AnchorAtPoint (this RectTransform tr, float x, float y) {
		Vector2 anchorPoint = new Vector2 (x, y);
		tr.anchorMax = anchorPoint;
		tr.anchorMin = anchorPoint;
	}

	/// <summary>
	/// Anchors at point.
	/// </summary>
	/// <param name="anchorPoint">Anchor point.</param>
	public static void AnchorAtPoint (this RectTransform tr, Vector2 anchorPoint) {
		tr.anchorMax = anchorPoint;
		tr.anchorMin = anchorPoint;
	}

	public static void ResetScaleRot (this RectTransform tr) {
		tr.localScale = Vector3.one;
		tr.localRotation = Quaternion.Euler(Vector3.zero);
	}
}

public static class GameObjectExtension {
	
	public static RectTransform RectTransform (this GameObject obj) {
		return obj.GetComponent<RectTransform>();
	}

	public static Button Button (this GameObject obj) {
		return obj.GetComponent<Button>();
	}

	public static Text Text (this GameObject obj) {
		return obj.GetComponent<Text>();
	}

	public static Image Image (this GameObject obj) {
		return obj.GetComponent<Image>();
	}

	public static SpriteRenderer SpriteRenderer (this GameObject obj) {
		return obj.GetComponent<SpriteRenderer>();
	}

	public static ShowHide ShowHide (this GameObject obj) {
		return obj.GetComponent<ShowHide>();
	}

	public static Light Light (this GameObject obj) {
		return obj.GetComponent<Light>();
	}

	public static void SetRate (this ParticleSystem sys, float newRate) {
		ParticleSystem.EmissionModule temp = sys.emission;
		ParticleSystem.MinMaxCurve curve = temp.rate;
		curve = new ParticleSystem.MinMaxCurve(newRate);
		temp.rate = curve;
	}
}
