using UnityEngine;
using System.Collections;

public class LightningFader : MonoBehaviour {

	Light _light;
	SpriteRenderer _renderer;
	public float fadeSpeed;
	float baseIntensity;

	void Awake () {
		_light = gameObject.Light();
		_renderer = gameObject.SpriteRenderer();
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
		_renderer.color = temp;

	}
}
