using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BlinkingIcon : MonoBehaviour {

	public Color maxColor;
	public float blinkInterval;

	[SerializeField]
	float progress;

	void Start () {
		progress = 0f;
	}

	void Update () {
		if (progress >= 2f*Mathf.PI) progress = 0;
		Color color = maxColor;
		color.a = 0.5f+0.5f*Mathf.Sin(progress);
		GetComponent<Image>().color = color;
		progress += blinkInterval;
	}
}
