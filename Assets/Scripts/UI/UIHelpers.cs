﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Class to hold UI-related helper functions.
/// </summary>
public class UIHelpers : MonoBehaviour {

	/// <summary>
	/// Creates a button GameObject with default properties.
	/// </summary>
	/// <param name="buttonName"></param>
	/// <returns></returns>
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

	/// <summary>
	/// Creates a button GameObject with default properties
	/// and a child text GameObject.
	/// </summary>
	/// <param name="buttonText">Text to show on button.</param>
	/// <returns></returns>
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

	/// <summary>
	/// Creates a text GameObject with default properties.
	/// </summary>
	/// <param name="textName"></param>
	/// <returns></returns>
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

	/// <summary>
	/// Creates an image GameObject with default properties.
	/// </summary>
	/// <param name="imageName"></param>
	/// <returns></returns>
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