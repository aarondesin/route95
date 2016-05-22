using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Class to store decoration data.
/// </summary>
public class Decoration : MonoBehaviour, IPoolable {

	#region Decoration Enums

	/// <summary>
	/// Type of decoration distribution.
	/// </summary>
	public enum Distribution {
		Random,     // Truly random, based on density
		Roadside,   // For signs, placed alongside road facing either direction
		CloseToRoad // Placed close to road (good for small objects)
	}

	/// <summary>
	/// Group to assign decoration to.
	/// </summary>
	public enum Group {
		None,
		Vegetation,
		Rocks,
		RoadSigns
	}

	#endregion
	#region Decoration Structs

	/// <summary>
	/// Info for relevant decoration group.
	/// </summary>
	[System.Serializable]
	public struct GroupInfo {
		public Group group;
		public int numActive;
		public int maxActive;
	}

	#endregion
	#region Decoration Vars

	[Tooltip("Decoration group.")]
	public Group group = Group.None;

	[Tooltip("Is this decoration dynamic/physics-enabled?")]
	public bool dynamic = false;

	[Tooltip("Average number of this decoration per chunk.")]
	public float density;

	[Tooltip("Distribution type to use for this decoration.")]
	public Distribution distribution; // Distribution type to use

	[Tooltip("Base position offset to use when placing.")]
	public Vector3 positionOffset;

	[Tooltip("Base rotation offset to use when placing.")]
	public Vector3 rotationOffset;

	[Tooltip("Range of randomization in height.")]
	public Vector2 heightRange;

	[Tooltip("Range of randomization in width.")]
	public Vector2 widthRange;

	[Tooltip("Range of randomization in pitch.")]
	public Vector2 pitchRange;

	[Tooltip("Range of randomization in yaw.")]
	public Vector2 yawRange;

	[Tooltip("Range of randomization in roll.")]
	public Vector2 rollRange;

	#endregion
	#region Unity Callbacks

	void Awake () {
		Randomize();
	}

	void FixedUpdate () {
		if (dynamic) {

			// Remove if decoration fell below
			if (transform.position.y < -WorldManager.instance.heightScale) 
				WorldManager.instance.RemoveDecoration (gameObject);

			// Push with wind
			GetComponent<Rigidbody>().AddForce(WorldManager.instance.wind);
		}
	}

	#endregion
	#region IPoolable Implementations

	void IPoolable.OnPool() {
		gameObject.SetActive(false);
	}

	void IPoolable.OnDepool() {
		gameObject.SetActive(true);
		transform.localScale = Vector3.one;
		Randomize();
	}

	#endregion
	#region Decoration Methods

	// Starts with base position/rotation, and adds variance
	public void Randomize () {
		transform.position += positionOffset;
		transform.rotation = Quaternion.Euler(rotationOffset.x, rotationOffset.y, rotationOffset.z);

		// Randomize scale (width and height)
		transform.localScale = new Vector3 (
			transform.localScale.x * Random.Range (widthRange[0], widthRange[1]),
			transform.localScale.y * Random.Range (heightRange[0], heightRange[1]),
			transform.localScale.z * Random.Range (widthRange[0], widthRange[1])
		);

		// Randomize rotation
		transform.Rotate ( new Vector3 (
			Random.Range (pitchRange[0], pitchRange[1]),
			Random.Range (yawRange[0], yawRange[1]),
			Random.Range (rollRange[0], rollRange[1])
		), Space.World);
	}

	#endregion
}
