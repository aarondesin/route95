using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SongProgressBar : MonoBehaviour {

	public static SongProgressBar instance;

	public Image background;
	public Image bar;

	float value;

	void Start () {
		instance = this;
	}

	void Update () {
		RectTransform bgtr = background.GetComponent<RectTransform>();
		RectTransform batr = bar.GetComponent<RectTransform>();
		batr.sizeDelta = new Vector2 (value*bgtr.rect.width, bgtr.rect.height);
		batr.anchoredPosition3D = new Vector3 (
				batr.sizeDelta.x/2f,
				0f,
				0f
		);
	}

	public void SetValue (float v) {
		value = Mathf.Clamp01(v);
	}
}
