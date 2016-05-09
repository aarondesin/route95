﻿using UnityEngine;
using System.Collections;

public class Moon : MonoBehaviour {
	public static Moon instance;

	#region Moon Vars

	private float xScale = 1000f;
	private float yScale = 1000f;
	private float zScale = 1000f;

	private Vector3 sunTarget; // target for the sun to point at: the car or the origin

	#endregion
	#region Unity Callbacks

	void Start() {
		instance = this;
		this.GetComponent<Light> ().range = 100f;
		this.GetComponent<Light> ().type = LightType.Directional;
		GetComponent<Light>().shadowBias = 1f;
		GetComponent<Light>().cullingMask = (1 << 0 | 1 << 1 | 1 << 2 | 1 << 4 | 1 << 5 | 1 << 8 | 1 << 9);
		transform.SetParent (PlayerMovement.instance.transform);
		transform.localScale = new Vector3 (100f, 100f, 100f);
	}

	void Update() {
		UpdateTransform();
	}

	#endregion
	#region Moon Callbacks
		
	private void UpdateTransform(){
		sunTarget = PlayerMovement.instance.transform.position;

		float newX = -xScale * Mathf.Cos(WorldManager.instance.timeOfDay);
		float newY = -yScale * Mathf.Sin(WorldManager.instance.timeOfDay);
		float newZ = zScale * Mathf.Cos(WorldManager.instance.timeOfDay + Mathf.PI/5);
		this.transform.position = new Vector3(newX, newY, newZ);

		this.transform.LookAt (sunTarget);
	}

	#endregion

}
