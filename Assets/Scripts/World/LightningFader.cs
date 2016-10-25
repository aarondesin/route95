﻿using Route95.Core;
using UnityEngine;
using System.Collections;

namespace Route95.World {

/// <summary>
/// Class to handle fading of lightning.
/// </summary>
public class LightningFader : MonoBehaviour {

	#region LightningFader Vars

	Light _light;             // Reference to light
	SpriteRenderer _renderer; // Lightning sprite renderer
	public float fadeSpeed;   // Lightning fade speed
	float baseIntensity;      // Base lightning intensity

	#endregion
	#region Unity Callbacks

	void Awake () {
		_light = gameObject.GetComponent<Light>();
		_renderer = gameObject.GetComponent<SpriteRenderer>();
	}

	void Start () {
		baseIntensity = WorldManager.Instance.baseLightningIntensity;
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

	#endregion
}
}
