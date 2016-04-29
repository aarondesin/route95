﻿using UnityEngine;
using System.Collections;

public class Sun : MonoBehaviour {
	public static Sun instance;

	private float xScale = 1000;
	private float yScale = 1000;
	private float zScale = 1000;

	private Vector3 sunTarget; // target for the sun to point at: the car or the origin

	private void UpdateTransform(){
		sunTarget = PlayerMovement.instance.transform.position;
		float newX = xScale * Mathf.Cos(WorldManager.instance.timeOfDay);
		float newY = yScale * Mathf.Sin(WorldManager.instance.timeOfDay);
		float newZ = -zScale * Mathf.Cos(WorldManager.instance.timeOfDay + Mathf.PI/5);
		this.transform.position = new Vector3(newX, newY, newZ);
		transform.localScale = new Vector3 (100f, 100f, 100f);

		this.transform.LookAt (sunTarget);
	}

	void Start() {
		instance = this;
		transform.parent = PlayerMovement.instance.transform;
		this.GetComponent<Light> ().range = 100f;
		this.GetComponent<Light> ().type = LightType.Directional;
		GetComponent<Light>().shadowBias = 1f;
		GetComponent<Light>().cullingMask = (1 << 0 | 1 << 1 | 1 << 2 | 1 << 4 | 1 << 5 | 1 << 8 | 1 << 9);
	}
	
	// Update is called once per frame
	void Update() {
		UpdateTransform();
	}
		
}
