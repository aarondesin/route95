﻿using UnityEngine;
using System.Collections;

public class Decoration : MonoBehaviour {

	public float density; // 0-1, density of population

	public Vector3 positionOffset;
	public Vector3 rotationOffset;

	public Vector2 heightRange;
	public Vector2 widthRange;

	public Vector2 pitchRange;
	public Vector2 yawRange;
	public Vector2 rollRange;

	void Start () {
	}

	// Starts with base position/rotation, and adds variance
	public void Randomize () {
		GetComponent<Transform>().position += positionOffset;
		GetComponent<Transform>().rotation = Quaternion.Euler(rotationOffset.x, rotationOffset.y, rotationOffset.z);

		// Randomize scale (width and height)
		GetComponent<Transform>().localScale = new Vector3 (
			Random.Range (widthRange[0], widthRange[1]),
			Random.Range (heightRange[0], heightRange[1]),
			Random.Range (widthRange[0], widthRange[1])
		);

		// Randomize rotation
		GetComponent<Transform>().Rotate ( new Vector3 (
			Random.Range (pitchRange[0], pitchRange[1]),
			Random.Range (yawRange[0], yawRange[1]),
			Random.Range (rollRange[0], rollRange[1])
		), Space.World);
	}
}
