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

	public void OnPointerEnter (PointerEventData eventData) {
		if (objects != null && InputManager.instance.selected == null) {
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
	}

	public void OnPointerExit (PointerEventData eventData) {
		if (objects != null && InputManager.instance.selected == null) {
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
		button.GetComponent<RectTransform>().localScale = new Vector3 (1f,1f,1f);
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
		text.GetComponent<Text>().resizeTextForBestFit = true;
		return text;
	}

	public static GameObject MakeImage (string imageName) {
		GameObject image = new GameObject (imageName,
			typeof (RectTransform),
			typeof (CanvasRenderer),
			typeof (Image)
		);
		return image;
	}


}
