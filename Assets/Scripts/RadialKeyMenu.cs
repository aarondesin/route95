using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class RadialKeyMenu : MonoBehaviour {
	public static RadialKeyMenu instance;

	List<GameObject> objects;

	private float radius;
	private float scale;
	public float baseScale;
	public float scaleFactor;
	public GameObject confirmButton;

	void LateStart () {
		instance = this;
		Refresh();
	}

	void Update () {
		if (instance == null) LateStart();
	}
		

	public void Refresh () {
		if (objects == null) objects = new List<GameObject>();
		else {
			foreach (GameObject obj in objects) Destroy (obj);
			objects.Clear();
		}

		radius = (GetComponent<RectTransform>().rect.width - baseScale) / 2f;
		scale = baseScale;

		// Layer one
		int numKeys = Enum.GetValues(typeof(Key)).Length;
		for (int i=1; i < numKeys; i++) { // i=1 so that it skips Key.None
			Key key = (Key)i;
			float angle = (float)i / (float)(numKeys-1) * 2f * Mathf.PI;
			GameObject button = UI.MakeTextButton(key.ToString());

			RectTransform tr = button.GetComponent<RectTransform>();
			tr.SetParent (gameObject.GetComponent<RectTransform>());
			tr.sizeDelta = new Vector2 (scale, scale);
			tr.localScale = new Vector3 (1f, 1f, 1f);
			tr.anchorMin = new Vector2 (0.5f, 0.5f);
			tr.anchorMax = new Vector2 (0.5f, 0.5f);
			tr.anchoredPosition3D = new Vector3 (
				radius * Mathf.Cos (angle),
				radius * Mathf.Sin (angle),
				0f
			);

			Text text = button.GetComponentInChildren<Text>();
			if (key.ToString().Contains("Sharp")) text.text = key.ToString()[0] + "#";
			text.font = GameManager.instance.font;
			text.fontSize = (int)(scale/2f);
			text.color = Color.white;

			Image img = button.GetComponent<Image>();
			img.sprite = GameManager.instance.circleIcon;

			button.GetComponent<Button>().onClick.AddListener (delegate {
				MusicManager.instance.currentSong.key = key;
				Refresh();
			});

			objects.Add (button);
		}

		// Layer two
		radius *= scaleFactor;
		scale *= scaleFactor;
		int numScales = ScaleInfo.AllScales.Count;
		for (int i=0; i < numScales; i++) {
			ScaleInfo scalei = ScaleInfo.AllScales[i];
			float angle = (float)i / (float)numScales * 2f * Mathf.PI;
			GameObject button = UI.MakeTextButton(scalei.name);

			RectTransform tr = button.GetComponent<RectTransform>();
			tr.SetParent (gameObject.GetComponent<RectTransform>());
			tr.sizeDelta = new Vector2 (scale, scale);
			tr.localScale = new Vector3 (1f, 1f, 1f);
			tr.anchorMin = new Vector2 (0.5f, 0.5f);
			tr.anchorMax = new Vector2 (0.5f, 0.5f);
			tr.anchoredPosition3D = new Vector3 (
				radius * Mathf.Cos (angle),
				radius * Mathf.Sin (angle),
				0f
			);

			Text text = button.GetComponentInChildren<Text>();
			text.font = GameManager.instance.font;
			text.fontSize = (int)(baseScale/2f);
			text.color = Color.white;

			Image img = button.GetComponent<Image>();
			img.sprite = GameManager.instance.circleIcon;

			button.GetComponent<Button>().onClick.AddListener (delegate {
				MusicManager.instance.currentSong.scale = scalei.scaleIndex;
				Refresh();
			});

			objects.Add (button);
				
		}

		// Confirm button
		if (MusicManager.instance.currentSong.key != Key.None &&
			MusicManager.instance.currentSong.scale != -1) {
			confirmButton.SetActive (true);
		} else
			confirmButton.SetActive (false);

	}


}
