using UnityEngine;
using System.Collections;

public class LightningFader : MonoBehaviour {

	Light _light;
	SpriteRenderer renderer;
	public float fadeSpeed;
	float baseIntensity;

	void Awake () {
		_light = gameObject.Light();
		renderer = gameObject.SpriteRenderer();
	}

	void Start () {
		baseIntensity = WorldManager.instance.baseLightningIntensity;
	}

	void Update () {
		if (_light.intensity > 0f) _light.intensity -= baseIntensity* fadeSpeed * Time.deltaTime;
		else {
			gameObject.SetActive(false);
			return;
		}

		Color temp = Color.white;
		temp.a = _light.intensity;
		renderer.color = temp;

	}
}
