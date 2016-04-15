using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ShowHide : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
	public List<GameObject> objects;

	public void OnPointerEnter (PointerEventData eventData) {
		if (objects != null)
			foreach (GameObject obj in objects) obj.SetActive(true);
	}

	public void OnPointerExit (PointerEventData eventData) {
		if (objects != null)
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
		button.GetComponent<RectTransform>().localScale = new Vector3 (1f,1f,1f);
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
