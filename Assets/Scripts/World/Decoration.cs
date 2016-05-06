﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Decoration : MonoBehaviour {

	public enum Distribution {
		Random, // truly random, based on density
		Roadside, // for signs, placed alongside road facing either direction
		CloseToRoad // placed close to road (good for small objects)
	}

	public enum Group {
		None,
		Vegetation,
		Rocks,
		RoadSigns
	}

	[System.Serializable]
	public struct GroupInfo {
		public Group group;
		public int maxActive;
	}

	#region Decoration Vars

	public static Dictionary<Group, int> numDecorations;
	public Group group = Group.None;
	public bool dynamic = false;

	[Tooltip("Average number of this decoration per chunk.")]
	public float density;
	public Distribution distribution;

	public Vector3 positionOffset;
	public Vector3 rotationOffset;

	public Vector2 heightRange;
	public Vector2 widthRange;

	public Vector2 pitchRange;
	public Vector2 yawRange;
	public Vector2 rollRange;

	#endregion
	#region Unity Callbacks

	void FixedUpdate () {
		if (dynamic) {
			GetComponent<Rigidbody>().AddForce(WorldManager.instance.wind);
			if (transform.position.y < -WorldManager.instance.heightScale) WorldManager.instance.RemoveDecoration (gameObject);
		}
	}

	#endregion
	#region Decoration Methods

	// Starts with base position/rotation, and adds variance
	public void Randomize () {
		GetComponent<Transform>().position += positionOffset;
		GetComponent<Transform>().rotation = Quaternion.Euler(rotationOffset.x, rotationOffset.y, rotationOffset.z);

		// Randomize scale (width and height)
		GetComponent<Transform>().localScale = new Vector3 (
			WorldManager.instance.chunkSize*Random.Range (widthRange[0], widthRange[1]),
			WorldManager.instance.chunkSize*Random.Range (heightRange[0], heightRange[1]),
			WorldManager.instance.chunkSize*Random.Range (widthRange[0], widthRange[1])
		);

		// Randomize rotation
		GetComponent<Transform>().Rotate ( new Vector3 (
			Random.Range (pitchRange[0], pitchRange[1]),
			Random.Range (yawRange[0], yawRange[1]),
			Random.Range (rollRange[0], rollRange[1])
		), Space.World);
	}

	#endregion
}
