using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ShowHide : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
	public List<GameObject> objects;

	public void OnPointerEnter (PointerEventData eventData) {
		foreach (GameObject obj in objects) obj.SetActive(true);
	}

	public void OnPointerExit (PointerEventData eventData) {
		foreach (GameObject obj in objects) obj.SetActive(false);
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
		return button;
	}

	public static GameObject MakeText (string textName) {
		GameObject text = new GameObject (textName,
			typeof (RectTransform),
			typeof (CanvasRenderer),
			typeof (Text)
		);
		return text;
	}
}
